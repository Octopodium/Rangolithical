using UnityEngine;

[CreateAssetMenu(fileName = "Regiao_1", menuName = "RegiaoInfo")]
public class RegiaoInfo : ScriptableObject {
    public string nome;

    [Tooltip("Caminho do Resources para a pasta com as SalaInfos referente a esta região. Por exemplo, a região 1 seria 'Salas/1'.")]
    public string caminhoSalaInfos;
    public int numeroDaRegiao;

    public SalaInfo[] salas {
        get { return Resources.LoadAll<SalaInfo>(caminhoSalaInfos); }
    }
}
