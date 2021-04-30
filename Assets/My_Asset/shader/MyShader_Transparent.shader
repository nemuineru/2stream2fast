Shader "Custom/MyShader_Transparent"
{
	Properties
	{
		_Intensity ("trans" , float) = 0
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

			float _Intensity;

			void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = fixed4(IN.uv_MainTex.x, IN.uv_MainTex.x * IN.uv_MainTex.y, IN.uv_MainTex.y, 1);
			float alpha = 1 - (abs(dot(IN.worldNormal,IN.viewDir)));
			o.Alpha = _Intensity * alpha;
			}

            ENDCG
    }
}
