Shader "Unlit/Undistort"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _fx("fx", Float) = 0
        _fy("fy", Float) = 0
        _cx("cx", Float) = 0
        _cy("cy", Float) = 0
        _k1("k1", Float) = 0
        _k2("k2", Float) = 0
        _k3("k3", Float) = 0
        _p1("p1", Float) = 0
        _p2("p2", Float) = 0
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
            float _fx;
            float _fy;
            float _cx;
            float _cy;
            float _k1;
            float _k2;
            float _k3;
            float _p1;
            float _p2;
            float2 _uvc;

            sampler2D _MainTex;

            v2f vertexFunc(appdata IN)
            {
                v2f OUT;
                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                return OUT;
            }

            inline float2 normalize_px_coordinates(float2 uv)
            {
                float u = (uv[0] - _cx) / _fx;
                float v = (uv[1] - _cy) / _fy;
                return float2((u+1)/2, (v+1)/2);
            }

            float2 unnormalize_px_coordinates(float2 uv)
            {
                float u = uv[0] * _fx + _cx;
                float v = uv[1] * _fy + _cy;
                return float2(u, v);
            }

            fixed4 fragmentFunc(v2f i) : SV_TARGET
            {
                _uvc = normalize_px_coordinates(float2(_cx, _cy)); // can't do outside of function, stupid because it does not need to be recomputed everytime
                float xc = _uvc[0];
                float yc = _uvc[1];
                float xd = i.uv[0];
                float yd = i.uv[1];

                float dx = xd - xc;
                float dy = yd - yc;
                float r = sqrt(pow(dx, 2) + pow(dy, 2));

                float xu = xd + dx * (_k1 * pow(r, 2) + _k2 * pow(r, 4) + _k3 * pow(r, 6)) + (_p1 * (pow(r, 2) + 2 * pow(dx, 2)) + 2 * _p2 * dx * dy);
                float yu = yd + dy * (_k1 * pow(r, 2) + _k2 * pow(r, 4) + _k3 * pow(r, 6)) + (2 * _p1 * dx * dy + _p2 * (pow(r, 2) + 2 * pow(dy, 2)));
                float2 uvDistorted = float2(xu, yu);

                if (uvDistorted[0] < 0 || uvDistorted[0] > 1 || uvDistorted[1] < 0 || uvDistorted[1] > 1) 
                {
                    return tex2D(_MainTex, i.uv)*_Color;
                } 
                else 
                {
                    return tex2D(_MainTex, uvDistorted)*_Color;
                }
            }


            ENDCG
        }
    }
}