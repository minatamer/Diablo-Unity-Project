using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuControl : MonoBehaviour
{
    public Audio soundSettingsController; // Assign the SoundSettingsController script
    public GameObject creditsCanvas;      // Assign the Credits Canvas in the Inspector
    public GameObject levelSelectPanel;        // Assign the LevelSelectPanel GameObject
    public Toggle basicLevelToggle;           // Assign the Basic Level toggle in the Inspector
    public GameObject soundSettingsPanel; // Assign the Sound Settings Panel GameObject in the Inspector

    public Toggle bossLevelToggle;
    void Start()
    {
        // Ensure the level toggles are unticked initially
        basicLevelToggle.isOn = false;
        bossLevelToggle.isOn = false;

        // Add listeners to prevent both toggles from being ticked at the same time
        basicLevelToggle.onValueChanged.AddListener((isOn) => OnLevelToggleChanged(isOn, bossLevelToggle));
        bossLevelToggle.onValueChanged.AddListener((isOn) => OnLevelToggleChanged(isOn, basicLevelToggle));

        //By default lets say basic level
        PlayerPrefs.SetString("SelectedLevel", "Basic");
    }
    public void OpenLevelSelectPanel()
    {
        levelSelectPanel.SetActive(true); // Show the level selection panel
        soundSettingsPanel.SetActive(false); // Hide the sound settings panel if it's open


    }

    public void HideLevelSelectPanel()
    {
        levelSelectPanel.SetActive(false); // Hide the level selection panel
    }
    public void StartNewGame()
    {
        SceneManager.LoadScene("CharacterChoose");
    }

    
    

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSoundSettings()
    {
        soundSettingsController.ToggleSoundPanel();
        levelSelectPanel.SetActive(false);   // Hide the level select panel if open

    }

    public void ShowCredits()
    {
        // Activate the Credits canvas
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(true);
            levelSelectPanel.SetActive(false); // Hide the level select panel if it's open
            soundSettingsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Credits canvas is not assigned in the Inspector.");
        }
    }

    public void HideCredits()
    {
        // Deactivate the Credits canvas
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Credits canvas is not assigned in the Inspector.");
        }
    }
    private void OnLevelToggleChanged(bool isOn, Toggle otherToggle)
    {
        if (isOn)
        {
            otherToggle.isOn = false; // Untick the other toggle if this one is ticked
        }
    }

    public void ConfirmLevelSelection()
    {
        if (basicLevelToggle.isOn)
        {
            Debug.Log("Basic Level selected");
            PlayerPrefs.SetString("SelectedLevel", "Basic");
        }
        else if (bossLevelToggle.isOn)
        {
            Debug.Log("Boss Level selected");
            PlayerPrefs.SetString("SelectedLevel", "Boss");
        }
        //else
        //{
        //    Debug.LogError("No level selected! Please select a level.");
        //    return; // Exit if no level is selected
        //}
        HideLevelSelectPanel();
    }
    }
