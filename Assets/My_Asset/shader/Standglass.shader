Shader "Custom/StandGlassShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Intensity_trans ("trans" , float) = 0.7
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 200

		CGPROGRAM
		#pragma surface surf Standard alpha:fade
		#pragma target 3.0
		
		struct Input{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		float _Intensity_trans;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
			o.Alpha = (o.Albedo.r * 0.3 + o.Albedo.g * 0.6 + o.Albedo.b * 0.1 < _Intensity_trans )? 1 : 0.2; 
		}
		ENDCG
    }
}
