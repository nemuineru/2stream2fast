using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCourse : MonoBehaviour
{
    public string CourseName;
    public string CourseSceneName;
    

    //コース名とコースのシーンを決定し､その名前の情報を送る.

    public void SendCourseName() {
        LoadedRanks.instance.Coursename = CourseName;
        LoadedRanks.instance.SceneString = CourseSceneName;
        InMenu.instance.gameObject.GetComponent<Animator>().SetBool("isCourseSelected",true);
        StopCoroutine(InMenu.instance.FindRank()); 
        StartCoroutine(InMenu.instance.FindRank());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
