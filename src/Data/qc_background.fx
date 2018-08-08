Texture2D Texture;
SamplerState TextureSampler;

cbuffer cbPerFrame 
{
	float4x4 WvpTransform;
	float4x4 TextureTransform;
	float4 BackgroundColor;
};

struct VertexIn
{
	float3 Position : SV_POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexOut
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexOut VS(VertexIn vin)
{
	VertexOut vout;

	vout.Position = mul(float4(vin.Position.x, vin.Position.y, 0.0f, 1.0f), WvpTransform);
	vout.TexCoord = mul(float4(vin.TexCoord.x, vin.TexCoord.y, 0.0f, 1.0f), TextureTransform).xy;

	return vout;
}

float4 PS(VertexOut pin) : SV_TARGET
{
	return BackgroundColor * Texture.Sample(TextureSampler, pin.TexCoord);	
}

technique Main
{
	pass P0
	{
#ifdef SM4 
		VertexShader = compile vs_4_0_level_9_1 VS();
		PixelShader = compile ps_4_0_level_9_1 PS();
#else
		VertexShader = compile vs_2_0 VS();
		PixelShader = compile ps_2_0 PS();
#endif
	}
}
