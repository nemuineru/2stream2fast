Shader "Custom/CellularDistort"
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
			
			//ノイズ用構造体.
			struct NoiseProfile{
				float2 col00;
				float2 col01;
				float2 col10;
				float2 col11;
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
			
			NoiseProfile ProfileSet(fixed3 st, float offs){
				NoiseProfile Profs;
				Profs.col00 = rand3(st + fixed3(0,offs,0));
				Profs.col01 = rand3(st + fixed3(0,offs,1));
				Profs.col10 = rand3(st + fixed3(1,offs,0));
				Profs.col11 = rand3(st + fixed3(1,offs,1));
				return Profs;
			}

			//パーリンノイズ生成.
			float perlinNoise(fixed3 st){
			fixed3 p = floor(st);
            fixed2 f = frac(st.xz);
			fixed2 u = f*f*(3.0-2.0*f);
			
			NoiseProfile noiseLayer1;
			NoiseProfile noiseLayer2;

            noiseLayer1 = ProfileSet(p,0.0);
            noiseLayer2 = ProfileSet(p,1.0);

			float lerpPtsv01 = lerp(noiseLayer1.col00,noiseLayer1.col10,u.x);
			float lerpPtsv02 = lerp(noiseLayer1.col01,noiseLayer1.col11,u.x);
			

			float PointA = 
			lerp(
			lerp( dot(noiseLayer1.col00,f - fixed2(0,0)) , dot(noiseLayer1.col10,f - fixed2(1,0) ), u.x) ,
			lerp( dot(noiseLayer1.col01,f - fixed2(0,1)) , dot(noiseLayer1.col11,f - fixed2(1,1) ), u.x) ,
			u.y) + 0.5f;
			
			float PointB = 
			lerp(
			lerp( dot(noiseLayer2.col00,f - fixed2(0,0)) , dot(noiseLayer2.col10,f - fixed2(1,0) ), u.x) ,
			lerp( dot(noiseLayer2.col01,f - fixed2(0,1)) , dot(noiseLayer2.col11,f - fixed2(1,1) ), u.x) ,
			u.y) + 0.5f;

			return lerp(PointA, PointB, 0.5);
			}


			void surf (Input IN, inout SurfaceOutputStandard o) {
				float2 st = IN.uv_MainTex * _Resol;
				float2 st_i = floor(st);
				float2 st_f = frac(st);
				fixed3 col = fixed3(1.0,1.0,1.0);

				float dist = 10.0;
				float2 M_pos = float2(0.0,0.0);

				float3 st3s = float3(st.x,1,st.y) * _Resol_Col;
				float2 distortions = float2(0.0,0.0);
				float amp = 1.0;
				for(int resol = 0;resol < 8;resol++){
					distortions += amp * (-float2(0.5,0.5) + float2(perlinNoise(st3s),perlinNoise(st3s + 1.0)));
					st3s *= 2.01;
					amp *= 0.5;
				}

				//タイル処理. st_iに隣接した値としての点の最小距離を出す.
				for(fixed tile_x = -1;tile_x <= 1;tile_x++){
					for(fixed tile_y = -1;tile_y <= 1;tile_y++){
					float2 tile_attr = float2(tile_x,tile_y);
					float2 pointpos = rand2(st_i + tile_attr);
					pointpos = 0.5 + 0.5 * sin(_Time.x * 20.0 + 9.30 * pointpos);

					float ptdist = length(pointpos + tile_attr - st_f - distortions);
						if(dist > ptdist){
						dist = min(dist,ptdist);
						M_pos = st_i + tile_attr;
						}
					}
				}

				//col = col * dist;
				col = col * length(rand2(M_pos));
				//col.r += step(0.99,st_f.x) + step(0.99,st_f.y);
				col.b += step(.8,abs(sin(dist * 27.0)));
				o.Albedo = col;
			}

            ENDCG
    }
}
