Shader "Hidden/FutureColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorSpeed ("Color Switching Speed", Float) = 2.0
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
            float4 _MainTex_TexelSize;

            float _ColorSpeed = 1.;

            void mainImage(out float4 fragColor, in float2 uv)
            {
                float iTime = _Time.y;

                float4 edge = float4(0.0, 0.0, 0.0, 1.0);
                float4 col = tex2D(_MainTex, uv);

                // top
                edge += tex2D(_MainTex, uv + float2(0.0,  _MainTex_TexelSize.y)) * -1.0;

                // left
                edge += tex2D(_MainTex, uv + float2(-_MainTex_TexelSize.x, 0.0)) * -1.0;

                // center
                edge += col * 4.1 + (0.0025 * sin(_Time.y * 8.0) * cos(_Time.y * 32.0));

                // right
                edge += tex2D(_MainTex, uv + float2(_MainTex_TexelSize.x, 0.0)) * -1.0;

                // bottom
                edge += tex2D(_MainTex, uv + float2(0.0, -_MainTex_TexelSize.y)) * -1.0;
                
                float colMin = min(min(col.r, col.g), col.b);
                float colMax = max(max(col.r, col.g), col.b);
                float colDelta = (colMax - colMin);
                
                float H = .0;
                float S = .0;
                float V = colMax;

                if (colDelta == 0.0)
                {
                    H = 0.0;
                }
                else
                {
                    S = colDelta / colMax;
                
                    if(colMax == col.r)
                    {
                        H = 60. * ( 0. + (col.g - col.b) / colDelta );
                    }
                    else if(colMax == col.g)
                    {
                        H = 60. * ( 2. + (col.b - col.r) / colDelta );
                    }
                    else//if(colMax == col.b)
                    {
                        H = 60. * ( 4. + (col.r - col.g) / colDelta );
                    }
                    if (H < 0.0)
                        H += 360.0;
                }
                
                float oH = H;
                H += iTime * _ColorSpeed * 32.;
                H += sin(iTime) * _ColorSpeed * 60.;
                if(abs(H - oH) <= 60.)
                {
                    H += 120.;
                }
                
                H = fmod(H, 360.);
                
                float R = .0;
                float G = .0;
                float B = .0;
                
                if(H < 60.)
                {
                    R = 1.0;
                    G = H/60.;
                }
                else if(H < 120.)
                {
                    R = 1.-((H-60.)/60.);
                    G = 1.0;
                }
                else if(H < 180.)
                {
                    G = 1.0;
                    B = (H-120.)/60.;
                }
                else if(H < 240.)
                {
                    G = 1.-((H-180.)/60.);
                    B = 1.;
                }
                else if(H < 300.)
                {
                    R = (H-240.)/60.;
                    B = 1.;
                }
                else if(H < 360.)
                {
                    R = 1.0;
                    B = 1.-((H-300.)/60.);
                }
                
                R = lerp(1., R, S);
                G = lerp(1., G, S);
                B = lerp(1., B, S);
                R *= V;
                G *= V;
                B *= V;
                
                fragColor = float4(R, G, B, 1.) + (float4(H, S, V, 1.) * .005 * sin(iTime)) + max(edge, float4(.0, .0, .0, .0)) *  3.;
                //fragColor = float4(H, S, V, 1.);
                //fragColor = col;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = float4(0.,0.,0.,1.0);
                mainImage(col, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
