Shader "Hidden/Griffin/MaskVisualizer"
{
    Properties
    {
		_Color ("Color", Color) = (1,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest LEqual
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
                #include "LivePreviewCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

			fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (
                float4 vertex : POSITION, // vertex position input
                float2 uv : TEXCOORD0, // texture coordinate input
                out float4 outpos : SV_POSITION // clip space position output
                )
            {
                v2f o; 
                o.uv = uv;
                outpos = UnityObjectToClipPos(vertex);
                return o;
            }

            fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
            {
				screenPos.xy = floor(screenPos.xy * 0.25)*0.5;
				float checker = -frac(screenPos.r + screenPos.g);
                clip(checker);

				fixed4 texColor = tex2D(_MainTex, i.uv);
				fixed4 col = float4(_Color.rgb, _Color.a*texColor.r);

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
