using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject tutorialPanel;
    public string currentNodeName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Sets and gets the current node by its name.
    public void SetCurrentNode(Node node)
    {
        currentNodeName = node.name;
    }

    public string GetCurrentNodeName()
    {
        return currentNodeName;
    }

    // Single method to toggle tutorial panel.
    public void ToggleTutorial(bool isVisible)
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(isVisible);
    }

    // Load scene by index.
    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Delay coroutine inbetween loading scenes.
    public void DelayLoadScene(int sceneIndex, float delay)
    {
        StartCoroutine(DelayScene(sceneIndex, delay));
    }

    private IEnumerator DelayScene(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneIndex);
    }
    
    // Quits the game/application.
    public void QuitGame()
    {
        Application.Quit();
    }
}
