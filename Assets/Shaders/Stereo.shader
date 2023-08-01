Shader "Unlit/Stereo"
{
    Properties
    {
        _LeftTex("Texture", 2D) = "white" {}
        _RightTex("Texture", 2D) = "white" {}

        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off
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

            sampler2D _LeftTex;
            sampler2D _RightTex;

            v2f vertexFunc(appdata IN)
            {
                v2f OUT;
                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                return OUT;
            }

            fixed4 fragmentFunc(v2f i) : SV_TARGET
            {
                if(unity_StereoEyeIndex == 0) // Left eye
                    return tex2D(_LeftTex, i.uv)*_Color;
                else // Right eye
                    return tex2D(_RightTex, i.uv)*_Color;
            }

            ENDCG
        }
    }
}