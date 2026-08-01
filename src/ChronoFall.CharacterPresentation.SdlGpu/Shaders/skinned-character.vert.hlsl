struct VertexInput
{
    float3 Position : POSITION0;
    float3 Normal : TEXCOORD0;
    uint4 Joints : TEXCOORD1;
    float4 Weights : TEXCOORD2;
};

struct VertexOutput
{
    float4 Position : SV_Position;
    float3 Normal : TEXCOORD0;
};

StructuredBuffer<float4x4> JointPalette : register(t0, space0);

cbuffer CameraConstants : register(b0, space1)
{
    float4x4 ViewProjection;
};

VertexOutput main(VertexInput input)
{
    float4x4 skin =
        JointPalette[input.Joints.x] * input.Weights.x +
        JointPalette[input.Joints.y] * input.Weights.y +
        JointPalette[input.Joints.z] * input.Weights.z +
        JointPalette[input.Joints.w] * input.Weights.w;

    VertexOutput output;
    float4 skinnedPosition = mul(float4(input.Position, 1.0), skin);
    output.Position = mul(skinnedPosition, ViewProjection);
    output.Normal = normalize(mul(float4(input.Normal, 0.0), skin).xyz);
    return output;
}
