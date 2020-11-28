using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioManager>().Play("menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRun()
    {
        GameManager.gm.StartRun();
        FindObjectOfType<AudioManager>().Stop("menu");
    }
}
