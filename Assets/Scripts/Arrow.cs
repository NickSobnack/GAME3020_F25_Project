using UnityEngine;

public class Arrow : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Arrow hit the player!");
            Destroy(gameObject);
        }
    }
}
