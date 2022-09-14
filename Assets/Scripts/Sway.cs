using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    [SerializeField] private float intensity;
    [SerializeField] private float smooth;

    private Quaternion origin_rotation;

    private void Start()
    {
        origin_rotation = transform.localRotation;
    }

    private void Update()
    {
        updateSway();
    }

    private void updateSway()
    {
        float xMouse = Input.GetAxis("Mouse X");
        float yMouse = Input.GetAxis("Mouse Y");

        Quaternion xAdjustment = Quaternion.AngleAxis(-intensity * xMouse, Vector3.up);
        Quaternion yAdjustment = Quaternion.AngleAxis(intensity * yMouse, Vector3.right);
        Quaternion rotation = origin_rotation * xAdjustment * yAdjustment;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, Time.deltaTime * smooth);
    }
}
