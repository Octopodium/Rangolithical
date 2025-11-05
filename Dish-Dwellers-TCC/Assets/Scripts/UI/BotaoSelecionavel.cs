using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BotaoSelecionavel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    Button botao;

    void Awake() {
        botao = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (botao.interactable) {
            botao.Select();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(botao.interactable) botao.OnDeselect(eventData);
    }

}
