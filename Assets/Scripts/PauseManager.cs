using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // Updated to manage all panels and pause the game when any menu panels are open.
    [Header("Panels")]
    [SerializeField] private GameObject optionsPanel;
    //[SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject settingsPanel;

    private bool isPaused = false;

    private void PauseGame()
    {
        bool anyPanelOpen = isPaused
            //|| tutorialPanel.activeSelf
            || settingsPanel.activeSelf;

        Time.timeScale = anyPanelOpen ? 0f : 1f;
    }

    public void ToggleOptions()
    {
        isPaused = !isPaused;
        optionsPanel?.SetActive(isPaused);

        if (!isPaused)
        {
            //tutorialPanel?.SetActive(false);
            settingsPanel?.SetActive(false);
        }
        PauseGame();
    }

    public void ToggleTutorial()
    {
        //tutorialPanel.SetActive(!tutorialPanel.activeSelf);
        PauseGame();
    }

    public void ToggleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        PauseGame();
    }

    public void MainMenuButton(int sceneIndex)
    {
        Time.timeScale = 1f;
        GameManager.Instance.ResetGame();
        GameManager.Instance.ChangeScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit.");
    }
}