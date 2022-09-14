using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private AudioSource swing;
    [SerializeField] private AudioSource hit1;
    [SerializeField] private AudioSource hit2;

    private bool hasAttacked;

    private int currentDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (hasAttacked && other.gameObject.layer == 11)
        {
            other.gameObject.GetComponent<Health>().TakeDamage(currentDamage);
            int rand = Random.Range(0,2);
            if(rand == 0)
            {
                hit1.Play();
            }
            else
            {
                hit2.Play();
            }
        }
    }

    public void SetAttack(int damage, bool isAttacking)
    {
        if(isAttacking)
            swing.Play();
        currentDamage = damage;
        hasAttacked = isAttacking;
    }
}
