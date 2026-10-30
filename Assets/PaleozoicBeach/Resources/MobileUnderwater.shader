Shader "PaleozoicBeach/MobileUnderwater"
{
    Properties { _Color ("Color", Color) = (0.3,0.4,0.35,1) }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            fixed4 _Color;
            struct v2f { float4 position : SV_POSITION; half3 normal : TEXCOORD0; UNITY_FOG_COORDS(1) };
            v2f vert(appdata_base input)
            {
                v2f output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.normal = UnityObjectToWorldNormal(input.normal);
                UNITY_TRANSFER_FOG(output, output.position);
                return output;
            }
            fixed4 frag(v2f input) : SV_Target
            {
                half diffuse = saturate(dot(normalize(input.normal), normalize(_WorldSpaceLightPos0.xyz)));
                fixed4 color = fixed4(_Color.rgb * (UNITY_LIGHTMODEL_AMBIENT.rgb + _LightColor0.rgb * diffuse), 1);
                UNITY_APPLY_FOG(input.fogCoord, color);
                return color;
            }
            ENDCG
        }
    }
}
