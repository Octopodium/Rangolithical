using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class SelacaoDePersonagem : MonoBehaviour {
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

        if (coresDisponiveis.Count == 0) coresDisponiveis = new List<Color>(cores); // Repete cores se todas forem utilizadas

        Color cor = coresDisponiveis.First();
        coresDisponiveis.RemoveAt(0);

        indicador.SetCor(cor);
        indicador.SetTexto("P"+ deviceIdCounter);
        deviceIdCounter++;

        indicadores.Add(indicador);
        
        indicador.Selecionar(personagem);

        return indicador;
    }

    SeletorPersonagem GetSeletor(QualPersonagem personagem) {
        return personagem == QualPersonagem.Angler ? seletorAngler : seletorHeater;
    }

    public void Selecionar(IndicadorSeletor indicador) {
        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        seletor.Selecionar(indicador);
    }

    public void Desconfirmar(IndicadorSeletor indicador) {
        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        if (seletor.indicadorAtual == indicador) seletor.Confirmar(null);
    }

    public void Confirmar(IndicadorSeletor indicador) {
        if (indicador.selecionandoNada) {
            OnCancelar?.Invoke();
            return;
        }

        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        seletor.Confirmar(indicador);
    }

    public void Pronto(IndicadorSeletor indicador) {
        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        if (seletor.indicadorAtual != indicador) indicador.Confirmar();
        else seletor.Pronto(indicador);

        if (estaoProntos) OnAmbosPronto?.Invoke(seletorAngler.indicadorAtual, seletorHeater.indicadorAtual);
    }

    public void Sumir(IndicadorSeletor indicador) {
        SeletorPersonagem seletor = GetSeletor(indicador.personagemSelecionado);
        if (seletor.indicadorAtual == indicador) seletor.Confirmar(null);

        coresDisponiveis.Insert(0, indicador.cor);
        indicadores.Remove(indicador);

        RefazerNumeros();
    }


    public void ParaEsquerda(IndicadorSeletor indicador) {
        indicador.Selecionar(QualPersonagem.Angler);
    }

    public void ParaDireita(IndicadorSeletor indicador) {
        indicador.Selecionar(QualPersonagem.Heater);
    }

    public void ParaBaixo(IndicadorSeletor indicador) {
        if (selecaoLocal && !indicador.estaConfirmado) {
            indicador.SelecionandoNada();
            indicador.transform.SetParent(seletorCancelar);
            return;
        }

        if (indicador.estaConfirmado) indicador.Desconfirmar();
    }

    public void ParaCima(IndicadorSeletor indicador) {
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
        gameObject.SetActive(true);

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
        gameObject.SetActive(false);
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
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }


    #endregion
}
