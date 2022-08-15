using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    void Start(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    [SerializeField] GameObject info;
    // Start is called before the first frame update
    public void Play(){
        FindObjectOfType<SceneDataShare>().LoadScene(1);
    }
    public void Quit(){
        Application.Quit();
    }
    public void Info(){
        info.SetActive(!info.activeSelf);
    }
}
