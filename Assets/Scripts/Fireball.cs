using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    // Start is called before the first frame update
      private sor_script sorScript;
    void Start()
    {
          sorScript = FindObjectOfType<sor_script>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


   void OnTriggerEnter(Collider other)
{
    if (other.gameObject.CompareTag("Minion")  || other.gameObject.CompareTag("Minion2")|| other.gameObject.CompareTag("Minion3")) 
    {
        sorScript.SetFireBallDestroyed(true);
        sorScript.SetCurrentFireball(null);
        MinionController enemyScript = other.gameObject.GetComponent<MinionController>();//update with ennemy script name
        enemyScript.hp -= 5; //5
        enemyScript.UpdateHealthBar();
        Destroy(gameObject); // Destroy the fireball
     }

    if (other.gameObject.CompareTag("Demon") ||other.gameObject.CompareTag("Demon11")||other.gameObject.CompareTag("Demon12"))
    {
        sorScript.SetFireBallDestroyed(true);
        sorScript.SetCurrentFireball(null);
        DemonController enemyScript = other.gameObject.GetComponent<DemonController>();//update with ennemy script name
        enemyScript.hp -= 5; //5
        enemyScript.UpdateHealthBar();
        Destroy(gameObject); // Destroy the fireball
    }

        if (other.gameObject.CompareTag("Boss"))
        {
            sorScript.SetFireBallDestroyed(true);
            sorScript.SetCurrentFireball(null);
            BossController enemyScript = other.gameObject.GetComponent<BossController>();//update with ennemy script name
            if (enemyScript.auraActive)
            {
                gameController.Instance.healthPoints -= 20;
                enemyScript.auraActive = false;
            }
            else
            {
                if (enemyScript.currentShieldHealth > 0)
                {
                    enemyScript.currentShieldHealth -= 5;
                    if (enemyScript.currentShieldHealth < 0)
                    {
                        int damage = enemyScript.currentShieldHealth;
                        enemyScript.currentShieldHealth = 0;
                        enemyScript.hp += damage;

                    }
                }
                else
                {
                    enemyScript.hp -= 5;
                }
            }

            //enemyScript.UpdateHealthBar();
            Destroy(gameObject); // Destroy the fireball
        }
    }

}
