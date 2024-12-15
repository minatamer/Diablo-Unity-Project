using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RingOfFlames : MonoBehaviour
{
    private HashSet<GameObject> enemiesWithinRing;
    private bool isChecking = false; // Flag to control 1-second interval
    public float ringRadius = 5f;   // Radius of the ring
    private Dictionary<GameObject, float> damageTimers = new Dictionary<GameObject, float>();
    public float damageInterval = 1.0f; // Damage interval in seconds

    void Start()
    {
        enemiesWithinRing = new HashSet<GameObject>();
    }

    //void Update()
    //{
    //    if (!isChecking)
    //    {
    //        StartCoroutine(ApplyDamageToEnemiesInsideRing());
    //    }
    //}

    private void OnTriggerEnter(Collider enemyObject)
    {
        if (enemyObject.CompareTag("Minion") || enemyObject.CompareTag("Minion2") || enemyObject.CompareTag("Minion3") || enemyObject.CompareTag("Demon") || enemyObject.CompareTag("Demon11") || enemyObject.CompareTag("Demon12") || enemyObject.CompareTag("Boss"))
        {
            //StartCoroutine(ApplyDamageToEnemiesInsideRing());

                ApplyDamage(enemyObject.gameObject, 10); // Apply initial damage
            damageTimers[enemyObject.gameObject] = Time.time;


        }
           
    }

    private void OnTriggerStay(Collider enemyObject)
    {
        if (enemyObject.CompareTag("Minion") || enemyObject.CompareTag("Minion2") ||
            enemyObject.CompareTag("Minion3") || enemyObject.CompareTag("Demon") ||
            enemyObject.CompareTag("Demon11") || enemyObject.CompareTag("Demon12") ||
            enemyObject.CompareTag("Boss"))
        {
            GameObject enemy = enemyObject.gameObject;

            // Check if this enemy is in the dictionary
            if (damageTimers.TryGetValue(enemy, out float lastDamageTime))
            {
                // If enough time has passed, apply damage
                if (Time.time - lastDamageTime >= damageInterval)
                {
                    ApplyDamage(enemy, 2); // Apply damage
                    damageTimers[enemy] = Time.time; // Update last damage time
                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove the enemy from the dictionary when it leaves the trigger
        if (damageTimers.ContainsKey(other.gameObject))
        {
            damageTimers.Remove(other.gameObject);
        }
    }

    private IEnumerator ApplyDamageToEnemiesInsideRing()
    {
        isChecking = true;

        // Detect all colliders within the ring radius
        Collider[] allColliders = Physics.OverlapSphere(transform.position, ringRadius);

        foreach (Collider collider in allColliders)
        {
            GameObject enemyObject = collider.gameObject;

            // Check if the collider belongs to a valid enemy
            if (enemyObject.CompareTag("Minion") || enemyObject.CompareTag("Minion2") || enemyObject.CompareTag("Minion3") || enemyObject.CompareTag("Demon") || enemyObject.CompareTag("Demon11") || enemyObject.CompareTag("Demon12") || enemyObject.CompareTag("Boss"))
            {
                Debug.Log("ENEMY RING DETECTED");
                if (enemiesWithinRing.Contains(enemyObject))
                {
                    ApplyDamage(enemyObject, 2); // Apply continuous damage
                }
                else
                {
                    ApplyDamage(enemyObject, 10); // Apply initial damage
                    enemiesWithinRing.Add(enemyObject);
                }
            }
        }

        yield return new WaitForSeconds(1f);
        isChecking = false;
    }


    private void ApplyDamage(GameObject enemyObject, int damage)
    {
        if (enemyObject.CompareTag("Minion") || enemyObject.CompareTag("Minion2") || enemyObject.CompareTag("Minion3"))
        {
            MinionController enemyScript = enemyObject.GetComponent<MinionController>();
            enemyScript.hp -= damage;
            enemyScript.getHit();
            enemyScript.UpdateHealthBar();
        }
        else if (enemyObject.CompareTag("Demon") || enemyObject.CompareTag("Demon11") || enemyObject.CompareTag("Demon12"))
        {
            DemonController enemyScript = enemyObject.GetComponent<DemonController>();
            enemyScript.hp -= damage;
            enemyScript.getHit();
            enemyScript.UpdateHealthBar();
        }
        else if (enemyObject.CompareTag("Boss"))
        {
            BossController enemyScript = enemyObject.GetComponent<BossController>();

            if (enemyScript.minions.Count > 0)
            {
                Debug.Log(""); //do nothing
            }
            else if (enemyScript.auraActive)
            {
                int auraDamage = damage + 15;
                gameController.Instance.healthPoints -= auraDamage;
                enemyScript.auraActive = false;
            }
            else
            {
                if (enemyScript.currentShieldHealth > 0)
                {
                    enemyScript.currentShieldHealth -= damage;
                    if (enemyScript.currentShieldHealth < 0)
                    {
                        int damageAfterShield = enemyScript.currentShieldHealth;
                        enemyScript.currentShieldHealth = 0;
                        enemyScript.hp += damageAfterShield;

                    }
                    enemyScript.UpdateShieldBar();
                }
                else
                {
                    enemyScript.hp -= damage;
                    enemyScript.getHit();
                    enemyScript.UpdateHealthBar();
                }
            }

            enemyScript.UpdateHealthBar();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the ring radius in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ringRadius);
    }
}
