using UnityEngine;

[CreateAssetMenu(fileName = "Sala_X_X", menuName = "SalaInfo")]
public class SalaInfo : ScriptableObject {
    public string nomeDaSala;
    public string nomeNoSeletor;
    [Tooltip("Por exemplo: 2-1, 1-1, 2-2")]
    public string caminhoParaSala;
    public Sprite desenhoDaSala;
    public ColecionavelData[] colecionaveis;
    public RegiaoInfo regiao;
    public int numeroDaSala;

    public int numeroDaRegiao { get{ return regiao.numeroDaRegiao; }}
}
