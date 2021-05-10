using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public Rigidbody instanceRigidbdy;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AngleSet(3.0f, 4.0f);
    }

    void fix()
    {
        float dots = Vector3.Dot(transform.up, Vector3.up);
        Vector3 look = transform.up;
        Vector3 look_up_trans = new Vector3(look.x, 0f, look.z);

        Vector3 vectFwd = Vector3.forward;
        Vector3 vectFwdToRight = Quaternion.Euler(0f, 90f, 0f) * vectFwd.normalized;


        Debug.DrawLine(transform.position, transform.position + vectFwd, Color.red);
        Debug.DrawLine(transform.position, transform.position + vectFwdToRight, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.magenta);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.blue);

        Quaternion rot = Quaternion.Lerp(transform.rotation,Quaternion.FromToRotation(transform.up,Vector3.up),Time.fixedDeltaTime);

        Debug.DrawLine(transform.position, transform.position + rot.eulerAngles * (1 - dots), Color.green);
        if (dots > -0.3f)
        {
            instanceRigidbdy.MoveRotation(rot);
        }
        else {
            rot = Quaternion.Lerp(Quaternion.FromToRotation(-transform.up, Vector3.up), transform.rotation ,Time.fixedDeltaTime);
            instanceRigidbdy.MoveRotation(rot);
        }
    }

    void AngleSet(float stability, float speed)
    {
        Vector3 predictedUp = Quaternion.AngleAxis(
            instanceRigidbdy.angularVelocity.magnitude 
            * Mathf.Rad2Deg * stability / speed,
            instanceRigidbdy.angularVelocity) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        instanceRigidbdy.AddTorque(torqueVector * speed * speed);

    }
}
