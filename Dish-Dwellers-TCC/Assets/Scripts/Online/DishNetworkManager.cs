using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class DishNetworkManager : NetworkManager {
    [Header("Dish Network Manager")]

    public GameObject heaterPrefab;
    public GameObject anglerPrefab;

    public LobbyPlayer[] lobbyPlayers; // Players do lobby (para escolher personagem)
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

    // Chamado quando um player se conecta ao servidor
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        if (lobbyPlayers.Length == 0) {
            lobbyPlayers = new LobbyPlayer[2];
        } else if (lobbyPlayers[0] != null && lobbyPlayers[1] != null) {
            return;
        }

        // Cria um novo jogador no lobby
        GameObject player = Instantiate(playerPrefab);

        LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();
        lobbyPlayer.isPlayerOne = lobbyPlayers[0] == null;

        GameObject startPos = GetStartPosition(lobbyPlayer.isPlayerOne);
        if (startPos != null) {
            player.transform.position = startPos.transform.position;
            player.transform.rotation = startPos.transform.rotation;
        }

        if (lobbyPlayer.isPlayerOne) lobbyPlayers[0] = lobbyPlayer;
        else lobbyPlayers[1] = lobbyPlayer;
        player.name = $"[connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    GameObject GetStartPosition(bool isPlayerOne) {
        GameObject startPos = GameObject.Find("SpawnPoint " + (isPlayerOne ? "1" : "2"));
        return startPos;
    }

    Personagem GetPersonagemNaoUsado(LobbyPlayer lobbyPlayer) {
        if (players.Length < 2 || (players[0] == null && players[1] == null)) return Personagem.Angler;
        int i = lobbyPlayers[0] == lobbyPlayer ? 0 : 1;
        if (players[i] != null) return players[i].personagem == QualPersonagem.Heater ? Personagem.Heater : Personagem.Angler;
        int j = i == 0 ? 1 : 0;
        return players[j].personagem == QualPersonagem.Heater ? Personagem.Angler : Personagem.Heater;
    }

    [Server]
    public void GerarPlayer(LobbyPlayer lplayer) {
        int i = lobbyPlayers[0] == lplayer ? 0 : 1;

        if (players.Length < 2)
            players = new Player[2] { null, null };

        Player p = players[i];
        if (p != null) return;


        Personagem per = GetPersonagemNaoUsado(lplayer);
        // Pega o prefab do personagem correto
        GameObject playerPrefab = (per == Personagem.Heater) ? heaterPrefab : anglerPrefab;


        GameObject spawnpoint = GetStartPosition(lplayer.isPlayerOne);
        Vector3 spawnPos = spawnpoint != null ? spawnpoint.transform.position : lplayer.transform.position;
        Quaternion spawnRot = spawnpoint != null ? spawnpoint.transform.rotation : lplayer.transform.rotation;

        GameObject player = Instantiate(playerPrefab, spawnPos, spawnRot);

        // Substitui o player atual (LobbyPlayer) do cliente pelo novo player (Player)
        NetworkServer.ReplacePlayerForConnection(lplayer.connectionToClient, player, ReplacePlayerOptions.Destroy);

        players[i] = player.GetComponent<Player>();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        if (conn != null) {
            if (lobbyPlayers[0] != null && lobbyPlayers[0].connectionToClient == conn) {
                lobbyPlayers[0].FoiDesconectado();
                lobbyPlayers[0] = null;
            }
            else if (lobbyPlayers[1] != null && lobbyPlayers[1].connectionToClient == conn) {
                lobbyPlayers[1].FoiDesconectado();
                lobbyPlayers[1] = null;
            }
        }

        if (players != null && players.Length > 0) {
            if (players[0] != null) players[0].conectado = false;
            if (players.Length > 1 && players[1] != null) players[1].conectado = false;
        }

        SairDoLobby();

        base.OnServerDisconnect(conn);
    }

    public void SairDoLobby() {
        if (betterEOSLobby != null)
            betterEOSLobby.SairDoLobby();
    }

    #region No Lobby

    [Server]
    LobbyPlayer GetLobbyPlayer(NetworkConnectionToClient conn) {
        if (lobbyPlayers == null) return null;

        foreach (LobbyPlayer lobbyPlayer in lobbyPlayers) {
            if (lobbyPlayer == null) continue;
            if (lobbyPlayer.connectionToClient == conn) return lobbyPlayer;
        }

        return null;
    }


    // Tenta iniciar o jogo (se os dois jogadores estiverem prontos e com nomes)
    public void IniciarJogo() {
        if (lobbyPlayers[0] == null || lobbyPlayers[1] == null) return;

        players = new Player[lobbyPlayers.Length];

        for (int i = 0; i < lobbyPlayers.Length; i++) {
            // Para cada lobbyPlayer, cria uma instancia de jogador na cena (Player)
            LobbyPlayer lobbyPlayer = lobbyPlayers[i];
            if (lobbyPlayer == null) continue;

            GameObject player = Instantiate(playerPrefab, lobbyPlayer.transform.position, lobbyPlayer.transform.rotation);

            // Substitui o player atual (LobbyPlayer) do cliente pelo novo player (Player)
            NetworkServer.ReplacePlayerForConnection(lobbyPlayer.connectionToClient, player, ReplacePlayerOptions.Destroy);

            players[i] = player.GetComponent<Player>();
        }

        // Informa a todos os clientes que o jogo começou
        foreach (Player player in players) {
            player.conectado = true;
        }
    }

    #endregion No Lobby


    #region In Game

    public struct RequestPassaDeSalaMessage : NetworkMessage {
        public bool passarDeSala;
        public string salaAtual;

        public RequestPassaDeSalaMessage(bool passarDeSala = true, string salaAtual = "") {
            this.passarDeSala = passarDeSala;
            this.salaAtual = salaAtual;
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

        NetworkServer.SendToAll(new AcaoPassaDeSalaMessage(msg.passarDeSala));
    }

    #endregion In Game
}
