using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Safe Zone");
            Animator healAnimator = collision.GetComponentInChildren<Animator>();
            healAnimator.SetTrigger("Heal");
        }
    }
}