/* Generate a random float from a 3 dimensional seed. */
float rand(float3 co)
{
    return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
}

/* Given an angle in radians and an axis, build a rotation matrix */
float3x3 RotationMatrix(float angle, float3 axis)
{
    float c, s;
    sincos(angle, s, c);

    float t = 1 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    return float3x3(
        t * x * x + c, t * x * y - s * z, t * x * z + s * y,
        t * x * y + s * z, t * y * y + c, t * y * z - s * x,
        t * x * z - s * y, t * y * z + s * x, t * z * z + c
    );
}

/* 
* Construct a matrix to transform our blade from tangent space
* to local space.
* This is the same process used when sampling normal maps.
*/
float3x3 TangentToLocal(float3 Normal, float4 Tangent)
{
    float3 vBinormal = cross(Normal, Tangent) * Tangent.w;
    return float3x3(
        Tangent.x, vBinormal.x, Normal.x,
        Tangent.y, vBinormal.y, Normal.y,
        Tangent.z, vBinormal.z, Normal.z
    );
}