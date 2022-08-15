using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    Transform clickEffect;
    bool checkEnd = false, distractCharged = true;
    Transform EndIco;
    // Start is called before the first frame update
    void Start()
    { clickEffect = GameObject.FindGameObjectWithTag("ClickEffect").transform; }
    public void RechargeDistract()
    { distractCharged = true; }
    public void CheckEnd(Transform endIco)
    { checkEnd = true; EndIco = endIco; }

    // Update is called once per frame
    void Update()
    {
        //if leftclick raycast
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                //if the raycast hits an interactable object, call the interact method
                if (hit.collider.gameObject.GetComponent<InteractibleTile>() != null)
                { hit.collider.gameObject.GetComponent<InteractibleTile>().Interact(); }
                else if(hit.collider.tag == "EndPickup")
                {
                    FindObjectOfType<Goal>().BeginExpand();
                    EndIco = hit.collider.gameObject.transform;
                    checkEnd = true;
                }
            }
        }
        if(Input.GetMouseButtonDown(1) && distractCharged){
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                Debug.Log($"FaceDot {Vector3.Dot(hit.normal, Vector3.up)}");
                //if the normal faces up
                if (Mathf.Approximately(Vector3.Dot(hit.normal, Vector3.up),1f))
                {
                    //do pointer stuff
                    clickEffect.position = hit.point;
                    clickEffect.GetComponent<ParticleSystem>().Play();
                    FindObjectOfType<PlayerAi>().SetDistraction(hit.point);
                    distractCharged = false;
                    FindObjectOfType<UIManager>().Distracted();
                }
                else
                { Debug.Log("Not facing up"); }
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        { FindObjectOfType<UIManager>().TogglePauseMenu(!FindObjectOfType<UIManager>().paused); }
        if(Input.GetKeyDown(KeyCode.F3))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
        if(checkEnd){
            //raycast from the player to the goal
            RaycastHit hit;
            Physics.Raycast(transform.position, EndIco.position - transform.position, out hit);
            if(hit.distance < 0.5f)
            { FindObjectOfType<UIManager>().EnableTransition(); this.enabled = false; }
        }

    }
}
