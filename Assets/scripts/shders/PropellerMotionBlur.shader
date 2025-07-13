Shader "Custom/PropellerRadialBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurStrength ("Blur Strength", Range(0, 1)) = 0.5
        _Samples ("Blur Samples", Range(1, 64)) = 32
        _Center ("Blur Center", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurStrength;
            int _Samples;
            float2 _Center;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 dir = i.uv - _Center;
                float2 normDir = normalize(dir);
                float len = length(dir);

                fixed4 col = tex2D(_MainTex, i.uv);
                for (int s = 1; s < _Samples; ++s)
                {
                    float t = (float)s / (_Samples - 1);
                    float angle = t * _BlurStrength * 6.2831853; // 2π
                    float2 rot = float2(
                        cos(angle) * dir.x - sin(angle) * dir.y,
                        sin(angle) * dir.x + cos(angle) * dir.y
                    );
                    col += tex2D(_MainTex, _Center + rot);
                }

                col /= _Samples;
                return col;
            }
            ENDCG
        }
    }
}