using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NCMB;
using System.Linq;
using System;

public class LoadedRanks : MonoBehaviour
{
    static public LoadedRanks instance;

    static public List<GameSystem_InGame.Record> Record = new List<GameSystem_InGame.Record>();

    public GameSystem_InGame.Record RivalRecord;

    public GameSystem_InGame.Record SenderGhostRec;

    int QueryGetMax = 5;

    public string Coursename = "";

    public string PlayerName;

    public string SceneString;

    public int selected;

    public bool isloading;


    //NCMBで位置情報をセット.
    [HideInInspector]
    NCMBObject GhostObject;
    private NCMBQuery<NCMBObject> QueryObj;


    private void Awake()
    {
        Time.timeScale = 1f;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        //シーンを跨いでもオブジェクトがロードされるようにする｡
    }


    //データをセーブ.
    public void SaveData(GameSystem_InGame.Record recs, string CourseName)
    {
        SenderGhostRec = recs;
        Debug.Log(SenderGhostRec.recordTime.ToString("f1") + "," + recs.Pos.Count);
        QueryObj = new NCMBQuery<NCMBObject>("RecordClass");

        QueryObj.CountAsync((int count, NCMBException error) =>
        {
            if (error != null)
            {
                //件数取得失敗時の処理
                Debug.Log("件数の取得に失敗しました");
            }
            else
            {
                SendData(SenderGhostRec, count, CourseName);
            }
        });
    }

    //データを送る.
    void SendData(GameSystem_InGame.Record recs, int count, string CourseName)
    {
        NCMBObject RecObj = new NCMBObject("RecordClass");
        RecObj["id"] = count + 1;
        RecObj["RacerName"] = PlayerName;
        RecObj["CourseName"] = CourseName;
        RecObj["TotalTime"] = SenderGhostRec.recordTime;
        List<Vector3> PosList = SenderGhostRec.Pos.ConvertAll(output => output.Pos);
        RecObj["Pos_XList"] = PosList.ConvertAll(output => output.x);
        RecObj["Pos_YList"] = PosList.ConvertAll(output => output.y);
        RecObj["Pos_ZList"] = PosList.ConvertAll(output => output.z);
        List<Vector3> RotList = SenderGhostRec.Pos.ConvertAll(output => output.Rot.eulerAngles);
        RecObj["Rot_XList"] = RotList.ConvertAll(output => output.x);
        RecObj["Rot_YList"] = RotList.ConvertAll(output => output.y);
        RecObj["Rot_ZList"] = RotList.ConvertAll(output => output.z);
        RecObj["postimeList"] = recs.Pos.ConvertAll(output => output.posTime);
        //頭悪い実装　でもこれじゃないと通過できないらしい
        RecObj["SegmentList"] = SenderGhostRec.segmentTime;
        RecObj.SaveAsync((NCMBException error) =>
        {
            if (error != null)
            {
                //件数取得失敗時の処理
                Debug.Log("Data Failed to save.");
            }
            else
            {
                Debug.Log("Data saved Successfully.");
            }
        });
    }

    //データをロードする.
    public List<GameSystem_InGame.Record> LoadData(string CourseName)
    {
        Record = new List<GameSystem_InGame.Record>();
        List<GameSystem_InGame.Record> Recs = new List<GameSystem_InGame.Record>();
        QueryObj = new NCMBQuery<NCMBObject>("RecordClass");

        QueryObj.CountAsync((int count, NCMBException error) =>
        {
            if (error != null)
            {
                //件数取得失敗時の処理
                Debug.Log("件数の取得に失敗しました");
            }
            else
            {
                Recs = ReceiveData(count, CourseName, false);
            }
        });
        Debug.Log("Data Exported count as " + Recs.Count());
        return Recs;
    }

    public List<GameSystem_InGame.Record> LoadDataFastest(string CourseName)
    {
        Record = new List<GameSystem_InGame.Record>();
        List<GameSystem_InGame.Record> Recs = new List<GameSystem_InGame.Record>();
        QueryObj = new NCMBQuery<NCMBObject>("RecordClass");

        QueryObj.CountAsync((int count, NCMBException error) =>
        {
            if (error != null)
            {
                //件数取得失敗時の処理
                Debug.Log("件数の取得に失敗しました");
                isloading = false;
            }
            else
            {
                Recs = ReceiveData(count, CourseName, true);
            }
        });
        Debug.Log("Data Exported count as " + Recs.Count());
        return Recs;
    }

    //データを受信.
    List<GameSystem_InGame.Record> ReceiveData(int Datanum, string CourseName, bool isLoadFastest)
    {
        List<int> arrayRandNum = Enumerable.Range(1, Datanum).OrderBy(n => Guid.NewGuid()).Take(QueryGetMax).ToList();
        List<int> Request = new List<int>();
        if (Datanum < QueryGetMax)
        {
            for (int i = 0; i < Datanum; i++)
            {
                Request.Add(arrayRandNum[i]);
            }
        }
        else
        {
            for (int i = 0; i < QueryGetMax; i++)
            {
                Request.Add(arrayRandNum[i]);
            }
        }
        if (isLoadFastest)
        {
            QueryObj.WhereGreaterThan("TotalTime", 0f).AddAscendingOrder("TotalTime");
        }
        else
        {
            QueryObj.WhereContainedIn("id", arrayRandNum);
        }

        List<GameSystem_InGame.Record> Recs = new List<GameSystem_InGame.Record>();

        QueryObj.FindAsync((List<NCMBObject> objectList, NCMBException Error) =>
        {
            //取得失敗
            if (Error != null)
            {
                //エラーコード表示
                Debug.Log("データの取得に失敗しました");
                isloading = false;
                return;
            }
            //取得成功時.
            else
            {
                //取得した全データから最速ライバルデータを生成する｡
                //またこれも頭悪い実装　もう少し効率化出来たらOKなんだけどなぁ.
                foreach (NCMBObject NCMBObj in objectList)
                {
                    GameSystem_InGame.Record LoadRec = new GameSystem_InGame.Record();
                    LoadRec.Name = (string)NCMBObj["RacerName"];
                    LoadRec.CourseName = (string)NCMBObj["CourseName"];
                    LoadRec.recordTime = float.Parse(NCMBObj["TotalTime"].ToString());
                    List<float> PosX = Floater(NCMBObj["Pos_XList"]);
                    List<float> PosY = Floater(NCMBObj["Pos_YList"]);
                    List<float> PosZ = Floater(NCMBObj["Pos_ZList"]);
                    List<float> RotX = Floater(NCMBObj["Rot_XList"]);
                    List<float> RotY = Floater(NCMBObj["Rot_YList"]);
                    List<float> RotZ = Floater(NCMBObj["Rot_ZList"]);
                    List<float> Postimes = Floater(NCMBObj["postimeList"]);
                    for (int Colmn = 0; Colmn < Postimes.Count(); Colmn++)
                    {
                        GameSystem_InGame.RecordedPos CPos = new GameSystem_InGame.RecordedPos();
                        CPos.Pos = new Vector3(PosX[Colmn], PosY[Colmn], PosZ[Colmn]);
                        CPos.Rot = Quaternion.Euler(RotX[Colmn], RotY[Colmn], RotZ[Colmn]);
                        CPos.posTime = Postimes[Colmn];
                        LoadRec.Pos.Add(CPos);
                    }
                    LoadRec.segmentTime = Floater(NCMBObj["SegmentList"]);
                    Recs.Add(LoadRec);
                }
            }
            Recs.OrderByDescending(Records => Records.recordTime);
            Debug.Log("Data Loaded Successfully. Data Numbers are " + Recs.Count.ToString());
            isloading = false;
        });
        Record = Recs;
        return Recs;
    }

    List<float> Floater(object obj)
    {
        ArrayList array = (ArrayList)obj;
        List<float> flt = new List<float>();
        for (int f = 0; f < array.Count; f++)
        {
            string parser;
            parser = array[f].ToString();
            flt.Add(float.Parse(parser));
        }
        return flt;
    }

    public void LoadScene(string LoadSceneString)
    {
        SceneManager.LoadScene(LoadSceneString, LoadSceneMode.Single);
    }

    public void LoadScenewithComposedStr()
    {
        SceneManager.LoadScene(SceneString, LoadSceneMode.Single);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
