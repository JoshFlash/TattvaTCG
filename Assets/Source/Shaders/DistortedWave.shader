Shader "TattvaGames/DistortedWave" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0,1)) = 0.1
        _Speed ("Speed", Range(0,3)) = 0.5
        _Color ("Color", Color) = (1,1,1,1)
        _DarknessCutoff ("DarknessCutoff", Range(0,1)) = 0.1
        _FadeThreshold ("FadeThreshold", Range(0,3)) = 0.5
        _FadeStrength ("FadeStrength", Range(1,10)) = 2
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
            float _Distortion;
            float _Speed;
            float4 _Color;
            
            float _DarknessCutoff;
            float _FadeThreshold;
            float _FadeStrength;
            float _Intensity;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float2 scrollSinUV(float2 uv, float time) {
                float speed = _Speed * 0.1;
                float offset = time * speed + 0.01;
                offset *= offset;
                return uv + float2(offset, offset);
            }
            
            float2 scrollCosUV(float2 uv, float time) {
                float speed = -_Speed * 0.1;
                float offset = time * speed - 0.01;
                offset *= offset;
                return uv + float2(-offset, offset);
            }
            
            float4 frag (v2f i) : SV_Target {
                float4 tex = tex2D(_MainTex, scrollSinUV(i.uv, _Time.y) + scrollCosUV(i.uv, _Time.y));
                float2 distortion = (tex.rg - 0.5) * _Distortion * _Distortion;
                float4 distortedTex = tex2D(_MainTex, scrollCosUV(i.uv + distortion, -_Time.y));
                float4 col = _Color * distortedTex;

                float4 tex2 = tex2D(_MainTex, scrollSinUV(i.uv, _Time.y) + scrollCosUV(i.uv, -_Time.y));
                float2 distortion2 = (tex2.rg - 0.5) * _Distortion * _Distortion;
                float4 distortedTex2 = tex2D(_MainTex, scrollSinUV(i.uv + distortion2, _Time.y));
                col *= distortedTex2;

                float darkness = col.r + col.g + col.b;
                if (darkness < _DarknessCutoff * _DarknessCutoff) {
                    discard;
                }
                if (darkness < _FadeThreshold)
                {
                    col.a /= _FadeStrength;
                }
                else
                {
                    col.a *= _Intensity;
                }
                
                col.a *= 1.1;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
