Shader "Custom/Texture_TileMix"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Resol("Resolution",float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
		
            CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			float _Resol;
			sampler2D _MainTex;

			struct Input {
			float2 uv_MainTex;
			};

			float circle(fixed2 st, float rad){
				float t = distance(st , fixed2(0.5,0.5));
				return smoothstep(rad+0.0001,rad,t);
			}

			void surf (Input IN, inout SurfaceOutputStandard o) {
				fixed2 st = IN.uv_MainTex * _Resol;
				fixed3 col = fixed3(0.0,0.0,0.0);
				st = frac(st);

				col = fixed3(tex2D(_MainTex,IN.uv_MainTex).rgb * circle(st,0.6));
				o.Albedo = col;
			}

            ENDCG
    }
}
