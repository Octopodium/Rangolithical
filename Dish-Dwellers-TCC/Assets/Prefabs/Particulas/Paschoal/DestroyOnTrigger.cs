using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        Destroy(gameObject);
    }
}
