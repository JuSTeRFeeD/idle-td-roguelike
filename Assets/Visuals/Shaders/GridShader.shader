Shader "Custom/GridShader"
{
    Properties
    {
        _GridColor ("Grid Color", Color) = (1, 1, 1, 1)  // Цвет сетки
        _CellSize ("Cell Size", Float) = 1.0             // Размер клеток
        _LineWidth ("Line Width", Float) = 0.05          // Толщина линий
        _GridScale ("Grid Scale", Float) = 1.0           // Масштаб сетки
        _TransparencyStart ("Transparency Start", Float) = 0.2 // Момент начала прозрачности
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha  // Прозрачность
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
            float _TransparencyStart; // Новое поле для настройки начала прозрачности

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex.xyz * _GridScale; // Масштабирование сетки по объекту
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Вычисляем позицию в сетке
                float2 gridUV = abs(frac(i.worldPos.xz / _CellSize) - 0.5); 
                
                // Определяем, находимся ли мы на линии
                float2 lineDistances = gridUV / _LineWidth; // Отношение позиции к ширине линии
                float l = step(gridUV.x, _LineWidth) + step(gridUV.y, _LineWidth); // Проверка на линию

                if (l > 0.0)
                {
                    // Плавная прозрачность от центра линии к краям
                    float transparencyFactorX = smoothstep(_TransparencyStart, 1.0, lineDistances.x);
                    float transparencyFactorY = smoothstep(_TransparencyStart, 1.0, lineDistances.y);
                    float transparencyFactor = min(transparencyFactorX, transparencyFactorY);

                    // Применяем прозрачность к цвету сетки
                    fixed4 finalColor = _GridColor;
                    finalColor.a *= (1.0 - transparencyFactor); // Плавный переход прозрачности
                    return finalColor;
                }

                // Прозрачные области
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
    FallBack Off
}
