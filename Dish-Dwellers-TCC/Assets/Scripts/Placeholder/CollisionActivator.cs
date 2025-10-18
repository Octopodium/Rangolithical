using UnityEngine;

public class ToggleObjectOnHit : MonoBehaviour
{
    [Header("Objeto a ser alternado")]
    [SerializeField] private GameObject targetObject;

    [Header("Configurações")]
    [SerializeField] private bool useTrigger = false; 
    [SerializeField] private string activatorTag = ""; 

    private void OnCollisionEnter(Collision collision)
    {
        if (useTrigger) return; 
        TryToggle(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return; 
        TryToggle(other.gameObject);
    }

    private void TryToggle(GameObject hitter)
    {
        if (!string.IsNullOrEmpty(activatorTag) && !hitter.CompareTag(activatorTag))
            return;

        if (targetObject != null)
        {
            bool newState = !targetObject.activeSelf;
            targetObject.SetActive(newState);
        }
        else
        {
            Debug.LogWarning($"{name} não tem um objeto alvo atribuído!");
        }
    }
}