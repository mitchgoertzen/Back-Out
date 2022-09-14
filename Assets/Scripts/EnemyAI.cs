
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private AudioSource sound1;
    [SerializeField] private AudioSource sound2;

    [SerializeField] private float patrolSpeed;
    [SerializeField] private float chasingSpeed;
    [SerializeField] private float attackingSpeed;
    [SerializeField] private float walkPointRange;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float attackForce;
    [SerializeField] private float attackRange;

    [SerializeField] private GameObject projectile;

    [SerializeField] private int attackStrength;

    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    [SerializeField] private NavMeshAgent agent;

    private bool playerInAttackRange;
    private bool teleporting;
    private bool walkPointSet;
    private bool alreadyAttacked;

    private float soundWaitTime;
    private float currentWaitTime;
    private float lookTime;

    private GameObject player;

    private Health health;

    private int visibilty;

    private Random rnd = new Random();

    private Renderer rend;

    private Vector3 direction;
    private Vector3 walkPoint;

    private VisibilityCheck visibiltyCheck;



    private void Awake()
    {
        rend = transform.Find("Anchor/Design").GetComponent<Renderer>();
        lookTime = 0;
        visibiltyCheck = GetComponent<VisibilityCheck>();
        player = GameObject.FindGameObjectWithTag("Player");
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chasingSpeed;
        soundWaitTime = Random.Range(10, 25);
    }

    private void Update()
    {
        if(currentWaitTime > soundWaitTime)
        {
            soundWaitTime = Random.Range(10, 25);
            int rand = Random.Range(0, 2);
            currentWaitTime = 0;
            if(rand == 0)
            {
                sound1.Play();
            }
            else if(rand == 1)
            {
                sound2.Play();
            }
        }
        else
        {
            currentWaitTime += Time.deltaTime;
        }
        if (player.GetComponent<PlayerAttributes>().IsDead())
        {
            Patroling();
        }
        else
        {
            visibilty = visibiltyCheck.IsVisible();

            if (!teleporting && player != null)
            {
                playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

                switch (visibilty)
                {
                    //blocked
                    case 0:
                        {
                            ChasePlayer(chasingSpeed);
                        }
                        break;
                    //player is looking at enemy
                    case 1:
                        {
                            if (playerInAttackRange)
                            {
                                AttackPlayer();
                            }
                            else
                            {
                                agent.SetDestination(transform.position);
                                lookTime += Time.deltaTime;
                                if (lookTime >= 2f)
                                {
                                    lookTime = 0;
                                    StartCoroutine(Teleport());
                                }
                            }
                        }
                        break;
                    //player is NOT looking at enemy
                    case 2:
                        {
                            if (playerInAttackRange)
                            {
                                AttackPlayer();
                            }
                            else
                            {
                                ChasePlayer(chasingSpeed * 2f);
                            }
                        }
                        break;
                }
            }
        }

    }

    private void Patroling()
    {

        agent.speed = patrolSpeed;
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        NavMeshHit navHit;
        while (!NavMesh.SamplePosition(walkPoint, out navHit, 1f, NavMesh.AllAreas))
        {

            randomZ = Random.Range(-walkPointRange, walkPointRange);
            randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        }

        walkPointSet = true;
    }

    private void ChasePlayer(float speed)
    {

        lookTime = 0;
        agent.SetDestination(player.transform.position);
        agent.speed = speed * (player.GetComponent<Motion>().GetSpeed()/2);
    }

    private void AttackPlayer()
    {

        lookTime = 0;

        LookAtPlayer();

        float distance = direction.magnitude;
        agent.speed = attackingSpeed * (player.GetComponent<Motion>().GetSpeed() / 2);

        if (!health.GetIsHit() && direction.magnitude > 10f)
            agent.SetDestination(player.transform.position);
        else
            agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            GameObject rb = Instantiate(projectile, transform.Find("Anchor/Projectile Point").position, Quaternion.identity);
            rb.GetComponent<Projectile>().setStrength(attackStrength);
            rb.GetComponent<Rigidbody>().AddForce(direction * attackForce, ForceMode.Impulse);
            rb.GetComponent<Rigidbody>().AddForce(transform.up * Mathf.Max(1f, distance / 2), ForceMode.Impulse);
            Destroy(rb, 3f);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void LookAtPlayer()
    {
        direction = (player.transform.position - transform.position);
        var newRotation = Quaternion.LookRotation(direction, Vector3.forward);
        newRotation.x = 0f;
        newRotation.z = 0f;
        transform.rotation = newRotation;
    }

    IEnumerator Teleport()
    {
        SetPosition();
        while (!agent.isOnNavMesh)
        {
            SetPosition();
        }
        LookAtPlayer();
        if (agent.isOnNavMesh)
            agent.SetDestination(transform.position);
        rend.enabled = false;
        teleporting = true;
        float time = Random.Range(0.25f, 2f);
        yield return new WaitForSeconds(time);
        teleporting = false;

        rend.enabled = true;
    }

    private void SetPosition()
    {
        Vector3 randomOffset = player.transform.forward * Random.Range(5, 15);
        randomOffset.x += Random.Range(-20, 20);
        Vector3 targetPos = player.transform.position - randomOffset;
        transform.position = targetPos;
    }

}
