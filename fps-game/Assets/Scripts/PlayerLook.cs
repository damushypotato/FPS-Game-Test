using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLook : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public float minSens;
    public float maxSens;

    public Slider sensXSlider;
    public Slider sensYSlider;

    public Transform cam;
    public Transform orientation;
    public CameraEffects camEffects;

    float mouseX;
    float mouseY;

    float multipler = 0.01f;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sensXSlider.value = (PlayerPrefs.GetFloat("sensX", sensX) - minSens) / (maxSens - minSens);
        sensYSlider.value = (PlayerPrefs.GetFloat("sensY", sensY) - minSens) / (maxSens - minSens);
    }

    private void Update()
    {
        if (!PauseMenu.GameIsPaused) MyInput();

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, camEffects.tilt);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    private void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multipler;
        xRotation -= mouseY * sensY * multipler;

        xRotation = Mathf.Clamp(xRotation, -90f, 90);
    }

    public void SetSensXFromSlider()
    {
        Vector2 sens = SliderValToSens();
        sensX = sens.x;
        PlayerPrefs.SetFloat("sensX", sensX);
    }
    public void SetSensYFromSlider()
    {
        Vector2 sens = SliderValToSens();
        sensY = sens.y;
        PlayerPrefs.SetFloat("sensY", sensY);
    }
    
    Vector2 SliderValToSens()
    {
        Vector2 sens;
        sens.x = Mathf.Lerp(minSens, maxSens, sensXSlider.value);
        sens.y = Mathf.Lerp(minSens, maxSens, sensYSlider.value);
        return sens;
    }
}
