using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu Panel")]
    [SerializeField] private GameObject pausePanel;

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeButton()
    {
        if (isPaused)
            TogglePause();
    }

    public void SettingsButton()
    {
        Debug.Log("Settings menu opened.");
    }

    public void MainMenuButton(int sceneIndex)
    {
        Time.timeScale = 1f;
        GameManager.Instance.ResetGame();
        GameManager.Instance.ChangeScene(sceneIndex);
    }
}
