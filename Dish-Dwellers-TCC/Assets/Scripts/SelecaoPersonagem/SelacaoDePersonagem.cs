using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using Mirror;

[RequireComponent(typeof(Sincronizavel))]
public class SelacaoDePersonagem : MonoBehaviour, SincronizaMetodo {
    Sincronizavel _sincronizavel;
    [HideInInspector] public Sincronizavel sincronizavel {
        get {
            if (_sincronizavel == null) _sincronizavel = GetComponent<Sincronizavel>();
            return _sincronizavel;
        }
    }


    public GameObject painelPrincipal;

    public GameObject prefabIndicadorSeletor;
    public Text descricaoSuperior;

    public SeletorPersonagem seletorAngler, seletorHeater;

    bool selecaoLocal = true;
    public Text textoCancelar;
    public Transform seletorCancelar;

    int deviceIdCounter = 1;

    public List<Color> cores = new List<Color>();
    List<Color> coresDisponiveis = new List<Color>();

    List<IndicadorSeletor> indicadores = new List<IndicadorSeletor>();

    public System.Action<IndicadorSeletor, IndicadorSeletor> OnAmbosPronto;
    public System.Action OnCancelar;
    public bool estaoProntos {get { return seletorAngler.estaPronto && seletorHeater.estaPronto; }}

    #region Base

    void Awake() {
        actions = new Actions();
    }

    public IndicadorSeletor AdicionarSeletor(QualPersonagem personagem = QualPersonagem.Angler) {
        GameObject obj = Instantiate(prefabIndicadorSeletor);
        IndicadorSeletor indicador = obj.GetComponent<IndicadorSeletor>();

        indicador.onSelecionou += () => Selecionar(indicador);
        indicador.onConfirmou += () => Confirmar(indicador);
        indicador.onDesconfirmou += () => Desconfirmar(indicador);
        indicador.onPronto += () => Pronto(indicador);
        indicador.onSumiu += () => Sumir(indicador);
        indicador.onEsquerda += () => ParaEsquerda(indicador);
        indicador.onDireita += () => ParaDireita(indicador);
        indicador.onCima += () => ParaCima(indicador);
        indicador.onBaixo += () => ParaBaixo(indicador);


        sincronizavel.AddSub(indicador.subSincronizavel);


        if (coresDisponiveis.Count == 0) coresDisponiveis = new List<Color>(cores); // Repete cores se todas forem utilizadas

        Color cor = coresDisponiveis.First();
        coresDisponiveis.RemoveAt(0);

        indicador.SetCor(cor);
        indicador.SetTexto("P"+ deviceIdCounter);
        deviceIdCounter++;

        indicadores.Add(indicador);

        Sincronizador.instance.TravarSincronizacao(() => {
            indicador.Selecionar(personagem);
        });
        
        return indicador;
    }

    SeletorPersonagem GetSeletor(QualPersonagem personagem) {
        return personagem == QualPersonagem.Angler ? seletorAngler : seletorHeater;
    }

    [Sincronizar]
    public void Selecionar(IndicadorSeletor indicador) {
        gameObject.Sincronizar(indicador);

        indicador.HandleSelecionar();

        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        seletor.Selecionar(indicador);
    }

    [Sincronizar]
    public void Desconfirmar(IndicadorSeletor indicador) {
        gameObject.Sincronizar(indicador);
        
        indicador.HandleDesconfirmar();

        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        if (seletor.indicadorAtual == indicador) seletor.Confirmar(null);
    }

    [Sincronizar]
    public void Confirmar(IndicadorSeletor indicador) {
        gameObject.Sincronizar(indicador);

        if (indicador.selecionandoNada) {
            OnCancelar?.Invoke();
            return;
        }

        indicador.HandleConfirmar();

        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        seletor.Confirmar(indicador);
    }

    [Sincronizar]
    public void Pronto(IndicadorSeletor indicador) {
        gameObject.Sincronizar(indicador);

        indicador.HandleDarPronto();
        
        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        if (seletor.indicadorAtual != indicador) indicador.Confirmar();
        else seletor.Pronto(indicador);

        if (estaoProntos) OnAmbosPronto?.Invoke(seletorAngler.indicadorAtual, seletorHeater.indicadorAtual);
    }

    [Sincronizar]
    public void Sumir(IndicadorSeletor indicador) {
        gameObject.Sincronizar(indicador);

        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        if (seletor.indicadorAtual == indicador) seletor.Confirmar(null);

        coresDisponiveis.Insert(0, indicador.cor);
        indicadores.Remove(indicador);

        RefazerNumeros();
    }


    [Sincronizar]
    public void ParaEsquerda(IndicadorSeletor indicador) {
        gameObject.Sincronizar(indicador);

        indicador.Selecionar(QualPersonagem.Angler);
    }

    [Sincronizar]
    public void ParaDireita(IndicadorSeletor indicador) {
        gameObject.Sincronizar(indicador);

        indicador.Selecionar(QualPersonagem.Heater);
    }

    [Sincronizar]
    public void ParaBaixo(IndicadorSeletor indicador) {
        gameObject.Sincronizar(indicador);

        if (selecaoLocal && !indicador.estaConfirmado) {
            indicador.SelecionandoNada();
            indicador.transform.SetParent(seletorCancelar);
            return;
        }

        if (indicador.estaConfirmado) indicador.Desconfirmar();
    }

    [Sincronizar]
    public void ParaCima(IndicadorSeletor indicador) {
        gameObject.Sincronizar(indicador);

        if (selecaoLocal && indicador.selecionandoNada) {
            indicador.Selecionar(QualPersonagem.Angler);
            return;
        }

        if (!indicador.estaConfirmado) indicador.Confirmar();
    }


    void RefazerNumeros() {
        deviceIdCounter = 1;
        foreach (IndicadorSeletor indicador in indicadores) {
            indicador.SetTexto("P" + deviceIdCounter);
            deviceIdCounter++;
        }

        seletorAngler.AtualizarVisual();
        seletorHeater.AtualizarVisual();
    }

    #endregion

    #region Selecao Local

    public List<InputDevice> devices = new List<InputDevice>();
    Dictionary<InputDevice, IndicadorSeletor> seletores = new Dictionary<InputDevice, IndicadorSeletor>();
    Actions actions;

    public void ComecarSelecaoLocal() {
        painelPrincipal.SetActive(true);

        textoCancelar.gameObject.SetActive(true);
        foreach (Transform child in seletorCancelar) {
            Destroy(child.gameObject);
        }
        seletorCancelar.gameObject.SetActive(true);

        seletorAngler.Reset();
        seletorHeater.Reset();

        deviceIdCounter = 1;

        coresDisponiveis = new List<Color>(cores);

        seletores.Clear();
        
        actions.Enable();
        actions.Player.Get().actionTriggered += OuveAcoesParaSetarSeletores;
        InputSystem.onDeviceChange += RemoveDevicesDesconectadosDaSelecao;
        OnAmbosPronto += HandleTerminouSelecaoLocal;
        OnCancelar += OnCancelarLocal;

        selecaoLocal = true;
        Time.timeScale = 0;
    }

    protected void OuveAcoesParaSetarSeletores(InputAction.CallbackContext ctx) {
        InputDevice device = ctx.control.device;
        if (device == null || !ctx.performed) return;

        if (!seletores.ContainsKey(device)) {
            seletores[device] = GameManager.instance.selecaoDePersonagem.AdicionarSeletor();
        } else {
            IndicadorSeletor seletor = seletores[device];
            
            if(ctx.action.name == "Move" || ctx.action.name == "Aim") {
                Vector2 input = ctx.ReadValue<Vector2>();
                if (Mathf.Abs(input.x) > 0.5f && !seletor.estaPronto) {
                    if (input.x > 0) seletor.SelecionarDireita();
                    else seletor.SelecionarEsquerda();
                } else if (Mathf.Abs(input.y) > 0.5f  && !seletor.estaPronto) {
                    if (input.y < 0) seletor.SelecionarBaixo();
                    else seletor.SelecionarCima();
                }

            } else if (ctx.action.name == "Interact" || ctx.action.name == "Attack") {
                if (seletor.estaConfirmado && !seletor.estaPronto) seletor.DarPronto();
                else seletor.Confirmar();
            }
        }
    }

    protected void RemoveDevicesDesconectadosDaSelecao(InputDevice device, InputDeviceChange change) {
        if (change != InputDeviceChange.Removed && change != InputDeviceChange.Disconnected) return;

        if (seletores.ContainsKey(device)) {
            seletores[device].Sumir();
            seletores.Remove(device);
        }
    }

    protected void HandleTerminouSelecaoLocal(IndicadorSeletor angler, IndicadorSeletor heater) {
        OnAmbosPronto -= HandleTerminouSelecaoLocal;
        OnCancelar -= OnCancelarLocal;
        InputSystem.onDeviceChange -= RemoveDevicesDesconectadosDaSelecao;
        actions.Player.Get().actionTriggered -= OuveAcoesParaSetarSeletores;
        actions.Disable();
        
        InputDevice deviceAngler = null, deviceHeater = null;
        foreach (InputDevice device in seletores.Keys) {
            IndicadorSeletor indicador = seletores[device];
            if (indicador == angler) deviceAngler = device;
            else if (indicador == heater) deviceHeater = device;
        }

        QualPlayer playerAngler = GameManager.instance.GetQualPlayer(QualPersonagem.Angler);
        QualPlayer playerHeater = GameManager.instance.GetQualPlayer(QualPersonagem.Heater);

        GameManager.instance.inputController.SetPlayerDevice(playerAngler, deviceAngler);
        GameManager.instance.inputController.SetPlayerDevice(playerHeater, deviceHeater);

        seletores.Clear();
        painelPrincipal.SetActive(false);
        Time.timeScale = 1.0f;
    }

    protected void OnCancelarLocal() {
        OnAmbosPronto -= HandleTerminouSelecaoLocal;
        OnCancelar -= OnCancelarLocal;
        InputSystem.onDeviceChange -= RemoveDevicesDesconectadosDaSelecao;
        actions.Player.Get().actionTriggered -= OuveAcoesParaSetarSeletores;
        actions.Disable();

        GameManager.instance.SetModoSingleplayer();

        seletores.Clear();
        painelPrincipal.SetActive(false);
        Time.timeScale = 1.0f;
    }


    #endregion

    #region Selecao Multiplayer


    Dictionary<bool, IndicadorSeletor> seletorMultiplayer = new Dictionary<bool, IndicadorSeletor>();

    [Sincronizar]
    public void ComecarSelecaoOnline() {
        gameObject.Sincronizar();
        painelPrincipal.SetActive(true);

        textoCancelar.gameObject.SetActive(false);
        seletorCancelar.gameObject.SetActive(false);

        seletorAngler.Reset();
        seletorHeater.Reset();

        deviceIdCounter = 1;

        coresDisponiveis = new List<Color>(cores);

        seletorMultiplayer.Clear();

        foreach (Player p in GameManager.instance.jogadores) {
            bool ehMeu = p.isLocalPlayer;
            IndicadorSeletor seletor = AdicionarSeletor(p.personagem);
            seletorMultiplayer.Add(ehMeu, seletor);
        }

        actions.Enable();
        actions.Player.Get().actionTriggered += OuveAcoesParaSetarSeletoresOnline;
        OnAmbosPronto += HandleTerminouSelecaoOnline;
        selecaoLocal = false;
        Time.timeScale = 0;
    }


    protected void OuveAcoesParaSetarSeletoresOnline(InputAction.CallbackContext ctx) {
        IndicadorSeletor seletor = seletorMultiplayer[true];
            
        if(ctx.action.name == "Move" || ctx.action.name == "Aim") {
            Vector2 input = ctx.ReadValue<Vector2>();
            if (Mathf.Abs(input.x) > 0.5f && !seletor.estaPronto) {
                if (input.x > 0) seletor.SelecionarDireita();
                else seletor.SelecionarEsquerda();
            } else if (Mathf.Abs(input.y) > 0.5f  && !seletor.estaPronto) {
                if (input.y < 0) seletor.SelecionarBaixo();
                else seletor.SelecionarCima();
            }

        } else if (ctx.action.name == "Interact" || ctx.action.name == "Attack") {
            if (seletor.estaConfirmado && !seletor.estaPronto) seletor.DarPronto();
            else seletor.Confirmar();
        }
    }

    protected void HandleTerminouSelecaoOnline(IndicadorSeletor angler, IndicadorSeletor heater) {
        OnAmbosPronto -= HandleTerminouSelecaoOnline;
        actions.Player.Get().actionTriggered -= OuveAcoesParaSetarSeletoresOnline;
        actions.Disable();
    

        bool conexaoAngler = false, conexaoHeater = false;

        foreach (bool meu in seletorMultiplayer.Keys) {
            IndicadorSeletor indicador = seletorMultiplayer[meu];
            if (indicador == angler) conexaoAngler = meu;
            else if (indicador == heater) conexaoHeater = meu;
        }

        Player anglerPlayer = GameManager.instance.GetPlayer(QualPersonagem.Angler);
        Player heaterPlayer = GameManager.instance.GetPlayer(QualPersonagem.Heater);

        if (anglerPlayer.isLocalPlayer != conexaoAngler) {
            Sincronizador.instance.TrocarPersonagens();
        }


        seletorMultiplayer.Clear();
        painelPrincipal.SetActive(false);
        Time.timeScale = 1.0f;
    }

    #endregion

}
