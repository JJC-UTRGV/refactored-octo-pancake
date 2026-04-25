using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    private const string VolumeKey = "settings.masterVolume";
    private const string FullscreenKey = "settings.fullscreen";
    private const string QualityKey = "settings.quality";
    private static readonly List<string> QualityLabels = new List<string> { "Low", "High" };

    private MenuLogic menuLogic;
    private Slider volumeSlider;
    private Toggle fullscreenToggle;
    private TMP_Dropdown qualityDropdown;
    private TextMeshProUGUI volumeValueText;
    private bool initialized;

    public void Initialize(MenuLogic owner)
    {
        menuLogic = owner;

        ApplySavedSettings();
        BuildPanelIfNeeded();
        SyncControls();
        initialized = true;
    }

    private void OnEnable()
    {
        if (initialized)
            SyncControls();
    }

    private void ApplySavedSettings()
    {
        AudioListener.volume = PlayerPrefs.GetFloat(VolumeKey, AudioListener.volume);

        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        Screen.fullScreen = fullscreen;

        int savedQuality = PlayerPrefs.GetInt(QualityKey, GetDropdownQualityIndex(QualitySettings.GetQualityLevel()));
        SetQuality(savedQuality);
    }

    private void BuildPanelIfNeeded()
    {
        Transform existingContent = transform.Find("GeneratedSettingsContent");
        if (existingContent != null)
        {
            existingContent.gameObject.SetActive(false);
            Destroy(existingContent.gameObject);
        }

        Image overlay = GetComponent<Image>();
        if (overlay != null)
            overlay.color = new Color(0.08f, 0.1f, 0.12f, 0.82f);

        RectTransform content = CreateRect("GeneratedSettingsContent", transform);
        content.anchorMin = new Vector2(0.5f, 0.5f);
        content.anchorMax = new Vector2(0.5f, 0.5f);
        content.pivot = new Vector2(0.5f, 0.5f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(480f, 390f);

        Image panelImage = content.gameObject.AddComponent<Image>();
        panelImage.color = new Color(0.95f, 0.97f, 0.95f, 0.96f);

        VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 28, 28);
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        TextMeshProUGUI title = CreateText("Title", content, "SETTINGS", 36f, FontStyles.Bold);
        title.color = new Color(0.15f, 0.18f, 0.16f);
        AddLayout(title.gameObject, -1f, 52f);

        CreateVolumeRow(content);
        CreateFullscreenRow(content);
        CreateQualityRow(content);
        CreateCloseButton(content);
    }

    private void CreateVolumeRow(Transform parent)
    {
        RectTransform row = CreateRow("VolumeRow", parent);
        TextMeshProUGUI label = CreateText("Label", row, "Volume", 22f, FontStyles.Normal);
        label.color = new Color(0.15f, 0.18f, 0.16f);
        AddLayout(label.gameObject, 130f, 42f);

        volumeSlider = CreateSlider(row);
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.onValueChanged.AddListener(SetVolume);
        AddLayout(volumeSlider.gameObject, 190f, 42f);

        volumeValueText = CreateText("Value", row, "100%", 20f, FontStyles.Normal);
        volumeValueText.alignment = TextAlignmentOptions.Center;
        volumeValueText.color = new Color(0.15f, 0.18f, 0.16f);
        AddLayout(volumeValueText.gameObject, 70f, 42f);
    }

    private void CreateFullscreenRow(Transform parent)
    {
        RectTransform row = CreateRow("FullscreenRow", parent);
        TextMeshProUGUI label = CreateText("Label", row, "Fullscreen", 22f, FontStyles.Normal);
        label.color = new Color(0.15f, 0.18f, 0.16f);
        AddLayout(label.gameObject, 334f, 42f);

        fullscreenToggle = CreateToggle(row);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        AddLayout(fullscreenToggle.gameObject, 42f, 42f);
    }

    private void CreateQualityRow(Transform parent)
    {
        RectTransform row = CreateRow("QualityRow", parent);
        TextMeshProUGUI label = CreateText("Label", row, "Quality", 22f, FontStyles.Normal);
        label.color = new Color(0.15f, 0.18f, 0.16f);
        AddLayout(label.gameObject, 130f, 42f);

        qualityDropdown = CreateDropdown(row);
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(QualityLabels);
        qualityDropdown.onValueChanged.AddListener(SetQuality);
        AddLayout(qualityDropdown.gameObject, 292f, 42f);
    }

    private void CreateCloseButton(Transform parent)
    {
        Button button = CreateButton("CloseButton", parent, "CLOSE");
        button.onClick.AddListener(Close);
        AddLayout(button.gameObject, 180f, 46f);
    }

    private void SyncControls()
    {
        if (volumeSlider != null)
            volumeSlider.SetValueWithoutNotify(AudioListener.volume);

        if (volumeValueText != null)
            volumeValueText.text = Mathf.RoundToInt(AudioListener.volume * 100f) + "%";

        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);

        if (qualityDropdown != null)
            qualityDropdown.SetValueWithoutNotify(GetDropdownQualityIndex(QualitySettings.GetQualityLevel()));
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(VolumeKey, AudioListener.volume);

        if (volumeValueText != null)
            volumeValueText.text = Mathf.RoundToInt(AudioListener.volume * 100f) + "%";
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt(FullscreenKey, fullscreen ? 1 : 0);
    }

    public void SetQuality(int qualityIndex)
    {
        if (QualitySettings.names.Length == 0)
            return;

        int dropdownQuality = Mathf.Clamp(qualityIndex, 0, QualityLabels.Count - 1);
        int unityQuality = GetUnityQualityIndex(dropdownQuality);

        QualitySettings.SetQualityLevel(unityQuality);
        PlayerPrefs.SetInt(QualityKey, dropdownQuality);
    }

    private static int GetUnityQualityIndex(int dropdownQualityIndex)
    {
        if (QualitySettings.names.Length <= 1)
            return 0;

        return dropdownQualityIndex <= 0 ? 0 : QualitySettings.names.Length - 1;
    }

    private static int GetDropdownQualityIndex(int unityQualityIndex)
    {
        if (QualitySettings.names.Length <= 1)
            return 0;

        return unityQualityIndex >= QualitySettings.names.Length - 1 ? 1 : 0;
    }

    public void Close()
    {
        PlayerPrefs.Save();

        if (menuLogic != null)
            menuLogic.CloseSettings();
        else
            gameObject.SetActive(false);
    }

    private static RectTransform CreateRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        return rect;
    }

    private static RectTransform CreateRow(string name, Transform parent)
    {
        RectTransform row = CreateRect(name, parent);
        row.sizeDelta = new Vector2(0f, 42f);

        HorizontalLayoutGroup layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 14f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        AddLayout(row.gameObject, -1f, 42f);
        return row;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string text, float size, FontStyles style)
    {
        RectTransform rect = CreateRect(name, parent);
        TextMeshProUGUI label = rect.gameObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.fontStyle = style;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.enableAutoSizing = true;
        label.fontSizeMin = 14f;
        label.fontSizeMax = size;
        label.raycastTarget = false;
        return label;
    }

    private static Slider CreateSlider(Transform parent)
    {
        RectTransform root = CreateRect("VolumeSlider", parent);
        Slider slider = root.gameObject.AddComponent<Slider>();

        RectTransform background = CreateRect("Background", root);
        Stretch(background);
        Image backgroundImage = background.gameObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.72f, 0.76f, 0.72f);

        RectTransform fillArea = CreateRect("Fill Area", root);
        fillArea.anchorMin = new Vector2(0f, 0.25f);
        fillArea.anchorMax = new Vector2(1f, 0.75f);
        fillArea.offsetMin = new Vector2(8f, 0f);
        fillArea.offsetMax = new Vector2(-8f, 0f);

        RectTransform fill = CreateRect("Fill", fillArea);
        Stretch(fill);
        Image fillImage = fill.gameObject.AddComponent<Image>();
        fillImage.color = new Color(0.26f, 0.54f, 0.36f);

        RectTransform handleArea = CreateRect("Handle Slide Area", root);
        Stretch(handleArea);
        handleArea.offsetMin = new Vector2(8f, 0f);
        handleArea.offsetMax = new Vector2(-8f, 0f);

        RectTransform handle = CreateRect("Handle", handleArea);
        handle.sizeDelta = new Vector2(20f, 30f);
        Image handleImage = handle.gameObject.AddComponent<Image>();
        handleImage.color = new Color(0.12f, 0.16f, 0.13f);

        slider.targetGraphic = handleImage;
        slider.fillRect = fill;
        slider.handleRect = handle;
        slider.direction = Slider.Direction.LeftToRight;
        return slider;
    }

    private static Toggle CreateToggle(Transform parent)
    {
        RectTransform root = CreateRect("FullscreenToggle", parent);
        Toggle toggle = root.gameObject.AddComponent<Toggle>();

        RectTransform background = CreateRect("Background", root);
        background.anchorMin = new Vector2(0.5f, 0.5f);
        background.anchorMax = new Vector2(0.5f, 0.5f);
        background.sizeDelta = new Vector2(30f, 30f);
        Image backgroundImage = background.gameObject.AddComponent<Image>();
        backgroundImage.color = Color.white;

        RectTransform checkmark = CreateRect("Checkmark", background);
        checkmark.anchorMin = new Vector2(0.5f, 0.5f);
        checkmark.anchorMax = new Vector2(0.5f, 0.5f);
        checkmark.sizeDelta = new Vector2(18f, 18f);
        Image checkmarkImage = checkmark.gameObject.AddComponent<Image>();
        checkmarkImage.color = new Color(0.26f, 0.54f, 0.36f);

        toggle.targetGraphic = backgroundImage;
        toggle.graphic = checkmarkImage;
        return toggle;
    }

    private static TMP_Dropdown CreateDropdown(Transform parent)
    {
        RectTransform root = CreateRect("QualityDropdown", parent);
        Image rootImage = root.gameObject.AddComponent<Image>();
        rootImage.color = Color.white;

        TMP_Dropdown dropdown = root.gameObject.AddComponent<TMP_Dropdown>();
        TextMeshProUGUI caption = CreateText("Caption Text", root, string.Empty, 20f, FontStyles.Normal);
        caption.color = new Color(0.15f, 0.18f, 0.16f);
        Stretch(caption.rectTransform);
        caption.rectTransform.offsetMin = new Vector2(12f, 0f);
        caption.rectTransform.offsetMax = new Vector2(-36f, 0f);

        TextMeshProUGUI arrowLabel = CreateText("Arrow", root, "v", 20f, FontStyles.Bold);
        arrowLabel.alignment = TextAlignmentOptions.Center;
        arrowLabel.color = new Color(0.15f, 0.18f, 0.16f);
        RectTransform arrow = arrowLabel.rectTransform;
        arrow.anchorMin = new Vector2(1f, 0.5f);
        arrow.anchorMax = new Vector2(1f, 0.5f);
        arrow.anchoredPosition = new Vector2(-18f, 0f);
        arrow.sizeDelta = new Vector2(18f, 18f);

        RectTransform template = CreateDropdownTemplate(root);
        TextMeshProUGUI itemText = template.Find("Viewport/Content/Item/Item Label").GetComponent<TextMeshProUGUI>();
        dropdown.targetGraphic = rootImage;
        dropdown.captionText = caption;
        dropdown.itemText = itemText;
        dropdown.template = template;
        template.gameObject.SetActive(false);
        return dropdown;
    }

    private static RectTransform CreateDropdownTemplate(Transform parent)
    {
        RectTransform template = CreateRect("Template", parent);
        template.anchorMin = new Vector2(0f, 0f);
        template.anchorMax = new Vector2(1f, 0f);
        template.pivot = new Vector2(0.5f, 1f);
        template.anchoredPosition = new Vector2(0f, -4f);
        template.sizeDelta = new Vector2(0f, 120f);

        Image templateImage = template.gameObject.AddComponent<Image>();
        templateImage.color = new Color(1f, 1f, 1f, 0.98f);
        ScrollRect scrollRect = template.gameObject.AddComponent<ScrollRect>();
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        RectTransform viewport = CreateRect("Viewport", template);
        Stretch(viewport);
        Mask mask = viewport.gameObject.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        Image viewportImage = viewport.gameObject.AddComponent<Image>();
        viewportImage.color = Color.white;

        RectTransform content = CreateRect("Content", viewport);
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(0f, 32f);

        RectTransform item = CreateRect("Item", content);
        item.anchorMin = new Vector2(0f, 1f);
        item.anchorMax = new Vector2(1f, 1f);
        item.pivot = new Vector2(0.5f, 1f);
        item.anchoredPosition = Vector2.zero;
        item.sizeDelta = new Vector2(0f, 32f);
        Toggle itemToggle = item.gameObject.AddComponent<Toggle>();
        Image itemImage = item.gameObject.AddComponent<Image>();
        itemImage.color = Color.white;
        TextMeshProUGUI itemLabel = CreateText("Item Label", item, string.Empty, 18f, FontStyles.Normal);
        itemLabel.color = new Color(0.15f, 0.18f, 0.16f);
        itemLabel.alignment = TextAlignmentOptions.MidlineLeft;
        Stretch(itemLabel.rectTransform);
        itemLabel.rectTransform.offsetMin = new Vector2(12f, 0f);
        itemLabel.rectTransform.offsetMax = new Vector2(-12f, 0f);

        itemToggle.targetGraphic = itemImage;
        scrollRect.content = content;
        scrollRect.viewport = viewport;
        scrollRect.horizontal = false;
        return template;
    }

    private static Button CreateButton(string name, Transform parent, string text)
    {
        RectTransform root = CreateRect(name, parent);
        Image image = root.gameObject.AddComponent<Image>();
        image.color = Color.white;

        Button button = root.gameObject.AddComponent<Button>();
        button.targetGraphic = image;

        TextMeshProUGUI label = CreateText("Text", root, text, 28f, FontStyles.Bold);
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.15f, 0.18f, 0.16f);
        Stretch(label.rectTransform);
        return button;
    }

    private static void AddLayout(GameObject go, float preferredWidth, float preferredHeight)
    {
        LayoutElement layout = go.GetComponent<LayoutElement>();
        if (layout == null)
            layout = go.AddComponent<LayoutElement>();

        if (preferredWidth > 0f)
            layout.preferredWidth = preferredWidth;

        if (preferredHeight > 0f)
            layout.preferredHeight = preferredHeight;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
