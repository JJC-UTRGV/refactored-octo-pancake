Shader "MiniGolf/Skybox Clouds"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.30, 0.58, 0.95, 1)
        _HorizonColor ("Horizon Color", Color) = (0.80, 0.92, 1.00, 1)
        _CloudColor ("Cloud Color", Color) = (1.00, 1.00, 1.00, 1)
        _CloudShadowColor ("Cloud Shadow Color", Color) = (0.63, 0.73, 0.84, 1)
        _CloudCoverage ("Cloud Coverage", Range(0, 1)) = 0.42
        _CloudSoftness ("Cloud Softness", Range(0.01, 0.5)) = 0.16
        _CloudScale ("Cloud Scale", Range(0.5, 8)) = 2.4
        _CloudHeight ("Cloud Height", Range(0, 1)) = 0.22
        _CloudDrift ("Cloud Drift", Range(0, 1)) = 0.06
        _CloudDirection ("Cloud Direction", Vector) = (1, 0.25, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "PreviewType" = "Skybox"
        }

        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 direction : TEXCOORD0;
            };

            fixed4 _TopColor;
            fixed4 _HorizonColor;
            fixed4 _CloudColor;
            fixed4 _CloudShadowColor;
            float _CloudCoverage;
            float _CloudSoftness;
            float _CloudScale;
            float _CloudHeight;
            float _CloudDrift;
            float4 _CloudDirection;

            float hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float noise(float2 p)
            {
                float2 cell = floor(p);
                float2 local = frac(p);
                local = local * local * (3.0 - 2.0 * local);

                float a = hash(cell);
                float b = hash(cell + float2(1.0, 0.0));
                float c = hash(cell + float2(0.0, 1.0));
                float d = hash(cell + float2(1.0, 1.0));

                return lerp(lerp(a, b, local.x), lerp(c, d, local.x), local.y);
            }

            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;

                [unroll]
                for (int octave = 0; octave < 5; octave++)
                {
                    value += amplitude * noise(p);
                    p = p * 2.03 + 17.17;
                    amplitude *= 0.5;
                }

                return value;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.direction = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 direction = normalize(i.direction);
                float skyHeight = saturate(direction.y * 0.5 + 0.5);
                float3 sky = lerp(_HorizonColor.rgb, _TopColor.rgb, pow(skyHeight, 0.7));

                float cloudFade = smoothstep(-0.05, 0.22, direction.y) * (1.0 - smoothstep(0.88, 1.0, direction.y));
                float planeHeight = max(0.12, direction.y + _CloudHeight);
                float2 cloudUv = direction.xz / planeHeight;
                float2 windDirection = normalize(_CloudDirection.xy + float2(0.0001, 0.0));
                cloudUv = cloudUv * _CloudScale + windDirection * _Time.y * _CloudDrift;

                float cloudField = fbm(cloudUv);
                float cloudMask = smoothstep(_CloudCoverage, _CloudCoverage + _CloudSoftness, cloudField) * cloudFade;
                float cloudHighlight = smoothstep(_CloudCoverage, 1.0, cloudField);
                float3 cloudColor = lerp(_CloudShadowColor.rgb, _CloudColor.rgb, cloudHighlight);

                sky = lerp(sky, cloudColor, saturate(cloudMask * 0.9));
                sky += _HorizonColor.rgb * smoothstep(0.0, 0.08, direction.y) * 0.06;

                return fixed4(sky, 1.0);
            }
            ENDCG
        }
    }

    FallBack Off
}
