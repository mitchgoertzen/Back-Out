using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private int minLevel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            Debug.Log("collision");
            if (other.gameObject.GetComponent<PlayerAttributes>().GetVapourLevel() >= minLevel)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
    
}
