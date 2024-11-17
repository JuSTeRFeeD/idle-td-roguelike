/* Define the structs we will use for the shader programs */
struct vertexInput
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
};

struct vertexOutput
{
	float4 vertex : SV_POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
};

struct TessellationFactors 
{
	float edge[3] : SV_TessFactor;
	float inside : SV_InsideTessFactor;
};

float _MaxStages;
float _ViewLOD;
float _BaseStages;

/* Calculate the tessellation factor of each edge depeding on the distance from the camera */
float TessellationEdgeFactor (
	vertexInput cp0, vertexInput cp1
) {
		/* Calculate the worldspace center point of these two vertices (the edge midpoint)*/
		float3 p0 = mul(unity_ObjectToWorld, float4(cp0.vertex.xyz, 1)).xyz;
		float3 p1 = mul(unity_ObjectToWorld, float4(cp1.vertex.xyz, 1)).xyz;
		float edgeLength = distance(p0, p1);
		float3 edgeCenter = (p0 + p1) * 0.5;

		/* Get the distance from the edge midpoint to the camera in world space */
		float viewDistance = distance(edgeCenter, _WorldSpaceCameraPos);
		return min(_MaxStages, edgeLength / (viewDistance / _ViewLOD))  + _BaseStages;
}

/* In the hull shader we (the amount it will be subdivided)*/
TessellationFactors constantFunction (InputPatch<vertexInput, 3> patch)
{
	TessellationFactors f;
	f.edge[0] = TessellationEdgeFactor(patch[1], patch[2]);
	f.edge[1] = TessellationEdgeFactor(patch[2], patch[0]);
	f.edge[2] = TessellationEdgeFactor(patch[0], patch[1]);
	f.inside = (f.edge[0] + f.edge[1] + f.edge[2]) * (1 / 3.0);
	return f;
}

/* These attributes are defined to let the program know we will output clockwise triangles */
[UNITY_domain("tri")]
[UNITY_outputcontrolpoints(3)]
[UNITY_outputtopology("triangle_cw")]
/* Use fractional_odd partitioning to allow for interpolation between constant function values. */
[UNITY_partitioning("fractional_odd")]
/* Reference the function defined above that will return us the constant function for the edge. */
[UNITY_patchconstantfunc("constantFunction")]
/* Hull shader */
vertexInput hull (InputPatch<vertexInput, 3> patch, uint id : SV_OutputControlPointID)
{
	return patch[id];
}

/* Passthrough function */
vertexOutput tessVert(vertexInput v)
{
	vertexOutput o;
	o.vertex = v.vertex;
	o.normal = v.normal;
	o.tangent = v.tangent;
	return o;
}

/* Domain shader */
[UNITY_domain("tri")]
vertexOutput domain(TessellationFactors factors, OutputPatch<vertexInput, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
{
	vertexInput v;

	/* Define a function in the preprocessor that interpolates each field using barycentric coordinates */
	#define DOMAIN_INTERPOLATE(fieldName) v.fieldName = patch[0].fieldName * barycentricCoordinates.x + patch[1].fieldName * barycentricCoordinates.y + patch[2].fieldName * barycentricCoordinates.z;

 	/* Use the defined function to interpolate all the attributes each vertex has (normal, position, tangent). If we need to interpolate more attributes they should be used here. */
	DOMAIN_INTERPOLATE(vertex)
	DOMAIN_INTERPOLATE(normal)
	DOMAIN_INTERPOLATE(tangent)

	return tessVert(v);
}

/* Passthrough function */
vertexInput vert(vertexInput v)
{
	return v;
}