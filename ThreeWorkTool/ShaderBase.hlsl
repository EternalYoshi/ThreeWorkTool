//Barebones Vertex Shader.
cbuffer MatrixBuffer : register(b0)
{
	matrix world;
	matrix view;
	matrix projection;
};

struct VS_INPUT
{
	float3 pos : POSITION;
	float4 color : COLOR;
};

struct PS_INPUT
{
	float4 pos : SV_POSITION;
	float4 color : COLOR;
};

PS_INPUT VS(VS_INPUT input)
{
	PS_INPUT output;
	float4 worldPos = mul(float4(input.pos, 1.0f), world);
	float4 viewPos = mul(worldPos, view);
	output.pos = mul(viewPos, projection);
	output.color = input.color;
	return output;
}

//Barebones Pixel Shader.
float4 PS(PS_INPUT input) : SV_TARGET
{
	return input.color;
}