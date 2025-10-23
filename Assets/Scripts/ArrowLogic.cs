using UnityEngine;

public class ArrowLogic : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }
}
