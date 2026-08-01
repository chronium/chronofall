struct FragmentInput
{
    float4 Position : SV_Position;
    float3 Normal : TEXCOORD0;
};

cbuffer SurfaceConstants : register(b0, space3)
{
    float4 BaseColor;
    float4 LightDirection;
};

float4 main(FragmentInput input) : SV_Target0
{
    float diffuse = 0.30 + 0.70 * saturate(dot(normalize(input.Normal), normalize(-LightDirection.xyz)));
    return float4(BaseColor.rgb * diffuse, BaseColor.a);
}
