using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    Transform ico;
    // Start is called before the first frame update
    void Start()
    { ico = transform.GetChild(3); ico.localScale = Vector3.zero; ico.gameObject.SetActive(false); }

    // Update is called once per frame
    void Update()
    {
        //randomly rotate the ico
        ico.Rotate(Vector3.up, Time.deltaTime * Random.Range(0, 360) * 0.1f);
        ico.Rotate(Vector3.right, Time.deltaTime * Random.Range(0, 360) * 0.1f);
        ico.Rotate(Vector3.forward, Time.deltaTime * Random.Range(0, 360) * 0.1f);
    }
    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player"){ StartCoroutine(Win()); }
    }
    IEnumerator Win(){
        yield return new WaitForSeconds(1);
        FindObjectOfType<PlayerAi>().ClearLines();
        FindObjectOfType<PlayerAi>().enabled = false;
        Transform player = FindObjectOfType<PlayerAi>().transform;
        GetComponent<AudioSource>().Play();
        while(player.localScale.x > 0.1f){
            player.localScale = Vector3.Lerp(player.localScale, Vector3.zero, 0.1f);
            yield return new WaitForEndOfFrame();
        }
        player.localScale = Vector3.zero;
        yield return new WaitForSeconds(1);
        ico.gameObject.SetActive(true);
        while(ico.localScale.x < 0.25f){
            ico.localScale = Vector3.Lerp(ico.localScale, Vector3.one * 0.3f, 0.05f);
            yield return new WaitForEndOfFrame();
        }
        FindObjectOfType<PlayerInteractor>().CheckEnd(ico);
    }
    public void BeginExpand()
    { StartCoroutine(ExpandIco()); }
    IEnumerator ExpandIco(){
        while(true){
            ico.localScale = Vector3.Lerp(ico.localScale, ico.localScale * 1.2f, 0.1f);
            yield return new WaitForEndOfFrame();
        }
    }
}
