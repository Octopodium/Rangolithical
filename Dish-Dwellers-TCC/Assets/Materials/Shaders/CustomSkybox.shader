Shader "Hidden/SombraCircular"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Lower Color", Color) = (0.5, 0.5, 0.5, 1)
        _Color2 ("Upper Color" , Color) = (0, 0.5, 0.5, 1)
    }
    SubShader
    {
        ZWrite Off
        Tags{
            "Queue" = "Background"
            "PreviewType" = "Plane"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _Color;
            float _Alpha;

            float4 frag (v2f i) : SV_Target
            {
                float4 col;
                col.a = (tex2D(_MainTex, i.uv).a > 0) * _Alpha;
                col.rgb = _Color.rgb;
                return col;
            }
            ENDCG
        }
    }
}
