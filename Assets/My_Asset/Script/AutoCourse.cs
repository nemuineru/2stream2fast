using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCourse : MonoBehaviour
{
    public List<Waypoint> waypoints;
    // Start is called before the first frame update
    void Awake()
    {
        //キュービックベジエ曲線でコースを生成.
        for (int i = 0; i < waypoints.Count; i++)
        {

            //接線方向、グローバルポジションを想定する.
            Vector3 towards, Pos;
            Vector3 WPoint1, WPoint2, WPoint1_RHandle, WPoint2_LHandle;
            Waypoint FirstWayPt = waypoints[i];
            Waypoint SecondWayPt;
            if (i != waypoints.Count - 1)
            {
                SecondWayPt = waypoints[i + 1];
            }
            else
            {
                SecondWayPt = waypoints[0];
            }

            if (FirstWayPt.isHandleMirror)
            {
                FirstWayPt.RHandle = -FirstWayPt.LHandle;
            }
            if (SecondWayPt.isHandleMirror)
            {
                SecondWayPt.RHandle = -SecondWayPt.RHandle;
            }

            WPoint1 = FirstWayPt.transform.position;
            WPoint1_RHandle = FirstWayPt.transform.position + FirstWayPt.RHandle;
            WPoint2_LHandle = SecondWayPt.transform.position + SecondWayPt.LHandle;
            WPoint2 = SecondWayPt.transform.position;

            for (int x = 0; x < FirstWayPt.Resolution; x++)
            {
                //助長だけど分かりやすく接線や
                float f = (float)x / FirstWayPt.Resolution;
                float t_1 = f;
                float t_2 = 1 - f;
                Vector3 lerpPos1 = t_1 * WPoint1 + t_2 * WPoint1_RHandle;
                Vector3 lerpPos2 = t_1 * WPoint1_RHandle + t_2 * WPoint2_LHandle;
                Vector3 lerpPos3 = t_1 * WPoint2_LHandle + t_2 * WPoint2;
                Vector3 QuadPos1 = t_1 * lerpPos1 + t_2 * lerpPos2;
                Vector3 QuadPos2 = t_1 * lerpPos2 + t_2 * lerpPos3;
                Pos = t_1 * QuadPos1 + t_2 * QuadPos2;
                towards = (QuadPos2 - QuadPos1).normalized;

                //内と外にポールやチェックポイントを置く.
                Vector3 OutPosMath = Quaternion.Euler(0f, 90, 0f) * new Vector3(towards.x,0,towards.z);
                Vector3 InsidePosMath = Quaternion.Euler(0f, -90, 0f) * new Vector3(towards.x, 0, towards.z);

                Vector3 OutPlace = OutPosMath.normalized * ((1 - f) * FirstWayPt.Width + f * SecondWayPt.Width) + Pos;
                Vector3 InsidePlace = InsidePosMath.normalized * ((1 - f) * FirstWayPt.Width + f * SecondWayPt.Width) + Pos;
                if (FirstWayPt.Checkpoint != null && x == 0)
                {
                    Instantiate(FirstWayPt.Checkpoint, OutPlace,Quaternion.identity);
                    Instantiate(FirstWayPt.Checkpoint, InsidePlace, Quaternion.identity);
                }
                else if (FirstWayPt.Pole != null) {
                    Instantiate(FirstWayPt.Pole, OutPlace, Quaternion.identity);
                    Instantiate(FirstWayPt.Pole, InsidePlace, Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
