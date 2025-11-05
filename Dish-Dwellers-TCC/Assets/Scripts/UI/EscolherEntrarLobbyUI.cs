using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class PartidaInfo {
    public static PartidaInfo instance;
    public enum Modo { Singleplayer, Local, Host, Entrar };
    public Modo modo = Modo.Host;

    public ModoDeJogo modoDeJogo { 
        get {
            if (modo == Modo.Singleplayer) return ModoDeJogo.SINGLEPLAYER;
            if (modo == Modo.Local) return ModoDeJogo.MULTIPLAYER_LOCAL;
            if (modo == Modo.Host) return ModoDeJogo.MULTIPLAYER_ONLINE;
            if (modo == Modo.Entrar) return ModoDeJogo.MULTIPLAYER_ONLINE;
            return ModoDeJogo.INDEFINIDO;
        }
    }

}

public class EscolherEntrarLobbyUI : MonoBehaviour {
    public static EscolherEntrarLobbyUI instance;

    public string cenaLobby = "Lobby";
    public string cenaPrimeiraFase = "1-1";
    public GameObject menuOpcoes, lobbyOpcoes, seletorFase;
    public UINavigationManager selectionManager;
    public GameObject primeiroSelectFase, primeiroDeselectFase;
    


    void Awake() {
        instance = this;

        
    }

    public void JogarOffline() {
        if (GameManager.instance != null)
            GameManager.instance.isOnline = false;
        
        PartidaInfo.instance = new PartidaInfo();
        PartidaInfo.instance.modo = PartidaInfo.Modo.Singleplayer;

        SelecaoDeFase();

        //SceneManager.LoadScene(cenaPrimeiraFase, LoadSceneMode.Single);
    }

    public void JogarLocal() {
        if (GameManager.instance != null)
            GameManager.instance.isOnline = false;
        
        PartidaInfo.instance = new PartidaInfo();
        PartidaInfo.instance.modo = PartidaInfo.Modo.Local;

        SelecaoDeFase();

        //SceneManager.LoadScene(cenaPrimeiraFase, LoadSceneMode.Single);
    }

    public void Hostear(string cena = "") {
        PartidaInfo.instance = new PartidaInfo();
        PartidaInfo.instance.modo = PartidaInfo.Modo.Host;
        DishNetworkManager manager = NetworkManager.singleton as DishNetworkManager;
        manager.ServerChangeScene(cena == ""? cenaLobby : cena);
    }

    public void Entrar(string cena = "", bool entrarDeFato = true) {
        PartidaInfo.instance = new PartidaInfo();
        PartidaInfo.instance.modo = PartidaInfo.Modo.Entrar;
        if (entrarDeFato)
            SceneManager.LoadScene(cena == ""? cenaLobby : cena, LoadSceneMode.Single);
    }

    public void MostrarOpcoes() {
        menuOpcoes.SetActive(false);
        lobbyOpcoes.SetActive(true);
    }

    public void Voltar() {
        menuOpcoes.SetActive(true);
        lobbyOpcoes.SetActive(false);
    }

    public void SelecaoDeFase(){
        seletorFase.SetActive(true);
        selectionManager.TrocaFirstSelected(primeiroSelectFase);
    }

    public void DesselecaoDeFase(){
        seletorFase.SetActive(false);
        selectionManager.TrocaFirstSelected(primeiroDeselectFase);
    }
}
