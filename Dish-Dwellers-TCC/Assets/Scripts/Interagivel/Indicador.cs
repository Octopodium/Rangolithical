using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class Indicador : MonoBehaviour {
    [System.Serializable]
    public class ImagensDeIndicador {
        public Sprite padrao;
        public Sprite trancado;
        public Sprite forca;
    }

    public ParentConstraint parentConstraint;
    int sourceId = -1;
    public InteragivelBase interagivel { get; private set; }
    public bool ativo => gameObject.activeSelf;

    public Image imagemIndicador;
    public ImagensDeIndicador imagens;
    public MotivoNaoInteracao imagemAtual = MotivoNaoInteracao.Nenhum;

    void Awake() {
        /*parentConstraint = GetComponent<ParentConstraint>();
        if (parentConstraint == null) parentConstraint = gameObject.AddComponent<ParentConstraint>();*/
        parentConstraint.rotationAxis = Axis.None; // Desabilita rotação ao ser pego

        imagemIndicador.sprite = GetSprite(imagemAtual);
    }

    public Sprite GetSprite(MotivoNaoInteracao motivo) {
        switch (motivo) {
            case MotivoNaoInteracao.Nenhum:
                return imagens.padrao;
            case MotivoNaoInteracao.Fraco:
                return imagens.forca;
            case MotivoNaoInteracao.Trancado:
                return imagens.trancado;   
        }

        return imagens.padrao;
    }

    public void Mostrar(InteragivelBase interagivel, MotivoNaoInteracao motivo = MotivoNaoInteracao.Nenhum) {
        if (interagivel == null) return;
        if (this.interagivel == interagivel) return;
        if (this.interagivel != null) RemoverUltimo();

        imagemAtual = motivo;
        imagemIndicador.sprite = GetSprite(imagemAtual);

        this.interagivel = interagivel;

        sourceId = parentConstraint.AddSource(new ConstraintSource() {
            sourceTransform = interagivel.transform,
            weight = 1f
        });

        parentConstraint.SetTranslationOffset(sourceId, interagivel.offsetIndicador);
        parentConstraint.constraintActive = true;

        gameObject.SetActive(true);
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
}
