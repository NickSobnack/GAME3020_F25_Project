using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public Tooltip tooltip;

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

    public static void Show()
    {
        Instance.tooltip.gameObject.SetActive(true);
    }
    
    public static void Hide()
    {
        Instance.tooltip.gameObject.SetActive(false);
    }
}
