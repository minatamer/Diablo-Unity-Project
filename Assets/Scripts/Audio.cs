using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    public GameObject soundPanel;          // Assign the Sound Panel in the Inspector
    public Slider musicSlider;            // Assign the Music Volume slider in the Inspector
    public Slider effectsSlider;          // Assign the Effects Volume slider in the Inspector
    public AudioMixer audioMixer;         // Assign the MainAudioMixer in the Inspector

    void Start()
    {
        // Load saved volume levels
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f); // Default to 0.5 (50% volume)
        float effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 0.5f);

        // Set sliders and mixer levels
        musicSlider.value = musicVolume;
        effectsSlider.value = effectsVolume;

        SetMusicVolume(musicVolume);
        SetEffectsVolume(effectsVolume);

        // Add listeners to sliders
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
    }

    public void ToggleSoundPanel()
    {
        soundPanel.SetActive(!soundPanel.activeSelf);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume <= 0.0001f) // Prevent extreme values
        {
            audioMixer.SetFloat("MusicVolume", -80f); // Minimum volume (mute)
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20); // Convert to decibels
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetEffectsVolume(float volume)
    {
        if (volume <= 0.0001f) // Prevent extreme values
        {
            audioMixer.SetFloat("EffectsVolume", -80f); // Minimum volume (mute)
        }
        else
        {
            audioMixer.SetFloat("EffectsVolume", Mathf.Log10(volume) * 20); // Convert to decibels
        }
        PlayerPrefs.SetFloat("EffectsVolume", volume);
    }
}
