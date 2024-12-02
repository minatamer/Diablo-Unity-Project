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
        //for every ennemy reduce its damage points by 10
        // then by 2 if stayed longer 
        GameObject enemyObject = enemy.gameObject;

        if(enemiesWithinRing.Contains(enemyObject)){

        // EnemyScript enemyScript = enemyObject.GetComponent<EnemyScript>();//update with ennemy script name
        // enemyScript.damagePoints -= 2;
            Debug.Log("decreased by 2 ");

        }
        else {
         // EnemyScript enemyScript = enemyObject.GetComponent<EnemyScript>();//update with ennemy script name
         // enemyScript.damagePoints -= 10;
            Debug.Log("decreased by 10 ");
            enemiesWithinRing.Add(enemyObject);

        }
        yield return new WaitForSeconds(1f);
        isChecking = false; 
    }

   
}
