using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHacking : MonoBehaviour
{
    public float amountOfHack=3;
    public float amountOfHackLeft;
    public float timeOfHack=5f;

    bool canHack;
    public bool boom = false;

    GameObject currentInteractObject = null;
    GameObject civilian;

    void Update()
    {
        PlantHack();
        CheckNumberOfHack();
        Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("InteractObject"))
        {
            currentInteractObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InteractObject"))
        {
            if (currentInteractObject == other.gameObject)
            {
                currentInteractObject = null;
            }
        }
    }

    void PlantHack()
    {
        if (Input.GetKeyDown(KeyCode.W) && currentInteractObject && canHack)
        {
            currentInteractObject.SendMessage("Hacking");
        }
    }

    void Explode()
    {
        civilian = GameObject.FindGameObjectWithTag("InteractObject");
        if (Input.GetKey(KeyCode.X))
        {
            boom = true;
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            boom = false;
        }
    }

    void CheckNumberOfHack()
    {
        amountOfHackLeft = amountOfHack;

        if (amountOfHackLeft > 0)
        {
            canHack = true;
        }
        else
        {
            canHack = false;
        }
    }
}
