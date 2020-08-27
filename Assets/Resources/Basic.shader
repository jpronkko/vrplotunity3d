// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Basic"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc" // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // for _LightColor0

            struct appdata
            {
                float4 vertex : POSITION;
                //float2 uv : TEXCOORD0;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed3 color : COLOR0;
                //half3 worldNormal : TEXCOORD1;
                //float4 worldPosition : TEXCOORD2;
                half tone : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                //o.worldNormal = UnityObjectToWorldNormal(v.normal);
                //o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half3 worldPosition = mul(unity_ObjectToWorld, v.vertex);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.tone = 0.33f * (worldPosition.r + worldPosition.g + worldPosition.b);
                o.color =  (nl * _LightColor0) + (worldNormal * 0.5 + 0.5);
                //(v.normal * 0.5 + 0.5) *
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);

                //return col;
                //return _Color;
                //half tone = 0.3f*(i.worldPosition.r + i.worldPosition.g + i.worldPosition.b);
                //col = i.worldPosition;
                
                //col.rgb = i.worldNormal * 0.5 + 0.5;
                //col = lerp(i.worldPosition, _Color, 0.9f);
                //col.rgb = lerp(i.worldNormal * 0.5 + 0.5, col.rgb, 0.4f);

                fixed4 col = 0.3 * i.tone * _Color + 0.7 * _Color;
                col.rgb *= i.color;
                return col;
            }
            ENDCG
        }
    }
}
