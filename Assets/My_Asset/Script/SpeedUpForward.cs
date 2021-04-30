using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpForward : MonoBehaviour
{
    public float speed = 10.0f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            other.gameObject.GetComponent<Rigidbody>().velocity += transform.forward * speed;
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
