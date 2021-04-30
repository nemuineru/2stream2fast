Shader "Custom/MyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
		
            CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0
			struct Input {
			float2 uv_MainTex;
			};

			void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = fixed4(IN.uv_MainTex.x, IN.uv_MainTex.x * IN.uv_MainTex.y, IN.uv_MainTex.y, 1);
			}

            ENDCG
    }
}
