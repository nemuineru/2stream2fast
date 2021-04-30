Shader "Custom/Noise"
{
    Properties
    {
        _MainTex ("Texture1", 2D) = "white" {}
        _SubTex ("Texture2", 2D) = "white" {}
		_RandomNum ("random Number",float) = 1.0
		_Layerlength ("Y-Layer Length",float) = 0.5
		_ChangeSpeed ("Changing Speeds",float) = 0.0
		_Fix ("size",float) = 0.0
		_Resolution("Resolution", Range(1,8)) = 1 
		_Intensity("Intensity", float) =  0.0
		_CutoffLv("Cut Off", float) =  1.0

    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 200
		
            CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			float _RandomNum;
			float _Fix;
			float _ChangeSpeed;
			float _Layerlength;
			float _Intensity;
			float _CutoffLv;
			int _Resolution;

			struct Input {
			float2 uv_MainTex;
			float2 uv_SubTex;
			float3 worldPos;
			};
			
			struct NoiseProfile{
				float2 col00;
				float2 col01;
				float2 col10;
				float2 col11;
			};
			
			fixed2 rand(fixed3 uvs){
			fixed2 st = fixed2(dot(uvs, fixed3(12.9898,78.233,_RandomNum) ) , dot(uvs, fixed3(259.72,184.233,_RandomNum + 1.0) ) );
			return -1.0 + 2.0 * frac(sin(st)*43758.5453123);
			}
			
			NoiseProfile ProfileSet(fixed3 st, float offs){
				NoiseProfile Profs;
				Profs.col00 = rand(st + fixed3(0,offs,0));
				Profs.col01 = rand(st + fixed3(0,offs,1));
				Profs.col10 = rand(st + fixed3(1,offs,0));
				Profs.col11 = rand(st + fixed3(1,offs,1));
				return Profs;
			}

			sampler2D _MainTex;
			sampler2D _SubTex;

			float perlinNoise(fixed3 st){
			fixed3 p = floor(st);
            fixed2 f = frac(st.xz);
			fixed2 u = f*f*(3.0-2.0*f);
			
			NoiseProfile noiseLayer1;
			NoiseProfile noiseLayer2;

            noiseLayer1 = ProfileSet(p,0.0);
            noiseLayer2 = ProfileSet(p,_Layerlength);

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

			return lerp(PointA,PointB, abs(st.y % _Layerlength)/_Layerlength);
			}

			void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed3 UV1 = IN.worldPos * _Fix + fixed3(0.0 ,(fixed)_Time * _ChangeSpeed ,0.0);
			float c = 0;
			for(int f = 1;f <= _Resolution;f++){
				c += perlinNoise(UV1) * (1.0 / (1.0+f));
				UV1 = UV1 * 2.01;
			}
			c = c > _Intensity ? 1 : 1 + (c - pow(_Intensity,_CutoffLv));

			o.Albedo = fixed4(lerp( tex2D(_MainTex, IN.uv_MainTex),tex2D(_SubTex, IN.uv_SubTex),1 - c * c));
			}
            ENDCG
    }
	FallBack "Diffuse"
}
