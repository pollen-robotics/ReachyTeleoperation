Shader "Unlit/Undistort"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTexRight ("TextureRight", 2D) = "white" {}

        _l_MapX ("l_MapX", 2D) = "white" {}
        _l_MapY ("l_MapY", 2D) = "white" {}
        _r_MapX ("r_MapX", 2D) = "white" {}
        _r_MapY ("r_MapY", 2D) = "white" {}

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

            sampler2D _MainTex;
            sampler2D _MainTexRight;

            sampler2D_float _l_MapX;
            sampler2D_float _l_MapY;
            sampler2D_float _r_MapX;
            sampler2D_float _r_MapY;

            float2 xy;

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
                {
                    xy[0] = tex2D(_l_MapX, i.uv)[0];
                    xy[1] = tex2D(_l_MapY, i.uv)[0];

                    return tex2D(_MainTex, xy)*_Color;
                }
                else // Right eye
                {                    
                    xy[0] = tex2D(_l_MapX, i.uv)[0];
                    xy[1] = tex2D(_l_MapY, i.uv)[0];

                    return tex2D(_MainTexRight, xy)*_Color;
                }
            }

            ENDCG
        }
    }
}