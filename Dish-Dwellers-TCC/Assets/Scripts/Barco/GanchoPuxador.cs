using UnityEngine;

public class GanchoPuxador : MonoBehaviour
{
    public Barco barco;
    public void HandlePuxada(){
        barco.IniciarPuxada(transform.position);
    }

}
