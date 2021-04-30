using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class HoverCraft : MonoBehaviour
{
    public Waves waveinst;
    public GameObject thrust_L, thrust_R;
    public List<InstEffs> floatpoint;
    public float Speed = 1.0f;
    public float depthSubmergeAmount = 1.0f;
    public float displacementAmount = 3.0f;
    public float waterDrag = 0.99f;
    public float waterAngulerDrag = 0.5f;
    public ParticleSystem splashObj;
    public ParticleSystem WaterJet_L, WaterJet_R, WaterJetFront1, WaterJetFront2;
    ParticleSystem.EmissionModule moduleL,moduleR;
    ParticleSystem.MainModule FrontModule1, FrontModule2;

    public AudioSource EngineForward, EngineIdle;
    AudioMixer AudEngineGroup;

    List<bool> FloatPointEffinsted = new List<bool>();

    bool isCraftOnSurface;
    bool isEffInsted = false;

    Rigidbody rigs;
    MeshCollider MeshC;
    // Start is called before the first frame update
    void Start()
    {
        MeshC = GetComponent<MeshCollider>();
        rigs = GetComponent<Rigidbody>();
        moduleL = WaterJet_L.emission;
        moduleR = WaterJet_R.emission;
        FrontModule1 = WaterJetFront1.main;
        FrontModule2 = WaterJetFront2.main;
        AudEngineGroup = EngineForward.outputAudioMixerGroup.audioMixer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isCraftOnSurface = false;
        rigs.AddForceAtPosition(Physics.gravity * 1.0f, rigs.worldCenterOfMass,ForceMode.Acceleration);
        foreach (InstEffs point in floatpoint)
        {
            float waveheight = Waves.instance.GetwaveHeight(point.transform.position);
            if (point.transform.position.y < waveheight + 0.1f && Vector3.Dot(transform.up,Vector3.down) < 0.0) {
                isCraftOnSurface = true;
            }
                if (point.transform.position.y < waveheight)
            {
                float dispMultiple = Mathf.Clamp01((waveheight - point.transform.position.y) / depthSubmergeAmount) * displacementAmount;
                rigs.AddForceAtPosition(Vector3.up * Mathf.Abs(Physics.gravity.y) * dispMultiple / floatpoint.Count, point.transform.position, ForceMode.Acceleration);
                rigs.AddForce(dispMultiple * -rigs.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                rigs.AddTorque(dispMultiple * -rigs.angularVelocity * waterAngulerDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                if (point.transform.position.y - waveheight < -0.1f && rigs.velocity.y < -0.2f && isEffInsted == false)
                {
                    isEffInsted = true;
                    Instantiate(splashObj,point.transform.position,Quaternion.identity);
                }
            }
            if (rigs.velocity.y > 0 && isEffInsted == true) {
                isEffInsted = false;
            }
        }
        RaycastHit HitOnTerrain;
        //Terrainレイヤーのみにヒットする.
        int layermask = LayerMask.GetMask(new string[] {"Terrain"});

        Physics.Raycast(transform.position, -transform.up,out HitOnTerrain,0.02f,layermask);
        Debug.DrawLine(transform.position,HitOnTerrain.point,Color.red);
        if (HitOnTerrain.transform != null) {
            isCraftOnSurface = true;
        }

        if (isCraftOnSurface)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 vels = thrust_L.transform.forward;
                vels = new Vector3(vels.x, 0f, vels.z).normalized;
                rigs.AddForceAtPosition(vels * Speed, thrust_L.transform.position, ForceMode.Force);
            }
            if (Input.GetMouseButton(1))
            {
                Vector3 vels = thrust_R.transform.forward;
                vels = new Vector3(vels.x, 0f, vels.z).normalized;
                rigs.AddForceAtPosition(vels * Speed, thrust_R.transform.position, ForceMode.Force);
            }
            moduleL.rateOverTime = Mathf.Clamp01((rigs.velocity.magnitude - 0.5f) / 12.0f) * 100f;
            moduleR.rateOverTime = Mathf.Clamp01((rigs.velocity.magnitude - 0.5f) / 12.0f) * 100f;
            FrontModule1.startSpeed = Mathf.Clamp01((rigs.velocity.magnitude - 0.5f) / 10.0f) * 20f;
            FrontModule2.startSpeed = Mathf.Clamp01((rigs.velocity.magnitude - 0.5f) / 10.0f) * 20f;
            FrontModule1.maxParticles = 200;
            FrontModule2.maxParticles = 200;
        }
        else {
            moduleL.rateOverTime = 0f;
            moduleR.rateOverTime = 0f;
            FrontModule1.startSpeed = 0f;
            FrontModule1.maxParticles = 0;
            FrontModule2.startSpeed = 0f;
            FrontModule2.maxParticles = 0;
        }
        Vector2 RigVelXZ = new Vector2(rigs.velocity.x, rigs.velocity.z);
        float rigVel = Mathf.Clamp((RigVelXZ.magnitude - 0.3f) / 10.0f, 0.0001f, 1.0f);
        if (RigVelXZ.magnitude > 0.3f)
        {
            AudEngineGroup.SetFloat("Engine_Vol",Mathf.Clamp(20.0f * Mathf.Log10(rigVel),-45f,0f));
            AudEngineGroup.SetFloat("Engine_PitchShift", 0.98f + rigVel * 0.4f);
        }
        else
        {
            AudEngineGroup.SetFloat("Engine_Vol", -45f);
            AudEngineGroup.SetFloat("Engine_PitchShift", 0.98f);
        }
        EngineIdle.outputAudioMixerGroup.audioMixer.SetFloat("Engine_Idle_Vol",Mathf.Clamp(20.0f  * Mathf.Log10(1 - rigVel),-45f,-10f));
    }

}
