using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfFlames : MonoBehaviour
{
    private HashSet<GameObject> enemiesWithinRing;
    private bool isChecking = false; // Flag to control 1-second interval
    public float ringRadius = 5f;   // Radius of the ring

    void Start()
    {
        enemiesWithinRing = new HashSet<GameObject>();
    }

    void Update()
    {
        if (!isChecking)
        {
            StartCoroutine(ApplyDamageToEnemiesInsideRing());
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
            if (enemyObject.CompareTag("Minion") || enemyObject.CompareTag("Demon") || enemyObject.CompareTag("Boss"))
            {
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
        if (enemyObject.CompareTag("Minion"))
        {
            MinionController enemyScript = enemyObject.GetComponent<MinionController>();
            enemyScript.hp -= damage;
            enemyScript.getHit();
            enemyScript.UpdateHealthBar();
        }
        else if (enemyObject.CompareTag("Demon"))
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
