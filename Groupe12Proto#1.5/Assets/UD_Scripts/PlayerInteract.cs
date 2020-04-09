using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    GameObject currentInteractObject = null;

    private void Update()
    {
        KillCivilian();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("InteractObject"))
        {
            Debug.Log(other.name);
            currentInteractObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InteractObject"))
        {
            if(currentInteractObject == other.gameObject)
            {
                currentInteractObject = null;
            }
        }
    }

    void KillCivilian()
    {
        if(Input.GetKeyDown(KeyCode.C) && currentInteractObject)
        {
            currentInteractObject.SendMessage("Kill");
        }
    }
}
