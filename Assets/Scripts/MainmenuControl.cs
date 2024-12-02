using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuControl : MonoBehaviour
{
    public void StartNewGame()
    {
        SceneManager.LoadScene("CharacterChoose");
    }

    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
