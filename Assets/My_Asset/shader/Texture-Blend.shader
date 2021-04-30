Shader "Custom/Texture-Blend"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _SubTex ("Sub Texture", 2D) = "Red" {}
		_Blend ("Blending Texture", 2D) = "Black" {}
		_Intensity("Intensity", float) =  0.0
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

		CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		
		sampler2D _MainTex;
		sampler2D _SubTex;
		sampler2D _Blend;
		float _Intensity;

		struct Input{
			float2 uv_MainTex;
			float2 uv_SubTex;
			float2 uv_Blend;
		};

		fixed4 toGrayscale(fixed4 fix){
			fixed gray = fix.r * 0.2 + fix.g + 0.7 + fix.b * 0.1;
			return fixed4(gray,gray,gray,gray);
		}

		fixed toGrayNum(fixed4 fix){
			fixed gray = (fix.r * 0.1 + fix.g * 0.8 + fix.b * 0.1); // / 3.0 ;
			return fixed(gray);
		}

		void surf(Input IN, inout SurfaceOutputStandard o){
			fixed4 c1 = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c2 = tex2D(_SubTex,  IN.uv_SubTex);
			fixed4 blend = tex2D(_Blend, IN.uv_Blend);

			fixed f = toGrayNum(blend) > _Intensity ? 1 : 1 + (toGrayNum(blend) - (_Intensity));
			
			o.Albedo = lerp(c1,c2,f);
		}
		ENDCG
    }
	FallBack "Diffuse"
}
