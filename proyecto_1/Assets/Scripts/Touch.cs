using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Touch : MonoBehaviour
{
    //public int live = 5;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameManager.DanioAlJugador(1);
        /*
        live--;
        Debug.Log($"\n\nTE HAN DADO!! TU VIDA ES DE {live}\n\n");
        if(live < 1)
        {
            Time.timeScale = 0f;
            //CambioEscena();
            Debug.Log($"\n\nGAME OVER!!\n\n");
        }*/
    }
    /*
    public void CambioEscena()
    {
        SceneManager.LoadScene("Menu");
    }
    */
}
