using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseUI : MonoBehaviour {
    public GameObject[] telasInternasPause;
    public GameObject telaPrincipalPause;

    [Header("Event System")]
    public EventSystem eventSystem;
    public GameObject primeiroSelecionadoPause;

    bool inicializado = false;

    [Header("Opcoes - Controles Locais")]
    public Selectable campoEmCimaDoPainelDeControles;
    public GameObject controlesLocaisPanel;
    public Button singleplayerButton, multiplayerButton, ajustarMultiplayerButton;


    // Chamado no UIManager
    public void Inicializar() {
        GameManager.OnPause += HandlePausa;

        if (eventSystem == null) {
            eventSystem = FindFirstObjectByType<EventSystem>();
        }

        inicializado = true;
    }


    private void OnDestroy() {
        GameManager.OnPause -= HandlePausa;
        inicializado = false;
    }

    public void HandlePausa(bool estado){
        if (gameObject == null || !inicializado) {
            return;
        }

        if (estado) {
            eventSystem.SetSelectedGameObject(primeiroSelecionadoPause);

            telaPrincipalPause?.SetActive(true);

            if (telasInternasPause.Length > 0) {
                foreach (GameObject tela in telasInternasPause) {
                    tela.SetActive(false);
                }
            }


            // Tela de Opções possui campos que só aparecem em determinados modos de jogo
            if (GameManager.instance != null) {
                bool multLocal = GameManager.instance.modoDeJogo == ModoDeJogo.MULTIPLAYER_LOCAL;
                bool single = GameManager.instance.modoDeJogo == ModoDeJogo.SINGLEPLAYER;

                Navigation navigation = campoEmCimaDoPainelDeControles.navigation;

                if (multLocal || single) {
                    controlesLocaisPanel.SetActive(true);
                    singleplayerButton.gameObject.SetActive(multLocal);
                    ajustarMultiplayerButton.gameObject.SetActive(multLocal);
                    multiplayerButton.gameObject.SetActive(single);
                    navigation.selectOnDown = multLocal ? singleplayerButton : multiplayerButton;
                } else {
                    navigation.selectOnDown = null;
                    controlesLocaisPanel.SetActive(false);
                }

                campoEmCimaDoPainelDeControles.navigation = navigation;
            }
        }

        gameObject.SetActive(estado);
    }

    public void DespauseNoResume(){ 
        if(GameManager.instance != null){
            GameManager.instance.Despausar();
        }
    }

    public void VoltarParaMenu() {
        if(GameManager.instance != null){
            GameManager.instance.VoltarParaMenu();
        }
    }


    public void SetModoSingleplayer() {
        if (GameManager.instance == null) return;

        GameManager.instance.Despausar();
        GameManager.instance.SetModoSingleplayer();
    }

    public void SetModoMultiplayerLocal() {
        if (GameManager.instance == null) return;

        GameManager.instance.Despausar();
        eventSystem.SetSelectedGameObject(null);
        GameManager.instance.SetModoMultiplayerLocal();
    }

    public void RedefinirControlesMultiplayerLocal() {
        if (GameManager.instance == null) return;

        GameManager.instance.Despausar();
        eventSystem.SetSelectedGameObject(null);
        GameManager.instance.RedefinirControlesMultiplayerLocal();
    }
}
