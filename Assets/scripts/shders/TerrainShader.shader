Shader "Custom/TerrainShader"
{
     Properties
    {
        _minY    ("Min Height", Float) = 0
        _maxY    ("Max Height", Float) = 1

        _Color0  ("Color 0", Color) = (0,0,0,1)
        _Color1  ("Color 1", Color) = (0.1,0.1,0.3,1)
        _Color2  ("Color 2", Color) = (0.2,0.4,0.2,1)
        _Color3  ("Color 3", Color) = (0.4,0.3,0.1,1)
        _Color4  ("Color 4", Color) = (0.6,0.5,0.2,1)
        _Color5  ("Color 5", Color) = (0.7,0.6,0.3,1)
        _Color6  ("Color 6", Color) = (0.8,0.7,0.4,1)
        _Color7  ("Color 7", Color) = (0.9,0.8,0.5,1)
        _Color8  ("Color 8", Color) = (1.0,0.9,0.7,1)
        _Color9  ("Color 9", Color) = (1.0,1.0,1.0,1)
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
            // URP Core include для TransformObjectToHClip/World и проч.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Вершинный и пиксельный шейдеры
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            // Вход вершин
            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            // Передаём в фрагмент
            struct Varyings
            {
                float4 posHCS   : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            // Свойства материала
            CBUFFER_START(UnityPerMaterial)
                float _minY;
                float _maxY;
                float4 _Color0;
                float4 _Color1;
                float4 _Color2;
                float4 _Color3;
                float4 _Color4;
                float4 _Color5;
                float4 _Color6;
                float4 _Color7;
                float4 _Color8;
                float4 _Color9;
            CBUFFER_END

            // Вершинный шейдер: считаем SV_POSITION и WorldPos
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // из Core.hlsl:
                OUT.posHCS   = TransformObjectToHClip(IN.positionOS);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS);
                return OUT;
            }

            // Пиксельный шейдер
            half4 frag(Varyings IN) : SV_Target
            {
                // нормализуем высоту в [0..1]
                float t = saturate((IN.worldPos.y - _minY) / (_maxY - _minY));

                // число цветовых остановок
                const int levels = 10;
                // масштабируем в [0..levels-1]
                float scaled = t * (levels - 1);
                int idx  = (int)floor(scaled);
                float frac = scaled - idx;
                idx = clamp(idx, 0, levels - 1);
                int next = min(idx + 1, levels - 1);

                // собираем массив цветов
                float4 colors[levels] = {
                    _Color0, _Color1, _Color2, _Color3, _Color4,
                    _Color5, _Color6, _Color7, _Color8, _Color9
                };

                // плавно интерполируем между colors[idx] и colors[next]
                float3 col = lerp(colors[idx].rgb, colors[next].rgb, frac);

                return float4(col, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
