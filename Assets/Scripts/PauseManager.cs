using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // Updated to manage all panels and pause the game when any menu panels are open.
    [Header("Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopPanel;
    private bool isPaused, shopWasOpen = false;

    private void PauseGame()
    {
        bool anyPanelOpen = isPaused
            || settingsPanel.activeSelf;

        Time.timeScale = anyPanelOpen ? 0f : 1f;
    }

    // Opens option panel and keeps shop panel closed, restores shop panel if it was flagged as open before.
    public void ToggleOptions()
    {
        isPaused = !isPaused;
        optionsPanel?.SetActive(isPaused);

        if (isPaused)
        {
            BackgroundBlurManager.Instance.RegisterPanelOpened();
            optionsPanel.transform.SetAsLastSibling();

            if (shopPanel != null && shopPanel.activeSelf)
            {
                shopWasOpen = true;
                shopPanel.SetActive(false);
            }
        }
        else
        {
            BackgroundBlurManager.Instance.RegisterPanelClosed();
            settingsPanel?.SetActive(false);

            if (shopWasOpen)
            {
                shopPanel.SetActive(true);
                shopWasOpen = false;
            }
        }
        PauseGame();
    }

    // Similarly, opens settings panel and keeps shop panel closed, restores shop panel if it was flagged as open before.
    public void ToggleSettings()
    {
        bool opening = !settingsPanel.activeSelf;
        settingsPanel.SetActive(opening);

        if (opening)
        {
            settingsPanel.transform.SetAsLastSibling();

            if (shopPanel != null && shopPanel.activeSelf)
            {
                shopWasOpen = true;
                shopPanel.SetActive(false);
            }
        }
        else
        {
            if (shopWasOpen && !isPaused)
            {
                shopPanel.SetActive(true);
                shopWasOpen = false;
            }
        }
        PauseGame();
    }

    public void ToggleTutorial()
    {
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