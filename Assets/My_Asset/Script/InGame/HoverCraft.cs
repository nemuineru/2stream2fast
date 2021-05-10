using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class HoverCraft : MonoBehaviour
{
    public RigType rigType = RigType.Player;   
    public Waves waveinst;
    public GameObject thrust_L, thrust_R;
    public List<InstEffs> floatpoint;
    public float Speed = 1.0f;
    public float depthSubmergeAmount = 1.0f;
    public float displacementAmount = 3.0f;
    public float waterDrag = 0.99f;
    public float waterAngulerDrag = 0.5f;
    public ParticleSystem splashObj, splashBoostObj;
    public ParticleSystem WaterJet_L, WaterJet_R, WaterJetFront1, WaterJetFront2;
    ParticleSystem.EmissionModule moduleL,moduleR;
    ParticleSystem.MainModule FrontModule1, FrontModule2;

    public AudioSource EngineForward, EngineIdle;
    AudioMixer AudEngineGroup;

    List<bool> FloatPointEffinsted = new List<bool>();

    [ReadOnly]
    public bool isCraftOnSurface;
    bool isEffInsted = false;

    [HideInInspector]
    public Rigidbody rigidBody;
    MeshCollider MeshC;

    Input input;

    public enum RigType {
        Player,
        Ghost,
        Test
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (rigType != RigType.Test)
        {
            MeshC = GetComponent<MeshCollider>();
            moduleL = WaterJet_L.emission;
            moduleR = WaterJet_R.emission;
            FrontModule1 = WaterJetFront1.main;
            FrontModule2 = WaterJetFront2.main;
            AudEngineGroup = EngineForward.outputAudioMixerGroup.audioMixer;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit HitOnTerrain;
        //Terrainレイヤーのみにヒットする.
        int layermask = LayerMask.GetMask(new string[] { "Terrain" });

        Physics.Raycast(transform.position, -transform.up, out HitOnTerrain, 0.2f, layermask);
        if (HitOnTerrain.transform != null)
        {
            isCraftOnSurface = true;
        }
        else {
            isCraftOnSurface = false;
        }
        if (GameSystem_InGame.instance != null && !GameSystem_InGame.instance.isGameEnded)
        {
            Vector3 rigidBdy_XZ = new Vector3(rigidBody.velocity.x, 0f ,rigidBody.velocity.z);
            Vector3 rig_UpwardPlus = rigidBdy_XZ + new Vector3(0.0f, rigidBdy_XZ.magnitude, 0.0f);
            if (rigType != RigType.Test)
            {
                rigidBody.AddForceAtPosition(Physics.gravity * 1.0f, rigidBody.worldCenterOfMass, ForceMode.Acceleration);
                float dot = Vector3.Dot(transform.up, Vector3.up);
                //角速度と現在のオブジェクトの向きからどの方向に回転を加えようとしているのかを確認.
                Quaternion rotmode = Quaternion.Euler(rigidBody.angularVelocity);
                float AngleDiff = Vector3.Dot(rotmode * transform.up, Vector3.up)
                     - Vector3.Dot(transform.up, Vector3.up);
                Debug.DrawLine(transform.position, transform.position + rotmode * transform.up, Color.green);
                foreach (InstEffs point in floatpoint)
                {
                    float waveheight = Waves.instance.GetwaveHeight(point.transform.position);
                    float dispMultiple = Mathf.Clamp01((waveheight - point.transform.position.y) / depthSubmergeAmount) * displacementAmount;
                    if (point.transform.position.y < waveheight + 0.1f && Vector3.Dot(transform.up, Vector3.down) < 0.0)
                    {
                        isCraftOnSurface = true;
                    }

                        Vector3 ForcePos = Vector3.up * Mathf.Abs(Physics.gravity.y) * dispMultiple / floatpoint.Count;

                        Vector3 ForceAll = dispMultiple * -rigidBody.velocity * waterDrag * Time.fixedDeltaTime;

                        Vector3 torqueForce = dispMultiple * -rigidBody.angularVelocity * waterAngulerDrag * Time.fixedDeltaTime;
                    
                       
                    if (point.transform.position.y < waveheight)
                    {
                        rigidBody.AddForceAtPosition(ForcePos, point.transform.position, ForceMode.Acceleration);
                        rigidBody.AddTorque(torqueForce, ForceMode.VelocityChange);
                        rigidBody.AddForce(ForceAll, ForceMode.VelocityChange);
                        if (point.transform.position.y - waveheight < -0.1f && rigidBody.velocity.y < -0.2f && isEffInsted == false)
                        {
                            isEffInsted = true;
                            Instantiate(splashObj, point.transform.position, Quaternion.identity);
                            if (splashBoostObj != null && rigidBody.velocity.y < -1.0f && GameSystem_InGame.instance.isGameStarted)
                            {
                                if (Input.GetAxis("RShoulder") > 0.1 || Input.GetAxis("LShoulder") > 0.1)
                                {
                                    rigidBody.AddForce(rig_UpwardPlus.normalized * Speed / 10f, ForceMode.Impulse);
                                }
                                Instantiate(splashBoostObj, transform.position,  transform.rotation, transform);
                            }
                        }
                    }
                    if (rigidBody.velocity.y > 0 && isEffInsted == true)
                    {
                        isEffInsted = false;
                    }
                }
                //もしその速度が転覆する速度で転覆しそうであるなら...

                //Transform.upをtransform.forwardの周りで回転してワールド全体の上方向に向けさせる回転をEularでRigidbodyに与える.
                if ((dot < 0.3f && rigidBdy_XZ.magnitude < 0.1f && isCraftOnSurface) || !isCraftOnSurface)
                {
                    Vector3 predictedUp = Quaternion.AngleAxis(
                        rigidBody.angularVelocity.magnitude * Mathf.Rad2Deg * 0.5f / Speed,
                        rigidBody.angularVelocity
                    ) * transform.up;

                    Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
                    // Uncomment the next line to stabilize on only 1 axis.
                    //torqueVector = Vector3.Project(torqueVector, transform.forward);
                    rigidBody.AddTorque(torqueVector * Speed * Mathf.Pow((1 - dot),2f));

                }

                if (!GameSystem_InGame.instance.isGameStarted)
                {
                    rigidBody.velocity = rigidBody.velocity - rigidBdy_XZ;
                }
                if (isCraftOnSurface)
                {
                    if (rigType == RigType.Player && GameSystem_InGame.instance.isGameStarted && !GameSystem_InGame.instance.isGameEnded && Vector3.Dot(transform.up, Vector3.up) > 0f)
                    {
                        if (Input.GetAxis("LShoulder") > 0.1)
                        {
                            Vector3 vels = thrust_L.transform.forward;
                            vels = new Vector3(vels.x, 0f, vels.z).normalized * Input.GetAxis("LShoulder");
                            rigidBody.AddForceAtPosition(vels * Speed, thrust_L.transform.position, ForceMode.Force);
                        }
                        if (Input.GetAxis("RShoulder") > 0.1)
                        {
                            Vector3 vels = thrust_R.transform.forward;
                            vels = new Vector3(vels.x, 0f, vels.z).normalized * Input.GetAxis("RShoulder");
                            rigidBody.AddForceAtPosition(vels * Speed, thrust_R.transform.position, ForceMode.Force);
                        }
                    }
                    moduleL.rateOverTime = Mathf.Clamp01((rigidBody.velocity.magnitude - 0.5f) / 12.0f) * 100f;
                    moduleR.rateOverTime = Mathf.Clamp01((rigidBody.velocity.magnitude - 0.5f) / 12.0f) * 100f;
                    FrontModule1.startSpeed = Mathf.Clamp01((rigidBody.velocity.magnitude - 0.5f) / 10.0f) * 20f;
                    FrontModule2.startSpeed = Mathf.Clamp01((rigidBody.velocity.magnitude - 0.5f) / 10.0f) * 20f;
                    FrontModule1.maxParticles = 200;
                    FrontModule2.maxParticles = 200;
                }
                else
                {
                    moduleL.rateOverTime = 0f;
                    moduleR.rateOverTime = 0f;
                    FrontModule1.startSpeed = 0f;
                    FrontModule1.maxParticles = 0;
                    FrontModule2.startSpeed = 0f;
                    FrontModule2.maxParticles = 0;
                }
                float rigVel = Mathf.Clamp((rigidBdy_XZ.magnitude - 0.3f) / 10.0f, 0.0001f, 1.0f);
                if (rigidBdy_XZ.magnitude > 0.3f)
                {
                    AudEngineGroup.SetFloat("Engine_Vol", Mathf.Clamp(20.0f * Mathf.Log10(rigVel), -45f, 0f));
                    AudEngineGroup.SetFloat("Engine_PitchShift", 0.98f + rigVel * 0.4f);
                }
                else
                {
                    AudEngineGroup.SetFloat("Engine_Vol", -45f);
                    AudEngineGroup.SetFloat("Engine_PitchShift", 0.98f);
                }
                EngineIdle.outputAudioMixerGroup.audioMixer.SetFloat("Engine_Idle_Vol", Mathf.Clamp(20.0f * Mathf.Log10(1 - rigVel), -45f, -10f));
            }
            else
            {
                moduleL.rateOverTime = Mathf.Clamp01((rigidBody.velocity.magnitude - 0.5f) / 12.0f) * 100f;
                moduleR.rateOverTime = Mathf.Clamp01((rigidBody.velocity.magnitude - 0.5f) / 12.0f) * 100f;
                FrontModule1.startSpeed = Mathf.Clamp01((rigidBody.velocity.magnitude - 0.5f) / 10.0f) * 20f;
                FrontModule2.startSpeed = Mathf.Clamp01((rigidBody.velocity.magnitude - 0.5f) / 10.0f) * 20f;
                FrontModule1.maxParticles = 200;
                FrontModule2.maxParticles = 200;

                float rigVel = Mathf.Clamp((rigidBdy_XZ.magnitude - 0.3f) / 10.0f, 0.0001f, 1.0f);
                if (rigidBdy_XZ.magnitude > 0.3f)
                {
                    AudEngineGroup.SetFloat("Engine_Vol", Mathf.Clamp(20.0f * Mathf.Log10(rigVel), -45f, 0f));
                    AudEngineGroup.SetFloat("Engine_PitchShift", 0.98f + rigVel * 0.4f);
                }
                else
                {
                    AudEngineGroup.SetFloat("Engine_Vol", -45f);
                    AudEngineGroup.SetFloat("Engine_PitchShift", 0.98f);
                }
                EngineIdle.outputAudioMixerGroup.audioMixer.SetFloat("Engine_Idle_Vol", Mathf.Clamp(20.0f * Mathf.Log10(1 - rigVel), -45f, -10f));
            }
        }

        else
        {
            rigidBody.AddForceAtPosition(Physics.gravity * 1.0f, rigidBody.worldCenterOfMass, ForceMode.Acceleration);
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
                    rigidBody.AddForceAtPosition(Vector3.up * Mathf.Abs(Physics.gravity.y) * dispMultiple / floatpoint.Count, point.transform.position, ForceMode.Acceleration);
                    rigidBody.AddForce(dispMultiple * -rigidBody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    rigidBody.AddTorque(dispMultiple * -rigidBody.angularVelocity * waterAngulerDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }
            }
            Vector2 RigVelXZ = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
            float rigVel = Mathf.Clamp((RigVelXZ.magnitude - 0.3f) / 10.0f, 0.0001f, 1.0f);
            if (AudEngineGroup != null)
            {
                if (RigVelXZ.magnitude > 0.3f)
                {
                    AudEngineGroup.SetFloat("Engine_Vol", Mathf.Clamp(20.0f * Mathf.Log10(rigVel), -45f, 0f));
                    AudEngineGroup.SetFloat("Engine_PitchShift", 0.98f + rigVel * 0.4f);
                }
                else
                {
                    AudEngineGroup.SetFloat("Engine_Vol", -45f);
                    AudEngineGroup.SetFloat("Engine_PitchShift", 0.98f);
                }
                EngineIdle.outputAudioMixerGroup.audioMixer.SetFloat("Engine_Idle_Vol", Mathf.Clamp(20.0f * Mathf.Log10(1 - rigVel), -45f, -10f));
            }
        }
    }
}
