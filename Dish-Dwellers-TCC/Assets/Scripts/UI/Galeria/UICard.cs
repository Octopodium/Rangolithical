using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    [Header("UI Items")]
    public Button comprar;
    public TMP_Text nome;
    public TMP_Text description;
    public GameObject cadeado;
    public GameObject[] requisitos;

    public CardSO card;

    public void Initialize(CardSO card){
        this.card = card;
    }
}