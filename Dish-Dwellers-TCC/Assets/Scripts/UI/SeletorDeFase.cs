using UnityEngine;
using UnityEngine.SceneManagement;

public class SeletorDeFase : MonoBehaviour
{
    public void IrParaSala(string salaFase){
        if(salaFase != null){
            SceneManager.LoadScene(salaFase, LoadSceneMode.Single);
        }
    }

}
