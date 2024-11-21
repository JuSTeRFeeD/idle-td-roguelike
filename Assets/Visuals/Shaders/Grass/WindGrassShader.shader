Shader "Custom/GrassShader"
{
    Properties
    {
        _BladeColor ("Blade Base Color", Color) = (0.2, 0.8, 0.2, 1)
        _GradientColor ("Blade Tip Color", Color) = (0.1, 0.4, 0.1, 1)
        _BladeHeight ("Blade Height", Float) = 0.5
        _WindStrength ("Wind Strength", Float) = 0.1
        _WindSpeed ("Wind Speed", Float) = 1.0
        _WindFrequency ("Wind Frequency", Float) = 2.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
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
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            // Shader properties
            float4 _BladeColor;
            float4 _GradientColor;
            float _BladeHeight;
            float _WindStrength;
            float _WindSpeed;
            float _WindFrequency;

            // Vertex shader
            v2f vert(appdata v)
            {
                v2f o;

                // Мировая позиция вершины
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Ветер: смещение вершины травы
                float windOffset = sin(_Time.y * _WindSpeed + worldPos.x * _WindFrequency) * _WindStrength;

                // Если вершина выше по высоте, сдвигаем её под воздействием ветра
                if (v.uv.y > 0.5)
                {
                    worldPos.x += windOffset;
                }

                // Высота травинки
                worldPos.y += v.uv.y * _BladeHeight;

                // Преобразование в пространство клипа
                o.vertex = UnityObjectToClipPos(float4(worldPos, 1.0));

                // Градиентный цвет: от основания (_BladeColor) к вершине (_GradientColor)
                o.color = lerp(_BladeColor, _GradientColor, v.uv.y);

                return o;
            }

            // Fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
