using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Vector3 LHandle, RHandle;
    public GameObject Pole;
    public GameObject Checkpoint;
    public float Width;
    public int Resolution;
    public bool isHandleMirror;
    public bool isCraftPassed;
    [Range(0f, 100f)]
    public float passMinPercentage = 50f;
    [SerializeField, ReadOnly]
    public float passedPercentage = 50f;
    [SerializeField]
    public List<Line> Lines = new List<Line>();


    [HideInInspector]
    public Vector3 InterPoint, OuterPoint;
    // Start is called before the first frame update

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        if (!isHandleMirror)
        {
            Gizmos.DrawLine(transform.position, transform.position + LHandle);
            Gizmos.DrawWireSphere(transform.position + LHandle, 0.25f);
            Gizmos.DrawLine(transform.position, transform.position + RHandle);
            Gizmos.DrawSphere(transform.position + RHandle, 1f);
        }
        else {
            Gizmos.DrawLine(transform.position, transform.position + LHandle);
            Gizmos.DrawWireSphere(transform.position + LHandle, 0.25f);
            Gizmos.DrawLine(transform.position, transform.position - LHandle);
            Gizmos.DrawSphere(transform.position - LHandle, 1f);
        }
        foreach (Line line in Lines) {
            Gizmos.DrawLine(line.Pos1, line.Pos2);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
