Shader "Custom/TerrainShaderWithTexture"
{
      Properties
    {
        _minY       ("Min Height", Float) = 0
        _maxY       ("Max Height", Float) = 1
        _LayerCount ("Number of Layers", Int) = 10

        _Tex0 ("Texture 0", 2D) = "white" {}
        _Tex1 ("Texture 1", 2D) = "white" {}
        _Tex2 ("Texture 2", 2D) = "white" {}
        _Tex3 ("Texture 3", 2D) = "white" {}
        _Tex4 ("Texture 4", 2D) = "white" {}
        _Tex5 ("Texture 5", 2D) = "white" {}
        _Tex6 ("Texture 6", 2D) = "white" {}
        _Tex7 ("Texture 7", 2D) = "white" {}
        _Tex8 ("Texture 8", 2D) = "white" {}
        _Tex9 ("Texture 9", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 posHCS   : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float _minY;
                float _maxY;
                int _LayerCount;
            CBUFFER_END

            TEXTURE2D(_Tex0); SAMPLER(sampler_Tex0);
            TEXTURE2D(_Tex1); SAMPLER(sampler_Tex1);
            TEXTURE2D(_Tex2); SAMPLER(sampler_Tex2);
            TEXTURE2D(_Tex3); SAMPLER(sampler_Tex3);
            TEXTURE2D(_Tex4); SAMPLER(sampler_Tex4);
            TEXTURE2D(_Tex5); SAMPLER(sampler_Tex5);
            TEXTURE2D(_Tex6); SAMPLER(sampler_Tex6);
            TEXTURE2D(_Tex7); SAMPLER(sampler_Tex7);
            TEXTURE2D(_Tex8); SAMPLER(sampler_Tex8);
            TEXTURE2D(_Tex9); SAMPLER(sampler_Tex9);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.posHCS = TransformObjectToHClip(IN.positionOS);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS);
                return OUT;
            }

            float4 SampleTexture(int index, float2 uv)
            {
                if (index == 0) return SAMPLE_TEXTURE2D(_Tex0, sampler_Tex0, uv);
                if (index == 1) return SAMPLE_TEXTURE2D(_Tex1, sampler_Tex1, uv);
                if (index == 2) return SAMPLE_TEXTURE2D(_Tex2, sampler_Tex2, uv);
                if (index == 3) return SAMPLE_TEXTURE2D(_Tex3, sampler_Tex3, uv);
                if (index == 4) return SAMPLE_TEXTURE2D(_Tex4, sampler_Tex4, uv);
                if (index == 5) return SAMPLE_TEXTURE2D(_Tex5, sampler_Tex5, uv);
                if (index == 6) return SAMPLE_TEXTURE2D(_Tex6, sampler_Tex6, uv);
                if (index == 7) return SAMPLE_TEXTURE2D(_Tex7, sampler_Tex7, uv);
                if (index == 8) return SAMPLE_TEXTURE2D(_Tex8, sampler_Tex8, uv);
                if (index == 9) return SAMPLE_TEXTURE2D(_Tex9, sampler_Tex9, uv);
                return float4(1, 0, 1, 1); // fallback magenta
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float t = saturate((IN.worldPos.y - _minY) / (_maxY - _minY));
                int levels = clamp(_LayerCount, 2, 10);

                float scaled = t * (levels - 1);
                int idx = (int)floor(scaled);
                float frac = scaled - idx;
                idx = clamp(idx, 0, levels - 1);
                int next = min(idx + 1, levels - 1);

                float2 uv = IN.worldPos.xz * 0.1; // UV-проекция по XZ (можно заменить)

                float4 tex0 = SampleTexture(idx, uv);
                float4 tex1 = SampleTexture(next, uv);

                float4 finalColor = lerp(tex0, tex1, frac);
                return finalColor;
            }

            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
