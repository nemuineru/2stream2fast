using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public Slider slider;
    public string sliderParamName;
    public AudioSource testSound;
    public AudioMixer mixer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mixer.SetFloat(sliderParamName,20f * Mathf.Log10(Mathf.Clamp(slider.value,0.0001f,100f)/100f));
    }

    public void Sounds() {
        testSound.Play();
    }
}
