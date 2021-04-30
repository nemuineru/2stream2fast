Shader "Custom/Cellular Noise"
{
    Properties
    {
        _Nums ("Num", int) = 10
		_RandomNum("Rand",float) = 130.0
		_Rest("Rest",float) = 0.01
		[Toggle(Invert)]
        _Invert ("Invert", float) = 0.01
		_Brightness("Brightness", Range(0.0, 1.0)) = 0.5
		_Color("Color",Color) = (1,1,1,1)
		_Color2("Color2",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma shader_feature Invert

            #include "UnityCG.cginc"

			int _Nums;
			float _RandomNum;
			float _Rest;
			fixed _Brightness;
			fixed4 _Color;
			fixed4 _Color2;

			fixed2 rand(fixed3 uvs){
			fixed2 st = fixed2(dot(uvs, fixed3(12.9898,78.233,_RandomNum) ) , dot(uvs, fixed3(259.72,184.233,_RandomNum + 1.0) ) );
			return frac(sin(st)*43758.5453123);
			}

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed2 st = i.uv;
				float dist = 10.0;
				for(int f = 0;f < _Nums ; f++)
				{
				fixed3 randnums = fixed3(0,0.5 + f,0);
				fixed2 rands = rand(randnums);
				dist = min(dist,distance(st,rands));
				}
                // sample the texture
				float Gray = _Brightness * dist;
				#ifdef Invert
					float4 col = Gray * Gray > _Rest ? _Color : _Color2;
				#else
					float4 col = Gray * Gray < _Rest ? _Color : _Color2;
				#endif
                return col;
            }
            ENDCG
        }
    }
}
