using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;
using TMPro;


public class BossController : MonoBehaviour
{
    public int phase = 2; 

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
    public Image healthBarImage;
    public TMP_Text points_text ;

    public Image shieldBarImage;
    public TMP_Text points_shield_text ;





    private void Start()
    {
        lilithAnimator = GetComponent<Animator>();
        shieldActive = false;
        auraActive = false;
        minionSpawnPoints = new Vector3[3];
        GenerateRandomSpawnPoints();
        points_text.text = "50/50";
        shieldBarImage.gameObject.SetActive(false);
        points_shield_text.gameObject.SetActive(false);
       
      

    }

    public void getHit()
    {
        if (!lilithAnimator.GetCurrentAnimatorStateInfo(1).IsName("Hurt"))
        {
            lilithAnimator.SetTrigger("Hit");
        }

    }
    private void GenerateRandomSpawnPoints()
    {
        Vector3 spawnPosition1 = new Vector3(6, 0f, 6);
        minionSpawnPoints[0] = spawnPosition1;

        Vector3 spawnPosition2 = new Vector3(6, 0f, 5);
        minionSpawnPoints[1] = spawnPosition2;

        Vector3 spawnPosition3 = new Vector3(-4, 0f, -5);
        minionSpawnPoints[2] = spawnPosition3;
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
    public void UpdateHealthBar()
    {
            points_text.text = hp + "/50";
            healthBarImage.fillAmount =  ((float)hp / 50);
    }

    public void UpdateShieldBar()
    {
        points_shield_text.text = currentShieldHealth + "/50";
        shieldBarImage.fillAmount = ((float)currentShieldHealth / 50);
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
                        Animator animator = agent.GetComponent<Animator>();
                        animator.SetBool("Run", true);
                    }

                }

                float distance = Vector3.Distance(player.transform.position, transform.position);
                if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Divebomb"))
                {
                    //Debug.Log(lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Divebomb") && lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.65f && lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.651f && distance >= 1f && distance <= 15f)
                    {
                        if (PlayerPrefs.GetString("SelectedCharacter") == "Barbarian")
                        {//barbarian

                            BarbarianAnimatorController barbarianScript = player.gameObject.GetComponent<BarbarianAnimatorController>();
                            if (barbarianScript.shield == false)
                            {
                                gameController.Instance.healthPoints -= 20;
                                barbarianScript.getHit();
                            }
                        }
                        else
                        {//arisa
                            gameController.Instance.healthPoints -= 20;
                            sor_script SorScript = player.gameObject.GetComponent<sor_script>();
                            SorScript.getHit();
                        }
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
                 shieldBarImage.gameObject.SetActive(true);
                 points_shield_text.gameObject.SetActive(true);
                 points_shield_text.text = "50/50";
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
                 shieldBarImage.gameObject.SetActive(false);
                 points_shield_text.gameObject.SetActive(false);
                 shield.SetActive(false);

                shieldRegenTimer += Time.deltaTime;
                //Debug.Log(shieldRegenTimer);
                if (shieldRegenTimer >= shieldRegenDelay)
                {

                    currentShieldHealth = shieldHealth;
                    shieldActive = true;
                    shieldRegenTimer = 0f;
                    shieldBarImage.gameObject.SetActive(true);
                    points_shield_text.gameObject.SetActive(true);
                    points_shield_text.text = "50/50";
                }

            }
            else
            {
                 shieldBarImage.gameObject.SetActive(true);
                 points_shield_text.gameObject.SetActive(true);
                 shield.SetActive(true);
            }

            //if (!shieldActive && currentShieldHealth <= 0)
            //{

            //}

            if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spikes"))
            {
                //Debug.Log(lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spikes") )
                {

                    RaiseSpikes = true;

                }
            }
            if (lilithAnimator.GetCurrentAnimatorStateInfo(0).IsName("Cast") && lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.90f && lilithAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.92f)
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
        GameObject player = GameObject.FindWithTag("clonedPlayer");
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

        }
    }





}
