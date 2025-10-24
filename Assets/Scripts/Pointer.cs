using UnityEngine;

public class Pointer : MonoBehaviour
{
    public float rotationSpeed = 30f; 
    public float speed = 5f;         
    public float heightOffset = 0.5f; 
    public float pulse = 1f;   

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Spins the pointer around its Y axis and makes it pop up and down.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * speed, Space.Self);
        float bob = Mathf.Sin(Time.time * pulse * Mathf.PI * 2f) * heightOffset;
        transform.position = startPos + new Vector3(0f, bob, 0f);
    }
}
