Shader "Custom/VertColor"
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
            #pragma surface surf Lambert vertex:vert //Vertシェーダーを指定してメソッドを作製.
			#pragma target 3.0

            struct Input
            {
				float4 vertColor;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            void vert (inout appdata_full v, out Input o)
            {
				UNITY_INITIALIZE_OUTPUT(Input, o);
                o.vertColor = v.color;
            }

            void surf (Input IN, inout SurfaceOutput o)
            {
                o.Albedo = IN.vertColor.rgb;
            }
			ENDCG
    }
	FallBack "Diffuse"
}
