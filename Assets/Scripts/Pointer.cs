using UnityEngine;

public class Pointer : MonoBehaviour
{
    public float rotationSpeed = 30f; 
    public float speed = 5f;         
    public float heightOffset = 0.2f; 
    public float pulse = 1f;   

    private Vector3 startPos;

    private void Start()
    {
        startPos = new Vector3(0f, 1.2f, 0f);
    }

    // Bobs the pointer up and down to indicate current player target.
    void Update()
    {
        float bob = Mathf.Sin(Time.time * pulse * Mathf.PI * 2f) * heightOffset;
        transform.localPosition = startPos + new Vector3(0f, bob, 0f);
    }
}
