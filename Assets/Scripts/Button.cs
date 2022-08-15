using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] float dist, range = 0.3f;
    Vector3 compairePoint;
    AudioSource audioSource;
    Transform ai;
    bool reset = false;
    void Start()
    {
        compairePoint = transform.position + transform.TransformDirection(Vector3.up * 0.5f);
        Debug.DrawLine(transform.position, compairePoint, Color.grey, 100);
        ai = FindObjectOfType<PlayerAi>().transform;
        audioSource = transform.GetChild(2).GetComponent<AudioSource>();
        audioSource.transform.position = obj.transform.position;
    }
    void Update()
    {
        dist = Vector3.Distance(compairePoint, ai.position);
        if(dist < range){
            if(!reset){
                //check if any of the objects children are a player
                for(int i = 0; i < obj.transform.childCount; i++){
                    if(obj.transform.GetChild(i).GetComponent<PlayerAi>()){
                        obj.transform.GetChild(i).GetComponent<PlayerAi>().PlatformRemoved();
                        break;
                        
                    }
                }
                obj.SetActive(!obj.activeSelf);
                audioSource.Play();
                reset = true;
            }
        }
        else{ reset = false; }
    }
}
