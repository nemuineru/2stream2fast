using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSystem_InGame : MonoBehaviour
{
    public HoverCraft craft;
    public TMP_Text text;

    Rigidbody craftRig;
    // Start is called before the first frame update
    void Start()
    {
        craftRig = craft.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (craftRig.velocity.magnitude * 3.6f/1.852).ToString("f1") + " knot";
        if (Input.GetKeyDown(KeyCode.R)) {
            craft.transform.position = new Vector3(0, 0, 0.3f);
            craft.transform.rotation = Quaternion.identity;
        }
    }
}
