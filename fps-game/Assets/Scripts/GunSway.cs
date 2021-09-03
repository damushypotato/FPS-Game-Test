using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSway : MonoBehaviour
{

    public float amount = 0f;
    public float maxAmountX = 0f;
    public float maxAmountY = 0f;
    public float smoothing = 0f;
    public float threshhold = 0f;

    private Vector3 iPos;

    // Start is called before the first frame update
    void Start()
    {
        iPos = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float mX = -Input.GetAxis("Mouse X") * amount;
        float mY = -Input.GetAxis("Mouse Y") * amount;

        mX = Mathf.Clamp(mX, -maxAmountX, maxAmountX);
        mY = Mathf.Clamp(mY, -maxAmountY, maxAmountY);

        Vector3 fPos = new Vector3(mX, mY, 0);
        if (Mathf.Abs((fPos + iPos - transform.localPosition).magnitude) > threshhold)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, fPos + iPos, Time.fixedDeltaTime * smoothing);
        }

    }
}
