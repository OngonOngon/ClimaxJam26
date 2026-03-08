Shader "Custom/PaletteSwapURP"
{
    Properties
    {
        [HideInInspector] _BlitTexture("Source", 2D) = "white" {}
        _PaletteTex ("Palette Texture", 2D) = "white" {}
        _ColorCount ("Number of Colors", Float) = 16
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off ZTest Always Blend Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _BlitTexture;
            sampler2D _PaletteTex;
            float _ColorCount;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. Získání barvy ze scény
                half4 sceneColor = tex2D(_BlitTexture, IN.uv);

                // 2. Výpočet jasu (luminance)
                float luminance = dot(sceneColor.rgb, float3(0.2126, 0.7152, 0.0722));

                // 3. Posterizace (rozdělení jasu na fixní počet kroků)
                // Přidáváme 0.5 pro samplování středu pixelu v paletě
                float steppedLuminance = (floor(saturate(luminance) * _ColorCount) + 0.5) / _ColorCount;

                // 4. Výběr barvy z palety (X = jas, Y = střed textury)
                float2 paletteUV = float2(steppedLuminance, 0.5);
                half4 finalColor = tex2D(_PaletteTex, paletteUV);

                return finalColor;
            }
            ENDHLSL
        }
    }
}