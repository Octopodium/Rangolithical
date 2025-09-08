using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviour {
    public UnityEvent<Collider> onTriggerEnter, onTriggerExit, onTriggerStay;
    public System.Action<Collider> onTriggerEnterAction, onTriggerExitAction, onTriggerStayAction;
    public string tagFilter;

    private void OnTriggerEnter(Collider other) {
        if(string.IsNullOrEmpty(tagFilter) || other.CompareTag(tagFilter)) {
            onTriggerEnter?.Invoke(other);
            onTriggerEnterAction?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(string.IsNullOrEmpty(tagFilter) || other.CompareTag(tagFilter)) {
            onTriggerExit?.Invoke(other);
            onTriggerExitAction?.Invoke(other);
        }
    }

    private void OnTriggerStay(Collider other) {
        if(string.IsNullOrEmpty(tagFilter) || other.CompareTag(tagFilter)) {
            onTriggerStay?.Invoke(other);
            onTriggerStayAction?.Invoke(other);
        }
    }
}
