using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private float maxSpawnDepth;
    [SerializeField] private float spawnDelay;

    [SerializeField] private GameObject enemy;

    [SerializeField] private int enemySpawnAmount;
    [SerializeField] private int spawnResetTime;

    private bool activated = false;

    private float width;
    private float currentTime = 1f;

    private int enemyCount;

    void Start()
    {
        width = GetComponent<Collider>().bounds.size.x;
    }

    private void Update()
    {
        if (activated)
        {
            currentTime += Time.deltaTime;
            if ((int)currentTime % spawnResetTime == 0)
            {
                enemyCount = 0;
                currentTime = 1f;
                activated = false;
            }
        }
    }
    

    IEnumerator EnemyDrop()
    {
       
        activated = true;
        while (enemyCount < enemySpawnAmount)
        {

            Vector3 position = transform.position - (transform.forward * Random.Range(1,maxSpawnDepth));
            position.x = Random.Range(transform.position.x - width/2, transform.position.x + width/2);

            RaycastHit hit;
            if (Physics.Raycast(position, -Vector3.up, out hit))
            {
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(hit.point, out navHit, 2f, NavMesh.AllAreas))
                {
                    yield return new WaitForSeconds(spawnDelay);
                    Instantiate(enemy, hit.point, Quaternion.identity);

                    enemyCount++;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!activated && other.gameObject.layer == 9)
        {
            StartCoroutine(EnemyDrop());
        }
    }
}
