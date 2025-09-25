using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    // Animation event to destroy the vfx after it animates.
    public void DestroyPopupEffect()
    {
        Destroy(gameObject);
    }

    // Animation event to pause the game when win/lose stage occurs.
    public void GameStateEffect()
    {
        Time.timeScale = 0f;
    }
}
