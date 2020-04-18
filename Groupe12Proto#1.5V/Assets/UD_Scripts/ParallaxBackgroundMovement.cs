using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackgroundMovement : MonoBehaviour
{
    [SerializeField]
    private Transform mainCameraPosition;

    [SerializeField]
    private float backgroundMoveSpeed;
    private float directionX;

    [SerializeField]
    private float offsetByX = 13f;

    public PlayerController pC;

    void Update()
    {
        if (pC.isMoving == true)
        {
            Moving();
        }
    }

    void Moving()
    {
        directionX = Input.GetAxis("Horizontal") * backgroundMoveSpeed * Time.deltaTime;

        transform.position = new Vector2(transform.position.x + directionX, transform.position.y);

        if (transform.position.x - mainCameraPosition.position.x < -offsetByX)
        {
            transform.position = new Vector2(mainCameraPosition.position.x + offsetByX, transform.position.y);
        }
        else if (transform.position.x - mainCameraPosition.position.x > offsetByX)
        {
            transform.position = new Vector2(mainCameraPosition.position.x - offsetByX, transform.position.y);
        }
    }
}
