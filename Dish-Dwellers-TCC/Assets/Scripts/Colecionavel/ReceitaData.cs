using System.Collections.Generic;
using UnityEngine;

public enum TipoReceita { Foto, Skin }

[CreateAssetMenu(fileName = "Receita", menuName = "Colecionavel/Receita")]
public class ReceitaData : ScriptableObject {
    public string nome;
    public TipoReceita tipo;
    public IngredienteData[] requisitos;

    public string id {
        get { return name; }
    }

    public bool Fazer() {
        return ColecionavelController.instance.Fazer(this);
    }

    public bool TemRequisitos() {
        return ColecionavelController.instance.PodeFazer(this);
    }
}
