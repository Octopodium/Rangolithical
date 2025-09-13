using UnityEngine;
using UnityEngine.Events;

public class Destrutivel : MonoBehaviour
{
    public UnityEvent OnDestruido;

    public void Destroi() {
        OnDestruido?.Invoke();
    }
}
