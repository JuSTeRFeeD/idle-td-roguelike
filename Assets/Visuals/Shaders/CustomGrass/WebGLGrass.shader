Shader "Custom/WebGLGrass"
{
    Properties
    {
        _TopColor("Top Color", Color) = (0.57, 0.84, 0.32, 1.0)
        _BottomColor("Bottom Color", Color) = (0.0625, 0.375, 0.07, 1.0)
        _WindStrength("Wind Strength", Range(0, 1)) = 0.5
        _BladeHeight("Blade Height", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="AlphaTest" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            float _WindStrength;
            float _BladeHeight;
            float4 _TopColor;
            float4 _BottomColor;

            v2f vert(appdata v)
            {
                v2f o;
                float time = _Time.y * _WindStrength;
                float windOffset = sin(v.vertex.x * 0.1 + time) * 0.1;
                v.vertex.y += windOffset * _BladeHeight;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float heightFactor = saturate(i.worldPos.y / _BladeHeight);
                return lerp(_BottomColor, _TopColor, heightFactor);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
