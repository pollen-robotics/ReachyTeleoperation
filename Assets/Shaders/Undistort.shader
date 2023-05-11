Shader "Unlit/Undistort"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTexRight ("TextureRight", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _l_fx("_l_fx", Float) = 0
        _l_fy("_l_fy", Float) = 0
        _l_cx("_l_cx", Float) = 0
        _l_cy("_l_cy", Float) = 0
        _l_k1("_l_k1", Float) = 0
        _l_k2("_l_k2", Float) = 0
        _l_k3("_l_k3", Float) = 0
        _l_p1("_l_p1", Float) = 0
        _l_p2("_l_p2", Float) = 0

        _r_fx("_r_fx", Float) = 0
        _r_fy("_r_fy", Float) = 0
        _r_cx("_r_cx", Float) = 0
        _r_cy("_r_cy", Float) = 0
        _r_k1("_r_k1", Float) = 0
        _r_k2("_r_k2", Float) = 0
        _r_k3("_r_k3", Float) = 0
        _r_p1("_r_p1", Float) = 0
        _r_p2("_r_p2", Float) = 0

        _rightTexOffsetX("_rightTexOffsetX", Float) = 0
        _rightTexOffsetY("_rightTexOffsetY", Float) = 0
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
            float _l_fx;
            float _l_fy;
            float _l_cx;
            float _l_cy;
            float _l_k1;
            float _l_k2;
            float _l_k3;
            float _l_p1;
            float _l_p2;

            float _r_fx;
            float _r_fy;
            float _r_cx;
            float _r_cy;
            float _r_k1;
            float _r_k2;
            float _r_k3;
            float _r_p1;
            float _r_p2;

            float2 _uvc;
            float _rightTexOffsetX;
            float _rightTexOffsetY;

            sampler2D _MainTex;
            sampler2D _MainTexRight;

            v2f vertexFunc(appdata IN)
            {
                v2f OUT;
                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                return OUT;
            }

            inline float2 normalize_px_coordinates(float2 uv, float cx, float cy, float fx, float fy)
            {
                float u = (uv[0] - cx) / fx;
                float v = (uv[1] - cy) / fy;
                return float2((u+1)/2, (v+1)/2);
            }

            inline float2 getUvDistorted(v2f i, float cx, float cy, float fx, float fy, float k1, float k2, float k3, float p1, float p2)
            {
                _uvc = normalize_px_coordinates(float2(cx, cy), cx, cy, fx, fy); // can't do outside of function, stupid because it does not need to be recomputed everytime
                float xc = _uvc[0];
                float yc = _uvc[1];
                float xd = i.uv[0];
                float yd = i.uv[1];

                float dx = xd - xc;
                float dy = yd - yc;
                float r = sqrt(pow(dx, 2) + pow(dy, 2));

                float xu = xd + dx * (k1 * pow(r, 2) + k2 * pow(r, 4) + k3 * pow(r, 6)) + (p1 * (pow(r, 2) + 2 * pow(dx, 2)) + 2 * p2 * dx * dy);
                float yu = yd + dy * (k1 * pow(r, 2) + k2 * pow(r, 4) + k3 * pow(r, 6)) + (2 * p1 * dx * dy + p2 * (pow(r, 2) + 2 * pow(dy, 2)));
                float2 uvDistorted = float2(xu, yu);

                return uvDistorted;
            }

            fixed4 fragmentFunc(v2f i) : SV_TARGET
            {
                float2 l_uvDistorted = getUvDistorted(i, _l_cx, _l_cy, _l_fx, _l_fy, _l_k1, _l_k2, _l_k3, _l_p1, _l_p2);
                float2 r_uvDistorted = getUvDistorted(i, _r_cx, _r_cy, _r_fx, _r_fy, _r_k1, _r_k2, _r_k3, _r_p1, _r_p2);

                if(unity_StereoEyeIndex == 0) // Left eye
                {
                    if (l_uvDistorted[0] < 0 || l_uvDistorted[0] > 1 || l_uvDistorted[1] < 0 || l_uvDistorted[1] > 1) 
                        return tex2D(_MainTex, i.uv)*_Color;
                    else
                        return tex2D(_MainTex, l_uvDistorted)*_Color;
                }
                else // Right eye
                {                    
                    if (r_uvDistorted[0] < 0 || r_uvDistorted[0] > 1 || r_uvDistorted[1] < 0 || r_uvDistorted[1] > 1) 
                        return tex2D(_MainTexRight, i.uv + float2(_rightTexOffsetX, _rightTexOffsetY))*_Color;
                    else
                        return tex2D(_MainTexRight, r_uvDistorted + float2(_rightTexOffsetX, _rightTexOffsetY))*_Color;

                }
            }

            ENDCG
        }
    }
}