using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class floater : MonoBehaviour
{
    public Waves waveinst;
    public List<InstEffs> floatpoint;
    List<bool> FloatPointEffinsted = new List<bool>();
    public float depthSubmergeAmount = 1.0f;
    public float displacementAmount = 3.0f;
    public float waterDrag = 0.99f;
    public float waterAngulerDrag = 0.5f;
    bool isCraftOnSurface;

    Rigidbody rigs;
    public ParticleSystem splashObj;

    // Start is called before the first frame update
    void Start()
    {
        rigs = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isCraftOnSurface = false;
        rigs.AddForceAtPosition(Physics.gravity * 1.0f, rigs.worldCenterOfMass, ForceMode.Acceleration);
        foreach (InstEffs point in floatpoint)
        {
            float waveheight = Waves.instance.GetwaveHeight(point.transform.position);
            if (point.transform.position.y < waveheight + 0.1f && Vector3.Dot(transform.up, Vector3.down) < 0.0)
            {
                isCraftOnSurface = true;
            }
            if (point.transform.position.y < waveheight)
            {
                float dispMultiple = Mathf.Clamp01((waveheight - point.transform.position.y) / depthSubmergeAmount) * displacementAmount;
                rigs.AddForceAtPosition(Vector3.up * Mathf.Abs(Physics.gravity.y) * dispMultiple / floatpoint.Count, point.transform.position, ForceMode.Acceleration);
                rigs.AddForce(dispMultiple * -rigs.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                rigs.AddTorque(dispMultiple * -rigs.angularVelocity * waterAngulerDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                if (point.isEffinsted == false && point.transform.position.y - waveheight < 0.5f && splashObj != null)
                {
                    point.isEffinsted = true;
                    Instantiate(splashObj, point.transform.position, Quaternion.Euler(Vector3.zero));
                }
            }
            else
            {
                point.isEffinsted = false;
            }
        }

    }
}
