using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Look : MonoBehaviour
{
    [SerializeField] private Canvas settingsMenu;

    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float maxAngle;

    [SerializeField] private Slider sensitivitySlider;

    [SerializeField] private Transform player;
    [SerializeField] private Transform cam;

    private bool paused;

    private static bool cursorLocked = true;

    private Quaternion camCentre;


    void Start()
    {
        camCentre = cam.localRotation;
        LockCursor();
        settingsMenu.enabled = false;
    }

    void Update()
    {
        SetX(); 
        SetY();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cursorLocked = !cursorLocked;
            LockCursor();
        }
    }

    void SetY()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensitivitySlider.value * mouseSensitivity * Time.deltaTime;
        Quaternion adjustment = Quaternion.AngleAxis(mouseY, -Vector3.right);
        Quaternion delta = cam.localRotation * adjustment;

        if(Quaternion.Angle(camCentre, delta) < maxAngle)
        {
            cam.localRotation = delta;
        }
    }

    void SetX()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivitySlider.value* mouseSensitivity * Time.deltaTime;
        player.Rotate(Vector3.up * mouseX);
    }

    void LockCursor()
    {
        if (cursorLocked)
        {
            paused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            settingsMenu.enabled = false;
            Time.timeScale = 1;
        }
        else
        {
            paused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            settingsMenu.enabled = true;
            Time.timeScale = 0;
        }
    }

    public bool isPaused()
    {
        return paused;
    }
}
