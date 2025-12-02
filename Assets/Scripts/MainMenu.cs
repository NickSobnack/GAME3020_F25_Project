using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    private bool isVisible = false;

    public void StartGame(int gameSceneIndex)
    {
        GameManager.Instance.DelayLoadScene(gameSceneIndex, 0.5f);
    }

    public void ToggleTutorial()
    {
        isVisible = !isVisible;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(isVisible);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit.");
    }
}
