Shader "TattvaGames/DistortedWave" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondTex ("BG Texture", 2D) = "white" {}
        _Blending ("Blending", Range(0,1)) = 0
        _Distortion ("Distortion", Range(0,1)) = 0.1
        _TexScale ("Texture Scale", Float) = 1
        _Speed ("Speed", Range(0,3)) = 0.5
        _Color ("Color", Color) = (1,1,1,1)
        _DarknessCutoff ("DarknessCutoff", Range(0,2)) = 0.1
        _FadeThreshold ("FadeThreshold", Range(0,3)) = 0.5
        _FadeStrength ("FadeStrength", Range(0.5,10)) = 2
        _Intensity ("Intensity", Range(1,10)) = 2
    }
    
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100
        
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _SecondTex;
            float _Blending;
            float _Distortion;
            float _Speed;
            float4 _Color;
            
            float _DarknessCutoff;
            float _FadeThreshold;
            float _FadeStrength;
            float _Intensity;
            float _TexScale;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float2 scrollPosUV(float2 uv, float2 time, float timeOffset) {
                float speed = _Speed * 0.01;
                float2 offset = (time + timeOffset) * speed;
                return (uv  + offset) * _TexScale;
            }
            
            
            float4 frag (v2f i) : SV_Target {

                float2 timeNorm = float2(-_SinTime.w, _CosTime.w);
                float4 tex = tex2D(_MainTex, scrollPosUV(i.uv, timeNorm, _SinTime.z));
                float2 distortion = (tex.rg - 0.5) * _Distortion * _Distortion;
                float4 distortedTex = tex2D(_MainTex, scrollPosUV(i.uv + distortion, timeNorm, _SinTime.z));

                float2 timeNorm2 = float2(-_Time.y, _Time.z);
                float4 tex2 = tex2D(_SecondTex, scrollPosUV(i.uv, timeNorm2, 0));
                float2 distortion2 = (tex2.rg - 0.5) * _Distortion * _Distortion;
                float4 distortedTex2 = tex2D(_MainTex, scrollPosUV(i.uv + distortion2, timeNorm2, 0));

                float2 timeNorm3 = float2(_Time.z, _Time.y);
                float4 tex3 = tex2D(_MainTex, scrollPosUV(i.uv, timeNorm3, _SinTime.z));
                float2 distortion3 = (tex3.rg - 0.5) * _Distortion * _Distortion;
                float4 distortedTex3 = tex2D(_SecondTex, scrollPosUV(i.uv + distortion3, timeNorm3, _SinTime.z));

                float bgInfluence = _Blending / 2;
                float4 col = _Color * (distortedTex * (1 - _Blending) + distortedTex2 * bgInfluence + distortedTex3 * bgInfluence);

                float darkness = col.r + col.g + col.b;
                if (darkness < _DarknessCutoff * _DarknessCutoff) {
                    discard;
                }
                if (darkness < _FadeThreshold)
                {
                    col.a = lerp(col.a, 0, (1 - darkness/_FadeThreshold)) / _FadeStrength;
                }
                else
                {
                    col.a = lerp (col.a, col.a * _Intensity, 1 - col.a);
                }
                
                col.a *= 1.1;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
