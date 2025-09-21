using UnityEngine;
using System.Collections;

public class ObjectShaker : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;

    private Vector3 initialPosition;

    public bool shakeOnStart = false, endShakeOnTime = true;

    void Awake()
    {
        initialPosition = transform.localPosition;
    }

    void Start() {
        if (shakeOnStart) Shake();
    }

    public void Shake() 
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine());
    }

    public void StopShake()
    {
        StopAllCoroutines();
        transform.localPosition = initialPosition;
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;
        while (!endShakeOnTime || elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            float z = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = initialPosition + new Vector3(x, y, z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = initialPosition;
    }
}