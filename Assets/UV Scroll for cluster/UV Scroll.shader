// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:7840,x:33868,y:32903,varname:node_7840,prsc:2|emission-1210-OUT;n:type:ShaderForge.SFN_Code,id:2585,x:31080,y:32669,varname:node_2585,prsc:2,code:ZgBsAG8AYQB0ADMAIABsAG8AYwBhAGwAUABvAHMAIAA9ACAAdwBvAHIAbABkAFAAbwBzACAALQAgACAAbQB1AGwAKAB1AG4AaQB0AHkAXwBPAGIAagBlAGMAdABUAG8AVwBvAHIAbABkACwAIABmAGwAbwBhAHQANAAoADAALAAgADAALAAgADAALAAgADEAKQApAC4AeAB5AHoAOwAKAHIAZQB0AHUAcgBuACAAbABvAGMAYQBsAFAAbwBzADsA,output:2,fname:World2Local,width:698,height:112,input:2,input_1_label:worldPos|A-1345-XYZ;n:type:ShaderForge.SFN_Tex2d,id:8586,x:33443,y:33564,ptovrint:False,ptlb:Emission Tex,ptin:_EmissionTex,varname:node_8586,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2379-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9810,x:32080,y:32686,varname:node_9810,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-2585-OUT;n:type:ShaderForge.SFN_Append,id:2900,x:32270,y:32696,varname:node_2900,prsc:2|A-9810-R,B-9810-B;n:type:ShaderForge.SFN_FragmentPosition,id:1345,x:31022,y:32412,varname:node_1345,prsc:2;n:type:ShaderForge.SFN_Time,id:1240,x:32016,y:33236,varname:node_1240,prsc:2;n:type:ShaderForge.SFN_Color,id:2934,x:33221,y:33103,ptovrint:False,ptlb:Emission Color,ptin:_EmissionColor,varname:node_2934,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1019,x:33442,y:33221,varname:node_1019,prsc:2|A-2934-RGB,B-8586-RGB;n:type:ShaderForge.SFN_Code,id:9179,x:32172,y:32982,varname:node_9179,prsc:2,code:ZgBsAG8AYQB0ADIAIABwAG8AbABhAHIAOwAKAAoAdQB2ACAAPQAgADIAIAAqACAAdQB2ACAALQAgACgAdQB2AC8AMgApADsADQAKAGYAbABvAGEAdAAgAHIAIAA9ACAAMQAgAC0AIABzAHEAcgB0ACgAdQB2AC4AeAAgACoAIAB1AHYALgB4ACAAKwAgAHUAdgAuAHkAIAAqACAAdQB2AC4AeQApADsACgBmAGwAbwBhAHQAIAB0AGgAZQB0AGEAIAA9ACAAYQB0AGEAbgAyACgAdQB2AC4AeQAsACAAdQB2AC4AeAApACAAKgAgACgAMQAvACgAMgAqADMALgAxADQAMQA1ADkAMgA2ADUAMwA1ADkAKQApADsACgAKAHAAbwBsAGEAcgAuAHgAIAA9ACAAcgA7AAoAcABvAGwAYQByAC4AeQAgAD0AIAB0AGgAZQB0AGEAOwAKAAoAcgBlAHQAdQByAG4AIABwAG8AbABhAHIAOwA=,output:1,fname:Cartesian2Polar,width:595,height:211,input:1,input_1_label:uv|A-195-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2701,x:32845,y:32989,varname:node_2701,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9179-OUT;n:type:ShaderForge.SFN_Add,id:266,x:32750,y:33348,varname:node_266,prsc:2|A-2701-R,B-8574-OUT;n:type:ShaderForge.SFN_Append,id:9942,x:32962,y:33457,varname:node_9942,prsc:2|A-266-OUT,B-9131-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5278,x:32330,y:33420,ptovrint:False,ptlb:UV Scale X by Time,ptin:_UVScaleXbyTime,varname:node_5278,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:6798,x:32330,y:33575,ptovrint:False,ptlb:UV Scale Y by Time,ptin:_UVScaleYbyTime,varname:node_6798,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:8574,x:32522,y:33348,varname:node_8574,prsc:2|A-2041-OUT,B-5278-OUT;n:type:ShaderForge.SFN_Multiply,id:8368,x:32522,y:33541,varname:node_8368,prsc:2|A-4022-OUT,B-6798-OUT;n:type:ShaderForge.SFN_Add,id:9131,x:32750,y:33529,varname:node_9131,prsc:2|A-2701-G,B-8368-OUT;n:type:ShaderForge.SFN_Tex2d,id:5027,x:33221,y:32792,ptovrint:False,ptlb:Mask Tex,ptin:_MaskTex,varname:node_5027,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9310-OUT;n:type:ShaderForge.SFN_Multiply,id:1210,x:33637,y:33004,varname:node_1210,prsc:2|A-5027-RGB,B-1019-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3127,x:32840,y:32684,ptovrint:False,ptlb:Mask UV Scale,ptin:_MaskUVScale,varname:node_3127,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9310,x:33036,y:32728,varname:node_9310,prsc:2|A-3127-OUT,B-3066-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6175,x:32270,y:32855,ptovrint:False,ptlb:Emission UV Scale,ptin:_EmissionUVScale,varname:node_6175,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:195,x:32518,y:32776,varname:node_195,prsc:2|A-6175-OUT,B-5910-OUT;n:type:ShaderForge.SFN_Vector4Property,id:1630,x:31680,y:32822,ptovrint:False,ptlb:Center Pos,ptin:_CenterPos,varname:node_1630,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Add,id:5910,x:32131,y:32390,varname:node_5910,prsc:2|A-798-OUT,B-1630-XYZ;n:type:ShaderForge.SFN_Lerp,id:2379,x:33198,y:33581,varname:node_2379,prsc:2|A-9942-OUT,B-1611-OUT,T-3378-OUT;n:type:ShaderForge.SFN_Add,id:5481,x:32750,y:33726,varname:node_5481,prsc:2|A-5606-R,B-8574-OUT;n:type:ShaderForge.SFN_Add,id:7147,x:32750,y:33877,varname:node_7147,prsc:2|A-5606-G,B-8368-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5606,x:32522,y:33758,varname:node_5606,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-195-OUT;n:type:ShaderForge.SFN_Append,id:1611,x:32962,y:33811,varname:node_1611,prsc:2|A-5481-OUT,B-7147-OUT;n:type:ShaderForge.SFN_Sin,id:1963,x:31636,y:33372,varname:node_1963,prsc:2|IN-3096-OUT;n:type:ShaderForge.SFN_Slider,id:1935,x:31859,y:33570,ptovrint:False,ptlb:Time scale mode X,ptin:_TimescalemodeX,varname:node_1935,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:2041,x:32207,y:33334,varname:node_2041,prsc:2|A-1240-T,B-5696-OUT,T-1935-OUT;n:type:ShaderForge.SFN_Power,id:5696,x:31989,y:33372,varname:node_5696,prsc:2|VAL-6015-OUT,EXP-7781-OUT;n:type:ShaderForge.SFN_Abs,id:6015,x:31814,y:33372,varname:node_6015,prsc:2|IN-1963-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7781,x:31680,y:33574,ptovrint:False,ptlb:Repeat Intensity,ptin:_RepeatIntensity,varname:node_7781,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Pi,id:5040,x:31165,y:33394,varname:node_5040,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6160,x:31313,y:33394,varname:node_6160,prsc:2|A-1240-T,B-5040-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8,x:31148,y:33556,ptovrint:False,ptlb:BPM,ptin:_BPM,varname:node_8,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:60;n:type:ShaderForge.SFN_Divide,id:338,x:31313,y:33587,varname:node_338,prsc:2|A-8-OUT,B-7719-OUT;n:type:ShaderForge.SFN_Vector1,id:7719,x:31148,y:33643,varname:node_7719,prsc:2,v1:60;n:type:ShaderForge.SFN_Multiply,id:3096,x:31486,y:33492,varname:node_3096,prsc:2|A-6160-OUT,B-338-OUT;n:type:ShaderForge.SFN_Slider,id:3378,x:33133,y:33838,ptovrint:False,ptlb:Cartesian or Polar,ptin:_CartesianorPolar,varname:node_3378,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ToggleProperty,id:3015,x:31675,y:32479,ptovrint:False,ptlb:Use LocalPos,ptin:_UseLocalPos,varname:node_3015,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_TexCoord,id:4185,x:31675,y:32300,varname:node_4185,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:798,x:31934,y:32390,varname:node_798,prsc:2|A-4185-UVOUT,B-2900-OUT,T-3015-OUT;n:type:ShaderForge.SFN_Lerp,id:4022,x:32207,y:33665,varname:node_4022,prsc:2|A-1240-T,B-5696-OUT,T-5884-OUT;n:type:ShaderForge.SFN_Slider,id:5884,x:31843,y:33811,ptovrint:False,ptlb:Time scale mode Y,ptin:_TimescalemodeY,varname:_TimescalemodeX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:3066,x:32135,y:32253,varname:node_3066,prsc:2|A-698-OUT,B-1630-XYZ;n:type:ShaderForge.SFN_Lerp,id:698,x:31934,y:32253,varname:node_698,prsc:2|A-4185-UVOUT,B-2900-OUT,T-1871-OUT;n:type:ShaderForge.SFN_ToggleProperty,id:1871,x:31675,y:32200,ptovrint:False,ptlb:Use LocalPos (MaskTex),ptin:_UseLocalPosMaskTex,varname:_UseLocalPos_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;proporder:2934-8586-6175-5027-3127-1630-3015-1871-5278-6798-1935-5884-7781-8-3378;pass:END;sub:END;*/

Shader "forCluster/UV scroll" {
    Properties {
        [HDR]_EmissionColor ("Emission Color", Color) = (0.5,0.5,0.5,1)
        _EmissionTex ("Emission Tex", 2D) = "white" {}
        _EmissionUVScale ("Emission UV Scale", Float ) = 1
        _MaskTex ("Mask Tex", 2D) = "white" {}
        _MaskUVScale ("Mask UV Scale", Float ) = 1
        _CenterPos ("Center Pos", Vector) = (0,0,0,0)
        [MaterialToggle] _UseLocalPos ("Use LocalPos", Float ) = 0
        [MaterialToggle] _UseLocalPosMaskTex ("Use LocalPos (MaskTex)", Float ) = 0
        _UVScaleXbyTime ("UV Scale X by Time", Float ) = 1
        _UVScaleYbyTime ("UV Scale Y by Time", Float ) = 0
        _TimescalemodeX ("Time scale mode X", Range(0, 1)) = 0
        _TimescalemodeY ("Time scale mode Y", Range(0, 1)) = 0
        _RepeatIntensity ("Repeat Intensity", Float ) = 1
        _BPM ("BPM", Float ) = 60
        _CartesianorPolar ("Cartesian or Polar", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma target 2.0
            float3 World2Local( float3 worldPos ){
            float3 localPos = worldPos -  mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
            return localPos;
            }
            
            uniform sampler2D _EmissionTex; uniform float4 _EmissionTex_ST;
            float2 Cartesian2Polar( float2 uv ){
            float2 polar;
            
            uv = 2 * uv - (uv/2);
            float r = 1 - sqrt(uv.x * uv.x + uv.y * uv.y);
            float theta = atan2(uv.y, uv.x) * (1/(2*3.14159265359));
            
            polar.x = r;
            polar.y = theta;
            
            return polar;
            }
            
            uniform sampler2D _MaskTex; uniform float4 _MaskTex_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _EmissionColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _UVScaleXbyTime)
                UNITY_DEFINE_INSTANCED_PROP( float, _UVScaleYbyTime)
                UNITY_DEFINE_INSTANCED_PROP( float, _MaskUVScale)
                UNITY_DEFINE_INSTANCED_PROP( float, _EmissionUVScale)
                UNITY_DEFINE_INSTANCED_PROP( float4, _CenterPos)
                UNITY_DEFINE_INSTANCED_PROP( float, _TimescalemodeX)
                UNITY_DEFINE_INSTANCED_PROP( float, _RepeatIntensity)
                UNITY_DEFINE_INSTANCED_PROP( float, _BPM)
                UNITY_DEFINE_INSTANCED_PROP( float, _CartesianorPolar)
                UNITY_DEFINE_INSTANCED_PROP( fixed, _UseLocalPos)
                UNITY_DEFINE_INSTANCED_PROP( float, _TimescalemodeY)
                UNITY_DEFINE_INSTANCED_PROP( fixed, _UseLocalPosMaskTex)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
////// Lighting:
////// Emissive:
                float _MaskUVScale_var = UNITY_ACCESS_INSTANCED_PROP( Props, _MaskUVScale );
                float3 node_9810 = World2Local( i.posWorld.rgb ).rgb;
                float2 node_2900 = float2(node_9810.r,node_9810.b);
                float _UseLocalPosMaskTex_var = UNITY_ACCESS_INSTANCED_PROP( Props, _UseLocalPosMaskTex );
                float4 _CenterPos_var = UNITY_ACCESS_INSTANCED_PROP( Props, _CenterPos );
                float3 node_9310 = (_MaskUVScale_var*(float3(lerp(i.uv0,node_2900,_UseLocalPosMaskTex_var),0.0)+_CenterPos_var.rgb));
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(node_9310, _MaskTex));
                float4 _EmissionColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _EmissionColor );
                float _EmissionUVScale_var = UNITY_ACCESS_INSTANCED_PROP( Props, _EmissionUVScale );
                float _UseLocalPos_var = UNITY_ACCESS_INSTANCED_PROP( Props, _UseLocalPos );
                float3 node_195 = (_EmissionUVScale_var*(float3(lerp(i.uv0,node_2900,_UseLocalPos_var),0.0)+_CenterPos_var.rgb));
                float2 node_2701 = Cartesian2Polar( node_195.rg ).rg;
                float4 node_1240 = _Time;
                float _BPM_var = UNITY_ACCESS_INSTANCED_PROP( Props, _BPM );
                float _RepeatIntensity_var = UNITY_ACCESS_INSTANCED_PROP( Props, _RepeatIntensity );
                float node_5696 = pow(abs(sin(((node_1240.g*3.141592654)*(_BPM_var/60.0)))),_RepeatIntensity_var);
                float _TimescalemodeX_var = UNITY_ACCESS_INSTANCED_PROP( Props, _TimescalemodeX );
                float _UVScaleXbyTime_var = UNITY_ACCESS_INSTANCED_PROP( Props, _UVScaleXbyTime );
                float node_8574 = (lerp(node_1240.g,node_5696,_TimescalemodeX_var)*_UVScaleXbyTime_var);
                float _TimescalemodeY_var = UNITY_ACCESS_INSTANCED_PROP( Props, _TimescalemodeY );
                float _UVScaleYbyTime_var = UNITY_ACCESS_INSTANCED_PROP( Props, _UVScaleYbyTime );
                float node_8368 = (lerp(node_1240.g,node_5696,_TimescalemodeY_var)*_UVScaleYbyTime_var);
                float2 node_5606 = node_195.rg;
                float _CartesianorPolar_var = UNITY_ACCESS_INSTANCED_PROP( Props, _CartesianorPolar );
                float2 node_2379 = lerp(float2((node_2701.r+node_8574),(node_2701.g+node_8368)),float2((node_5606.r+node_8574),(node_5606.g+node_8368)),_CartesianorPolar_var);
                float4 _EmissionTex_var = tex2D(_EmissionTex,TRANSFORM_TEX(node_2379, _EmissionTex));
                float3 emissive = (_MaskTex_var.rgb*(_EmissionColor_var.rgb*_EmissionTex_var.rgb));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
