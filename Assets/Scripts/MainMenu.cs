using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel, optionsPanel;
    private bool isVisible = false;

    public void StartGame(int gameSceneIndex)
    {
        GameManager.Instance.ResetGame();
        GameManager.Instance.DelayLoadScene(gameSceneIndex, 0.5f);
    }

    public void ToggleTutorial()
    {
        tutorialPanel.SetActive(!tutorialPanel.activeSelf);
    }

    public void ToggleOptions()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit.");
    }
}
