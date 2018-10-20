// Code for camera tint shader used in the game developed by
// Adam Turner, Amie Xie & Shevon Mendis for Project 02 of COMP30019.
//
// Written by Adam Turner, October 2018.

Shader "cameraTintShader"
{
   Properties
   {
      _MainTex ("Source", 2D) = "white" {}
      _DesaturationValue ("Desaturation Value", Range(0,1)) = 0
      _Color ("Tint", Color) = (1,1,1,1)
   }
   SubShader
   {
      Cull Off 
      ZWrite Off 
      ZTest Always

      Pass
      {
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
			
         #include "UnityCG.cginc"

         struct appdata
         {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
         };

         struct v2f
         {
            float2 texcoord : TEXCOORD0;
            float4 position : SV_POSITION;
         };
         
         sampler2D _MainTex;
         float4 _MainTex_ST;
         float4 _Color;
         Float _DesaturationValue;
         
         v2f vert(appdata i)
         {
            v2f o;
			// convert object space point to camera clip space
            o.position = UnityObjectToClipPos(i.vertex);
			// change relative coordinates
            o.texcoord = i.texcoord;
            return o;
         }

         float4 frag(v2f i) : COLOR
         {
            float4 color = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST));
			// return a value between the original and grayscale colour			
            color.rgb = lerp(color.rgb, dot(color.rgb, float3(0.3, 0.59, 0.11)), _DesaturationValue);
            return color * _Color;
         }
         ENDCG
      }
   }
   Fallback Off
}