Shader "Custom/MyShader_Transparent_Rim"
{
	Properties
	{
		_Intensity_trans ("trans" , float) = 0
		_Intensity_rims ("rims" , float) = 0
	}
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 200
		
            CGPROGRAM
			#pragma surface surf Standard alpha:blend
			#pragma target 3.0

			struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
      		float3 viewDir;
			};

			float _Intensity_trans;
			float _Intensity_rims;

			void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 Rimcol = fixed4(IN.worldNormal.x,IN.worldNormal.y,IN.worldNormal.z,1);
			float dot_norm_view = dot(IN.worldNormal,IN.viewDir);
			float alpha = 1 - abs(dot_norm_view);
			float rim = 1 - _Intensity_rims * saturate(alpha);
			o.Alpha = _Intensity_trans * alpha;
			o.Albedo = fixed4(IN.uv_MainTex.x, IN.uv_MainTex.x * IN.uv_MainTex.y, IN.uv_MainTex.y, 1);
			o.Emission = Rimcol *  pow(rim,3);
			}

            ENDCG
    }
}
