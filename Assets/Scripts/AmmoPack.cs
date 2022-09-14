using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour
{

    [SerializeField] private int ammoType;

    private int ammoAmount;

    private float zRotation;
    private float zTranslation;
    private float currentHeight;

    // Start is called before the first frame update
    void Start()
    {
        ammoAmount = Random.Range(1, 7) * 5;
        zRotation = 6f;
        zTranslation = 0.05f;
        currentHeight = 0f;
        transform.Rotate(-90, 0, 0, Space.Self);
        Destroy(gameObject, 500f);
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        //add opacity flashing when close to being destroyed

        transform.Rotate(0, 0, zRotation * Time.fixedDeltaTime, Space.Self);
        currentHeight += zTranslation;
        if (currentHeight < -2 || currentHeight >= 2)
            zTranslation = -zTranslation;

        transform.Translate(new Vector3(0f, 0f, zTranslation * Time.fixedDeltaTime), Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            if(other.gameObject.GetComponent<Weapon>().PickupAmmo(ammoType, ammoAmount))
            {
                Destroy(gameObject);
            }
        }
    }
}
