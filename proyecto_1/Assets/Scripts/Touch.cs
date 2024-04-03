using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    public int live = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        live--;
        Debug.Log($"\n\nTE HAN DADO!! TU VIDA ES DE {live}\n\n");
        if(live < 1)
        {
            Time.timeScale = 0f;
            Debug.Log($"\n\nGAME OVER!!\n\n");
        }
    }
}
