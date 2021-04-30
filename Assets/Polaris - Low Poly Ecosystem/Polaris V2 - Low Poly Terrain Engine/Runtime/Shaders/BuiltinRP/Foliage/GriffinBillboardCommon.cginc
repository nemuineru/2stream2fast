#ifndef GRIFFIN_BILLBOARD_COMMON_INCLUDED
#define GRIFFIN_BILLBOARD_COMMON_INCLUDED

#include "UnityCG.cginc"

struct Input
{
	float2 uv_MainTex; 
	float2 imageTexcoord;
};

fixed4 _ImageTexcoords[256];
int _ImageCount;

void GetImageTexcoord(appdata_full i, inout Input IN)
{
	fixed3 normal = normalize(UnityObjectToWorldNormal(i.normal));
	fixed dotZ = dot(normal, fixed3(0,0,1));
	fixed dotX = dot(normal, fixed3(1,0,0));
	fixed rad = atan2(dotZ, dotX);
	rad = (rad + UNITY_TWO_PI) % UNITY_TWO_PI;
	fixed f = rad/UNITY_TWO_PI - 0.5/_ImageCount;
	int imageIndex = f*_ImageCount;

	fixed4 rect = _ImageTexcoords[imageIndex];
	fixed2 min = rect.xy;
	fixed2 max = rect.xy + rect.zw;

	fixed2 result = fixed2(
		lerp(min.x, max.x, i.texcoord.x),
		lerp(min.y, max.y, i.texcoord.y));
	IN.imageTexcoord = result;
}

void TreeBillboardVert(inout appdata_full i, out Input o)
{      
	UNITY_INITIALIZE_OUTPUT(Input, o);
	GetImageTexcoord(i, o);
}

#endif //GRIFFIN_BILLBOARD_COMMON_INCLUDED