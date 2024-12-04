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
    if (other.gameObject.CompareTag("Minion")  || other.gameObject.CompareTag("Minion2")) 
    {
        sorScript.SetFireBallDestroyed(true);
        sorScript.SetCurrentFireball(null);
        MinionController enemyScript = other.gameObject.GetComponent<MinionController>();//update with ennemy script name
        enemyScript.hp -= 5; //5
        Destroy(gameObject); // Destroy the fireball
     }

    if (other.gameObject.CompareTag("Demon"))
    {
        sorScript.SetFireBallDestroyed(true);
        sorScript.SetCurrentFireball(null);
        DemonController enemyScript = other.gameObject.GetComponent<DemonController>();//update with ennemy script name
        enemyScript.hp -= 5; //5
        Destroy(gameObject); // Destroy the fireball
    }
    }

}
