using UnityEngine;

[CreateAssetMenu(fileName = "colecionavel", menuName = "Colecionavel/Colecionavel")]
public class ColecionavelData : ScriptableObject {
    public IngredienteData ingrediente;
    public Sprite sprite_override;

    public string id { // [nome] + '_' + [id]. ex: cenoura_1, tomate_3
        get { return name; }
    }

    public Sprite sprite {
        get { return sprite_override != null ? sprite_override : ingrediente?.sprite;}
    }
}
