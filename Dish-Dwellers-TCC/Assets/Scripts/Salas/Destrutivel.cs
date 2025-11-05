using UnityEngine;
using UnityEngine.Events;

public class Destrutivel : MonoBehaviour
{
    Rigidbody rb;
    public UnityEvent OnDestruido;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void Destroi() {
        if (rb != null) {
           if (!rb.isKinematic) rb.linearVelocity = Vector3.zero; 
        }

        OnDestruido?.Invoke();
    }
}
