using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;  // Add this line to access UI components like Button

public class PauseScene : MonoBehaviour
{
    public static PauseScene Instance { get; private set; }
    private UnityEngine.EventSystems.EventSystem gameEventSystem;  // Store reference to the EventSystem
    private AudioSource[] gameAudioSources;  // Store reference to game audio sources
    private AudioSource pauseSceneAudioSource;  // Store reference to the pause scene's audio source

    //public AudioMixer audioMixer;
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure there's only one instance
            return;
        }
        Instance = this;
        // Find all audio sources in the game scene
        gameAudioSources = FindObjectsOfType<AudioSource>();

        // Exclude the audio source in the pause scene
        pauseSceneAudioSource = GetComponent<AudioSource>();

        gameEventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();

        if (gameEventSystem != null)
        {
            gameEventSystem.gameObject.SetActive(false); // Disable EventSystem in the game scene
        }

        // Disable extra AudioListeners
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++) // Leave one active listener
            {
                listeners[i].enabled = false;
            }
        }
        foreach (AudioSource audioSource in gameAudioSources)
        {
            if (audioSource != pauseSceneAudioSource) // Exclude the pause scene's audio source
            {
                audioSource.Pause();
            }
        }
        if (pauseSceneAudioSource != null)
        {
            pauseSceneAudioSource.Play();
        }

    }

    public void Restart()
    {
        Time.timeScale = 1;
        if (gameController.Instance.bossLevel == false)
        {
            SceneManager.LoadScene("gamescene");
        }
        else
        {
            if (PlayerPrefs.HasKey("healthPoints"))
            {
                PlayerPrefs.DeleteKey("healthPoints"); //this will make it so that if the player restarts he will restart with the restart configuration instead
            }
            SceneManager.LoadScene("BossScene");
        }
    }
    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        gameController.Instance.pause = false;
        Debug.Log("Resuming Game");
        Time.timeScale = 1; // Resume the game
        SceneManager.UnloadSceneAsync("PauseScene");

        if (gameEventSystem != null)
        {
            gameEventSystem.gameObject.SetActive(true); // Reactivate EventSystem in the game scene
        }

        //// Disable the EventSystem in the PauseScene if one exists
        //var pauseEventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        //if (pauseEventSystem != null)
        //{
        //    pauseEventSystem.gameObject.SetActive(false); // Deactivate PauseScene's EventSystem
        //}

        // Re-enable the Pause button
        Button pauseButton = FindObjectOfType<Button>(); // Assuming your pause button is set up as the first button in the scene
        if (pauseButton != null)
        {
            pauseButton.interactable = true;  // Make sure the button is interactable after resuming
        }

        // Resume the game scene's audio sources
        foreach (AudioSource audioSource in gameAudioSources)
        {
            if (audioSource != pauseSceneAudioSource) // Exclude the pause scene's audio source
            {
                audioSource.UnPause();
            }
        }

        // Stop the pause scene's audio source
        if (pauseSceneAudioSource != null)
        {
            pauseSceneAudioSource.Stop();
        }
    }
}
