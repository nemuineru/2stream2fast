Shader "Custom/RippleAndCircle"
{
    Properties
    {
		_MainTex("Main Texture", 2D) = "white" {}
		_Color1("Color", Color) = (0,0,0,0)
		_Color2("Ripple", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0

		struct Input{
			float3 worldPos;
			float2 uv_Maintex;
		};

		fixed4 _Color1;
		fixed4 _Color2;

		void surf(Input IN, inout SurfaceOutputStandard o){
			float2 dist = float2(IN.uv_Maintex.x - IN.worldPos.x,IN.uv_Maintex.y - IN.worldPos.z);
			float absDist = distance(float2(0,0),dist);
			float val = abs(sin(absDist * 12.5 - 200 * _Time));
			o.Albedo = val > 0.1 ? _Color1 : _Color2;
		}
	ENDCG
	}
	FallBack "Diffuse"
}
