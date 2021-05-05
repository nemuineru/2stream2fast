using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraKeep : MonoBehaviour
{
    public Rigidbody rgBody;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (new Vector2(rgBody.velocity.x, rgBody.velocity.y).magnitude > Mathf.Epsilon)
        {
            Vector3 rotates = new Vector3(rgBody.velocity.x, 0f, rgBody.velocity.z).normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(rotates, Vector3.up),0.99f);
        }
    }
}
