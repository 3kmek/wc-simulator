Shader "Custom/SoftOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.05)) = 0.01
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Front  // İç yüzeyleri kapat, sadece dış hatları göster
        ZWrite On

        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS);

                // Daha yumuşak normal hesaplaması
                float3 normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS) + float3(0.02, 0.02, 0.02));

                // Hafif dalgalı ve yumuşak bir outline için interpolasyon
                posInputs.positionWS += normalWS * (_OutlineWidth * (0.8 + 0.2 * sin(_Time.y * 1.5)));

                OUT.positionHCS = TransformWorldToHClip(posInputs.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
