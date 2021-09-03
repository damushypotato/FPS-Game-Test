using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GameObject player;
    Vector3 startPos;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPos = player.transform.position;
    }

    void Update()
    {
        if (player.transform.position.y < -20)
        {
            player.transform.position = startPos;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
