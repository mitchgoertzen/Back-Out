using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPlane : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private float currentTime = 0.1f;

    private Vector3 newPos;

    void Update()
    {
        newPos = transform.position;
        currentTime += Time.deltaTime;
        if (currentTime >= 30)
        {
            movementSpeed *= 1.5f;
            currentTime = 0;
        }
        newPos.z += Time.deltaTime * movementSpeed;
        transform.position = newPos;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            other.gameObject.GetComponent<PlayerAttributes>().TakeDamage(1, false);
        }
    }
}
