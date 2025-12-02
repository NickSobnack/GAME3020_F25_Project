using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    private bool tutorialVisible = false;

    public void StartGame(int gameSceneIndex)
    {
        GameManager.Instance.DelayLoadScene(gameSceneIndex, 0.5f);
    }

    public void ToggleTutorial()
    {
        tutorialVisible = !tutorialVisible;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(tutorialVisible);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit.");
    }
}
