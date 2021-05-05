using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankSetter : MonoBehaviour
{
    public GameSystem_InGame.Record Recs;
    public TMP_Text TMTEXT;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void LoadRecwithNoStr()
    {
        LoadedRanks.instance.LoadScenewithComposedStr();
    }


    public void LoadRecAndLoadScene()
    {
        if (Recs.Pos.Count != 0)
        {
            LoadedRanks.instance.RivalRecord = Recs;
            LoadedRanks.instance.LoadScenewithComposedStr();
        }
    }
}
