using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;    

public class VolumeHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AudioMixer audioMixer; 
    [SerializeField] private string exposedParam = "VolumeControl"; 

    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        //Load the saved volume (0.75 is the default if no save exists)
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        slider.value = savedVolume;
        ApplyVolume(savedVolume);

        //run code every time the user moves it
        slider.onValueChanged.AddListener(ApplyVolume);
    }

    public void ApplyVolume(float sliderValue)
    {
        // Prevent math errors: Log10(0) is impossible, so we treat very low as -80dB (silent)
        if (sliderValue <= 0.0001f)
        {
            audioMixer.SetFloat(exposedParam, -80f);
        }
        else
        {
            // Logarithmic conversion: 0-1 slider scale becomes -80dB to 0dB
            float dB = Mathf.Log10(sliderValue) * 20;
            audioMixer.SetFloat(exposedParam, dB);
        }

        // Save the value 
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }
}
