using UnityEngine;
using UnityEngine.EventSystems;

public class PauseUI : MonoBehaviour {
    public GameObject[] telasInternasPause;
    public GameObject telaPrincipalPause;

    [Header("Event System")]
    public EventSystem eventSystem;
    public GameObject primeiroSelecionadoPause;

    bool inicializado = false;


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
}
