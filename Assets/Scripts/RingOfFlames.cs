using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RingOfFlames : MonoBehaviour
{
     private HashSet<GameObject> enemiesWithinRing;

    // Start is called before the first frame update
        private bool isChecking = false; // Flag to control 1-second interval

    void Start()
    {
         enemiesWithinRing = new HashSet<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     private void OnTriggerStay(Collider other)
    {
        if(isChecking == false){
        // if (other.CompareTag("Enemy") ) //this label should be adjusted 
        // {
            StartCoroutine(CheckLogicEverySecond(other));
       // }
        }
    }

    private IEnumerator CheckLogicEverySecond(Collider enemy)
    {
        isChecking = true;
        Debug.Log($"Enemy detected:");
        GameObject enemyObject = enemy.gameObject;
        if(enemiesWithinRing.Contains(enemyObject)){

            if(enemyObject.tag.Contains("Minion")){
                MinionController enemyScript = enemyObject.GetComponent<MinionController>();
                enemyScript.hp -= 2;
                enemyScript.UpdateHealthBar();
            }
            if(enemyObject.tag.Contains("Demon")){
                DemonController enemyScript = enemyObject.GetComponent<DemonController>();
                enemyScript.hp -= 2;
                enemyScript.UpdateHealthBar();
            }
            if (enemyObject.tag.Contains("Boss"))
            {
                BossController enemyScript = enemyObject.GetComponent<BossController>();
                enemyScript.hp -= 2;
                //enemyScript.UpdateHealthBar();
            }


        }
        else {

            if(enemyObject.tag.Contains("Minion")){
             
                MinionController enemyScript = enemyObject.GetComponent<MinionController>();
                enemyScript.hp -= 10;
                enemyScript.UpdateHealthBar();
            }
            if(enemyObject.tag.Contains("Demon")){
                DemonController enemyScript = enemyObject.GetComponent<DemonController>();
                enemyScript.hp -= 10;
                enemyScript.UpdateHealthBar();
            }
            if (enemyObject.tag.Contains("Boss"))
            {
                BossController enemyScript = enemyObject.GetComponent<BossController>();
                enemyScript.hp -= 10;
                //enemyScript.UpdateHealthBar();
            }
            enemiesWithinRing.Add(enemyObject);
        }
        yield return new WaitForSeconds(1f);
        isChecking = false; 
    }

   
}
