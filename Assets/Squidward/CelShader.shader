// Code for cel shader used in the game developed by
// Adam Turner, Amie Xie & Shevon Mendis for Project 02 of COMP30019.
//
// Written by Adam Turner, October 2018.
//
// Based off tutorial by user _Beta
// https://www.youtube.com/watch?v=TIupEOkFG2s
// Outline logic from shader by Ippokratis Bournellis

Shader "Custom/CelShader"
{
	Properties
	{
		
		_Color("Color", Color) = (1,1,1,1) //Color multiplied to the texture
		_MainTex("Albedo (RGB)", 2D) = "white" {} //Texture
		_BumpTex("Normal", 2D) = "transparent" {} //Texture
		_HighlightStrength("Highlight Strength", Float) = 1.6
		_ShadowStrength("Shadow Strength", Float) = 1.2
		_BlurWidth("Shader Blur Width", Range(0,2)) = 0.2 //Blur between thresholds
		_OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1.0)	
		_Outline ("Outline width", Float) = 0.01	
	}
	SubShader
	{
	    Pass
	    {
	        // only show outline
            Cull Front
            ZWrite On
            
            CGPROGRAM
			
            #pragma vertex vert
 			#pragma fragment frag
 			
 			#include "UnityCG.cginc"
			
            struct appdata 
            {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 pos : SV_POSITION;
			};

            fixed _Outline;

            v2f vert (appdata v) 
            {
                v2f o;
			    o.pos = v.vertex;
			    // multiply vertex normal value by outline width
			    o.pos.xyz += normalize(v.normal.xyz) * _Outline * 0.01;
			    o.pos = UnityObjectToClipPos(o.pos);
			    return o;
            }
            
            fixed4 _OutlineColor;
            
            fixed4 frag(v2f i) : COLOR 
			{
			    // colour the outline
		    	return _OutlineColor;
			}
            
            ENDCG   
	    }
	    
		Tags { "RenderType"="Opaque" }
		LOD 100

        CGPROGRAM

        #pragma surface surf Toon fullforwardshadows 
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpTex;
        sampler2D _RampTex;
        
        struct Input 
        {
            float2 uv_MainTex;
            float2 uv_BumpTex;
        };
        
        half _BlurWidth;
        float _HighlightStrength;
        float _ShadowStrength;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o) 
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            //o.Normal = UnpackNormal (tex2D (_BumpTex, IN.uv_BumpTex));
        }
        
        fixed4 LightingToon(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            // calculate normal
            half NdotL = dot(s.Normal, lightDir);
            half cel;
            
            // shadow area if less than threshold
            if (NdotL < 0.5 - _BlurWidth / 2)
                cel = _ShadowStrength;
            // highlight area if greater than threshold
            else if (NdotL > 0.5 + _BlurWidth / 2)
                cel = _HighlightStrength;
            // blur section in between
            else
                cel = 2 - ((0.5 + _BlurWidth / 2 - NdotL) / _BlurWidth);
    
            half4 c;
            
            // combine values
            c.rgb = (cel + 0.3) / 2.5  * s.Albedo * _LightColor0.rgb * atten;
            c.a = s.Alpha;
    
            return c;
        }
        
        ENDCG
        
		}
	FallBack "Diffuse"
}
