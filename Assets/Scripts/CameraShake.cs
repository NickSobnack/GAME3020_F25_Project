using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeAmount = 0.02f;
    [SerializeField] private float shakeDuration = 0.5f;

    private Vector3 originalPos;
    private float shakeTimer = 0f;

    private void Awake()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeTimer -= Time.unscaledDeltaTime; 
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }

    public void TriggerShake()
    {
        Debug.Log("TriggerShake called");
        shakeTimer = shakeDuration;
    }
}