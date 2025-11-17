using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Replica visualmente um indicador
public class IndicadorFalso : MonoBehaviour {
    public bool ativo => gameObject.activeSelf;

    public Image imagemIndicador;

    public Player jogador;

    public float offsetQuandoDois = 0.1f;

    public bool mostrando { get { return gameObject.activeSelf; }}

    public bool copiando { get { return jogador != null; }}

   
    void Start() {
        
    }

    public void Copiar(Indicador indicador) {
        imagemIndicador.sprite = indicador.GetSprite();
        jogador = indicador.jogador;
        jogador.OnDeviceChange += HandleDeviceChange;

        GameManager.instance.OnAtualizarModoCamera += HandleGameModeChange;
        HandleGameModeChange();
    }

    public void Esconder() {
        imagemIndicador.sprite = null;

        if (jogador != null) {
            jogador.OnDeviceChange -= HandleDeviceChange;
            jogador = null;
        }
        
        GameManager.instance.OnAtualizarModoCamera -= HandleGameModeChange;
        gameObject.SetActive(false);
    }


    void HandleGameModeChange() {
        ModoDeJogo modo = GameManager.instance.modoDeJogo;
        if (singleplayerSwitchSetted) GameManager.instance.OnTrocarControle -= HandleSingleplayerSwitch;

        if (GameManager.instance.modoDeJogo == ModoDeJogo.SINGLEPLAYER) {
            GameManager.instance.OnTrocarControle += HandleSingleplayerSwitch;
            singleplayerSwitchSetted = true;
            HandleSingleplayerSwitch(GameManager.instance.playerAtual);
        } else {
            gameObject.SetActive(jogador.ehJogadorAtual);
        }
    }

    bool singleplayerSwitchSetted = false;
    void HandleSingleplayerSwitch(QualPlayer qual) {
        gameObject.SetActive(jogador != null ? jogador.qualPlayer == qual : false);
    }

    void HandleDeviceChange(InputDevice d) {
        RefreshDisplay();
    }

    public void RefreshDisplay() {
        imagemIndicador.sprite = jogador.indicador.GetSprite();
    }

    void OnDestroy() {
        if (singleplayerSwitchSetted) GameManager.instance.OnTrocarControle -= HandleSingleplayerSwitch;
        if (jogador != null) jogador.OnDeviceChange -= HandleDeviceChange;
    }
}
