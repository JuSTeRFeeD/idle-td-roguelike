Shader "Custom/VertexColorWithCavityValley"
{
    Properties
    {
        _MainColor("Main Color", Color) = (1, 1, 1, 1)
        _ValleyIntensity("Valley Intensity", Range(0, 1)) = 0.5 // Интенсивность эффекта Valley
        _ValleySharpness("Valley Sharpness", Range(0.1, 10)) = 2.0 // Резкость эффекта
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL; // Для расчета нормалей
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float3 worldNormal : NORMAL; // Мировая нормаль для расчета
                UNITY_VERTEX_OUTPUT_STEREO
            };

            uniform float4 _MainColor;
            uniform float _ValleyIntensity;
            uniform float _ValleySharpness;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.worldNormal = UnityObjectToWorldNormal(v.normal); // Преобразуем нормаль в мировые координаты
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Применение гамма-коррекции
                fixed4 gammaCorrectedColor = pow(i.color, 1.1f); // Преобразование в линейное пространство
                fixed4 finalColor = gammaCorrectedColor * 1.2f;

                // Рассчитываем эффект Cavity (Valley)
                float cavity = dot(i.worldNormal, float3(0, 1, 0)); // Направление для расчета глубины (впадины)
                cavity = pow(1.0 - abs(cavity), _ValleySharpness); // Используем резкость для усиления эффекта впадин
                cavity = saturate(cavity * _ValleyIntensity); // Контролируем интенсивность затенения

                // Применяем затенение впадин к финальному цвету
                finalColor.rgb *= (1.0 - cavity); // Затемняем цвет в впадинах

                return finalColor * _MainColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
