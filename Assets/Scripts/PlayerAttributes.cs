using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerAttributes : MonoBehaviour
{

    [SerializeField] private bool testMode;

    [SerializeField] private float maxHealth;
    [SerializeField] private float healthAutoRegenDelay;
    [SerializeField] private float healthPackRegenSpeed;


    [SerializeField] private HealthBar healthBar;
    [SerializeField] private VapourCollect vapourBar;

    [SerializeField] private Image damageOverlay;

    [SerializeField] private AudioSource damageSound;

    private bool underAttack;
    private bool hasBeenHit;
    private bool healthPickedUp;
    private bool dead;

    private Color newOverlayColour;
    private Color maxOverlayColour;

    private Coroutine overlayCoroutine;

    private float currentHealth;
    private float collectedVapour;
    private float currentHealthRegenDelay;
    private float currentHealthPackAmount;
    private float totalHealthPackAmount;
    private float colourSpeed = 3f;
    private float startTime;

    private Image deathOverlay;

    void Start()
    {
        deathOverlay = transform.Find("UI/Canvas/death").GetComponent<Image>();
        currentHealth = maxHealth;
        currentHealthRegenDelay = healthAutoRegenDelay;
        newOverlayColour = damageOverlay.color;
        maxOverlayColour = newOverlayColour;
    }

    void Update()
    {
        if (!dead)
        {
            if (hasBeenHit)
            {
                float t = (Mathf.Sin(Time.time - startTime) * colourSpeed);
                Color temp = Color.Lerp(damageOverlay.color, maxOverlayColour, t);
                temp.a = Round(temp.a, 2);
                damageOverlay.color = temp;
            }
            else if (damageOverlay.color != newOverlayColour)
            {
                float t = (Mathf.Sin(Time.time - startTime) * (colourSpeed / 5));
                Color temp = Color.Lerp(damageOverlay.color, newOverlayColour, t);
                temp.a = Round(temp.a, 2);
                damageOverlay.color = temp;
            }

            if (collectedVapour > 0)
            {
                collectedVapour -= Time.deltaTime / 2f;
                if (collectedVapour < 0)
                    collectedVapour = 0;
                vapourBar.SetLevel(collectedVapour);
            }
            if (underAttack)
            {
                currentHealthRegenDelay -= Time.deltaTime;
                if (currentHealthRegenDelay <= 0f)
                {
                    underAttack = false;
                }
            }
            else
            {
                if (currentHealth < maxHealth)
                {
                    currentHealth += 0.05f;
                    SetHealthBar();
                    damageOverlay.color = newOverlayColour;
                    if (currentHealth > maxHealth)
                        currentHealth = maxHealth;
                }
            }

            if (healthPickedUp)
            {
                if ((currentHealthPackAmount > 0) && currentHealth < maxHealth)
                {
                    float currentRegenAmount = totalHealthPackAmount * (healthPackRegenSpeed / 100f);
                    currentHealth += currentRegenAmount;
                    currentHealthPackAmount -= currentRegenAmount;
                    SetHealthBar();
                    damageOverlay.color = newOverlayColour;
                }
                else
                {
                    totalHealthPackAmount = 0;
                    healthPickedUp = false;
                    if (currentHealth > maxHealth)
                        currentHealth = maxHealth;
                }
            }
        }
        else
        {
            float t = (Mathf.Sin(Time.time - startTime) * colourSpeed/15f);
            Color temp = damageOverlay.color;
            temp.a = .98f;
            deathOverlay.color = Color.Lerp(deathOverlay.color, temp, t);
        }
    }

    public void TakeDamage(int damage, bool isProj)
    {
        if (!testMode)
        {
            if(isProj)
                damageSound.PlayOneShot(damageSound.clip);
            underAttack = true;
            currentHealthRegenDelay = healthAutoRegenDelay;
            currentHealth -= damage;
            SetHealthBar();

            if (overlayCoroutine != null)
            {
                StopCoroutine(overlayCoroutine);
            }

            maxOverlayColour.a = Mathf.Min(.5f, damageOverlay.color.a) + (damage * 2f) / 100f;
            overlayCoroutine = StartCoroutine(ShowDamageOverlay());

            if (currentHealth <= 0)
            {
                currentHealth = 0;
               StartCoroutine(Die());
            }
        }
    }

    IEnumerator Die()
    {
        dead = true;
        GetComponent<Motion>().enabled = false;
        GetComponent<Weapon>().enabled = false;
        transform.Find("Main Camera/Weapon").gameObject.SetActive(false);
        transform.Find("Main Camera").GetComponent<Look>().enabled = false;

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public string GetHealth()
    {
        return currentHealth.ToString();
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool AddHealth(int amount)
    {
        if (currentHealth == maxHealth)
            return false;

        totalHealthPackAmount += amount;
        currentHealthPackAmount = totalHealthPackAmount;
        healthPickedUp = true;
        return true;
    }

    private void SetHealthBar()
    {
        float tempA = Mathf.Max(0, .5f - (currentHealth / 100f)) * 1f;

        newOverlayColour.a = tempA;

        healthBar.SetHealth(currentHealth);
    }

    public float GetVapourLevel()
    {
        return collectedVapour;
    }

    public void collectVapour(float amount)
    {
        collectedVapour += amount;
        vapourBar.SetLevel(collectedVapour);
    }

    IEnumerator ShowDamageOverlay()
    {
        hasBeenHit = true;
        startTime = Time.time;
        yield return new WaitForSeconds(.5f);
        hasBeenHit = false;
        startTime = Time.time;
    }

    private static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public bool IsDead()
    {
        return dead;
    }
}
