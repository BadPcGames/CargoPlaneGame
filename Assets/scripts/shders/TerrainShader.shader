Shader "Custom/TerrainGradient"
{
    Properties
    {
        _TerrainGradient ("Terrain Gradient", 2D) = "white" {}
        _MinTerrainHeight ("Min Height", Float) = 0
        _MaxTerrainHeight ("Max Height", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _TerrainGradient;
        float _MinTerrainHeight;
        float _MaxTerrainHeight;

        struct Input
        {
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float heightNorm = saturate(
                (IN.worldPos.y - _MinTerrainHeight) / 
                max(0.0001, (_MaxTerrainHeight - _MinTerrainHeight))
            );

            float2 uv = float2(heightNorm, 0.5);
            fixed4 col = tex2D(_TerrainGradient, uv);

            o.Albedo = col.rgb;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
