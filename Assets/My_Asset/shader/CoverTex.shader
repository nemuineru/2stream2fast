Shader "Custom/CoverTex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
		_Color2("Color2",Color) = (1,1,1,1)
		_Snow("Snow", Range(0,1)) = 0.0
    }
    SubShader
    {
		Tags {}
		LOD 200

		CGPROGRAM
		#pragma surface surf ToonRamp
		#pragma target 3.0
		sampler2D _MainTex;

		struct Input{
			float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;
      		float3 viewDir;
		};
	   
		fixed2 rand(fixed3 uvs){
			fixed2 st = fixed2(dot(uvs, fixed3(12.9898,78.233,15405.231) ) , dot(uvs, fixed3(259.72,184.233,21405.231 + 1.0) ) );
			return -1.0 + 2.0 * frac(sin(st)*43758.5453123);
			}

		struct NoiseProfile{
				float2 col00;
				float2 col01;
				float2 col10;
				float2 col11;
		};

		NoiseProfile ProfileSet(fixed3 st, float offs){
				NoiseProfile Profs;
				Profs.col00 = rand(st + fixed3(0,offs,0));
				Profs.col01 = rand(st + fixed3(0,offs,1));
				Profs.col10 = rand(st + fixed3(1,offs,0));
				Profs.col11 = rand(st + fixed3(1,offs,1));
				return Profs;
		}

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

			return lerp(PointA,PointB, abs(st.y) % 1);
		}

		fixed4 _Color;
		fixed4 _Color2;
		half _Snow;

		fixed4 LightingToonRamp (SurfaceOutput s, fixed3 lightDir, fixed atten){
				half d = dot(s.Normal, lightDir) * 0.67 + 0.33; //内積によるライト設定
				fixed3 ramp = d > 0.98 ? _Color2 : lerp(s.Albedo, _Color2.rgb , d);
				fixed4 c;
				c.rgb = s.Albedo * _LightColor0.rgb * ramp; 
				//サーフェスシェーダの出力からライトモデル等を計算する.
				c.a = 0; //Alphaを0に設定しフェードさせない.
				return c;
			}

		void surf(Input IN,inout SurfaceOutput o){
			fixed3 upward = fixed3(0,1,0);
			float d = dot(IN.worldNormal, upward);
			//内積でノーマル方向と上向き方向との計算を行い、
			fixed4 c = tex2D(_MainTex,IN.uv_MainTex);
			//ノイズを加算した結果をその値として返す.
			float nz1 = 0;
			float nz2 = 0;
			fixed3 UV1 = IN.worldPos * 2.0;
			fixed3 UV2 = IN.worldNormal;
			for(int f = 1;f <= 6;f++){
				nz1 += perlinNoise(UV1) * (1.0 / (1.0+f));
				nz2 += perlinNoise(UV2) * (1.0 / (1.0+f));
				UV1 = UV1 * 2.01;
				UV2 = UV2 * 2.01;
			}
			float rim = dot(IN.worldNormal,IN.viewDir);
			c = lerp(lerp(c ,_Color * nz1 * (1 + nz2),d * _Snow),_Color2,1 - rim);
			//その線形補間を取る.

			o.Albedo = c.rgb;
			o.Alpha = 1;
		}
		ENDCG
    }
}
