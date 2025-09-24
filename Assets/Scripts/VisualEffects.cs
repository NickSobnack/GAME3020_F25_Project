using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    // Animation event to destroy the vfx after it animates.
    public void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
