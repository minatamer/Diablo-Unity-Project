using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonController : MonoBehaviour
{
     public int hp = 40;
        private Animator animator ;
           private   NavMeshAgent agent ;

    // Start is called before the first frame update
    void Start()
    {
          animator = GetComponent<Animator>();
         agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
                
            //     float position1Z = 187f;
            //     float position2Z = 120f;
            //     Vector3 currentPosition = transform.position;

            //     // Toggle between the two positions
            //     if (currentPosition.z >= position1Z)
            //     {
            //         agent.SetDestination(new Vector3(transform.position.x, transform.position.y, position2Z));
            //     }
            //     else
            //     {
            //         agent.SetDestination(new Vector3(transform.position.x, transform.position.y, position1Z));
            //     }

            //    if( animator.GetCurrentAnimatorStateInfo(0).IsName("Great Sword Slash")|| animator.GetCurrentAnimatorStateInfo(0).IsName("Great Sword Slash 0")){
            //            gameController.Instance.healthPoints -= 10;
            //     }

            //     if( animator.GetCurrentAnimatorStateInfo(0).IsName("Goalie Throw")){
            //            gameController.Instance.healthPoints -= 15;
            //    }

        
    }
    private void OnTriggerEnter(Collider other){
        //   if(other.gameObject.CompareTag("Player")){
            
        //        animator.SetBool("attack", true);
            

        // }

    }



}
