using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TweetButton : MonoBehaviour
{
    public void OnClickTwitterButton()
    {
        float times = GameSystem_InGame.instance.GhostRecord.recordTime;
        int minute = Mathf.FloorToInt(times / 60f);
        string textTime = minute.ToString("00") + ":" +
            Mathf.FloorToInt(times % 60f).ToString("00") + "." +
            Mathf.FloorToInt((times * 100f) % 100f).ToString("00");
        //urlの作成
        string esctext1 = " - をDualStreamにて記録しました.";
        string esctext2 = " タイム - ";
        string esctext3 = "コース名 : " + GameSystem_InGame.instance.CourseName;
        string url = esctext3 + esctext2 + textTime + esctext1;

        //Twitter投稿画面の起動
        naichilab.UnityRoomTweet.Tweet("2stream2fast", url, "DualStream","unity1week");
    }
}
