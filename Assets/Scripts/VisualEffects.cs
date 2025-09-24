using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    // Animation event to destroy the vfx after it animates.
    public void DestroyPopupEffect()
    {
        Destroy(gameObject);
    }

    public void GameOverEffect()
    {
        Time.timeScale = 0f;
    }
}
