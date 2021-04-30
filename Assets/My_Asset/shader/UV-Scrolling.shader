Shader "Custom/UV-Scrolling"
{
   Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Speed("UV-Speed", Vector) = (0.1,0.1,0.1,0.1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		

		sampler2D _MainTex;
		float4 _Speed;

		struct Input{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed2 uv = IN.uv_MainTex;
			float2 uvSpd = float2(_Speed.x,_Speed.y);
			uv += 0.1 * _Time * uvSpd;
			o.Emission = tex2D(_MainTex, uv);
		}
		ENDCG
    }
	FallBack "Diffuse"
}
