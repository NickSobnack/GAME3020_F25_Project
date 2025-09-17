using UnityEngine;

public class Arrow : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Using the player's BattleLogic script to check if they are blocking.
            // Blocking the arrow results in no damage taken while not blocking results in a hit.
            BattleLogic battleLogic = collision.gameObject.GetComponent<BattleLogic>();

                if (battleLogic.playerAnimator.GetBool("isBlocking"))
                    Debug.Log("Perfect");
                else 
                    Debug.Log("Player hit!");
            Destroy(gameObject);
        }
    }
}
