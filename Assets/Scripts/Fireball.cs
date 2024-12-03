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
    if (other.gameObject.CompareTag("Minion")) 
    {
        Debug.Log("Fireball hit enemy");
        sorScript.SetFireBallDestroyed(true);
        sorScript.SetCurrentFireball(null);
        MinionController enemyScript = other.gameObject.GetComponent<MinionController>();//update with ennemy script name
        enemyScript.hp -= 10; //10
       
        Destroy(gameObject); // Destroy the fireball
     }
}

}
