Shader "Hidden/SlashShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _SlashSide ("Slash Side", Integer) = 0
        _SlashOffset ("Slash Offset", Float) = 0.125
        _SlashX ("Slash Point", Float) = 0.5
        
        _TimeStart ("[SCRIPT] Time Start", Float) = 0.0
        _SlashSin ("[SCRIPT] Slash Angle Sin", Float) = 0.0
        _SlashCos ("[SCRIPT] Slash Angle Cos", Float) = 0.0
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
            
            int _SlashSide;
            fixed _SlashOffset;
            fixed _SlashX;
            float _SlashSin;
            float _SlashCos;
            
            #include "Common.cginc"

            float2 rotateUV(float2 uv)
            {
                float slashPoint = _SlashX;
                float s = _SlashSin;
                float c = _SlashCos;
                float yOff = .5;

                float2 uv_rotated = uv;
                uv_rotated.x -= slashPoint;
                uv_rotated.y -= yOff;
                            
                uv_rotated.x = uv_rotated.x * c - uv_rotated.y * s;
                uv_rotated.x += slashPoint;
                uv_rotated.y += yOff;
                return uv_rotated;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                //float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                //return col

                //return fixed4(i.uv.x, i.uv.y, 0, 1);
                
                float4 fragColor = float4(1,1,1,1);
                float iTime = _Time.y - _TimeStart;
                
                float slashPoint = _SlashX;
                float slashGap = 0.005;
                float whiteGapMult = 2.5; // 1 = no white
                
                // timing funcs
                float animTime = 0.5;
                float t = min(animTime, iTime * 2);
                float progress = t/animTime;
                float progressFirstHalf = max((t * .5)/(animTime*.5), .0);
                float progressLastHalf = max(max(0., t - animTime*.5)/(animTime*.5), .0);
                
                // calculate uv and screenshake
                float2 uv = i.uv;
                uv += float2(hash(posterize(iTime, 100.)), hash(-posterize(iTime, 100.))) * (1. - progressFirstHalf) * .125;
                
                // rotate uv to for angled slash
                float2 uv_rotated = rotateUV(uv);
                
                // white border for slash
                float whiteOverlay = .0;
                float tmpGap = slashGap * progress;
                if(uv_rotated.x > slashPoint + tmpGap && uv_rotated.x < slashPoint + tmpGap * whiteGapMult)
                {
                    // distance from start of fade
                    float x1 = uv_rotated.x;
                    float start = slashPoint + tmpGap;
                    float end   = slashPoint + tmpGap * whiteGapMult;

                    // smooth fade from 0→1 over the range
                    whiteOverlay = 0.5 * smoothstep(end, start, x1);
                }
                if(uv_rotated.x < slashPoint - tmpGap && uv_rotated.x > slashPoint - tmpGap * whiteGapMult)
                {
                    // distance from start of fade
                    float x1 = uv_rotated.x;
                    float start = slashPoint - tmpGap;
                    float end   = slashPoint - tmpGap * whiteGapMult;

                    // smooth fade from 0→1 over the range
                    whiteOverlay = 0.5 * smoothstep(end, start, x1);
                }
                
                // screen offset
                if (_SlashSide < 0)
                {
                    if(uv_rotated.x < slashPoint + tmpGap)
                    {
                        uv.y -= _SlashOffset * progressFirstHalf;
                    }
                }else
                {
                    if(uv_rotated.x > slashPoint + tmpGap)
                    {
                        uv.y -= _SlashOffset * progressFirstHalf;
                    }
                }
                // slash
                if(uv_rotated.x < slashPoint + tmpGap && uv_rotated.x > slashPoint - tmpGap)
                {
                    fragColor = float4(0., 0., 0., 0.);
                    return fragColor;
                }
                
                float4 col = float4(flashbangEffect(uv, t, _MainTex), 1.0);
                
                fragColor = col + float4(float3(1., 1., 1.) * whiteOverlay, 1.0);//float4(float3(uv,1.0),1.0);

                return fragColor;
            }
            ENDCG
        }
    }
}
