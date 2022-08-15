using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] GameObject info;
    void Start()
    { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    public void Play()
    { FindObjectOfType<SceneDataShare>().LoadScene(1); }
    public void Quit()
    { Application.Quit(); }
    public void Info()
    { info.SetActive(!info.activeSelf); }
}