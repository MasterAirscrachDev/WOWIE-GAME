using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlocker : MonoBehaviour
{
    [SerializeField] float range = 1f;
    Transform playerAI;
    // Start is called before the first frame update
    void Start()
    {
        playerAI = FindObjectOfType<PlayerAi>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(playerAI.position, transform.position) < range)
        {
            playerAI.GetComponent<PlayerAi>().BlockCollide();
        }
    }
}
