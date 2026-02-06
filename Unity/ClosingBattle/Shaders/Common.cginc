float hash(float2 p)
{
    p = frac(p * float2(123.34, 456.21));
    p += dot(p, p + 78.233);
    return frac(p.x * p.y);
}
float hash(float p)
{
    return hash(float2(p,p));
}

float2 hash2(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.xx + p3.yz) * p3.zy);
}

float posterize(float n, float l)
{
    return floor(n * l)/l;
}

float3 cabberationEffect(float offsetMod, float2 uv, float iTime, sampler2D tex)
{
    float offset = 1.+(hash(iTime)*0.25);
    offset *= offsetMod;
    float r = tex2D(tex, uv - float2(.01,.01)*offset).r;
    float g = tex2D(tex, uv - float2(.001,.01)*offset).g;
    float b = tex2D(tex, uv + float2(.01,.01)*offset).b;

    return float3(r,g,b);
}

float3 flashbangEffect(float2 uv, float iTime, sampler2D tex)
{
    float t = min(iTime, 0.6);
    float localT = t/0.6;

    float4 color = float4(cabberationEffect(1.-localT, uv, iTime, tex), 1.0);
    color = lerp(float4(color.b, color.g, color.r, color.a), color, localT);
    color = lerp(float4(1,1,1,1), color, min(1, localT + 0.5));
    
    return color.rgb;
}