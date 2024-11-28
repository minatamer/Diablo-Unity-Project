using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossController : MonoBehaviour
{
    public int phase = 1; 

    public int maxHealthPhase1 = 50;
    public int maxHealthPhase2 = 50;
    public int shieldHealth = 50;

    private int currentHealth;
    private int currentShieldHealth;
    private bool shieldActive;
    public bool auraActive = false;

    public float shieldRegenDelay = 10f;
    private float shieldRegenTimer;

    public Transform arenaCenter;
    private Vector3[] minionSpawnPoints;

    Animator lilithAnimator;

    public PlayableDirector spikesTimelineDirector;

    private bool Phase2CoroutineStarted = false;

    public GameObject auraSphere;

    public GameObject minion;

    public int minionsAlive = 0;


    private void Start()
    {
        lilithAnimator = GetComponent<Animator>();
        currentHealth = maxHealthPhase1;
        shieldActive = false;
        auraActive = false;
        minionSpawnPoints = new Vector3[3];
        GenerateRandomSpawnPoints();
        StartCoroutine(HandlePhase1Coroutine());

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

    private void Update()
    {
        if (!auraActive)
        {
            auraSphere.SetActive(false);
        }

        if (phase == 1)
        {
            if (currentHealth <= 0)
            {
                //GO TO PHASE 2 
                phase = 2;
                currentHealth = maxHealthPhase2;
                currentShieldHealth = shieldHealth;
                shieldActive = true;
                shieldRegenTimer = 0f;
                lilithAnimator.SetTrigger("Phase2");
                return;
            }


        }
        else if (phase == 2)
        {
            if (!Phase2CoroutineStarted)
            {
                Phase2CoroutineStarted = true;
                StartCoroutine(HandlePhase2Coroutine());
            }
            if (currentHealth <= 0)
            {
                lilithAnimator.SetTrigger("Die");
                //Destroy(gameObject); 
                return;
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

        }
    }

    private IEnumerator HandlePhase2Coroutine()
    {
        while (phase == 2 && currentHealth > 0) 
        {
            spikesTimelineDirector.time = 2f;
            spikesTimelineDirector.Stop();
            HandlePhase2(); 
            yield return new WaitForSeconds(10f); 
        }
    }

    private void HandlePhase2()
    {
        if (!auraActive )
        {
           //TODO: Reflect the Wanderer's damage and deal additional damage
           //TODO: Deactivate aura after one use
            lilithAnimator.SetTrigger("Cast");
           auraActive = true;
           auraSphere.SetActive(true);  
        }
        else
        {
           //TODO: Raise spikes from the ground and damage the Wanderer
           spikesTimelineDirector.Play();
            Debug.Log("setting spikes trigger");
           lilithAnimator.SetTrigger("Spikes");
        }
    }

    private IEnumerator HandlePhase1Coroutine()
    {
        while (phase == 1 && currentHealth > 0)
        {
            HandlePhase1();
            yield return new WaitForSeconds(10f);
        }
    }

    private void HandlePhase1()
    {
        if (minionsAlive == 0)
        {
            minionsAlive = 3;
            lilithAnimator.SetTrigger("Summon");
            for (int i = 0; i < minionSpawnPoints.Length; i++)
            {
                GameObject newMinion = Instantiate(minion, minionSpawnPoints[i], Quaternion.identity);

            }
        }
        else
        {
            lilithAnimator.SetTrigger("Divebomb");
            //TODO: Divebomb attack animation and damage logic
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

    //    currentHealth -= damage;
    //}





}
