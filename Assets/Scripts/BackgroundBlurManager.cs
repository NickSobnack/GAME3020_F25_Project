using UnityEngine;

public class BackgroundBlurManager : MonoBehaviour
{
    public static BackgroundBlurManager Instance;

    [SerializeField] private GameObject backgroundBlur;

    private int openPanelCount = 0;

    private void Awake()
    {
        Instance = this;
        backgroundBlur.SetActive(false);
    }

    public void RegisterPanelOpened()
    {
        openPanelCount++;
        backgroundBlur.SetActive(true);
    }

    public void RegisterPanelClosed()
    {
        openPanelCount = Mathf.Max(0, openPanelCount - 1);

        if (openPanelCount == 0)
            backgroundBlur.SetActive(false);
    }
}
