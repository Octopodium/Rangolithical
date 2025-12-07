using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    public Button botao;
    public Image newTarget;

    public void Start(){
        newTarget = GetComponentInChildren<Image>();
        newTarget = newTarget.GetComponentInChildren<Image>();
        botao.targetGraphic = newTarget;
    }
}
