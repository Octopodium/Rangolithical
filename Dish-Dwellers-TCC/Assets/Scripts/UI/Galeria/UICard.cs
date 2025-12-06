using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    [Header("UI Items - Mini Card")]
    public Button comprar;
    public Image cardImage;
    public GameObject cadeado;
    public GameObject requisitosPos;
    public GameObject requisitoPrefab;

    [Header("UI Items - Full Card")]
    public TMP_Text nomeFull;
    public TMP_Text descriptionFull;
    public Image cardImageFull;
    public GameObject requisitosFullPos;
    public Button fecharBtn;
    public Button comprarFullBtn;

    public CardSO card;

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
        if (requisitosPos == null || card.requisitos == null || requisitoPrefab == null) return;

        foreach (Transform child in requisitosPos.transform){
            Destroy(child.gameObject);
        }

        foreach (IngredienteData ingrediente in card.requisitos){
            GameObject requisitoUI = Instantiate(requisitoPrefab, requisitosPos.transform);
            
            Image img = requisitoUI.GetComponentInChildren<Image>();
            if (img != null && ingrediente.sprite != null){
                img.sprite = ingrediente.sprite;
            }
        }
    }

    private void ConfigurarRequisitosFull(){
        //if (requisitosFullPos == null || card.requisitos == null || requisitoPrefab == null) return;

        foreach (Transform child in requisitosFullPos.transform){
            Destroy(child.gameObject);
        }

        foreach (IngredienteData ingrediente in card.requisitos){
            GameObject requisitoUI = Instantiate(requisitoPrefab, requisitosFullPos.transform);
            
            Image img = requisitoUI.GetComponentInChildren<Image>();
            if (img != null && ingrediente.sprite != null){
                img.sprite = ingrediente.sprite;
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