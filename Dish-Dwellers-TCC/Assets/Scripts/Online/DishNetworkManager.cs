using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.SceneManagement;

public class DishNetworkManager : NetworkManager {
    [Header("Dish Network Manager")]

    public GameObject heaterPrefab;
    public GameObject anglerPrefab;

    public Player[] players = new Player[2]; // Players instanciados na cena (são criados a partir de um LobbyPlayer)

    public enum Personagem { Indefinido, Heater, Angler }

    BetterEOSLobby betterEOSLobby;

    public override void Awake() {
        base.Awake();

        NetworkServer.RegisterHandler<RequestPassaDeSalaMessage>(OnRequestedPassaDeSala);
    }

    public override void Start() {
        base.Start();

        betterEOSLobby = FindAnyObjectByType<BetterEOSLobby>();
        if (betterEOSLobby != null)
            betterEOSLobby.transform.SetParent(transform);
    }


    public override void OnClientConnect() { }

    // Chamado quando um player se conecta ao servidor
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        if (players.Length < 2) {
            players = new Player[2];
            players[0] = null;
            players[1] = null;
        }

        bool isPlayerOne = players[0] == null;


        Personagem per = GetPersonagemNaoUsado();
        // Pega o prefab do personagem correto
        GameObject playerPrefab = (per == Personagem.Heater) ? heaterPrefab : anglerPrefab;


        GameObject spawnpoint = GetStartPosition(isPlayerOne);
        Vector3 spawnPos = spawnpoint.transform.position;
        Quaternion spawnRot = spawnpoint.transform.rotation;

        GameObject player = Instantiate(playerPrefab, spawnPos, spawnRot);
        Player p = player.GetComponent<Player>();

        if (isPlayerOne) players[0] = p;
        else players[1] = p;

        player.name = $"[connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        p.conectado = true;
    }

    GameObject GetStartPosition(bool isPlayerOne) {
        GameObject startPos = GameObject.Find("SpawnPoint " + (isPlayerOne ? "1" : "2"));
        if (startPos == null) return gameObject;
        return startPos;
    }

    Personagem GetPersonagemNaoUsado() {
        if (players.Length < 2 || (players[0] == null && players[1] == null)) return Personagem.Angler;
        if (players[0] != null) return players[0].personagem == QualPersonagem.Heater ? Personagem.Angler : Personagem.Heater;
        return players[1].personagem == QualPersonagem.Heater ? Personagem.Angler : Personagem.Heater;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        if (networkSceneName.StartsWith("lobby")) {
            Player p = conn.identity.GetComponent<Player>();
            if (p != null) p.conectado = false;
        } else if (players != null && players.Length > 0) {
            // Quando um player disconecta fora do lobby, encerra conexão (DO TO: Permitir player desconectar e conectar de volta in game)
            if (players[0] != null) players[0].conectado = false;
            if (players.Length > 1 && players[1] != null) players[1].conectado = false;
        }

        SairDoLobby();

        base.OnServerDisconnect(conn);

        GameManager.instance?.SetarOnline();
    }

    public void SairDoLobby() {
        if (betterEOSLobby != null)
            betterEOSLobby.SairDoLobby();
    }


    #region In Game

    public struct RequestPassaDeSalaMessage : NetworkMessage {
        public bool passarDeSala;
        public string salaAtual;
        public string nomeDaCena;

        public RequestPassaDeSalaMessage(bool passarDeSala = true, string salaAtual = "", string nomeCena = "") {
            this.passarDeSala = passarDeSala;
            this.salaAtual = salaAtual;
            this.nomeDaCena = nomeCena;
        }
    }

    public struct AcaoPassaDeSalaMessage : NetworkMessage {
        public bool passarDeSala;

        public AcaoPassaDeSalaMessage(bool passarDeSala = true) {
            this.passarDeSala = passarDeSala;
        }
    }

    private string salaAtual = "";

    // Recebe a requisição de passar de sala e avisa todos os clientes para passar de sala
    private void OnRequestedPassaDeSala(NetworkConnectionToClient conn, RequestPassaDeSalaMessage msg) {
        if (players == null || players.Length == 0) return;
        if (players[0] == null || players[1] == null) return;
        if (msg.salaAtual == salaAtual) return; // Se chamou duas vezes para a mesma sala, não faz nada.
        salaAtual = msg.salaAtual;
        networkSceneName = msg.nomeDaCena;


        NetworkServer.SendToAll(new AcaoPassaDeSalaMessage(msg.passarDeSala));
    }

    public override void OnServerChangeScene(string newSceneName) {
        base.OnServerChangeScene(newSceneName);
        networkSceneName = newSceneName;
        Debug.Log("Updating " + networkSceneName);
    }

    public string GetCenaAtual() {
        return networkSceneName;
    }

    public BetterEOSLobby GetLobby() {
        return betterEOSLobby;
    }

    #endregion In Game
}
