Shader "Sprites/SlicedZSort" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Lighting Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float depth : SV_Depth;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.depth = o.vertex.z;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                col.a = col.a * col.a;
                if (col.a < 0.5)
                    discard;

                return col;
            }
            ENDCG
        }
    }

    FallBack "Sprites/Default"
}
