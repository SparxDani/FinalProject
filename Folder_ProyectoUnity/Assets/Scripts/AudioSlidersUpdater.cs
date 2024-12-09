using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioSlidersUpdater : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Audio Settings SOs")]
    [SerializeField] private SoundsSO masterVolumeSO;
    [SerializeField] private SoundsSO musicVolumeSO;
    [SerializeField] private SoundsSO effectsVolumeSO;

    private void Start()
    {
        UpdateSliders();
    }

    private void UpdateSliders()
    {
        if (masterVolumeSlider != null && masterVolumeSO != null)
            masterVolumeSlider.value = masterVolumeSO.currentVolume;

        if (musicVolumeSlider != null && musicVolumeSO != null)
            musicVolumeSlider.value = musicVolumeSO.currentVolume;

        if (sfxVolumeSlider != null && effectsVolumeSO != null)
            sfxVolumeSlider.value = effectsVolumeSO.currentVolume;
    }
}
