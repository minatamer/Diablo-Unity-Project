using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Choose : MonoBehaviour
{
    public Toggle barbarianToggle; // Assign the Barbarian toggle in the Inspector
    public Toggle sorcererToggle;  // Assign the Sorcerer toggle in the Inspector
    public Button confirmButton;   // Assign the Confirm button in the Inspector

    public GameObject barbarianObject; // Assign the Barbarian GameObject in the Inspector
    public GameObject sorcererObject;  // Assign the Sorcerer GameObject in the Inspector

    private string selectedCharacter = ""; // No character selected by default

    void Start()
    {
        // Ensure both characters are deactivated initially
        barbarianObject.SetActive(false);
        sorcererObject.SetActive(false);

        // Ensure toggles are unticked initially
        barbarianToggle.isOn = false;
        sorcererToggle.isOn = false;

        // Add listeners to toggles
        barbarianToggle.onValueChanged.AddListener(OnBarbarianToggleChanged);
        sorcererToggle.onValueChanged.AddListener(OnSorcererToggleChanged);

        // Add listener to the confirm button
        confirmButton.onClick.AddListener(OnConfirmSelection);
    }

    public void OnBarbarianToggleChanged(bool isOn)
    {
        if (isOn)
        {
            // Select Barbarian and deactivate Sorcerer
            selectedCharacter = "Barbarian";
            barbarianObject.SetActive(true);
            sorcererObject.SetActive(false);

            // Untick Sorcerer toggle
            sorcererToggle.isOn = false;
        }
    }

    public void OnSorcererToggleChanged(bool isOn)
    {
        if (isOn)
        {
            // Select Sorcerer and deactivate Barbarian
            selectedCharacter = "Sorcerer";
            sorcererObject.SetActive(true);
            barbarianObject.SetActive(false);

            // Untick Barbarian toggle
            barbarianToggle.isOn = false;
        }
    }

    public void OnConfirmSelection()
    {
        if (string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.LogWarning("No character selected!");
            return;
        }

        Debug.Log($"Confirmed selection: {selectedCharacter}");

        // Save the selected character for the gameplay scene
        PlayerPrefs.SetString("SelectedCharacter", selectedCharacter);

        // Load the gameplay scene
        SceneManager.LoadScene("gamescene"); // Replace with your actual gameplay scene name
    }
}
