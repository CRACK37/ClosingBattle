Shader "Hidden/WorldWarp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _TimeStart ("[SCRIPT] Time Start", Float) = 0.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _TimeStart;
            
            #include "Common.cginc"

            fixed4 frag (v2f i) : SV_Target
            {
                float iTime = _Time.y - _TimeStart;
                float2 uv = i.uv;
                float2 center = float2(tanh(radians(uv.x*(45. + 29.4))) - 0.25, 0.35);
                float2 toCenter = uv - center;
                float dist = length(toCenter);
                float distToRealCenter = length(uv - float2(0.5, 0.5));

                const float maxTime = 2.5;
                float t = min(iTime, maxTime);
                
                float2 originalUV = uv;
                uv *= (dist * 4.);
                uv.x *= .5;

                float lerpT = 1.-clamp((t/maxTime - 0.001) * maxTime * maxTime * maxTime * maxTime, 0., 1.);
                uv = lerp(uv, originalUV, lerpT);
                
                uv.x = (uv.x % 1.);
                uv.y = (uv.y % 1.);
                
                float4 col = float4(cabberationEffect(lerpT * 10., uv, iTime, _MainTex), 1.);
                col = lerp(col, float4(1,1,1,1), (1-clamp(distToRealCenter, 0.125, 1)) * (lerpT));
                
                return col;
            }
            ENDCG
        }
    }
}
