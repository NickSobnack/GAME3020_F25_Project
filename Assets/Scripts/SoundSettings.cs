using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] Sprite soundOnIcon;
    [SerializeField] Sprite soundOffIcon;

    Button button;
    Image image;
    private bool isMusicEnabled = true;

    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        button.onClick.AddListener(ToggleMusic);

        UpdateIcon();
    }

    // Turns music on/off and updates the button icon.
    private void ToggleMusic()
    {
        isMusicEnabled = !isMusicEnabled;
        AudioManager.Instance.ToggleMusic(isMusicEnabled);
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (image != null)
            if (isMusicEnabled)
                image.sprite = soundOnIcon;
            else
                image.sprite = soundOffIcon;
    }
}
