Shader "Custom/SemiTrasParentCube"
{
    Properties {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _Intensity ("Intensity", Range(0, 5)) = 1
        _Softness ("Softness", Range(0, 1)) = 0.5
    }
 
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
 
        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };
 
        float4 _MainColor;
        float _Intensity;
        float _Softness;
 
        void surf (Input IN, inout SurfaceOutputStandard o) {
            float distance = IN.worldPos.y;
            float alpha = pow(1.0 - saturate((distance - 0.1) / (1.0 - 0.1)), 1.0 / _Softness);
            o.Emission = _MainColor.rgb * alpha * _Intensity;
            o.Alpha = alpha;
        }
        ENDCG
    }
 
    FallBack "Diffuse"
}