Shader "Custom/URP_CenterGlow"
{
    Properties
    {
        _GlowColor ("Glow Color", Color) = (1, 1, 0, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 1.0
        _GlowRadius ("Glow Radius", Range(0.1, 3.0)) = 1.0
        _GlowPosition ("Glow Position", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"}
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "GLOW"
            Tags { "LightMode"="UniversalForward" }
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowRadius;
            float4 _GlowPosition;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS);
                OUT.positionHCS = posInputs.positionCS;
                OUT.worldPos = posInputs.positionWS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float distanceToCenter = distance(IN.worldPos.xyz, _GlowPosition.xyz);
                float glowFactor = exp(-distanceToCenter * _GlowRadius) * _GlowIntensity;

                return half4(_GlowColor.rgb * glowFactor, glowFactor);
            }
            ENDHLSL
        }
    }
}
