using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private ConsumableItem[] consumables = new ConsumableItem[2];

    [SerializeField] private MeshRenderer rend;

    private int healthAmount;
    private int consumableIndex = 0;

    private float zRotation;
    private float zTranslation;
    private float currentHeight;


    // Start is called before the first frame update
    void Start()
    {
        healthAmount = Random.Range(1, 3) * 25;

        if (healthAmount > 25)
        {
            Material[] currentMaterials = rend.materials;
            Material temp = currentMaterials[0];
            currentMaterials[0] = currentMaterials[2];
            currentMaterials[2] = temp;
            rend.materials = currentMaterials;
            transform.localScale = new Vector3(100,100,150);
            consumableIndex = 1;
        }
            zRotation = 6f;
            zTranslation = 0.05f;
            currentHeight = 0f;
            transform.Rotate(-90, 0, 0, Space.Self);
            Destroy(gameObject, 500f);
        }


        // Update is called once per frame
        void FixedUpdate()
    {
        //TODO: add opacity flashing when close to being destroyed

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
            if (other.gameObject.GetComponent<Inventory>().PickUp(consumables[consumableIndex]))
                Destroy(gameObject);
        }
    }
}