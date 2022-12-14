using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] int nextLevel;
    [SerializeField] Image recticle;
    [SerializeField] Toggle fpsToggle;
    [SerializeField] TMP_Text levelText, Coming, FPStext;
    [SerializeField] GameObject sceneTransition, pauseMenu;
    [SerializeField] Slider distract, SFX, Music;
    public bool paused = false;
    Transform playerAI, player;
    List<float> frames = new List<float>();
    // Start is called before the first frame update
    void Start()
    {
        playerAI = FindObjectOfType<PlayerAi>().transform;
        player = FindObjectOfType<PlayerInteractor>().transform;
        StartCoroutine(DistractCharge());
        levelText.text = "Level " + SceneManager.GetActiveScene().buildIndex;
        if(sceneTransition.activeSelf){ StartCoroutine(HideSceneTransition()); }
        StartCoroutine(FPS());
    }
    IEnumerator FPS(){
        yield return new WaitForSeconds(1);
        int sum = 0;
        for(int i = 0; i < frames.Count; i++){ sum += (int)frames[i]; }
        FPStext.text = $"FPS: {Mathf.Round(sum / frames.Count)}";
        StartCoroutine(FPS());
    }
    void Update(){
        frames.Add(1 / Time.deltaTime);
        //if there are more than 50 frames, remove the first one
        if(frames.Count > 50){ frames.RemoveAt(0); }
        RaycastHit hit;
        //raycast forwards from player
        if(Physics.Raycast(player.position, player.forward, out hit)){
            //check if line of sight from player to cursor + 0.5y
            Debug.DrawLine(playerAI.position, hit.point + (Vector3.up * 0.5f), Color.red, 0.1f);
            if(Physics.Linecast(playerAI.position, hit.point + (Vector3.up * 0.5f))){
                recticle.color = Color.red;
            }
            else{ recticle.color = Color.blue; }
        }
        else
        { recticle.color = Color.red; }
    }
    public void EnableComing(){
        Coming.enabled = true;
    }

    IEnumerator HideSceneTransition(){
        while(sceneTransition.transform.localPosition.x > -1920){
            sceneTransition.transform.localPosition = Vector3.Lerp(sceneTransition.transform.localPosition, new Vector3(-1930, 0, 0), 0.1f);
            yield return new WaitForSeconds(0.01f);
        }
        sceneTransition.SetActive(false);
        sceneTransition.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void SetSFXVolume(float volume = -1){
        if(volume == -1){
            //find all the audio sources and set their volume to the slider value
            AudioSource[] sources = FindObjectsOfType<AudioSource>();
            for(int i = 0; i < sources.Length; i++){
                if(sources[i].tag != "Music"){ sources[i].volume = SFX.value; }
            }
            SceneDataShare.sfxVolume = SFX.value;
        }
        else{ SFX.value = volume; }
    }
    public void SetMusicVolume(float volume = -1){
        if(volume == -1){
            //find all the audio sources and set their volume to the slider value
            AudioSource[] sources = FindObjectsOfType<AudioSource>();
            for(int i = 0; i < sources.Length; i++){
                if(sources[i].tag == "Music"){ sources[i].volume = Music.value; }
            }
            SceneDataShare.musicVolume = Music.value;
        }
        else{ Music.value = volume; } 
    }
    public void Reset()
    { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    IEnumerator DistractCharge(){
        while(true){
            int val = (int)distract.value;
            if(val < 100){ distract.value += 1; }
            else{
                FindObjectOfType<PlayerInteractor>().RechargeDistract();
                yield return new WaitForSeconds(3);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void ToMenu()
    { FindObjectOfType<SceneDataShare>().LoadScene(0); }
    public void Distracted()
    { distract.value = 0; }
    public void EnableTransition(){
        sceneTransition.SetActive(true);
        FindObjectOfType<SceneDataShare>().LoadScene(nextLevel);

    }
    public void ToggleFPS(bool toggle = false){
        if(toggle){
            fpsToggle.isOn = true;
            FPStext.transform.parent.gameObject.SetActive(true);
        }
        else{
            FPStext.transform.parent.gameObject.SetActive(fpsToggle.isOn );
        }
        SceneDataShare.fps = fpsToggle.isOn;
    }
    public void TogglePauseMenu(bool state){
        paused = state;
        pauseMenu.SetActive(state);
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }
}
