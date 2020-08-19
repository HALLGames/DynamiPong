using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Slider volumeSlider;
    public Button backButton;

    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChange);
        backButton.onClick.AddListener(Disable);
    }

    public void OnVolumeSliderChange(float value)
    {
        AudioListener.volume = value;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
