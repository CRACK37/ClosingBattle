Shader "ClosingBattle/Flashbang"
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

            #include "Common.cginc"
            
            sampler2D _MainTex;
            float _TimeStart;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float iTime = _Time.y - _TimeStart;
                fixed4 col = fixed4(flashbangEffect(i.uv, iTime, _MainTex), 1.0);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                
                return col;
            }
            ENDCG
        }
    }
}
