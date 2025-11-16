Shader "Custom/ShaderDissolve" {
    Properties{
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
        _SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0
        _BurnSize("Burn Size", Range(0.0, 1.0)) = 0.15
        _BurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
        _BurnColor("Burn Color", Color) = (1,1,1,1)
        _EmissionAmount("Emission", float) = 2.0
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200
            Cull Off
            CGPROGRAM
            #pragma surface surf Lambert addshadow
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _SliceGuide;
            float4 _SliceGuide_ST;
            sampler2D _BumpMap;
            sampler2D _BurnRamp;
            fixed4 _BurnColor;
            float _BurnSize;
            float _SliceAmount;
            float _EmissionAmount;

            struct Input {
                float2 uv_MainTex;
            };


            void surf(Input IN, inout SurfaceOutput o) {
                fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
                half burn = tex2D(_SliceGuide, IN.uv_MainTex * _SliceGuide_ST.xy + _SliceGuide_ST.zw).rgb - _SliceAmount * abs(_SinTime.w);
                clip(burn);

                if (burn < _BurnSize && _SliceAmount > 0) {
                    o.Emission = tex2D(_BurnRamp, float2(burn * (1 / _BurnSize), 0)) * _BurnColor * _EmissionAmount;
                }

                o.Albedo = col.rgb;
                o.Alpha = col.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}