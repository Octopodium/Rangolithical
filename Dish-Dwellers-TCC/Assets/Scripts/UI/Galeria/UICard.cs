using TMPro;
using UnityEngine;
using System.Collections;
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
    public RectTransform cardImageFullSize;
    public GameObject cadeadoFull;
    public GameObject requisitosFullPos;
    public GameObject polaroid;
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
        if(card.comprado){
            cadeado.SetActive(false);
            return;
        }
        //ConfiguracoesNormais();
        ConfigurarRequisitosMini();
    }

    public void ConstruirFullCard(){
        if (card == null) return;

        if (nomeFull != null) nomeFull.text = card.cardName;
        if (descriptionFull != null) descriptionFull.text = card.description;
        if (cardImageFull != null) cardImageFull.sprite = card.cardFull;
        if(card.comprado){
            SetarComprado();
            return;
        }
        ConfiguracoesNormais();
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
        bool podeComprar = true;
        foreach(IngredienteData ing in card.requisitos){
            bool hasIngrediente = ColecionavelController.instance.TemDisponivel(ing);
            Debug.Log(hasIngrediente);
            if(!hasIngrediente){
                podeComprar = false;
                break;
            }
        }

        if(podeComprar){
            card.comprado = true;
            SetarComprado();
            foreach(IngredienteData ing in card.requisitos) {
                ColecionavelData colecionavel = ColecionavelController.instance.PegarUtilizavel(ing);
                Debug.Log(colecionavel);
                if(colecionavel != null){
                    ColecionavelController.instance.Utilizar(colecionavel);
                }
            }
        }
    }

    public void SetarComprado(){
        cadeadoFull.SetActive(false);
        polaroid.SetActive(false);
        requisitosFullPos.SetActive(false);
        //cardImageFull.transform.localScale += new Vector3(0.3f, 0.3f, 0.3f); 
        //cardImageFullSize.sizeDelta = new Vector2(cardImageFull.sprite.texture.width, cardImageFull.sprite.texture.height)/5;
        StartCoroutine("ScaleIn");
        comprarFullBtn.gameObject.SetActive(false);
    }

    public void ConfiguracoesNormais(){
        cadeadoFull.SetActive(true);
        polaroid.SetActive(true);
        requisitosFullPos.SetActive(true);
        comprarFullBtn.gameObject.SetActive(true);
        cardImageFullSize.sizeDelta = new Vector2(180f, 180f);
    }

    IEnumerator ScaleIn(){
        float tempo = 0f;
        Vector2 initialSize = cardImageFullSize.sizeDelta;
        Vector2 finalSize = new Vector2(cardImageFull.sprite.texture.width, cardImageFull.sprite.texture.height)/5;
        while (cardImageFullSize.sizeDelta != finalSize){
            float animDurantion = tempo / 0.3f;
            cardImageFullSize.sizeDelta = Vector2.Lerp(initialSize, finalSize, animDurantion);
            tempo += Time.unscaledDeltaTime;
            yield return null;
        }
        cardImageFullSize.sizeDelta = finalSize;
    }
}