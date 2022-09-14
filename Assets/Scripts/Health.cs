using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private Color endColour;

    [SerializeField] private float colourSpeed;
    [SerializeField] private float maxHealth;

    [SerializeField] private GameObject healthPack;
    [SerializeField] private GameObject pistolAmmo;
    [SerializeField] private GameObject arAmmo;
    [SerializeField] private GameObject shotgunAmmo;
    [SerializeField] private GameObject sniperAmmo;

    [SerializeField] private int dropRate;

    [SerializeField] private MeshRenderer meshRenderer;

    private bool isHit;
    private bool isDead;

    private Color startColour;

    private float currentHealth;
    private float startTime;

    private GameObject player;

    private int dropTypes = 2;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isHit = false;
        currentHealth = maxHealth;
        startColour = meshRenderer.material.color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isHit)
        {
            float t = (Mathf.Sin(Time.time - startTime) * colourSpeed);
            meshRenderer.material.color = Color.Lerp(startColour, endColour, t);
        }else if(meshRenderer.material.color != startColour)
        {
            float t = (Time.time - startTime) * colourSpeed;
            meshRenderer.material.color = Color.Lerp(endColour, startColour, t);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        meshRenderer.material.color = startColour;
        isHit = true;
        startTime = Time.time;

        if (currentHealth <= 0)
        {
            Invoke("Die", 1 / colourSpeed);
        }
        else
        {
            Invoke("ResetColor", (10 / colourSpeed / 4));
        }
    }

    void ResetColor()
    {
        isHit = false;
        startTime = Time.time;
    }

    void Die()
    {
        if (!isDead)
        {
            isDead = true;
            player.GetComponent<PlayerAttributes>().collectVapour(maxHealth);
            DetermineDrop();
            Destroy(gameObject);
        }

    }

    private void DetermineDrop()
    {

        int random = Random.Range(0, dropRate);
        Vector3 position = transform.position;

        position.y += .5f;
        if (random <= dropRate / 2)
        {
           
            int drop = Random.Range(0, dropTypes);
            int ammoType = Random.Range(1, 4);
            GameObject newPack = pistolAmmo;
            switch (ammoType)
            {
                case 1:
                    newPack = pistolAmmo;
                break;
                case 2:
                    newPack = arAmmo;
                break;
                case 3:
                    newPack = shotgunAmmo;
                break;
            }

            Instantiate(newPack, position, Quaternion.identity);
        }
        else if(random == dropRate - 1)
        {
            Instantiate(healthPack, position, Quaternion.identity);
        }
    }

    public bool GetIsHit()
    {
        return isHit;
    }

}
