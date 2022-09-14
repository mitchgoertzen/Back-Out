using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] private Slider healthBar;

    private PlayerAttributes playerHealth;

    [SerializeField] private Color fullHealthColour;
    [SerializeField] private Color highHealthColour;
    [SerializeField] private Color midHealthColour;
    [SerializeField] private Color lowHealthColour;

    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttributes>();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = playerHealth.GetMaxHealth();
        healthBar.value = playerHealth.GetMaxHealth();
        healthBar.gameObject.transform.Find("Foreground").GetComponent<Image>().color = fullHealthColour;
    }

    public void SetHealth(float health)
    {
        healthBar.value = health;
        float difference = health;
        float intensity;
        Color startColour = midHealthColour;
        Color endColour = lowHealthColour;


        switch (health)
        {
            case var _ when health >= 66:
                {
                    difference = health - 66;
                    startColour = fullHealthColour;
                    endColour = highHealthColour;
                }
                break;

            case var _ when health >= 33:
                {
                    difference = health - 33;
                    startColour = highHealthColour;
                    endColour = midHealthColour;
                }
                break;

            default:
                break;
        }

        intensity = (difference * 100) / 33;
        healthBar.gameObject.transform.Find("Foreground").GetComponent<Image>().color = Color.Lerp(startColour, endColour, 1f - (intensity / 100f));
    }
}
