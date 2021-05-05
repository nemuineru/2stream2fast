using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextWay : MonoBehaviour
{
    HoverCraft craft;
    // Start is called before the first frame update
    void Start()
    {
        if (craft == null) {
            craft = GameSystem_InGame.instance.Craft;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 craftXZ = new Vector3(craft.transform.forward.x,0f,craft.transform.forward.z).normalized;
        Vector3 Diffs = (AutoCourse.instance.PrefavablePoint.transform.position - craft.transform.position).normalized;
        Vector3 DiffsXZ = new Vector3(Diffs.x,0f,Diffs.z);
        float Angles = -Vector3.Angle(craftXZ,DiffsXZ);
        transform.rotation = Quaternion.AngleAxis(Angles, Vector3.forward);
    }
}
