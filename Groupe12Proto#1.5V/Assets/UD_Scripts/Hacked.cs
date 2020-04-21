using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacked : MonoBehaviour
{
    //Test branch Git
    private float timeHackLeft;

    public bool hacked;
    private bool canExplode;

    public PlayerHacking PH;
    public PlayerScore PS;

    void Start()
    {
        hacked = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckHack();
        Timer();
        CheckExplosion();
    }

    void CheckHack()
    {
        if (PH.timeOfHack < timeHackLeft)
        {
            hacked = false;
            PH.amountOfHack += 1;
            timeHackLeft = 0f;
            canExplode = false;
        }
    }

    void Timer()
    {
        if (hacked == true)
        {
            timeHackLeft += Time.deltaTime;
        }
    }

    public void Hacking()
    {
        if (hacked == false)
        {
            hacked = true;
            timeHackLeft = 0;
            PH.amountOfHack -= 1;
            canExplode = true;
        }
        else
        {
            timeHackLeft = 0;
        }
    }

    public void CheckExplosion()
    {
        if (PH.boom==true && hacked)
        {
            Boom();
        }

    }

    public void Boom()
    {
        Debug.Log("Boom");
        if (hacked)
        {
            PH.amountOfHack += 1;
            PS.score += 1;
            gameObject.SetActive(false);
        }
    }
}
