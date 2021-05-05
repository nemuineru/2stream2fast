using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InMenu : MonoBehaviour
{
    static public InMenu instance;
    public TMP_InputField inputField;
    public RankSetter FirstList;
    public Transform VertList;
    public List<GameObject> ButtonList;
    public List<GameSystem_InGame.Record> Ranking = new List<GameSystem_InGame.Record>();
    public string Playername = "";
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //既にロードされている場合は無視.
        if (LoadedRanks.Record.Count == 0)
        {
            StartCoroutine(InternetRankLoad());
        }
        inputField.text = "Player";
    }

    // Update is called once per frame
    void Update()
    {
        Playername = inputField.text;
        if (Playername != "") {
            LoadedRanks.instance.PlayerName = Playername;
        }
    }

    public void ManuallyRankLoad()
    {
        StartCoroutine(InternetRankLoad());
    }

    public IEnumerator InternetRankLoad()
    {
        LoadedRanks.instance.isloading = true;
        FirstList.TMTEXT.text = "";
        LoadedRanks.instance.LoadDataFastest(LoadedRanks.instance.Coursename);
        StartCoroutine(FindRank());
        yield return null;
    }

    public IEnumerator FindRank()
    {
        //コピー元のボタン以外を消す｡
        if (Ranking.Count > 0)
        {
            ButtonList.ForEach(button => Destroy(button.gameObject));
        }
        FirstList.Recs = new GameSystem_InGame.Record();
        while (LoadedRanks.Record.Count == 0 && LoadedRanks.instance.isloading == true)
        {
            FirstList.TMTEXT.text = "ロード中...";
            yield return null;
        }
        if (LoadedRanks.Record.Count == 0)
        {
            FirstList.TMTEXT.text = "ロードに失敗しました.";
        }
        else
        {
            List<GameSystem_InGame.Record> type = LoadedRanks.Record.FindAll(match => match.CourseName == LoadedRanks.instance.Coursename);
            if (type.Count == 0)
            {
                FirstList.TMTEXT.text = "選択したコースデータのゴーストが存在しません.";
            }
            else
            {
                setRank(type);
            }
        }
    }

    //ランキングに合わせてボタンを自動生成する.
    public void setRank(List<GameSystem_InGame.Record> records)
    {
        int RankMaxCount = 5;
        FirstList.TMTEXT.text = "";
        Ranking = records;
        Debug.Log(Ranking.Count().ToString());
        Ranking.OrderByDescending(n => n.recordTime);
        int NextRank = 0;
        for (int buttonNum = 0; buttonNum < Mathf.Clamp(Ranking.Count, 0, RankMaxCount); buttonNum++)
        {
            GameSystem_InGame.Record buttonRec;
            if (buttonNum < 1)
            {
                NextRank = buttonNum;
                buttonRec = Ranking[NextRank];
            }
            else
            {
                NextRank = Mathf.Clamp
                    (NextRank + Mathf.CeilToInt(Random.value * Ranking.Count * 1.1f / (RankMaxCount * 1.0f)), 0, Ranking.Count - 1);
                Debug.Log("Loaded " + NextRank + "'s Record.");
                buttonRec = Ranking[NextRank];
            }
            string RecordName = buttonRec.Name;
            int minute = Mathf.FloorToInt(buttonRec.recordTime / 60f);
            string TimeText = minute.ToString("00") + ":" +
                Mathf.FloorToInt(buttonRec.recordTime % 60f).ToString("00") + "." +
                Mathf.FloorToInt((buttonRec.recordTime * 100f) % 100f).ToString("00");
            string WholeText = (NextRank + 1) + " :" + RecordName + " - " + TimeText;
            if (buttonNum == 0)
            {
                FirstList.Recs = buttonRec;
                FirstList.TMTEXT.text = WholeText;
            }
            //一つ以外のボタンはリストとして管理
            else {
                GameObject InstButton = Instantiate(FirstList.gameObject);
                InstButton.transform.SetParent(VertList.transform,true);
                RankSetter InstRank = InstButton.GetComponent<RankSetter>();
                InstRank.Recs = buttonRec;
                InstRank.TMTEXT.text = WholeText;
                ButtonList.Add(InstButton);
            }   
        }
    }

    public void SetAnimBools() {
        GetComponent<Animator>().SetBool("isCourseSelected", false);
    }
}
