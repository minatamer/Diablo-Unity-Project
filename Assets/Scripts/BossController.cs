using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using static UnityEngine.GraphicsBuffer;

public class BossController : MonoBehaviour
{
    public int phase = 0; 

    public int shieldHealth = 50;

    public int hp = 50;
    public int currentShieldHealth;
    private bool shieldActive;
    public bool auraActive = false;

    public float shieldRegenDelay = 10f;
    private float shieldRegenTimer;

    public Transform arenaCenter;
    private Vector3[] minionSpawnPoints;

    Animator lilithAnimator;


    private bool Phase2CoroutineStarted = false;

    public GameObject auraSphere;

    public GameObject minion;

    public List<GameObject> minions = new List<GameObject>();

    public GameObject SpikePrefab;

    private bool FireSpikes = false;
    private bool RaiseSpikes = false;
    private GameObject spikeOne;
    private GameObject spikeTwo;
    private GameObject spikeThree;

    public GameObject shield;


    private void Start()
    {
        lilithAnimator = GetComponent<Animator>();
        shieldActive = false;
        auraActive = false;
        minionSpawnPoints = new Vector3[3];
        GenerateRandomSpawnPoints();
        //

    }
    private void GenerateRandomSpawnPoints()
    {
        for (int i = 0; i < minionSpawnPoints.Length; i++)
        {
            // Generate random x and z values between -3 and 3, keeping y as 0
            float x = Random.Range(-3f, 3f);
            float z = Random.Range(-3f, 3f);
            Vector3 spawnPosition = new Vector3(x, 0f, z);

            // Store the spawn point in the array
            minionSpawnPoints[i] = spawnPosition;
        }
    }
    void FireInStraightLine(GameObject spike)
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 direction = (new Vector3(player.transform.position.x, player.transform.position.y - 3, player.transform.position.z) - spike.transform.position).normalized;
        Rigidbody rb = spike.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Vector3 force = new Vector3(direction.x, direction.y, direction.z) * 2f;
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    private void Update()
    {
        GameObject target = GameObject.FindWithTag("clonedPlayer");
        if (target == null)
        {
            target = GameObject.FindWithTag("Player");
        }
        transform.LookAt(target.transform);
        if (!auraActive)
        {
            auraSphere.SetActive(false);
        }
        if (phase == 0)
        {
            if (hp < 50)
            {
                phase = 1;
                StartCoroutine(HandlePhase1Coroutine());
            }
        }

        if (phase == 1)
        {
            if (hp <= 0)
            {
                //GO TO PHASE 2 
                phase = 2;
                lilithAnimator.SetTrigger("Die");
                return;
            }
            else
            {
                GameObject player = GameObject.FindWithTag("clonedPlayer");
                if(player == null)
                {
                    player = GameObject.FindWithTag("Player");
                }
                
                for (int i= minions.Count-1 ; i >= 0 ; i--)
                {
                    if (minions[i] == null)
                    {
                        minions.RemoveAt(i);
                    }
                    else
                    {
                        NavMeshAgent agent = minions[i].GetComponent<NavMeshAgent>();
                        agent.SetDestination(player.transform.position);
                        agent.stoppingDistance = 1.5f;
                    }

                }

                float distance = Vector3.Distance(player.transform.position, transform.position);
                if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Divebomb"))
                {
                    //Debug.Log(lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Divebomb") && lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.65f && lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.651f && distance >= 1f && distance <= 15f)
                    {
                        gameController.Instance.healthPoints -= 20;
                    }
                }



            }


        }
        else if (phase == 2)
        {
            if (!Phase2CoroutineStarted)
            {
                Debug.Log("dakhalt el if");
                hp = 50;
                currentShieldHealth = 50;
                shieldActive = true;
                shieldRegenTimer = 0f;
                lilithAnimator.SetTrigger("Phase2");
                shield.SetActive(true);
                Phase2CoroutineStarted = true;
                StartCoroutine(HandlePhase2Coroutine());
            }
            if (hp <= 0)
            {
                lilithAnimator.SetTrigger("Die");
                //Destroy(gameObject); 
                return;
            }
            if (currentShieldHealth <=0)
            {
                shield.SetActive(false);

            }
            else
            {
                shield.SetActive(true);
            }

            if (!shieldActive && currentShieldHealth < shieldHealth)
            {
                shieldRegenTimer += Time.deltaTime;
                if (shieldRegenTimer >= shieldRegenDelay)
                {
                    currentShieldHealth = shieldHealth;
                    shieldActive = true;
                    shieldRegenTimer = 0f;
                }
            }

            if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spikes"))
            {
                //Debug.Log(lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spikes") )
                {

                    RaiseSpikes = true;

                }
            }
            if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Cast") )
            {
                auraActive = true;
                auraSphere.SetActive(true);
            }
                

            if (spikeOne != null && spikeOne.transform.position.y >= 5f)
            {
                RaiseSpikes = false;

                Rigidbody rb = spikeOne.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;

                Rigidbody rb2 = spikeTwo.GetComponent<Rigidbody>();
                rb2.velocity = Vector3.zero;


                Rigidbody rb3 = spikeThree.GetComponent<Rigidbody>();
                rb3.velocity = Vector3.zero;

                FireSpikes = true;
            }

                        if(spikeOne != null && spikeOne.transform.position.y >= 5f)
            {
                RaiseSpikes = false;

                Rigidbody rb = spikeOne.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;

                Rigidbody rb2 = spikeTwo.GetComponent<Rigidbody>();
                rb2.velocity = Vector3.zero;


                Rigidbody rb3 = spikeThree.GetComponent<Rigidbody>();
                rb3.velocity = Vector3.zero;

                FireSpikes = true;
            }

            if (RaiseSpikes)
            {
                if(spikeOne != null)
                {
                    Rigidbody rb = spikeOne.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.velocity = Vector3.up * 4.0f;
                }

                if (spikeTwo != null)
                {
                    Rigidbody rb2 = spikeTwo.GetComponent<Rigidbody>();
                    rb2.isKinematic = false;
                    rb2.velocity = Vector3.up * 4.0f;
                }



                if (spikeThree != null)
                {
                    Rigidbody rb3 = spikeThree.GetComponent<Rigidbody>();
                    rb3.isKinematic = false;
                    rb3.velocity = Vector3.up * 4.0f;
                }

            }

            if (FireSpikes)
            {
                GameObject boss = GameObject.FindWithTag("Boss");
                if (spikeOne != null)
                {
                    float distance = Vector3.Distance(spikeOne.transform.position, boss.transform.position);
                    if (distance > 20f)
                    {
                        Destroy(spikeOne);
                        Destroy(spikeTwo);
                        Destroy(spikeThree);
                        FireSpikes = false;
                    }
                    else
                    {
                        FireInStraightLine(spikeOne);
                        FireInStraightLine(spikeTwo);
                        FireInStraightLine(spikeThree);
                    }
                   
                }
            }


        }
    }

    private IEnumerator HandlePhase2Coroutine()
    {
        while (phase == 2 && hp > 0) 
        {
            HandlePhase2(); 
            yield return new WaitForSeconds(10f); 
        }
    }

    private void HandlePhase2()
    {
        if (!auraActive )
        {
            lilithAnimator.SetTrigger("Cast"); 
        }
        else
        {
         
            //Incase there are old spikes Destroy them
            if(spikeOne != null)
            {
                Destroy(spikeOne);
            }
            if (spikeTwo != null)
            {
                Destroy(spikeTwo);
            }
            if (spikeThree != null)
            {
                Destroy(spikeThree);
            }
            FireSpikes = false;
            lilithAnimator.SetTrigger("Spikes");
           spikeOne = Instantiate(SpikePrefab, new Vector3(transform.position.x + 1.5f , transform.position.y+0.5f , transform.position.z), Quaternion.Euler(70, 0, 0));
           spikeTwo = Instantiate(SpikePrefab, new Vector3(transform.position.x -1.5f, transform.position.y + 0.5f, transform.position.z), Quaternion.Euler(70, 0, 0));
           spikeThree = Instantiate(SpikePrefab, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z + 1.5f), Quaternion.Euler(70, 0, 0));
        }
    }

    private IEnumerator HandlePhase1Coroutine()
    {
        while (phase == 1 && hp > 0)
        {
            HandlePhase1();
            yield return new WaitForSeconds(20f);
        }
    }

    private void HandlePhase1()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (minions.Count == 0)
        {
            lilithAnimator.SetTrigger("Summon");
            for (int i = 0; i < minionSpawnPoints.Length; i++)
            {
                GameObject newMinion = Instantiate(minion, minionSpawnPoints[i], Quaternion.identity);
                minions.Add(newMinion);
                NavMeshAgent agent = newMinion.GetComponent<NavMeshAgent>();
                newMinion.tag = "Minion";
                agent.SetDestination(player.transform.position);
                agent.speed = 5.0f;

            }
        }
        else
        {
            lilithAnimator.SetTrigger("Divebomb");


        }  
    }

    //public void TakeDamage(int damage)
    //{
    //    if (phase == 2 && shieldActive)
    //    {
    //        currentShieldHealth -= damage;
    //        if (currentShieldHealth <= 0)
    //        {
    //            shieldActive = false;
    //            damage = -currentShieldHealth; // Carry over excess damage
    //        }
    //        else
    //        {
    //            damage = 0;
    //        }
    //    }

    //    hp -= damage;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

        }
    }





}
