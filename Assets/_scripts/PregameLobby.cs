using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PregameLobby : MonoBehaviour
{

    public Transform cars;
    public GameObject cam;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLobby()
    {
        if (cam)
        { cam.SetActive(true); }

        if (cars)
        {
            foreach (Transform el in cars)
            { el.gameObject.SetActive(false); }
        }

    }

    public void EndLobby()
    {
        if (cam)
        { cam.SetActive(false); }

        if (cars)
        {
            foreach (Transform el in cars)
            { el.gameObject.SetActive(false); }
        }

    }

    public void PlayerJoined(int _faction)
    {
        if (cars)
        {
            if (cars.childCount > _faction)
            { cars.GetChild(_faction).gameObject.SetActive(true); }
        }
    }

}
