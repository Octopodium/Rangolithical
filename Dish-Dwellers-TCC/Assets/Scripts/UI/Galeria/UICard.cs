using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    [Header("UI Items - Mini Card")]
    public Button comprar;
    public TMP_Text nome;
    public TMP_Text description;
    public Image cardImage;
    public GameObject cadeado;
    public GameObject[] requisitos;

    [Header("UI Items - Full Card")]
    public TMP_Text nomeFull;
    public TMP_Text descriptionFull;
    public Image cardImageFull;
    public GameObject[] requisitosFull;
    public Button fecharBtn;
    public Button comprarFullBtn;

    public CardSO card;
    public bool isFullCard = false;

    public void Initialize(CardSO card){
        this.card = card;
        
        if (fecharBtn != null){
            fecharBtn.onClick.RemoveAllListeners();
            fecharBtn.onClick.AddListener(() => FindObjectOfType<Galeria>().FecharFullCard());
        }
        
        if (comprarFullBtn != null){
            comprarFullBtn.onClick.RemoveAllListeners();
            comprarFullBtn.onClick.AddListener(Comprar);
        }
    }

    public void ConstruirMiniCard(){
        if (card == null) return;
        if (cardImage != null) cardImage.sprite = card.cardPreview;

        ConfigurarRequisitosMini();
    }

    public void ConstruirFullCard(){
        if (card == null) return;

        if (nomeFull != null) nomeFull.text = card.cardName;
        if (descriptionFull != null) descriptionFull.text = card.description;
        if (cardImageFull != null) cardImageFull.sprite = card.cardFull;

        ConfigurarRequisitosFull();
    }

    private void ConfigurarRequisitosMini(){
        if (requisitos == null || card.requisitos == null) return;

        for (int i = 0; i < requisitos.Length; i++){
            if (i < card.requisitos.Length){
                requisitos[i].SetActive(true);
                //nao temos mais que um requisito ainda, mas quando tiver vai ter que checar pra mudar o numero
            }else{
                requisitos[i].SetActive(false);
            }
        }
    }

    private void ConfigurarRequisitosFull(){
        if (requisitosFull == null || card.requisitos == null) return;

        for (int i = 0; i < requisitosFull.Length; i++){
            if (i < card.requisitos.Length){
                requisitosFull[i].SetActive(true);
            }else{
                requisitosFull[i].SetActive(false);
            }
        }
    }

    public void Comprar(){
        foreach(IngredienteData ing in card.requisitos){
            if(ColecionavelController.instance.TemDisponivel(ing)){
                Debug.Log("tentando comprar");
            }
        }
    }
}