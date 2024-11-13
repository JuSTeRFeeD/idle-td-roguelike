Shader "Bytesized/Grass"
{
    Properties
    {
		[Header(Shading)]
        _TopColor("Top Color", Color) = (0.57, 0.84, 0.32, 1.0)
		_BottomColor("Bottom Color", Color) = (0.0625, 0.375, 0.07, 1.0)
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5
		[Header(Wind)]
		_WindStrength("Wind Strength", Range(0.0001, 1)) = 0.3
		[Header(Spacing)]
		_ViewLOD ("View Radius", Float) = 48
		_MaxStages ("Max Stages", Range(2, 64)) = 7
		_BaseStages ("Base Stages", Range(-64, 64)) = -0.5
		[Header(Grass Blades)]
		_BladeWidth("Blade Width", Range(0, 0.4)) = 0.05
		_BladeWidthRandom("Blade Width Random", Range(0, 0.4)) = 0.02
		_BladeHeight("Blade Height", Float) = 0.5
		_BladeHeightRandom("Blade Height Random", Float) = 0.3
		_BladeForward("Blade Stiffness Amount", Range(0, 1)) = 0.38
		_BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2
		_BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
    }

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Autolight.cginc"
	/* Include the tesselation, hull and domain shader which will help us increase the amount of grass on the mesh depending on the distance from the camera. */
	#include "GrassTessellation.cginc"
	/* Include some helper functions for creating matrices and random numbers */
	#include "Helpers.cginc"
	
	struct geometryOutput
	{
		float4 pos : SV_POSITION;		
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
		unityShadowCoord4 _ShadowCoord : TEXCOORD1;
	};

	geometryOutput VertexOutput(float3 pos, float3 normal, float2 uv)
	{
		geometryOutput o;
		o.pos = UnityObjectToClipPos(pos);
		o.normal = UnityObjectToWorldNormal(normal);
		o.uv = uv;
		o._ShadowCoord = ComputeScreenPos(o.pos);
	#if UNITY_PASS_SHADOWCASTER
		o.pos = UnityApplyLinearShadowBias(o.pos);
	#endif
		return o;
	}

	/* Create a vertex of the grass blade, increasing its scale the further away it is from the camera. This allows us to generate less grass the farther away from the camera it is. Improves performance. */
	geometryOutput GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 transformMatrix)
{
    float3 tangentPoint = float3(width, forward, height);
    float3 tangentNormal = normalize(float3(0, -1, forward));
    float3 localPosition = vertexPosition + mul(transformMatrix, tangentPoint);
    float3 localNormal = mul(transformMatrix, tangentNormal);
    return VertexOutput(localPosition, localNormal, uv);
}

	float _BladeHeight;
	float _BladeHeightRandom;
	float _BladeWidthRandom;
	float _BladeWidth;
	float _BladeForward;
	float _BladeCurve;
	float _BendRotationRandom;
	float _WindStrength;

	#define BLADE_SEGMENTS 3

	/* Geometry shader that takes in a single vertex and outputs a grass blade. We need 2 vertices per segment and one for the tip */
	[maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
	void geo(point vertexOutput IN[1], inout TriangleStream<geometryOutput> triStream)
	{
		/*
		* Each blade of grass is constructed in tangent space with respect
		* to the emitting vertex's normal and tangent vectors, where the width
		* lies along the X axis and the height along Z.
		*/
		float3 pos = IN[0].vertex.xyz;
		/* Construct rotation 2 matrices, one to make the blade face in a random rotation and the other to make the blade bend into the direction its facing */
		float3x3 facingRotationMatrix = RotationMatrix(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
		float3x3 bendRotationMatrix = RotationMatrix(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));
		/* Simulate the wind effect by feeding an unique seed to the sine & cosine functions */
		float2 windValue = float2(cos(_Time.y + pos.x), sin(_Time.y + pos.z)) * _WindStrength * .25;
		/* Build a rotation matrix from the wind sample and a normalized vector of it. */
		float3x3 windRotation = RotationMatrix(UNITY_PI * windValue, normalize(float3(windValue.x, windValue.y, 0)));
		/* Create a matrix to transform the vertices from tangent space to local space. This method is from Helpers.cginc */
		float3x3 tangentToLocal = TangentToLocal(IN[0].normal, IN[0].tangent);
		/* Create a new matrix that contains all of out transformations (wind, bend, facing, tangentToLocal). */
		float3x3 transformationMatrix = mul(mul(mul(tangentToLocal, windRotation), facingRotationMatrix), bendRotationMatrix);
		/* Create the same as above but without any bending. This will be used for the root segement of the grass blade. */
		float3x3 transformationMatrixWithoutBending = mul(tangentToLocal, facingRotationMatrix);

		/* Apply some randomness to the transformation values. */
		float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
		float width = (rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;
		float forward = rand(pos.yyz) * _BladeForward;
		for (int i = 0; i < BLADE_SEGMENTS; i++)
		{
			/* For each segment the width should decrease and the height should increase. */
			float t = i / (float)BLADE_SEGMENTS;
			float segmentHeight = height * t;
			float segmentWidth = width * (1 - t);
			float segmentForward = pow(t, _BladeCurve) * forward;

			/* For the root segment use the matrix without bending */
			float3x3 transformMatrix = i == 0 ? transformationMatrixWithoutBending : transformationMatrix;
			/* Create the necessary vertices to complete the triangle strip */
			triStream.Append(GenerateGrassVertex(pos, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix));
			triStream.Append(GenerateGrassVertex(pos, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix));
		}
		/* End the triangle strip by adding the tip of the grass blade */
		triStream.Append(GenerateGrassVertex(pos, 0, height, forward, float2(0.5, 1), transformationMatrix));
	}
	ENDCG

    SubShader
    {
		Cull Off

        Pass
        {
			Tags
			{
				"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
			}

            CGPROGRAM
            #pragma vertex vert
			#pragma geometry geo
            #pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_fwdbase
            
			#include "Lighting.cginc"

			float4 _TopColor;
			float4 _BottomColor;
			float _TranslucentGain;

			float4 frag(geometryOutput i,  fixed facing : VFACE) : SV_Target
            {
				/* Do some lighting on the fragments of the grass blade */	
				float3 normal = facing > 0 ? i.normal : -i.normal;
				float shadow = SHADOW_ATTENUATION(i);
				float NdotL = saturate(saturate(dot(normal, _WorldSpaceLightPos0)) + _TranslucentGain) * shadow;
				float3 ambient = ShadeSH9(float4(normal, 1));
				float4 lightIntensity = NdotL * _LightColor0 + float4(ambient, 1);
				/* Paint the grass by interpolating between the colors passed as uniforms */
				return lerp(_BottomColor, _TopColor * lightIntensity, i.uv.y);
            }
            ENDCG
        }

		Pass
		{
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_shadowcaster

			float4 frag(geometryOutput i) : SV_Target
			{
				/* Allow the grass to cast shadows. */
				SHADOW_CASTER_FRAGMENT(i)
			}

			ENDCG
		}
    }
}
