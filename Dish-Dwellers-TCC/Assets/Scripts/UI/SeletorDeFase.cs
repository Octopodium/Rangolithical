using UnityEngine;
using UnityEngine.SceneManagement;

public class SeletorDeFase : MonoBehaviour
{
    public void IrParaSala(string salaFase){
        SceneManager.LoadScene(salaFase, LoadSceneMode.Single);
    }

}
