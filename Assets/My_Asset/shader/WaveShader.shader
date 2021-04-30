Shader "Custom/WaveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_SubTex ("Intensity", 2D) = "White" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

            CGPROGRAM
            #pragma surface surf Lambert vertex:vert //Vertシェーダーを指定してメソッドを作製.
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _SubTex;

			struct appdata{
			float4 vertex : POSITION; //頂点１つ１つの空間座標
			float2 uv : TEXCOORD1; //頂点１つ１つのUV空間の座標
			};

			struct Input{
				float2 uv_MainTex;
				float2 uv_SubTex;
			};

			void vert(inout appdata_full v, out Input o){
				UNITY_INITIALIZE_OUTPUT(Input, o); //出力の値を初期化する.
				fixed4 texs = tex2Dlod(_SubTex,fixed4(v.texcoord.xy ,0,0));
				float gray = (texs.r * 0.3 + texs.g * 0.6 + texs.b * 0.1);
				float uv_to_inf = abs((0.5 - v.texcoord.x) * (0.5 - v.texcoord.x)) * 4; //テクスチャuvの外縁からどれだけ影響を受けるか.
				float amp = 1.5 * gray * (0.5 * abs(uv_to_inf) + 0.5) *sin(_Time * 100 + v.vertex.x / 1.2);
				v.vertex.xyz = float3(v.vertex.x,v.vertex.y+amp,v.vertex.z);
			}

			void surf(Input IN,inout SurfaceOutput o){
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
            ENDCG
    }
	FallBack "Diffuse"
}
