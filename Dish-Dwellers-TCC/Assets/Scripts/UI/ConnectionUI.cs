using EpicTransport;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public enum TipoDeTransport { IP, Epic }

public class ConnectionUI : MonoBehaviour {
    public static ConnectionUI instance;

    public string menuInicialScene = "MenuInicial";
    public DishNetworkManager networkManager;

    public Transform canvasDaConexao;
    public EventSystem eventSystem;
    public GameObject eossdkPrefab;


    [Header("Configuração de Conexão")]
    public TipoDeTransport tipoDeTransport = TipoDeTransport.IP;
    public GameObject ipPrefab, epicPrefab;
    public GameObject ipPanel, epicPanel;
    public Button ipButton, epicButton;
    public Button cancelarConexaoButton;
    public InputField ipField, epicField;

    [HideInInspector] public LobbyPlayer p1, p2;


    [Header("UI")]
    public GameObject telaEntrar;
    public GameObject telaCriar;
    public GameObject telaCarregamento;
    public TMP_Text telaCarregamentoTexto;
    public Button voltarButton;
    
    ConectorDeTransport conectorDeTransport;

    void Start() {
        instance = this;

        GameObject prefab = null;
        switch (tipoDeTransport) {
            case TipoDeTransport.IP:
                ipPanel.SetActive(true);
                epicPanel.SetActive(false);
                prefab = ipPrefab;
                conectorDeTransport = GetComponent<ConectorOnlineIP>();
                SelecionarBotao(ipButton);
                break;

            case TipoDeTransport.Epic:
                ipPanel.SetActive(false);
                epicPanel.SetActive(true);
                prefab = epicPrefab;
                conectorDeTransport = GetComponent<ConectarComEpic>();
                SelecionarBotao(epicButton);
                BetterEOSLobby.InstantiateSDK(eossdkPrefab);
                break;
        }

        GameObject managerInstancia = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        networkManager = managerInstancia.GetComponent<DishNetworkManager>();

        conectorDeTransport.Setup();
    }


    public void FecharEntrarLobby() {
        telaEntrar.SetActive(false);
    }


    public void AbrirCriarLobby() {
        telaCriar.SetActive(true);
    }

    public void FecharCriarLobby() {
        telaCriar.SetActive(false);
    }


    public void SelecionarBotao(Button botao) {
        eventSystem.SetSelectedGameObject(botao.gameObject);
        SetarNavigation();
    }

    public void SetarNavigation () {
        return;

        Navigation navigation = voltarButton.navigation;
        navigation.mode = Navigation.Mode.Explicit;

        bool taCarregando = telaCarregamento.activeSelf;
        if (taCarregando) {
            navigation.selectOnRight = cancelarConexaoButton;
            navigation.selectOnDown = cancelarConexaoButton;
            voltarButton.navigation = navigation;
            return;
        }

        if (tipoDeTransport == TipoDeTransport.IP && ipPanel.activeSelf) {
            navigation.selectOnRight = ipField;
            navigation.selectOnDown = ipField;
        } else if (tipoDeTransport == TipoDeTransport.IP && epicPanel.activeSelf) {
            navigation.selectOnRight = epicField;
            navigation.selectOnDown = epicField;
        }

        voltarButton.navigation = navigation;
    }


    #region Lobby 

    void OnDestroy() {
        if (canvasDaConexao != null && canvasDaConexao.gameObject != null)
            Destroy(canvasDaConexao.gameObject);
    }

    #endregion


    #region Entrar em Lobby
    
    // Chamado quando o cliente tenta criar um lobby
    public void ComecarHostear() {
        telaCriar.SetActive(false);

        MostrarCarregamento("Criando lobby...", SairDoLobby);
        conectorDeTransport.Hostear(status => EsconderCarregamento());
    }

    // Mostra modal para entrar em um lobby já criado
    public void PrepararPraEntrarLobby() {
        telaEntrar.SetActive(true);
        
        conectorDeTransport.Setup();
    }

    // Chamado quando um cliente tenta entrar em um lobby já criado
    public void EntrarNoLobby() {
        telaEntrar.SetActive(false);

        MostrarCarregamento("Tentando conectar...", CancelarEntrada);
        conectorDeTransport.ConectarCliente();
    }

    // Chamado quando um cliente entra no lobby com sucesso (pelo LobbyPlayer)
    public void EntrouNoLobby() {
        EsconderCarregamento();

        EscolherEntrarLobbyUI.instance.Entrar();
    }

    public void CancelarEntrada() {
        EsconderCarregamento();
        telaEntrar.SetActive(true);
        conectorDeTransport.EncerrarCliente();
    }

    System.Action OnCancelarCarregamento = null;
    public void HandleCancelarCarregamento() {
        if (OnCancelarCarregamento != null) OnCancelarCarregamento.Invoke();
        OnCancelarCarregamento = null;

        switch (tipoDeTransport) {
            case TipoDeTransport.IP:
                SelecionarBotao(ipButton);
                break;
            case TipoDeTransport.Epic:
                SelecionarBotao(epicButton);
                break;
        }
    }

    public void MostrarCarregamento(string texto, System.Action onCancelar = null) {
        telaCarregamento.SetActive(true);
        SelecionarBotao(cancelarConexaoButton);
        telaCarregamentoTexto.text = texto;
        OnCancelarCarregamento = onCancelar;
    }

    public void EsconderCarregamento() {
        telaCarregamento.SetActive(false);
    }

    #endregion


    public void SairDoLobby() {
        conectorDeTransport.EncerrarHost();

        Destroy(networkManager.gameObject);
        if (GameManager.instance != null)  Destroy(GameManager.instance.gameObject);
        SceneManager.LoadScene(menuInicialScene, LoadSceneMode.Single);
    }
}
