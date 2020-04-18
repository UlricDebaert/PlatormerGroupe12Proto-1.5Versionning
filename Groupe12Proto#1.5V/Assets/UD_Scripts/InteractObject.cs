using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    public PlayerScore PS;

    //Transplanter ce script dans Hacked pour éciter le bug de perte de hack quand tué au cac alors que hacked
    public void Kill()
    {
        gameObject.SetActive(false);
        PS.score+=1;
    }
}
