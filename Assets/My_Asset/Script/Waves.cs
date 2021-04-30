using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Waves : MonoBehaviour
{
    static public Waves instance;
    MeshFilter meshFilter;
    public MeshRenderer MeshRend, MeshBackRend;
    List<Material> mat = new List<Material>();
    List<Material> Bmat = new List<Material>();

    float _Length = 2.0f;
     float _Speed = 1.0f;
     float _Displacement = .0f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null) {
            Destroy(this);
        }
        meshFilter = GetComponent<MeshFilter>();
        MeshRend = GetComponent<MeshRenderer>();
    }

    void Start()
    {
       MeshRend.GetMaterials(mat);
       MeshBackRend.GetMaterials(Bmat);
        _Displacement = mat[0].GetFloat("_Displacement");
        _Speed = mat[0].GetFloat("_Speed");
        _Length = mat[0].GetFloat("_Length");
    }

    // Update is called once per frame
    void Update()
    {
        mat[0].SetFloat("_Times",Time.time);
        Bmat[0].SetFloat("_Times", Time.time);
    }

    public float GetwaveHeight(Vector3 IN)
    {
        return Mathf.Sin(Time.time * _Speed + IN.x / _Length) * _Displacement + Mathf.Sin(Time.time * _Speed / 2.0f + IN.y / _Length) * _Displacement;
    }
}
