#ifndef CETO_UNDERWATER_INCLUDED
#define CETO_UNDERWATER_INCLUDED


/*
* Applies the foam to the ocean color.
* Spectrum foam in x, overlay foam in y.
*/
fixed3 OceanWithFoamColor(float3 worldPos, fixed3 oceanCol, fixed4 foam)
{

	//foam.x == the wave (spectrum) foam.
	//foam.y == the overlay foam with foam texture.
	//foam.z == the overlay foam with no foam texture.

	fixed foamTexture = 0.0;

	#ifndef CETO_DISABLE_FOAM_TEXTURE
   		foamTexture += tex2D(Ceto_FoamTexture0, (worldPos.xz + Ceto_FoamTextureScale0.z) * Ceto_FoamTextureScale0.xy).a * 0.5;
		foamTexture += tex2D(Ceto_FoamTexture1, (worldPos.xz + Ceto_FoamTextureScale1.z) * Ceto_FoamTextureScale1.xy).a * 0.5;
	#else
		foamTexture = 1.0;
	#endif

	//Apply texture to the wave foam if that option is enabled.
    foam.x = lerp(foam.x, foam.x * foamTexture, Ceto_TextureWaveFoam);
	//Apply texture to overlay foam
   	foam.y = foam.y * foamTexture;
   
   	fixed foamAmount = foam.x + foam.y + foam.z;

	//apply the absorption coefficient to the foam based on the foam strength.
	//This will fade the foam add make it look like it has some depth and
	//since it uses the abs cof the color should match the water.
	fixed3 foamCol = Ceto_FoamTint * foamAmount * exp(-Ceto_AbsCof.rgb * (1.0-foamAmount) * 1.0);
	
	//TODO - find better way than lerp to blend.
	return lerp(oceanCol, foamCol, foamAmount);
	
}

/*
* Calculate a subsurface scatter color based on the view, normal and sun dir.
* NOTE - you have to add your directional light onto the ocean component for
* the sun direction to be used. A default sun dir of up is used otherwise.
*/
fixed3 SubSurfaceScatter(fixed3 V, fixed3 N, float surfaceDepth)
{

	fixed3 col = fixed3(0,0,0);

	#ifdef CETO_UNDERWATER_ON

		//The strength based on the view and up direction.
		fixed VU = 1.0 -  max(0.0, dot(V, fixed3(0,1,0)));
		VU *= VU;
		
		//The strength based on the view and sun direction.
		fixed VS = max(0, dot(reflect(V, fixed3(0,1,0)) * -1.0, Ceto_SunDir));
		VS *= VS;
		VS *= VS;
		
		float NX =  abs(dot(N, fixed3(1,0,0)));
		
		fixed s = NX * VU * 0.2 + NX * VU * VS;
		
		//apply a non linear fade to distance.
		fixed d = max(0.2, exp(-max(0.0, surfaceDepth)));

		//Apply the absorption coefficient base on the distance and tint final color.
		col = Ceto_SSSTint * exp(-Ceto_SSSCof.rgb * d * Ceto_SSSCof.a) * s;
		
	#endif
	
	return col;

}

/*
* Calculates the world position from the depth buffer value.
*/
float3 WorldPosFromDepth(float2 uv, float depth)
{

	float4 ndc = float4(uv.x * 2.0 - 1.0, uv.y * 2.0 - 1.0, depth * 2.0 - 1.0, 1);
	
	float4 worldPos = mul(Ceto_Camera_IVP, ndc);
	worldPos /= worldPos.w;

	return worldPos.xyz;
}

/*
* Samples the depth buffer with a distortion to the uv.
*/
float SampleDepthBuffer(float2 screenUV, half3 distortion)
{

	screenUV += distortion.xz;
	screenUV = saturate(screenUV);
	
	//float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV).x;	
	float depth = tex2D(Ceto_DepthBuffer, screenUV).x;

	return Linear01Depth(depth);
}

/*
* Returns the depth info needed to apply the underwater effect 
* calculated from the depth buffer. If a object does not write 
* into the depth buffer it will not show up.
*/
float4 SampleOceanDepthFromDepthBuffer(float2 screenUV, half3 distortion)
{

	screenUV += distortion.xz;
	screenUV = saturate(screenUV);
	
	//float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV).x
	float depth = tex2D(Ceto_DepthBuffer, screenUV).x;

	float3 worldPos = WorldPosFromDepth(screenUV, depth);

	float4 oceanDepth = float4(0,0,0,0);

	float ld = Linear01Depth(depth);
	
	oceanDepth.x = (worldPos.y-Ceto_OceanLevel) * -1.0;
	oceanDepth.y = ld * _ProjectionParams.z / Ceto_MaxDepthDist;
	oceanDepth.z = ld;
	oceanDepth.w = 0;
	
	return oceanDepth;
}

/*
* Returns the depth info needed to apply the underwater effect 
* from the ocean depths buffer. This is done by rendering the objects 
* in the ocean depth layer using a replacement shader. This is needed
* when the ocean is in the opaque queue. You can not get the info from 
* the depth buffer as Unity does a depth only pass the ocean mesh will
* have overridden the objects depth values that are below the mesh.
* If a object does not have its render type in the OceanDepths shader or
* its layer is not selected it will not show up.
*/
float4 SampleOceanDepth(float2 screenUV, half3 distortion)
{

	screenUV += distortion.xz;
	screenUV = saturate(screenUV);

	float4 oceanDepth = tex2D(Ceto_OceanDepth, screenUV);

	float ld = oceanDepth.y;

	//unnormalize.
	oceanDepth.x *= Ceto_MaxDepthDist;
	oceanDepth.y = ld * _ProjectionParams.z / Ceto_MaxDepthDist;
	oceanDepth.z = ld;
	oceanDepth.w = 0;

	return oceanDepth;

}

/*
* This is the ocean depth info that is written using the ocean
* depth replacement shader. X is relative to ocean level to conserve
* precision in the half format used for buffer.
* x = world y pos relative to ocean level normalized to ocean depth and up flipped.
* y = linear depth value.
*/
#define COMPUTE_OCEAN_DEPTH_PARAMETERS \
	o.depth = float4(0,0,0,0);\
	o.depth.x = (worldPos.y-Ceto_OceanLevel) * -1.0 / Ceto_MaxDepthDist;\
	o.depth.y = COMPUTE_DEPTH_01;\

/*
* Samples the refraction grab texture with a distortion to the uv.
*/
float4 SampleRefractionGrab(float2 grabUV, half3 distortion)
{
	grabUV += distortion.xz;
	grabUV = saturate(grabUV);

	return tex2D(Ceto_RefractionGrab, grabUV);
}

/*
* Computes the depth value used to apply the underwater effect.
*/
float2 OceanDepth(float2 screenUV, half3 distortion, float3 worldPos, float depth)
{

	float2 surfaceDepth;
	surfaceDepth.x = (worldPos.y-Ceto_OceanLevel) * -1.0;
	surfaceDepth.y = depth * _ProjectionParams.z / Ceto_MaxDepthDist;
	
	#ifdef CETO_USE_OCEAN_DEPTHS_BUFFER
		float2 oceanDepth = SampleOceanDepth(screenUV, distortion).xy;
	#else
		float2 oceanDepth = SampleOceanDepthFromDepthBuffer(screenUV, distortion).xy;
	#endif

	oceanDepth.x = max(0.0, oceanDepth.x - surfaceDepth.x) / Ceto_MaxDepthDist;
	oceanDepth.y = max(0.0, oceanDepth.y - surfaceDepth.y);
	
	return oceanDepth;
	
}

/*
* The refraction color when see from above the ocean mesh.
* Where depth is normalized between 0-1 based on Ceto_MaxDepthDist.
*/
fixed3 AboveRefractionColor(float2 grabUV, half3 distortion, float3 surfacePos, float depth)
{
	
	fixed3 grab = SampleRefractionGrab(grabUV, distortion).rgb * Ceto_RefractionIntensity;
	
	fixed3 col = grab * Ceto_AbsTint * exp(-Ceto_AbsCof.rgb * depth * Ceto_MaxDepthDist * Ceto_AbsCof.a);
	
	return col;
}

/*
* The refraction color when see from below the ocean mesh (under water).
*/
fixed3 BelowRefractionColor(float2 grabUV, half3 distortion)
{

	fixed3 grab = SampleRefractionGrab(grabUV, distortion).rgb * Ceto_RefractionIntensity;
	
	return grab;
}

/*
* The inscatter when seen from above the ocean mesh.
* Where depth is normalized between 0-1 based on Ceto_MaxDepthDist.
*/
fixed3 AddAboveInscatter(fixed3 col, float depth)
{

	//There are 3 methods used to apply the inscatter.
	half3 inscatterScale;
	inscatterScale.x = saturate(depth * Ceto_AboveInscatterScale);
	inscatterScale.y = saturate(1.0-exp(-depth * Ceto_AboveInscatterScale));
	inscatterScale.z = saturate(1.0-exp(-depth * depth * Ceto_AboveInscatterScale));
	
	//Apply mask to pick which methods result to use.
	//Better than conditional statement?
	half a = dot(inscatterScale, Ceto_AboveInscatterMode);
	
	return lerp(col, Ceto_AboveInscatterColor.rgb, a * Ceto_AboveInscatterColor.a);
}

/*
* The inscatter when seen from below the ocean mesh.
* Where depth is normalized between 0-1 based on Ceto_MaxDepthDist.
*/
fixed3 AddBelowInscatter(fixed3 col, float depth)
{
	//There are 3 methods used to apply the inscatter.
	half3 inscatterScale;
	inscatterScale.x = saturate(depth * Ceto_BelowInscatterScale);
	inscatterScale.y = saturate(1.0-exp(-depth * Ceto_BelowInscatterScale));
	inscatterScale.z = saturate(1.0-exp(-depth * depth * Ceto_BelowInscatterScale));
	
	//Apply mask to pick which methods result to use.
	//Better than conditional statement?
	half a = dot(inscatterScale, Ceto_BelowInscatterMode);
	
	return lerp(col, Ceto_BelowInscatterColor.rgb, a * Ceto_BelowInscatterColor.a);
}

/*
* The ocean color when seen from above the ocean mesh.
*/
fixed3 OceanColorFromAbove(half3 N, float4 screenUV, float3 surfacePos, float surfaceDepth, float dist)
{

	fixed3 col = Ceto_DefaultOceanColor;

	#ifdef CETO_UNDERWATER_ON

		//Fade by distance so distortion is less on far away objects.
		float distortionFade = 1.0 - clamp(dist * 0.01, 0.0001, 1.0);

		half3 distortion = N * Ceto_RefractionDistortion * distortionFade;
		distortion *= half3(1,0,0);
		
		#ifdef CETO_USE_OCEAN_DEPTHS_BUFFER
			float depth = SampleOceanDepth(screenUV.xy, distortion).z;
		#else
			float depth = SampleDepthBuffer(screenUV.xy, distortion);
		#endif

		//If the distorted depth is less than the ocean mesh depth
		//then the distorted uv is in front of a object. The distortion
		//cant be applied in this case as the color from the grab texture
		//will be from a object that is not under the water.
		distortion = (depth <= surfaceDepth) ? half3(0,0,0) : distortion;
		
		float2 oceanDepth = OceanDepth(screenUV, distortion, surfacePos, surfaceDepth);

		float depthBlend = lerp(oceanDepth.x, oceanDepth.y, Ceto_DepthBlend);
		
		fixed3 refraction = AboveRefractionColor(screenUV.zw, distortion, surfacePos, depthBlend);
		
		col = AddAboveInscatter(refraction, depthBlend);

	#endif
	
	return col;
	
}

/*
* This is the color of the underside of the mesh.
*/
fixed3 DefaultUnderSideColor()
{
	return Ceto_BelowInscatterColor.rgb;
}

/*
* The sky color when seen from below the ocean mesh.
*/
fixed3 SkyColorFromBelow(half3 N, float4 screenUV, float3 surfacePos, float surfaceDepth, float dist)
{

	fixed3 col = Ceto_DefaultOceanColor;

	#ifdef CETO_UNDERWATER_ON

		//Fade by distance so distortion is less on far away objects.
		float distortionFade = 1.0 - clamp(dist * 0.01, 0.0001, 1.0);
		
		half3 distortion = N * Ceto_RefractionDistortion * distortionFade;
		distortion *= half3(1,0,0);
		
	#ifdef CETO_USE_OCEAN_DEPTHS_BUFFER
		float depth = SampleOceanDepth(screenUV.xy, distortion).z;
	#else
		float depth = SampleDepthBuffer(screenUV.xy, distortion);
	#endif
		
		//If the distorted depth is less than the ocean mesh depth
		//then the distorted uv is in front of a object. The distortion
		//cant be applied in this case as the color from the grab texture
		//will be from a object that is not under the water.
		distortion = (depth <= surfaceDepth) ? half3(0,0,0) : distortion;
		
		fixed3 grab = BelowRefractionColor(screenUV.zw, distortion);
		
		col = grab;
		
	#endif
	
	return col;
	
}

/*
* Returns a blend value to use as the alpha to fade ocean into shoreline.
*/
float EdgeFade(float4 screenUV, float3 view, float3 surfacePos)
{

	float edgeFade = 1.0;

	#ifdef CETO_UNDERWATER_ON

		//Fade based on dist between ocean surface and bottom
	#ifdef CETO_USE_OCEAN_DEPTHS_BUFFER
		float surfaceDepth = (surfacePos.y - Ceto_OceanLevel) * -1.0;
		float oceanDepth = tex2D(Ceto_OceanDepth, screenUV.xy).x * Ceto_MaxDepthDist;
		float dist = oceanDepth - surfaceDepth;
	#else
		float db = tex2D(Ceto_DepthBuffer, screenUV.xy).x;
		float y = WorldPosFromDepth(screenUV.xy, db).y;
		float dist = surfacePos.y - y;
	#endif

		dist = max(0.0, dist);
		edgeFade = 1.0 - saturate(exp(-dist * Ceto_EdgeFade) * 2.0);

		//Restrict blending when viewing ocean from a shallow angle
		//as it will cause some artifacts at horizon.
		float viewMaskStr = 10.0;
		float viewMask = saturate(dot(view, fixed3(0, 1, 0)) * viewMaskStr);

		edgeFade = lerp(1.0, edgeFade, viewMask);

	#endif

	return edgeFade;
}

/*
* The underwater color used in the post effect shader.
*/ 
fixed3 UnderWaterColor(fixed3 belowColor, float dist)
{
	
	fixed3 col = belowColor;
	
	#ifdef CETO_UNDERWATER_ON
		
		col = belowColor * Ceto_BelowTint * exp(-Ceto_BelowCof.rgb * dist * Ceto_BelowCof.a);
		
		//For inscatter dist should be normalized to max dist.
		dist = dist / Ceto_MaxDepthDist;
		//Need to rescale otherwise the inscatter is to strong.
		dist *= 0.1;

		col = AddBelowInscatter(col, dist);

	#endif
	
	return col;

}

#endif








