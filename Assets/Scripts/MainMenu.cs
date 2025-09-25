using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject tutorialPanel;

    // Start the game by loading the main game scene.
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Opens & closes the tutorial panel.
    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
    }
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    // Quits the game application.
    public void QuitGame()
    {
        Application.Quit();
    }
}
