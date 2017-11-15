#ifndef CETO_MASKING_INCLUDED
#define CETO_MASKING_INCLUDED

float2 IsUnderWater(float2 uv)
{

	float4 mask = tex2D(Ceto_OceanMask, uv);

	float error = 0.01;

	if (mask.x <= TOP_MASK + error)
		mask.x = 0.0;
	else
		mask.x = 1.0;
	
	return mask.xy;
	
}
	



#endif