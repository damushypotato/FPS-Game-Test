using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    Camera cam;

    public float fov;
    public float wallRunFov;
    public float wallRunFovTime;
    public float camTilt;
    public float camTiltTime;

    public float tilt { get; private set; }

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void LerpFovWallrun(bool toWallrunFov)
    {
        float _fov = toWallrunFov ? wallRunFov : fov;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _fov, wallRunFovTime * Time.deltaTime);
    }

    public void LerpTiltWallrun(float multiplier)
    {
        tilt = Mathf.Lerp(tilt, camTilt * multiplier, camTiltTime * Time.deltaTime);
    }

}
