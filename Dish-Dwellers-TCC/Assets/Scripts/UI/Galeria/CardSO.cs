using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Galeria/Card")]
public class CardSO : ScriptableObject
{
    [Header("Item Description")]
    public string cardName;
    [TextArea(10,10)]
    public string description;
    public Sprite cardImage;

    public IngredienteData[] requisitos;
    public int id;
}
