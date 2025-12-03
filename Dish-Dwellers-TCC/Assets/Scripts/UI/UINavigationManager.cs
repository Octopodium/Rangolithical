using UnityEngine;
using UnityEngine.EventSystems;

public class UINavigationManager : MonoBehaviour
{
    public EventSystem eventSystem;
    public GameObject currentFirstSelected;

    void Awake() {
        if (eventSystem == null) {
            eventSystem = FindFirstObjectByType<EventSystem>();
        }

        TrocaFirstSelected(currentFirstSelected);
    }

    public void TrocaFirstSelected(GameObject firstSelected) {
        eventSystem.SetSelectedGameObject(firstSelected);
        currentFirstSelected = firstSelected;
    }
}
