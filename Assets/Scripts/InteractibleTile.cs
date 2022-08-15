using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleTile : MonoBehaviour
{
    [SerializeField] Vector3[] positions;
    Vector3 basePosition;
    int currentPosition = 0;
    public bool isMoving = false;
    float progress = 0, speed = 0.0005f;

    // Start is called before the first frame update
    void Start()
    {
        basePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = Vector3.Lerp(transform.position, basePosition + positions[currentPosition], progress);
        progress += speed;
        if(progress > 1){
            progress = 1;
        }
        if(Vector3.Distance(transform.position, basePosition + positions[currentPosition]) < 0.01f){
            isMoving = false;
        }
        else{
            isMoving = true;
        }
    }
    public void Interact(){
        currentPosition++;
        if(currentPosition >= positions.Length){
            currentPosition = 0;
            
        }
        progress = 0;
    }
}
