using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ConectorOnlineIP : ConectorDeTransport {
    NetworkManager networkManager;
    TelepathyTransport telepathyTransport;


    public InputField ipInputField, portInputField;
    bool setedUp = false;

    public override void Setup() {
        if (setedUp) return;
        setedUp = true;

        networkManager = NetworkManager.singleton;
        telepathyTransport = (TelepathyTransport)networkManager.transport;

        ipInputField.text = networkManager.networkAddress;
        portInputField.text = telepathyTransport.port.ToString();
    }

    System.Action<bool> hostAction = null;

    public override void Hostear(System.Action<bool> callback = null) {
        networkManager.StartHost();
        hostAction = callback;

        NetworkClient.OnConnectedEvent += HandleHostConnected;
        NetworkClient.OnDisconnectedEvent += HandleHostNotConnected;
    }

    void HandleHostConnected() {
        NetworkClient.OnConnectedEvent -= HandleHostConnected;
        NetworkClient.OnDisconnectedEvent -= HandleHostNotConnected;
        hostAction?.Invoke(true);
        hostAction = null;
    }

    void HandleHostNotConnected() {
        NetworkClient.OnConnectedEvent -= HandleHostConnected;
        NetworkClient.OnDisconnectedEvent -= HandleHostNotConnected;
        hostAction?.Invoke(false);
        hostAction = null;
    }

    System.Action<bool> clientAction = null;
    public override void ConectarCliente(System.Action<bool> callback = null) {
        networkManager.networkAddress = ipInputField.text;
        telepathyTransport.port = ushort.Parse(portInputField.text);
        networkManager.StartClient();

        clientAction = callback;

        NetworkClient.OnConnectedEvent += HandleClientConnected;
        NetworkClient.OnDisconnectedEvent += HandleClientNotConnected;
    }

    void HandleClientConnected() {
        NetworkClient.OnConnectedEvent -= HandleClientConnected;
        NetworkClient.OnDisconnectedEvent -= HandleClientNotConnected;
        clientAction?.Invoke(true);
        clientAction = null;
    }

    void HandleClientNotConnected() {
        NetworkClient.OnConnectedEvent -= HandleClientConnected;
        NetworkClient.OnDisconnectedEvent -= HandleClientNotConnected;
        clientAction?.Invoke(false);
        clientAction = null;
    }

    public override void EncerrarHost() {
        networkManager.StopHost();
        networkManager.StopClient();
        networkManager.StopServer();
    }

    public override void EncerrarCliente() {
        networkManager.StopClient();
    }
}