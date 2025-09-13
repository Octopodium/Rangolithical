using UnityEngine;
using UnityEngine.Events;

public class Destrutivel : MonoBehaviour
{
    public UnityEvent OnDestruido;


    private void OnDestroy(){
        OnDestruido?.Invoke();
    }

    public void Destroi() {
        OnDestruido?.Invoke();
    }
}
