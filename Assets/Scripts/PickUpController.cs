using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{

    [SerializeField] private Collider coll;

    [SerializeField] private float pickUpRange;
    [SerializeField] private float dropForceForward;
    [SerializeField] private float dropForceUpward;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private WeaponObject gun;

    private GameObject player;

    private Transform weapon;
    private Transform cam;

    private Vector3 playerInRange;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerInRange = player.transform.position - transform.position;
            if (playerInRange.magnitude <= pickUpRange)
            {
                PickUp();
            }
        }
    }

    private void PickUp()
    {
        player.GetComponent<Weapon>().PickUpWeapon(gun);
        Destroy(gameObject);
    }
}

