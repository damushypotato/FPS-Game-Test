using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    private void Update()
    {
        UpdatePos();
    }

    public void UpdatePos()
    {
        transform.position = cameraPosition.position;
    }
}
