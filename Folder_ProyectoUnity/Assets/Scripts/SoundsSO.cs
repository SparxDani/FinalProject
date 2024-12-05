using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
[CreateAssetMenu(fileName = "SoundsSO", menuName = "SoundsSO/SoundsSO", order = 0)]
public class SoundsSO : ScriptableObject
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string channelVolume;
    [SerializeField] private float currentVolume = 0.75f;
    private const float MIN_VOLUME = -80f;
    private const float MAX_VOLUME = 20f;

    private bool isMuted = false;

    public void UpdateVolume(Slider slider)
    {
        float newVolume = Mathf.Clamp(slider.value, 0.0001f, 1f);
        currentVolume = newVolume;
        ApplyVolume(newVolume);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        if (isMuted)
        {
            ApplyVolume(0f);
        }
        else
        {
            ApplyVolume(currentVolume);
        }
    }

    private void ApplyVolume(float volume)
    {
        float decibels = volume > 0 ? Mathf.Log10(volume) * MAX_VOLUME : MIN_VOLUME;
        mixer.SetFloat(channelVolume, decibels);
    }

    public void RestoreVolume()
    {
        ApplyVolume(isMuted ? 0f : currentVolume);
    }
}