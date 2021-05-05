using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPaddingAuto : MonoBehaviour
{
    public List<SetCourse> CourseButtonList = new List<SetCourse>();

    private void Awake()
    {
        if (CourseButtonList.Count != 0)
        {
            foreach (SetCourse course in CourseButtonList)
            {
                SetCourse GenCourseButton = Instantiate(course.gameObject).GetComponent<SetCourse>();
                GenCourseButton.transform.SetParent(transform, false);
            }
        }
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
