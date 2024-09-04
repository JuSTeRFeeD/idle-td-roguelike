Shader "Custom/GridShader"
{
    Properties
    {
        _GridColor ("Grid Color", Color) = (1, 1, 1, 1)
        _CellSize ("Cell Size", Float) = 1.0
        _LineWidth ("Line Width", Float) = 0.05
        _GridScale ("Grid Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 worldPos : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _GridColor;
            float _CellSize;
            float _LineWidth;
            float _GridScale;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex.xyz * _GridScale; // масштабирование сетки по объекту
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 gridUV = abs(frac(i.worldPos.xz / _CellSize) - 0.5); // находим позиции в сетке
                float liniya = step(gridUV.x, _LineWidth) + step(gridUV.y, _LineWidth); // проверка на линию

                if (liniya > 0.0)
                {
                    return _GridColor; // рисуем линии сетки
                }
                
                return fixed4(0, 0, 0, 0); // возвращаем прозрачный цвет для остальных пикселей
            }
            ENDCG
        }
    }
    FallBack Off
}
