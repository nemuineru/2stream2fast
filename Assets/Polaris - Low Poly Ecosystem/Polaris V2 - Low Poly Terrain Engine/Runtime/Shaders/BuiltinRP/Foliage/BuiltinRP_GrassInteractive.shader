// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Polaris/BuiltinRP/Foliage/GrassInteractive"
{
	Properties
	{
		_NoiseTex("_NoiseTex", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_Occlusion("Occlusion", Range( 0 , 1)) = 0.2
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.5
		[HideInInspector]_BendFactor("BendFactor", Float) = 1
		_WaveDistance("Wave Distance", Float) = 0.1
		_Wind("Wind", Vector) = (1,1,4,8)
		_VectorField("VectorField", 2D) = "gray" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "DisableBatching" = "True" }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NoiseTex;
		uniform float4 _Wind;
		uniform float _WaveDistance;
		uniform float _BendFactor;
		uniform sampler2D _VectorField;
		uniform float4x4 _WorldToNormalized;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Occlusion;
		uniform float _AlphaCutoff;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_vertex4Pos = v.vertex;
			float4 _VertexPos3_g8 = ase_vertex4Pos;
			float4 transform15_g8 = mul(unity_ObjectToWorld,_VertexPos3_g8);
			float2 appendResult22_g8 = (float2(transform15_g8.x , transform15_g8.z));
			float2 worldPosXZ21_g8 = appendResult22_g8;
			float _WindDirX11 = _Wind.x;
			float _WindDirX5_g8 = _WindDirX11;
			float _WindDirY12 = _Wind.y;
			float _WindDirY7_g8 = _WindDirY12;
			float2 appendResult19_g8 = (float2(_WindDirX5_g8 , _WindDirY7_g8));
			float _WindSpeed13 = _Wind.z;
			float _WindSpeed9_g8 = _WindSpeed13;
			float _WindSpread14 = _Wind.w;
			float _WindSpread10_g8 = _WindSpread14;
			float2 noisePos32_g8 = ( ( worldPosXZ21_g8 - ( appendResult19_g8 * _WindSpeed9_g8 * _Time.y ) ) / _WindSpread10_g8 );
			float temp_output_35_0_g8 = ( tex2Dlod( _NoiseTex, float4( noisePos32_g8, 0, 0.0) ).r * v.texcoord.xy.y );
			float _WaveDistance9 = _WaveDistance;
			float _WaveDistance12_g8 = _WaveDistance9;
			float _BendFactor71 = _BendFactor;
			float _BendFactor38_g8 = _BendFactor71;
			float4 appendResult42_g8 = (float4(_WindDirX5_g8 , ( temp_output_35_0_g8 * 0.5 ) , _WindDirY7_g8 , 0.0));
			float4 transform47_g8 = mul(unity_WorldToObject,( temp_output_35_0_g8 * _WaveDistance12_g8 * _BendFactor38_g8 * appendResult42_g8 ));
			float4 vertexOffset48_g8 = transform47_g8;
			float4 windVertexOffset62 = vertexOffset48_g8;
			float4 _VertexPos3_g3 = ase_vertex4Pos;
			float4 transform12_g3 = mul(unity_ObjectToWorld,_VertexPos3_g3);
			float4x4 myLocalVar55_g3 = _WorldToNormalized;
			float4x4 _WorldToNormalized8_g3 = myLocalVar55_g3;
			float4 break28_g3 = mul( transform12_g3, _WorldToNormalized8_g3 );
			float4 appendResult29_g3 = (float4(break28_g3.x , break28_g3.z , 0.0 , 0.0));
			float4 vectorFieldUV30_g3 = appendResult29_g3;
			float4 bendVector33_g3 = tex2Dlod( _VectorField, float4( vectorFieldUV30_g3.xy, 0, 0.0) );
			float4 break36_g3 = bendVector33_g3;
			float4 appendResult43_g3 = (float4(( ( break36_g3.r * 2.0 ) - 1.0 ) , ( ( break36_g3.g * 2.0 ) - 1.0 ) , ( ( break36_g3.b * 2.0 ) - 1.0 ) , 0.0));
			float4 remappedBendVector44_g3 = appendResult43_g3;
			float _BendFactor51_g3 = _BendFactor71;
			float4 vertexOffset52_g3 = ( remappedBendVector44_g3 * v.texcoord.xy.y * _BendFactor51_g3 );
			float4 touchBendingOffset83 = vertexOffset52_g3;
			float4 break87 = ( windVertexOffset62 + touchBendingOffset83 );
			float4 appendResult88 = (float4(break87.x , max( break87.y , 0.0 ) , break87.z , break87.w));
			float4 totalVertexOffset93 = appendResult88;
			v.vertex.xyz += totalVertexOffset93.xyz;
			float3 vertexNormal55 = float3(0,1,0);
			v.normal = vertexNormal55;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 _Color5 = _Color;
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 temp_output_24_0 = ( _Color5 * tex2D( _MainTex, uv0_MainTex ) );
			float _Occlusion18 = _Occlusion;
			float lerpResult33 = lerp( 0.0 , _Occlusion18 , ( ( 1.0 - i.uv_texcoord.y ) * ( 1.0 - i.uv_texcoord.y ) ));
			float4 albedoColor40 = ( temp_output_24_0 - float4( ( 0.5 * float3(1,1,1) * lerpResult33 ) , 0.0 ) );
			o.Albedo = albedoColor40.rgb;
			float alpha45 = temp_output_24_0.a;
			float temp_output_46_0 = alpha45;
			o.Alpha = temp_output_46_0;
			clip( temp_output_46_0 - _AlphaCutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Lambert keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
610;682;2328;679;3314.573;-1977.86;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;19;-2317.74,-2166.847;Inherit;False;678.778;1456.974;;19;71;70;15;16;5;18;4;17;7;6;11;12;13;9;81;14;10;80;8;Properties;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-2153.813,-808.1543;Float;False;Property;_BendFactor;BendFactor;6;1;[HideInInspector];Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;80;-2192.371,-1718.896;Float;True;Property;_VectorField;VectorField;9;0;Create;True;0;0;False;0;False;None;None;False;gray;LockedToTexture2D;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.Vector4Node;10;-2169.615,-1363.771;Float;False;Property;_Wind;Wind;8;0;Create;True;0;0;False;0;False;1,1,4,8;1,1,7,7;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-2168.615,-1464.77;Float;False;Property;_WaveDistance;Wave Distance;7;0;Create;True;0;0;False;0;False;0.1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;71;-1893.813,-804.1543;Float;False;_BendFactor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;75;-2991.674,1324.012;Inherit;False;1377.313;1322.777;;23;93;88;90;89;92;91;87;86;85;84;83;82;79;55;62;54;74;72;68;65;63;64;66;Vertex Animation, Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;81;-1880.192,-1716.869;Float;False;_VectorField;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-1882.614,-1356.771;Float;False;_WindDirX;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;41;-3517.229,-256.6267;Inherit;False;1899.109;1469.351;;6;40;39;37;38;44;45;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1882.614,-1199.772;Float;False;_WindSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-1884.614,-1118.772;Float;False;_WindSpread;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1882.614,-1281.771;Float;False;_WindDirY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-1881.614,-1463.77;Float;False;_WaveDistance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;-2941.674,1887.739;Inherit;False;9;_WaveDistance;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;-2927.726,2100.926;Inherit;False;81;_VectorField;1;0;OBJECT;0;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-2924.846,2193.444;Inherit;False;71;_BendFactor;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;64;-2918.674,1627.739;Inherit;False;12;_WindDirY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;-2932.674,1801.739;Inherit;False;14;_WindSpread;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-2922.274,1547.266;Inherit;False;11;_WindDirX;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;-2924.889,1976.733;Inherit;False;71;_BendFactor;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;-2928.674,1706.739;Inherit;False;13;_WindSpeed;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;37;-3415.318,290.2566;Inherit;False;1270.362;876.2831;;7;35;34;33;30;32;31;36;Occlusion;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;36;-3395.857,845.9573;Inherit;False;711;293;;4;26;27;28;29;Occlusion factor;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;113;-2208.612,2157.495;Inherit;False;GrassTouchBending;10;;3;de9a62c6bacea6b4888ce2cdd0a3d3f8;0;3;2;FLOAT4;0,0,0,0;False;4;SAMPLER2D;;False;50;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;74;-2539.172,1585.766;Inherit;True;GrassWindAnimation;0;;8;8d39a13fc2a7a164fa1708057ff071d3;0;7;1;FLOAT4;0,0,0,0;False;51;FLOAT;1;False;52;FLOAT;1;False;53;FLOAT;7;False;54;FLOAT;7;False;55;FLOAT;0.2;False;56;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;6;-2190.384,-1929.848;Float;True;Property;_MainTex;MainTex;3;0;Create;True;0;0;False;0;False;None;None;False;white;LockedToTexture2D;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;62;-2186.276,1573.266;Float;False;windVertexOffset;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;38;-3109.156,-135.5909;Inherit;False;957;392;;5;21;22;23;25;24;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-2263.971,-909.8423;Float;False;Property;_Occlusion;Occlusion;4;0;Create;True;0;0;False;0;False;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-3345.857,910.957;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1884.383,-1928.848;Float;False;_MainTex;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;83;-1876.468,2157.643;Float;False;touchBendingOffset;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;28;-3065.857,1028.956;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;27;-3060.857,895.9569;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-2955.059,2406.24;Inherit;False;83;touchBendingOffset;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;4;-2174.384,-2115.848;Float;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;84;-2943.238,2316.919;Inherit;False;62;windVertexOffset;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-3011.407,8.565916;Inherit;False;7;_MainTex;1;0;OBJECT;0;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-1886.246,-907.3323;Float;False;_Occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-2853.856,953.9564;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-2931.117,686.2404;Inherit;False;18;_Occlusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-2847.918,578.3406;Float;False;Constant;_Float1;Float 1;6;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-3059.156,100.409;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-2612.219,2353.699;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;5;-1885.383,-2116.848;Float;False;_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;21;-2705.05,8.684111;Inherit;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;32;-2659.255,452.257;Float;False;Constant;_Vector1;Vector 1;6;0;Create;True;0;0;False;0;False;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;87;-2462.538,2350.519;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;33;-2645.116,634.2404;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2634.255,340.2565;Float;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-2606.156,-85.59092;Inherit;False;5;_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-2321.157,-8.590945;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-2289.256,500.2567;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;89;-2180.767,2380.926;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;92;-2148.115,2512.909;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;91;-2147.102,2557.506;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;90;-2151.156,2325.397;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;88;-2025.692,2349.507;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;44;-2110.054,-11.27011;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.Vector3Node;54;-2400.55,1374.709;Float;False;Constant;_Up;Up;7;0;Create;True;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-2025.438,246.9899;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;55;-2180.177,1374.012;Float;False;vertexNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-1847.459,246.99;Float;False;albedoColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-1844.054,53.72989;Float;False;alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-1861.78,2343.466;Float;False;totalVertexOffset;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;-373.3654,450.1091;Inherit;False;55;vertexNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;-371.1169,354.3228;Inherit;False;93;totalVertexOffset;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-2263.971,-1012.744;Float;False;Property;_AlphaCutoff;Alpha Cutoff;5;0;Create;True;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-1884.992,-1011.489;Float;False;_AlphaCutoff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-342.2595,180.5466;Inherit;False;45;alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-367.3995,-0.407074;Inherit;False;40;albedoColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;110;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;Polaris/BuiltinRP/Foliage/GrassInteractive;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;15;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;71;0;70;0
WireConnection;81;0;80;0
WireConnection;11;0;10;1
WireConnection;13;0;10;3
WireConnection;14;0;10;4
WireConnection;12;0;10;2
WireConnection;9;0;8;0
WireConnection;113;4;79;0
WireConnection;113;50;82;0
WireConnection;74;51;63;0
WireConnection;74;52;64;0
WireConnection;74;53;65;0
WireConnection;74;54;66;0
WireConnection;74;55;68;0
WireConnection;74;56;72;0
WireConnection;62;0;74;0
WireConnection;7;0;6;0
WireConnection;83;0;113;0
WireConnection;28;0;26;2
WireConnection;27;0;26;2
WireConnection;18;0;17;0
WireConnection;29;0;27;0
WireConnection;29;1;28;0
WireConnection;23;2;22;0
WireConnection;86;0;84;0
WireConnection;86;1;85;0
WireConnection;5;0;4;0
WireConnection;21;0;22;0
WireConnection;21;1;23;0
WireConnection;87;0;86;0
WireConnection;33;0;34;0
WireConnection;33;1;35;0
WireConnection;33;2;29;0
WireConnection;24;0;25;0
WireConnection;24;1;21;0
WireConnection;30;0;31;0
WireConnection;30;1;32;0
WireConnection;30;2;33;0
WireConnection;89;0;87;1
WireConnection;92;0;87;2
WireConnection;91;0;87;3
WireConnection;90;0;87;0
WireConnection;88;0;90;0
WireConnection;88;1;89;0
WireConnection;88;2;92;0
WireConnection;88;3;91;0
WireConnection;44;0;24;0
WireConnection;39;0;24;0
WireConnection;39;1;30;0
WireConnection;55;0;54;0
WireConnection;40;0;39;0
WireConnection;45;0;44;3
WireConnection;93;0;88;0
WireConnection;16;0;15;0
WireConnection;110;0;42;0
WireConnection;110;9;46;0
WireConnection;110;10;46;0
WireConnection;110;11;73;0
WireConnection;110;12;59;0
ASEEND*/
//CHKSM=39B977D95F11A0DA56ADE8E08F89596462D0A55E