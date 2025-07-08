using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject[] coracoesEsquerda;
    [SerializeField] GameObject[] coracoesDireita;

    private Image img;

    public PauseUI pauseUI;


    void Awake() {
        Player.OnVidaMudada += HandleDisplayVida;
        
        //TiraMouse();
    }

    void Start() {
        if (pauseUI != null) {
            pauseUI.Inicializar();
        }
    }

    private void OnDestroy(){
        Player.OnVidaMudada -= HandleDisplayVida;
    }

    /// <summary>
    /// Toggle qualquer GameObject, sempre da SetActive no contrario da hierarquia
    /// </summary>

    public void AtivarEDesativarObjeto(GameObject objeto){
        objeto.SetActive(!objeto.activeInHierarchy);
    }

    public void HandleDisplayVida(Player player, int valor){
        GameObject[] coracoes = player.personagem == QualPersonagem.Angler ? coracoesEsquerda : coracoesDireita;

        for (int i = 0; i < coracoes.Length; i++){ 
            //percorre pela array de coracoes e ativa caso ele for menor que as vidas
            //como temos 3 de vida e a array tem 0,1,2 ele trata por i e nao pelo numero de vida
            img = coracoes[i].GetComponent<Image>();
            
            img.color = i < player.playerVidas ? new Color(1f, 0.75f, 0.75f, 1f) : new Color(0.3f, 0.1f, 0.1f, 1f);
            //coracoes[i].SetActive(i < player.playerVidas);
        }
    }

    public void TiraMouse(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void VoltarParaMenu() {
        if(GameManager.instance != null){
            GameManager.instance.VoltarParaMenu();
        }
    }
}
