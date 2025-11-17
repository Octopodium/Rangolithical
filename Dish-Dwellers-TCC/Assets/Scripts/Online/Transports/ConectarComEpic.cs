using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Mirror;

public class ConectarComEpic : ConectorDeTransport {
    public InputField idInput;

    private BetterEOSLobby _beOSLobby;
    public BetterEOSLobby beOSLobby {
        get {
            if (_beOSLobby == null) {
                _beOSLobby = FindFirstObjectByType<BetterEOSLobby>();
            }
            return _beOSLobby;
        }
    }

    bool lobbyCriado = false;
    bool serverIniciado = false;
    bool deuErro = false;


    public override void Setup() {
        idInput.text = "";

        lobbyCriado = false;
        serverIniciado = false;
        deuErro = false;

        beOSLobby.OnLobbyCriado += idLobby => {
            lobbyCriado = true;
            TentarCallbackHostear();
        };
        beOSLobby.OnCriarLobbyFalhou += () => { deuErro = true;  TentarCallbackHostear(); };

        beOSLobby.OnEntrouLobby += () => { callbackConectarCliente?.Invoke(true); callbackConectarCliente = null; };
        beOSLobby.OnEntrarLobbyFalhou += () => { deuErro = true;  callbackConectarCliente?.Invoke(false); callbackConectarCliente = null; };
    }

    System.Action<bool> callbackHostear, callbackConectarCliente;
    public override void Hostear(System.Action<bool> callback = null) {
        callbackHostear = callback;

        NetworkClient.OnConnectedEvent += HandleHostConnected;
        NetworkClient.OnDisconnectedEvent += HandleHostNotConnected;
        (NetworkManager.singleton as DishNetworkManager).OnHostStarted += HandleHostConnected;
        beOSLobby.CriarHost();
    }

    void HandleHostConnected() {
        (NetworkManager.singleton as DishNetworkManager).OnHostStarted -= HandleHostConnected;
        NetworkClient.OnConnectedEvent -= HandleHostConnected;
        NetworkClient.OnDisconnectedEvent -= HandleHostNotConnected;
        serverIniciado = true;
        TentarCallbackHostear();
    }

    void HandleHostNotConnected() {
        (NetworkManager.singleton as DishNetworkManager).OnHostStarted -= HandleHostConnected;
        NetworkClient.OnConnectedEvent -= HandleHostConnected;
        NetworkClient.OnDisconnectedEvent -= HandleHostNotConnected;
        deuErro = true;
        TentarCallbackHostear();
    }

    void TentarCallbackHostear() {
        if (deuErro) {
            callbackHostear?.Invoke(false);
            callbackHostear = null;
            return;
        }

        if (lobbyCriado && serverIniciado) {
            callbackHostear?.Invoke(true);
            callbackHostear = null;
        }
    }

    public override void ConectarCliente(System.Action<bool> callback = null) {
        callbackConectarCliente = callback;

        string id = idInput.text.Trim().ToUpper();
        beOSLobby.ConectarCliente(id);
    }

    public override void EncerrarHost() {
        beOSLobby.DesconectarHost();
    }

    public override void EncerrarCliente() {
        beOSLobby.DesconectarCliente();
    }


}
