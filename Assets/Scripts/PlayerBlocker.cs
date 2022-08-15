using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlocker : MonoBehaviour
{
    [SerializeField] float range = 1f;
    Transform playerAI;
    void Start()
    { playerAI = FindObjectOfType<PlayerAi>().transform; }
    void Update()
    {
        if(Vector3.Distance(playerAI.position, transform.position) < range)
        { playerAI.GetComponent<PlayerAi>().BlockCollide(); }
    }
}
