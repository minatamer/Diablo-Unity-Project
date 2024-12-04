using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    // Start is called before the first frame update
    public int hp = 20;
    Animator animator ;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other){

        if(other.gameObject.CompareTag("Player")){
            
               animator.SetBool("Punch", true);
                gameController.Instance.healthPoints -= 5;

        }


    }
    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

}
