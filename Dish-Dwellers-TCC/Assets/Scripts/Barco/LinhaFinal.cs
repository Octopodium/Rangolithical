using UnityEngine;

public class LinhaFinal : MonoBehaviour
{
    public Barco barco;

    public void TirarPlayers(){
        barco.SairDoBarco();
    }

    public void Destruir(){
        Destroy(this.gameObject);
    }

}
