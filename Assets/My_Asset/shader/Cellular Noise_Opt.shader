Shader "Custom/Cellular Noise_Opt"
{
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Resol("Resolution",float) = 0.0
		_Resol_Col("Color Resolution",float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
		
            CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			float _Resol;
			float _Resol_Col;
			sampler2D _MainTex;

			struct Input {
			float2 uv_MainTex;
			};
			
			float2 rand2(fixed2 uvs){
			float2 st = float2(
			dot(uvs, fixed2(12.9898,78.233)),
			dot(uvs, fixed2(259.72,184.233))
			);
			return 1.0 * frac(sin(st)*43758.5453123);
			}

			float3 rand3(fixed3 uvs){
			float3 st = float3(
			dot(uvs, fixed3(12.9898,78.233,15405.231)), 
			dot(uvs, fixed3(259.72,184.233,21405.231 + 1.0)), 
			dot(uvs, fixed3(201.72,91.233,1317.231 + 3.0))
			);
			return 1.0 * frac(sin(st)*43758.5453123);
			}


			void surf (Input IN, inout SurfaceOutputStandard o) {
				float2 st = IN.uv_MainTex * _Resol;
				float2 colst = IN.uv_MainTex * _Resol_Col;
				float2 st_i = floor(st);
				float2 st_f = frac(st);
				float2 col_i = floor(colst);
				float2 col_f = frac(colst);
				fixed3 col = fixed3(1.0,1.0,1.0);

				float dist = 10.0;
				float2 M_pos = float2(0.0,0.0);

				//タイル処理. st_iに隣接した値としての点の最小距離を出す.
				for(fixed tile_x = -1;tile_x <= 1;tile_x++){
					for(fixed tile_y = -1;tile_y <= 1;tile_y++){
					float2 tile_attr = float2(tile_x,tile_y);
					float2 pointpos = rand2(st_i + tile_attr);
					pointpos = 0.5 + 0.5 * sin(_Time.x * 20.0 + 9.30 * pointpos);

					float ptdist = length(pointpos + tile_attr - st_f);
						if(dist > ptdist){
						dist = min(dist,ptdist);
						M_pos = st_i + tile_attr;
						}
					}
				}

				col = col * length(rand2(M_pos));
				//col.r += step(0.99,st_f.x) + step(0.99,st_f.y);
				//col.b += step(.8,abs(sin(dist * 27.0)));
				o.Albedo = col;
			}

            ENDCG
    }
}
