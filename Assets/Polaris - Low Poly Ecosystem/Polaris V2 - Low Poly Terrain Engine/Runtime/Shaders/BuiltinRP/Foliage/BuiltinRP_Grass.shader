// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Polaris/BuiltinRP/Foliage/Grass"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_NoiseTex("_NoiseTex", 2D) = "white" {}
		_MainTex("MainTex", 2D) = "white" {}
		_Occlusion("Occlusion", Range( 0 , 1)) = 0.2
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.5
		[HideInInspector]_BendFactor("Bend Factor", Float) = 1
		_WaveDistance("Wave Distance", Float) = 0.1
		_Wind("Wind", Vector) = (1,1,4,8)
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
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Occlusion;
		uniform float _AlphaCutoff;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_vertex4Pos = v.vertex;
			float4 _VertexPos3_g3 = ase_vertex4Pos;
			half4 transform15_g3 = mul(unity_ObjectToWorld,_VertexPos3_g3);
			half2 appendResult22_g3 = (half2(transform15_g3.x , transform15_g3.z));
			float2 worldPosXZ21_g3 = appendResult22_g3;
			float _WindDirX11 = _Wind.x;
			float _WindDirX5_g3 = _WindDirX11;
			float _WindDirY12 = _Wind.y;
			float _WindDirY7_g3 = _WindDirY12;
			half2 appendResult19_g3 = (half2(_WindDirX5_g3 , _WindDirY7_g3));
			float _WindSpeed13 = _Wind.z;
			float _WindSpeed9_g3 = _WindSpeed13;
			float _WindSpread14 = _Wind.w;
			float _WindSpread10_g3 = _WindSpread14;
			float2 noisePos32_g3 = ( ( worldPosXZ21_g3 - ( appendResult19_g3 * _WindSpeed9_g3 * _Time.y ) ) / _WindSpread10_g3 );
			half temp_output_35_0_g3 = ( tex2Dlod( _NoiseTex, float4( noisePos32_g3, 0, 0.0) ).r * v.texcoord.xy.y );
			float _WaveDistance9 = _WaveDistance;
			float _WaveDistance12_g3 = _WaveDistance9;
			float _BendFactor71 = _BendFactor;
			float _BendFactor38_g3 = _BendFactor71;
			half4 appendResult42_g3 = (half4(_WindDirX5_g3 , ( temp_output_35_0_g3 * 0.5 ) , _WindDirY7_g3 , 0.0));
			half4 transform47_g3 = mul(unity_WorldToObject,( temp_output_35_0_g3 * _WaveDistance12_g3 * _BendFactor38_g3 * appendResult42_g3 ));
			float4 vertexOffset48_g3 = transform47_g3;
			float4 vertexOffset62 = vertexOffset48_g3;
			v.vertex.xyz += vertexOffset62.xyz;
			float3 vertexNormal55 = float3(0,1,0);
			v.normal = vertexNormal55;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 _Color5 = _Color;
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			half4 temp_output_24_0 = ( _Color5 * tex2D( _MainTex, uv0_MainTex ) );
			float _Occlusion18 = _Occlusion;
			half lerpResult33 = lerp( 0.0 , _Occlusion18 , ( ( 1.0 - i.uv_texcoord.y ) * ( 1.0 - i.uv_texcoord.y ) ));
			float4 albedoColor40 = ( temp_output_24_0 - half4( ( 0.5 * float3(1,1,1) * lerpResult33 ) , 0.0 ) );
			o.Albedo = albedoColor40.rgb;
			o.Alpha = 1.0;
			float alpha45 = temp_output_24_0.a;
			clip( alpha45 - _AlphaCutoff );
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
779;138;1906;885;3642.052;-1044.644;1.406114;True;False
Node;AmplifyShaderEditor.CommentaryNode;41;-3517.229,-256.6267;Inherit;False;1899.109;1469.351;;6;40;39;37;38;44;45;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;37;-3415.318,290.2566;Inherit;False;1270.362;876.2831;;7;35;34;33;30;32;31;36;Occlusion;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;19;-2325.277,-1703.304;Inherit;False;667.3564;1224.44;;17;71;70;9;11;14;13;12;10;8;5;4;18;17;7;6;78;77;Properties;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;36;-3395.857,845.9573;Inherit;False;711;293;;4;26;27;28;29;Occlusion factor;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;6;-2197.921,-1466.304;Float;True;Property;_MainTex;MainTex;3;0;Create;True;0;0;False;0;False;None;None;False;white;LockedToTexture2D;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1891.92,-1465.304;Float;False;_MainTex;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-2275.277,-689.3755;Float;False;Property;_Occlusion;Occlusion;4;0;Create;True;0;0;False;0;False;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;38;-3109.156,-135.5909;Inherit;False;957;392;;5;21;22;23;25;24;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-3345.857,910.957;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;27;-3060.857,895.9569;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-2181.921,-1652.304;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;22;-3011.407,8.565916;Inherit;False;7;_MainTex;1;0;OBJECT;0;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.OneMinusNode;28;-3065.857,1028.956;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-1897.552,-686.8655;Float;False;_Occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-2847.918,578.3406;Float;False;Constant;_Float1;Float 1;6;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-2169.119,-577.6877;Float;False;Property;_BendFactor;Bend Factor;6;1;[HideInInspector];Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;10;-2180.921,-1143.305;Float;False;Property;_Wind;Wind;8;0;Create;True;0;0;False;0;False;1,1,4,8;1,1,7,7;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-2179.921,-1244.304;Float;False;Property;_WaveDistance;Wave Distance;7;0;Create;True;0;0;False;0;False;0.1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-2931.117,686.2404;Inherit;False;18;_Occlusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;5;-1892.92,-1653.304;Float;False;_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-3059.156,100.409;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-2853.856,953.9564;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-1895.92,-898.3049;Float;False;_WindSpread;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1893.92,-1061.305;Float;False;_WindDirY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2634.255,340.2565;Float;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-2606.156,-85.59092;Inherit;False;5;_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-1892.92,-1243.304;Float;False;_WaveDistance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1893.92,-979.3051;Float;False;_WindSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;75;-2675.559,1405.766;Inherit;False;1054.499;767.7213;;9;68;66;64;65;72;63;54;55;62;Vertex Animation, Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;21;-2705.05,8.684111;Inherit;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;32;-2659.255,452.257;Float;False;Constant;_Vector1;Vector 1;6;0;Create;True;0;0;False;0;False;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;71;-1896.119,-575.6877;Float;False;_BendFactor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;33;-2645.116,634.2404;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-1893.92,-1136.305;Float;False;_WindDirX;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;-2616.559,1883.493;Inherit;False;14;_WindSpread;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;-2625.559,1969.493;Inherit;False;9;_WaveDistance;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;-2608.774,2058.487;Inherit;False;71;_BendFactor;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;-2612.559,1788.493;Inherit;False;13;_WindSpeed;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-2321.157,-8.590945;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-2289.256,500.2567;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-2606.159,1629.02;Inherit;False;11;_WindDirX;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;64;-2602.559,1709.493;Inherit;False;12;_WindDirY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;85;-2221.365,1665.604;Inherit;True;GrassWindAnimation;1;;3;8d39a13fc2a7a164fa1708057ff071d3;0;7;1;FLOAT4;0,0,0,0;False;51;FLOAT;1;False;52;FLOAT;1;False;53;FLOAT;7;False;54;FLOAT;7;False;55;FLOAT;0.2;False;56;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-2025.438,246.9899;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;54;-2084.435,1456.463;Float;False;Constant;_Up;Up;7;0;Create;True;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;44;-2110.054,-11.27011;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-1844.054,53.72989;Float;False;alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-1847.459,246.99;Float;False;albedoColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;55;-1864.062,1455.766;Float;False;vertexNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;62;-1870.16,1655.02;Float;False;vertexOffset;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;-373.3654,450.1091;Inherit;False;55;vertexNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-489.2595,126.5466;Inherit;False;45;alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-256.9694,146.2815;Inherit;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-1889.44,-797.708;Float;False;_AlphaCutoff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-367.3995,-0.407074;Inherit;False;40;albedoColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-2262.44,-810.708;Float;False;Property;_AlphaCutoff;Alpha Cutoff;5;0;Create;True;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;43;-358.0536,257.7299;Inherit;False;45;alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;-371.1169,354.3228;Inherit;False;62;vertexOffset;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;76;0,0;Half;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;Polaris/BuiltinRP/Foliage/Grass;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;Opaque;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;77;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;6;0
WireConnection;27;0;26;2
WireConnection;28;0;26;2
WireConnection;18;0;17;0
WireConnection;5;0;4;0
WireConnection;23;2;22;0
WireConnection;29;0;27;0
WireConnection;29;1;28;0
WireConnection;14;0;10;4
WireConnection;12;0;10;2
WireConnection;9;0;8;0
WireConnection;13;0;10;3
WireConnection;21;0;22;0
WireConnection;21;1;23;0
WireConnection;71;0;70;0
WireConnection;33;0;34;0
WireConnection;33;1;35;0
WireConnection;33;2;29;0
WireConnection;11;0;10;1
WireConnection;24;0;25;0
WireConnection;24;1;21;0
WireConnection;30;0;31;0
WireConnection;30;1;32;0
WireConnection;30;2;33;0
WireConnection;85;51;63;0
WireConnection;85;52;64;0
WireConnection;85;53;65;0
WireConnection;85;54;66;0
WireConnection;85;55;68;0
WireConnection;85;56;72;0
WireConnection;39;0;24;0
WireConnection;39;1;30;0
WireConnection;44;0;24;0
WireConnection;45;0;44;3
WireConnection;40;0;39;0
WireConnection;55;0;54;0
WireConnection;62;0;85;0
WireConnection;78;0;77;0
WireConnection;76;0;42;0
WireConnection;76;9;79;0
WireConnection;76;10;43;0
WireConnection;76;11;73;0
WireConnection;76;12;59;0
ASEEND*/
//CHKSM=02F98EBA0CE6B74DA99B441EF09398A49903A5BF