using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    [Header("Effect Properties")]
    [SerializeField] private float lifetime = 1.0f;      
    [SerializeField] private float maxScale = 1.5f;   
    [SerializeField] private float endScale = 0.5f;    

    private Vector3 startScale;
    private float timer;

    void Start()
    {
        startScale = transform.localScale;
        transform.localScale = startScale * 0.1f;
        timer = 0f;
    }

    // Scales up the sprite in the first hald of its lifetime and scales down in the second half.
    // Destroys the game object after its lifetime ends.
    void Update()
    {
        timer += Time.deltaTime;

        if (timer <= lifetime / 2f)
        {
            float t = timer / (lifetime / 2f);
            transform.localScale = Vector3.Lerp(startScale * 0.1f, startScale * maxScale, t);
        }
        else
        {
            float t = (timer - lifetime / 2f) / (lifetime / 2f);
            transform.localScale = Vector3.Lerp(startScale * maxScale, startScale * endScale, t);
        }
        if (timer >= lifetime)
        {
            Destroy(gameObject); 
        }
    }
}
