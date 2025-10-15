using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.DualShock;
using System.Collections.Generic;
public class InputController : MonoBehaviour {
    public PlayerInputManager playerInputManager; // Gerencia os inputs dos jogadores locais
    public Actions actions;

    public Color anglerColor, heaterColor;
    public PlayerInput player1Input, player2Input;
    public InputDevice player1Device, player2Device;

    public Action<InputAction.CallbackContext, QualPlayer> OnInputTriggered;

    void Awake() {
        InputUser.onChange += OnInputUserChanged;
    }

    void Start() {
        GameManager.instance.OnTrocarControle += AtualizarCorzinhaControle;
    }

    void OnDestroy() {
        InputUser.onChange -= OnInputUserChanged;
        actions?.Disable();
    }

    public void Inicializar() {
        actions = new Actions();
        actions.Enable();
    }

    public void Desabilitar() {
        actions.Disable();
    }

    #region Input e Devices

    public void RedefinirInputs() {
        if (player1Input.gameObject.activeSelf) player1Input.onActionTriggered -= HandleOnInputTriggeredP1;
        if (player2Input.gameObject.activeSelf) player2Input.onActionTriggered -= HandleOnInputTriggeredP2;

        player1Input.gameObject.SetActive(false);
        player2Input.gameObject.SetActive(false);

        ConfigurarInputs();
    }

    public void SetPlayerDevice(QualPlayer qualPlayer, InputDevice device) {
        PlayerInput playerInput = qualPlayer == QualPlayer.Player1 ? player1Input : player2Input;
        playerInput.user.UnpairDevices();
        InputUser.PerformPairingWithDevice(device, playerInput.user);

        // O Input System não reconhece automaticamente o esquema de controle, então é necessário definir manualmente... (??????)
        string controlScheme = GetControlSchemeName(device);
        playerInput.SwitchCurrentControlScheme(controlScheme, device);
        playerInput.ActivateInput();
    }

    public void ConfigurarInputs() {
        player1Input.gameObject.SetActive(true);
        player1Input.onActionTriggered += HandleOnInputTriggeredP1;

        if (GameManager.instance.modoDeJogo == ModoDeJogo.MULTIPLAYER_LOCAL) {
            player2Input.gameObject.SetActive(true);
            player2Input.onActionTriggered += HandleOnInputTriggeredP2;

            if (player1Input.user.valid) player1Input.user.UnpairDevices();
            if (player2Input.user.valid) player2Input.user.UnpairDevices();

            player1Input.DeactivateInput();
            player2Input.DeactivateInput();

            GameManager.instance.selecaoDePersonagem.ComecarSelecaoLocal();
        } else {
            player2Input.gameObject.SetActive(false);
            if (player2Input.user.valid) player2Input.user.UnpairDevices();
            player2Input.DeactivateInput();
            
            AtualizarCorzinhaControle(QualPlayer.Player1);
        }
    }
    
    QualPlayer GetQualPlayerFromInputUser(InputUser inputUser) {
        if (inputUser == player1Input.user) {
            return QualPlayer.Player1;
        } else if (inputUser == player2Input?.user) {
            return QualPlayer.Player2;
        }
        return QualPlayer.Player1; // Ou outro valor padrão
    }


    protected void OnInputUserChanged(InputUser inputUser, InputUserChange change, InputDevice device) {
        bool paired = change == InputUserChange.DevicePaired;
        bool unpaired = change == InputUserChange.DeviceUnpaired;

        if (!paired && !unpaired) return;

        QualPlayer player = GetQualPlayerFromInputUser(inputUser);
        AtualizarCorzinhaControle(player);
    }


    protected QualPlayer GetQualPlayerTratado(QualPlayer qualPlayer) {
        if (GameManager.instance.modoDeJogo == ModoDeJogo.SINGLEPLAYER) {
            return GameManager.instance.playerAtual;
        } else if (GameManager.instance.modoDeJogo == ModoDeJogo.MULTIPLAYER_ONLINE) {
            return QualPlayer.Player1;
        }
        return qualPlayer;

    }

    protected void HandleOnInputTriggeredP1(InputAction.CallbackContext ctx) {
        HandleOnInputTriggered(ctx, QualPlayer.Player1);
    }

    protected void HandleOnInputTriggeredP2(InputAction.CallbackContext ctx) {
        HandleOnInputTriggered(ctx, QualPlayer.Player2);
    }

    protected void HandleOnInputTriggered(InputAction.CallbackContext ctx, QualPlayer qualPlayer) {
        qualPlayer = GetQualPlayerTratado(qualPlayer);

        if (OnInputTriggered != null) {
            OnInputTriggered(ctx, qualPlayer);
        }
    }

    public PlayerInput GetPlayerInput(Player player) {
        return GetPlayerInput(player.qualPlayer);
    }

    public PlayerInput GetPlayerInput(QualPlayer qualPlayer) {
        if (GameManager.instance.modoDeJogo == ModoDeJogo.MULTIPLAYER_LOCAL) {
            return (qualPlayer == QualPlayer.Player1) ? player1Input : player2Input;
        } else {
            return (qualPlayer == GameManager.instance.playerAtual) ? player1Input : null;
        }
    }

    string GetControlSchemeName(InputDevice device) {
        switch (device) {
            case Gamepad:
                return "Gamepad";
            case Keyboard:
                return "Keyboard&Mouse";
            case Touchscreen:
                return "Touch";
            case Joystick:
                return "Joystick";
            default:
                return "Unknown";
        }
    }

    #endregion

    void AtualizarCorzinhaControle(QualPlayer playerAtual) {
        if (GameManager.instance == null) return;

        QualPersonagem personagem = GameManager.instance.GetQualPersonagem(playerAtual);
        PlayerInput playerInput = GetPlayerInput(playerAtual);

        if (playerInput == null) return;

        foreach (var device in playerInput.devices) {
            if (device is DualShockGamepad dualShockGamepad) {
                if (personagem == QualPersonagem.Angler) {
                    dualShockGamepad.SetLightBarColor(anglerColor);
                } else if (personagem == QualPersonagem.Heater) {
                    dualShockGamepad.SetLightBarColor(heaterColor);
                }
            }
        }
    }

}
