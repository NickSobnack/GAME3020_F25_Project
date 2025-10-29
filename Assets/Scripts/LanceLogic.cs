using UnityEngine;

public class LanceLogic : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    public float damage = 2f;

    private void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }
}
