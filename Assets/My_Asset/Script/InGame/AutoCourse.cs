using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AutoCourse : MonoBehaviour
{
    [HideInInspector]
    static public AutoCourse instance;
    public bool isCourseRanked = false;

    public string CourseName;
    public int MaxLaps = 3;
    public List<Waypoint> waypoints;
    [ReadOnly]
    public Waypoint RestartPoint;
    [ReadOnly]
    public Waypoint PrefavablePoint;
    [ReadOnly]
    public int Laps = 0;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for(int i = 0;i < waypoints.Count;i++)
        {
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
                Vector3 OutPosMath = Quaternion.Euler(0f, 90, 0f) * new Vector3(towards.x, 0, towards.z);
                Vector3 InsidePosMath = Quaternion.Euler(0f, -90, 0f) * new Vector3(towards.x, 0, towards.z);

                Vector3 OutPlace = OutPosMath.normalized * ((f) * FirstWayPt.Width + (1 - f) * SecondWayPt.Width) + Pos;
                Vector3 InsidePlace = InsidePosMath.normalized * ((f) * FirstWayPt.Width + (1 - f) * SecondWayPt.Width) + Pos;

                Gizmos.DrawLine(InsidePlace,OutPlace);
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
        PrefavablePoint = waypoints[1];
        GameSystem_InGame.instance.name = CourseName;
        GameSystem_InGame.instance.CourseName = CourseName;
        GameSystem_InGame.instance.MaxLap = MaxLaps;
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

                Vector3 OutPlace = OutPosMath.normalized * ((f) * FirstWayPt.Width + (1 - f) * SecondWayPt.Width) + Pos;
                Vector3 InsidePlace = InsidePosMath.normalized * ((f) * FirstWayPt.Width + (1 - f) * SecondWayPt.Width) + Pos;
                if (x == 0) {
                    FirstWayPt.InterPoint = InsidePlace;
                    FirstWayPt.OuterPoint = OutPlace;
                }
                if (FirstWayPt.Checkpoint != null && x == 0)
                {
                    Instantiate(FirstWayPt.Checkpoint, OutPlace,Quaternion.identity,transform);
                    Instantiate(FirstWayPt.Checkpoint, InsidePlace, Quaternion.identity, transform);
                }
                else if (FirstWayPt.Pole != null) {
                    Instantiate(FirstWayPt.Pole, OutPlace, Quaternion.identity, transform);
                    Instantiate(FirstWayPt.Pole, InsidePlace, Quaternion.identity, transform);
                }

                Line line = new Line
                {
                    Pos1 = OutPlace,
                    Pos2 = InsidePlace,
                    isPassed = false
                };

                waypoints[i].Lines.Add(line);
            }
        }
        //初期のポイントを設定｡
        RestartPoint = waypoints[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameSystem_InGame.instance.isGameStarted && !GameSystem_InGame.instance.isGameEnded)
        {
            Vector2 outvec;
            Vector3 transpos = GameSystem_InGame.instance.Craft.transform.position;
            Vector3 transVec = (GameSystem_InGame.instance.Craft.rigidBody.velocity +
                GameSystem_InGame.instance.Craft.transform.forward) * Time.deltaTime;
            //チェックポイント判定.
            for (int x = 0; x < waypoints.Capacity; x++)
            {
                Waypoint point = waypoints[x];
                Waypoint nextPoint;
                if (x != waypoints.Count - 1)
                {
                    nextPoint = waypoints[x + 1];
                }
                else
                {
                    nextPoint = waypoints[0];
                }
                for (int i = 0; i < point.Lines.Count; i++)
                {
                    if (LineSegmentsIntersection(V3toXZ(transpos), V3toXZ(transpos + transVec), V3toXZ(point.Lines[i].Pos1), V3toXZ(point.Lines[i].Pos2), out outvec))
                    {
                        point.Lines[i].isPassed = true;
                    }
                }
                point.passedPercentage = ((float)point.Lines.FindAll(n => n.isPassed == true).Count / point.Lines.Count()) * 100f;
                Debug.DrawLine(nextPoint.InterPoint + Vector3.up / 2, nextPoint.OuterPoint + Vector3.up / 2, Color.cyan);
                //再出発点ではない時のチェックポイント通過時..
                if ((LineSegmentsIntersection(V3toXZ(transpos), V3toXZ(transpos + transVec),
                    V3toXZ(point.InterPoint), V3toXZ(point.OuterPoint), out outvec)
                    && point.isCraftPassed == false) && nextPoint != RestartPoint)
                {
                    //曲点で設定された最低通過数を超えなければならない｡
                    if (point.passedPercentage > point.passMinPercentage)
                    {
                        GameSystem_InGame.instance.SetSegment();
                        RestartPoint = nextPoint;
                        if (x != waypoints.Count - 2)
                        {
                            if (x != waypoints.Count - 1)
                            {
                                PrefavablePoint = waypoints[x + 2];
                            }
                            else
                            {
                                PrefavablePoint = waypoints[1];
                            }
                        }
                        else
                        {
                            PrefavablePoint = waypoints[0];
                        }
                        GameSystem_InGame.instance.RespawnPos = nextPoint.transform.position;
                        GameSystem_InGame.instance.RespawnRot = Quaternion.LookRotation((nextPoint.RHandle - nextPoint.LHandle).normalized, Vector3.up);
                        point.isCraftPassed = true;
                        point.Lines.ForEach(n => n.isPassed = false);
                    }
                    else
                    {
                        point.Lines.ForEach(n => n.isPassed = false);
                        GameSystem_InGame.instance.ReturntoPoint();
                    }
                }
            }
            //すべてのチェックポイントが通過されたらリセット.
            if (waypoints.FindAll(way => way.isCraftPassed).Count == waypoints.Count())
            {
                waypoints.ForEach(way => way.isCraftPassed = false);
                RestartPoint = waypoints[0];
                Laps++;
                Debug.Log("Lap" + Laps);
                GameSystem_InGame.instance.Lap = Laps;
            }
            if (Laps == MaxLaps)
            {
                Laps = 0;
                GameSystem_InGame.instance.SetGhost();
                GameSystem_InGame.instance.Lap = Laps;
            }
        }
    }

    //線の交差を示す.
    public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }

    Vector2 V3toXZ(Vector3 INVec) {
        return new Vector2(INVec.x,INVec.z);
    }
}
