using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] float dist;
    Vector3 compairePoint;
    AudioSource audioSource;
    Transform ai;
    bool reset = false;
    // Start is called before the first frame update
    void Start()
    {
        compairePoint = transform.position + (Vector3.up * 0.5f);
        ai = FindObjectOfType<PlayerAi>().transform;
        audioSource = transform.GetChild(2).GetComponent<AudioSource>();
        audioSource.transform.position = obj.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(compairePoint, ai.position);
        if(dist < 0.3f){
            if(!reset){
                obj.SetActive(!obj.activeSelf);
                audioSource.Play();
                reset = true;
            }
        }
        else{
            reset = false;
        }
    }
}
