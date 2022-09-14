using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float radius = 2f;

    private GameObject player;

    private int strength;
    private int i = 0;

    private List<int> hitLayers = new List<int> { 0, 8, 16 };

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public void setStrength(int s)
    {
        strength = s;
    }

    public int getStrength()
    {
        return strength;
    }

    private void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.layer == 9)
        {
            player.GetComponent<PlayerAttributes>().TakeDamage(strength, true);
            Destroy(gameObject);
        }
        else if (hitLayers.Contains(collider.gameObject.layer))
        {
            Destroy(gameObject);
        }
    }
}

