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
    if (other.gameObject.CompareTag("test")) // Check for "enemy" tag
    {
        Debug.Log("Fireball hit enemy");
        sorScript.SetFireBallDestroyed(true);
        sorScript.SetCurrentFireball(null);
        Destroy(gameObject); // Destroy the fireball
     }
}

}
