using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using NCMB;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameSystem_InGame : MonoBehaviour
{
    float RectimeTicks = 0f;
    float RecctimeMin = 0.125f;

    public bool isGameStarted, isGameEnded, isGamePaused;
    public float CountDown = 5f;
    public float CountDownTimespeed = 0.6f;

    public GameObject ResultUI, PauseUI, GameUI;

    public CinemachineVirtualCamera ResultCam;


    public string CourseName;
    public string RacerName, RivalName;
    static public GameSystem_InGame instance;
    public HoverCraft Craft, Ghostcraft;
    public TMP_Text StatusText, segmentTime, LapTime, LapNum;
    public TMP_Text ResultTimeUI;
    [ReadOnly]
    public bool isDataSended = false;
    [ReadOnly]
    public float timeTick, GhosttimeTick;
    [HideInInspector]
    public float timeSegmentTick;
    [HideInInspector]
    public float recordtime;
    [ReadOnly]
    public int Lap;
    [ReadOnly]
    public int MaxLap = 10;

    public AudioSource buzzarSound;
    public AudioSource goalSound;

    public Animator CountDownanim;

    public Waypoint firstPoint;

    Rigidbody craftRig, GhostcraftRig;
    public Vector3 RespawnPos;
    public Quaternion RespawnRot;

    public List<Record> Recs;

    public Record GhostRecord = new Record();
    public Record RecRecord = new Record();

    [Serializable]
    public class Record {
        public string CourseName;
        public string Name;
        public float recordTime = 0f;
        public List<RecordedPos> Pos = new List<RecordedPos>();
        public List<float> segmentTime = new List<float>();
    }

    [Serializable]
    public class RecordedPos {
        public Vector3 Pos = new Vector3();
        public Quaternion Rot = new Quaternion();
        public float posTime;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
        if (LoadedRanks.instance != null) {
            GhostRecord = LoadedRanks.instance.RivalRecord;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        craftRig = Craft.GetComponent<Rigidbody>();
        GhostcraftRig = Ghostcraft.GetComponent<Rigidbody>();
        RespawnPos = firstPoint.transform.position;
        RespawnRot = Quaternion.LookRotation(-firstPoint.RHandle, Vector3.up);
        StartCoroutine(PrePare());
    }

    private void Update()
    {
        if (!isGameEnded && isGameStarted)
        {
            if (Input.GetButtonDown("Cancel") && !isGamePaused)
            {
                ReturntoPoint();
            }
            if (Input.GetButtonDown("Menu")) {
                isGamePaused = !isGamePaused;
            }
        }
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            GameUI.SetActive(false);
            PauseUI.SetActive(true);
        }
        else if(!isGameEnded)
        {
            Time.timeScale = 1f;
            GameUI.SetActive(true);
            PauseUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isGameEnded && isGameStarted)
        {
            GhosttimeTick += Time.fixedDeltaTime;
            if (GhostRecord.Pos.Count != 0)
            {
                RecordedPos posBefore = GhostRecord.Pos.FindLast(Sel => Sel.posTime < GhosttimeTick);
                RecordedPos posAfter = GhostRecord.Pos.Find(Sel => Sel.posTime > GhosttimeTick);
                if (posAfter != null && posBefore != null && GhosttimeTick < GhostRecord.recordTime)
                {
                    float t = posAfter.posTime - GhosttimeTick;
                    float f = posAfter.posTime - posBefore.posTime;
                    GhostcraftRig.MovePosition(posBefore.Pos * (t / f) + posAfter.Pos * (1 - t / f));
                    GhostcraftRig.MoveRotation(Quaternion.Lerp(posBefore.Rot, posAfter.Rot, t / f));
                }
                else
                {
                    GhostcraftRig.velocity = GhostcraftRig.velocity / 1.5f;
                }
            }


            timeTick += Time.fixedDeltaTime;
            timeSegmentTick += Time.fixedDeltaTime;
            
            RectimeTicks += Time.fixedDeltaTime;
            if (RecctimeMin < RectimeTicks)
            {
                RecordedPos transpos = new RecordedPos
                {
                    Pos = craftRig.transform.position,
                    Rot = craftRig.transform.rotation,
                    posTime = timeTick
                };

                //レコード時間に追加.
                RecRecord.Pos.Add(transpos);
                RectimeTicks = 0f;
            }
        }
        if (isGameEnded)
        {
            GhosttimeTick += Time.fixedDeltaTime;
            if (GhostRecord.Pos.Count != 0)
            {
                RecordedPos posBefore = GhostRecord.Pos.FindLast(Sel => Sel.posTime < GhosttimeTick);
                RecordedPos posAfter = GhostRecord.Pos.Find(Sel => Sel.posTime > GhosttimeTick);
                if (posAfter != null && posBefore != null && GhosttimeTick < GhostRecord.recordTime)
                {
                    float t = posAfter.posTime - GhosttimeTick;
                    float f = posAfter.posTime - posBefore.posTime;
                    craftRig.MovePosition(posBefore.Pos * (t / f) + posAfter.Pos * (1 - t / f));
                    craftRig.MoveRotation(Quaternion.Lerp(posBefore.Rot, posAfter.Rot, t / f));
                }
            }
            if (GhosttimeTick > GhostRecord.recordTime) {
                GhosttimeTick = 0f;
            }
        }
        if (!isGameStarted)
        {
            LapTime.text = " ---------- ";
            LapNum.text = "<size=30%>lap \n <size=100%>" + (Lap + 1) + "/" + MaxLap;
        }
        int minute = (int)(timeTick / 60f);
        StatusText.text = (craftRig.velocity.magnitude * 3.6f / 1.852).ToString("f1") + " knot \n" +
            minute.ToString("00") + ":" + Mathf.FloorToInt(timeTick % 60f).ToString("00") + "." + Mathf.FloorToInt((timeTick * 100f) % 100f).ToString("00");

    }

    public void ReturntoPoint()
    {
        Craft.transform.position = RespawnPos;
        Craft.transform.rotation = RespawnRot;
        Craft.rigidBody.velocity = Vector3.zero;
        Craft.rigidBody.angularVelocity = Vector3.zero;
    }

    public void SetSegment()
    {
        //Debug.Log("Segment Passed" + " as " + timeSegmentTick.ToString("f1"));
        RecRecord.segmentTime.Add(timeSegmentTick);
        timeSegmentTick = 0f;
        LapTimeShow(RecRecord.segmentTime.Count);
        //ゴーストが存在しないときは消す.
        if (GhostRecord.segmentTime.Count != 0)
        {
            float ghostSegmenttime = GhostRecord.segmentTime.GetRange(0, RecRecord.segmentTime.Count).Sum();
            Debug.Log("Ghost Goes as " + ghostSegmenttime.ToString("f1"));
            StartCoroutine(TimeSegmentShow(ghostSegmenttime, timeTick));
        }
        //ブザーサウンドの再生.
        buzzarSound.Play();
    }

    //ゴースト設定.
    public void SetGhost() {
        //Debug.Log("Ghost Set " + " as " + timeTick.ToString("f1"));
        //Debug.Log("Segment Number as " + RecRecord.segmentTime.Count());
        RecRecord.recordTime = timeTick;
        timeTick = 0f;
        GhostRecord = RecRecord;
        GhosttimeTick = 0f;
        StartCoroutine(EndGame());
    }

    IEnumerator PrePare(){
        while (CountDown > 0f)
        {
            CountDown -= Time.deltaTime * CountDownTimespeed;
            CountDownanim.SetFloat("CountDown",CountDown);
            yield return null;
        }
        isGameStarted = true;
    }

    IEnumerator EndGame()
    {
        isGameEnded = true;

        GameUI.SetActive(false);

        ResultCam.enabled = true;

        ResultUI.SetActive(true);

        float GhostRectime = GhostRecord.recordTime;
        int minute = Mathf.FloorToInt(GhostRectime / 60f);
        ResultTimeUI.text = "Your Time \n" + minute.ToString("00") + ":" +
            Mathf.FloorToInt(GhostRectime % 60f).ToString("00") + "." +
            Mathf.FloorToInt((GhostRectime * 100f) % 100f).ToString("00");
        yield return null;
    }

    //区間タイムの表示.
    IEnumerator TimeSegmentShow(float targettime,float currenttime) {
        float showtime = 2.0f;
        float tick = showtime;
        float differenceTime = Mathf.Abs(currenttime - targettime);
        string diffCol = "";
        if ((currenttime - targettime) < 0f)
        {
            diffCol = "<#0AF> - ";
        }
        else
        {
            diffCol = "<#FA0> + ";
        }
        int minute = Mathf.FloorToInt(differenceTime / 60f);
        segmentTime.text = diffCol + minute.ToString("00") + ":" +
            Mathf.FloorToInt(differenceTime % 60f).ToString("00") + "." +
            Mathf.FloorToInt((differenceTime * 100f) % 100f).ToString("00");

        while (tick > 0f) {
            tick -= Time.fixedDeltaTime;
            segmentTime.alpha = Mathf.Clamp01((tick + 1f) / showtime);
            segmentTime.color = segmentTime.color * Mathf.Clamp01(tick / showtime);
            yield return null;
        }
    }

    void LapTimeShow(int Segment)
    {
        LapTime.text = " ";
        float[] SegPrevs = new float[3];
        int[] minutes = new int[3];
        string[] timeText = new string[3];


        int MaxNum = Mathf.Clamp(Segment, 1, 3) - 1;
        Debug.Log(MaxNum.ToString() + "," + Segment.ToString());
        //３つまで､でも情報の取得はその情報を超えないように...
        for (int x = MaxNum; x >= 0; x--) {
            SegPrevs[x] = RecRecord.segmentTime[Segment - x - 1];
            int minute = Mathf.FloorToInt(SegPrevs[x] / 60f);
            string TimeText = minute.ToString("00") + ":" +
                Mathf.FloorToInt(SegPrevs[x] % 60f).ToString("00") + "." +
                Mathf.FloorToInt((SegPrevs[x] * 100f) % 100f).ToString("00");
            LapTime.text += (Segment - x) + " - " + TimeText;
            if (x != 0) {
                LapTime.text += "\n ";
            }
        }
        LapNum.text = "<size=30%>lap \n <size=100%>" + (Lap + 1) + "/" + MaxLap;
    }


    void RecordSet(Record rec) {
        RivalName = rec.Name;
        GhostRecord = rec;
    }

    public void RevokeLoadScene(string loadsceneSt)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(loadsceneSt, LoadSceneMode.Single);
    }

    public void RevokeReLoadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveRecs()
    {        
        if (AutoCourse.instance.isCourseRanked && isDataSended == false)
        {
            isDataSended = true;
            LoadedRanks.instance.SaveData(GhostRecord, CourseName);
        }
    }

    public void SetPause() {
        isGamePaused = !isGamePaused;
    }
}
