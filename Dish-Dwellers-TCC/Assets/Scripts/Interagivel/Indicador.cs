using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Indicador : MonoBehaviour {
    [System.Serializable]
    public class ImagensDeIndicador {
        public Sprite padraoTeclado;
        public Sprite padraoControle;
        public Sprite cancelarTeclado;
        public Sprite cancelarControle;

        public Sprite trancado;
        public Sprite forca;
    }

    public ParentConstraint parentConstraint;
    int sourceId = -1;
    public InteragivelBase interagivel { get; private set; }
    public bool ativo => gameObject.activeSelf;

    public Image imagemIndicador;
    public ImagensDeIndicador imagens;

    public Player jogador;
    MotivoNaoInteracao motivo;

    public float offsetQuandoDois = 0.1f;

    public bool mostrando { get { return gameObject.activeSelf; }}

    void Awake() {
        /*parentConstraint = GetComponent<ParentConstraint>();
        if (parentConstraint == null) parentConstraint = gameObject.AddComponent<ParentConstraint>();*/
        parentConstraint.rotationAxis = Axis.None; // Desabilita rotação ao ser pego

        jogador.OnDeviceChange += HandleDeviceChange;
    }

    public Sprite GetSprite() {
        switch (motivo) {
            case MotivoNaoInteracao.Nenhum:
                return jogador.controleAtual is Gamepad ? imagens.padraoControle : imagens.padraoTeclado;
            case MotivoNaoInteracao.Fraco:
                return imagens.forca;
            case MotivoNaoInteracao.Trancado:
                return imagens.trancado;
            case MotivoNaoInteracao.Cancelar:
                return jogador.controleAtual is Gamepad ? imagens.cancelarControle : imagens.cancelarTeclado;
        }

        return imagens.padraoTeclado;
    }

    public void Mostrar(InteragivelBase interagivel, MotivoNaoInteracao motivo = MotivoNaoInteracao.Nenhum) {
        if (interagivel == null) return;
        if (this.interagivel != null || this.interagivel != interagivel) RemoverUltimo();

        this.interagivel = interagivel;
        this.motivo = motivo;

        sourceId = parentConstraint.AddSource(new ConstraintSource() {
            sourceTransform = interagivel.indicadorTransform,
            weight = 1f
        });

        parentConstraint.SetTranslationOffset(sourceId, interagivel.offsetIndicador);
        parentConstraint.constraintActive = true;

        gameObject.SetActive(true);

        RefreshDisplay();
    }

    public void Refresh() {
        if (mostrando) {
            InteragivelBase interagivel = this.interagivel;
            MotivoNaoInteracao motivo = this.motivo;

            Esconder(interagivel);
            Mostrar(interagivel, motivo);
        }
    }

    void HandleDeviceChange(InputDevice d) {
        RefreshDisplay();
    }

    void FixedUpdate() {
        if (interagivel.indicadores.Count <= 1) {
            parentConstraint.SetTranslationOffset(sourceId, interagivel.offsetIndicador);
            return;
        }

        Vector3 dir = interagivel.indicadorTransform.InverseTransformDirection(Vector3.right);
        Indicador outro = interagivel.ProximoIndicador(this);
        Vector3 offset = interagivel.offsetIndicador;
        Vector3 offsetExtra = dir * GetOffsetBaseOnPlayers(jogador, outro.jogador);
        parentConstraint.SetTranslationOffset(sourceId, offset + offsetExtra);
    }

    public void RefreshDisplay() {
        imagemIndicador.sprite = GetSprite();
    }

    public float GetOffsetBaseOnPlayers(Player meu, Player seu) {
        return meu.transform.position.x < seu.transform.position.x ? -offsetQuandoDois : offsetQuandoDois;
    }

    public void Esconder(InteragivelBase interagivel) {
        if (this.interagivel == interagivel) Esconder();
    }

    public void Esconder() {
        RemoverUltimo();

        gameObject.SetActive(false);
        interagivel = null;
    }

    void RemoverUltimo() {
        if (sourceId != -1) {
            parentConstraint.RemoveSource(sourceId);
            parentConstraint.constraintActive = false;
            sourceId = -1;
        }
    }

    void OnDestroy() {
        jogador.OnDeviceChange -= HandleDeviceChange;
    }
}
