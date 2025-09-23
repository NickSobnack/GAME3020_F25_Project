using UnityEngine;

public class Arrow : MonoBehaviour
{
    void Start()
    {
        // Destroy arrow after 5 seconds.
        Destroy(gameObject, 5f);
    }
}
