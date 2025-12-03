using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class Galeria : MonoBehaviour {

    [Header("Configurações")]
    public CardSO[] todosCards;
    public Transform[] cardsPos;
    public UICard fullCardPanel;
    public UICard miniCardPrefab;
    public Transform containerLeft, containerRight;
    int i = 0;

    private Dictionary<int, UICard> miniCardsInstanciados = new Dictionary<int, UICard>();

    public void Start(){
        InicializarGaleria();
        i = 0;
        if (fullCardPanel != null) fullCardPanel.gameObject.SetActive(false);
    }

    public void InicializarGaleria(){        
        foreach (CardSO card in todosCards){
            UICard miniCard = Instantiate(miniCardPrefab, cardsPos[i]);
            miniCard.Initialize(card);
            miniCard.ConstruirMiniCard();
            
            miniCard.comprar.onClick.AddListener(() => AbrirFullCard(card));
            
            miniCardsInstanciados.Add(card.id, miniCard);
            i++;
        }
    }

    public void AbrirFullCard(CardSO card){
        if (fullCardPanel != null){
            fullCardPanel.gameObject.SetActive(true);
            fullCardPanel.Initialize(card);
            fullCardPanel.ConstruirFullCard();
        }
    }
    
    public void FecharFullCard(){
        if (fullCardPanel != null) fullCardPanel.gameObject.SetActive(false);
    }
}