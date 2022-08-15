using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataShare : MonoBehaviour
{
    [SerializeField] AudioClip[] music;
    [SerializeField] AudioSource musicSource;
    public static int songIndex;
    public static float musicVolume = 0.5f, sfxVolume = 0.8f, songProgress;
    public static bool musicStarted;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        try{
            FindObjectOfType<UIManager>().SetMusicVolume( musicVolume );
            FindObjectOfType<UIManager>().SetSFXVolume( sfxVolume );
        }
        catch{
            Debug.Log("onoh");
        }
        
        if(!musicStarted){
            musicStarted = true;
            songIndex = Random.Range(0, music.Length);
            musicSource.clip = music[songIndex];
            musicSource.Play();
            //musicSource.time
        }
        else{
            musicSource.clip = music[songIndex];
            musicSource.time = songProgress;
            musicSource.Play();
        }
    }
    public void LoadScene(int sceneIndex){
        songProgress = musicSource.time;
        SceneManager.LoadScene(sceneIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if(!musicSource.isPlaying){
            songIndex++;
            if(songIndex > music.Length - 1){
                songIndex = 0;
            }
            musicSource.clip = music[songIndex];
            musicSource.time = 0;
            musicSource.Play();
        }
    }
}
