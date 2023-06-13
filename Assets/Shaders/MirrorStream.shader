Shader "Unlit/MirrorStream"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTexRight ("TextureRight", 2D) = "white" {}
    }
    SubShader
    {
        // Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        // Blend SrcAlpha OneMinusSrcAlpha
        // ZWrite off
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            
            CGPROGRAM

            #pragma vertex vertexFunc
            #pragma fragment fragmentFunc

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;

            sampler2D _MainTex;
            sampler2D _MainTexRight;

            v2f vertexFunc(appdata IN)
            {
                v2f OUT;
                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                return OUT;
            }

            fixed4 fragmentFunc(v2f i) : SV_TARGET
            {
                return tex2D(_MainTex, i.uv);
            }

            ENDCG
        }
    }
}