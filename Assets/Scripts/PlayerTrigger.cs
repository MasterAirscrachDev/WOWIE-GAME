using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] float range = 1f;
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerInteractor>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        //if player is within range of the trigger, activate the object
        if(Vector3.Distance(player.position, transform.position) < range){
            obj.SetActive(true);
        }
        else{
            obj.SetActive(false);
        }
    }
}
