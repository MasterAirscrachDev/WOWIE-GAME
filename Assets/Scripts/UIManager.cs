using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] int nextLevel;
    [SerializeField] TMP_Text levelText;
    [SerializeField] GameObject sceneTransition, pauseMenu;
    [SerializeField] Slider distract, SFX, Music;
    public bool paused = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DistractCharge());
        levelText.text = "Level " + SceneManager.GetActiveScene().buildIndex;
        if(sceneTransition.activeSelf){ StartCoroutine(HideSceneTransition()); }
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
    public void TogglePauseMenu(bool state){
        paused = state;
        pauseMenu.SetActive(state);
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }
}
