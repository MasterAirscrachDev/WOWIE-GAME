using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float flySpeed = 10;
    [SerializeField] bool lockCursor;
    [SerializeField] float mouseSensitivity = 3.5f;
    //bool shift; 
    float cameraPitch = 0;
    Transform cam;
    [SerializeField] float accelerationRatio = 3;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    Vector2 currentMouseDelta = Vector2.zero, currentMouseDeltaVelocity = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0);
        //set camera pitch to the current pitch of the camera
        cameraPitch = cam.localEulerAngles.x;
        //set current mouse delta
        //currentMouseDelta = new Vector2(45, 0);
        if (lockCursor){ Cursor.lockState = CursorLockMode.Locked; /*Cursor.visible = false;*/ }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
        
        //DEBUGING
    }
    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);
        cam.localEulerAngles = Vector3.right * cameraPitch;
        if(Time.timeScale != 0){ transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity / Time.timeScale); }
        
    }
    void UpdateMovement(){
        //use shift to speed up flight
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        { flySpeed *= accelerationRatio; }
    
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        { flySpeed /= accelerationRatio; }

        if (Input.GetAxis("Vertical") != 0)
        { transform.Translate(Vector3.forward * (flySpeed * Input.GetAxis("Vertical"))); }
        if (Input.GetAxis("Horizontal") != 0)
        { transform.Translate(Vector3.right * (flySpeed * Input.GetAxis("Horizontal"))); }
    
        if (Input.GetKey(KeyCode.E))
        { transform.Translate(Vector3.up * flySpeed); }
        else if (Input.GetKey(KeyCode.Q))
        { transform.Translate(Vector3.down * flySpeed); }
    }
}
