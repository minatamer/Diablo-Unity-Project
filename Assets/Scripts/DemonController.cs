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
       if(gameObject.tag == "Demon"){


        float position1Z = 187f;
        float position2Z = 160f;
        Vector3 currentPosition = transform.position;

        // Toggle between the two positions
        if ((position1Z - 0.5f) <= currentPosition.z && currentPosition.z  <= (position1Z + 0.5f))
        {

            agent.SetDestination(new Vector3(transform.position.x, transform.position.y, position2Z));
        }
        if ((position2Z - 0.5f) <= currentPosition.z && currentPosition.z <= (position2Z + 0.5f))
        {
            agent.SetDestination(new Vector3(transform.position.x, transform.position.y, position1Z));
        }
       }


       if(gameObject.tag == "Demon11"){//camp 3 patrolling along z axis 

        float position1Z = 98.4f;
        float position2Z = 62f;
        Vector3 currentPosition = transform.position;

        // Toggle between the two positions
        if ((position1Z - 0.5f) <= currentPosition.z && currentPosition.z  <= (position1Z + 0.5f))
        {

            agent.SetDestination(new Vector3(transform.position.x, transform.position.y, position2Z));
        }
        if ((position2Z - 0.5f) <= currentPosition.z && currentPosition.z <= (position2Z + 0.5f))
        {
            agent.SetDestination(new Vector3(transform.position.x, transform.position.y, position1Z));
        }

       }
        if(gameObject.tag == "Demon12"){ //camp 3 patrolling along x axis 


        float position1X = 215f;
        float position2X= 255f;
        Vector3 currentPosition = transform.position;

        // Toggle between the two positions
        if ((position1X - 0.5f) <= currentPosition.x && currentPosition.x   <= (position1X + 0.5f))
        {

            agent.SetDestination(new Vector3(position2X, transform.position.y,  transform.position.z));
        }
        if ((position2X - 0.5f) <= currentPosition.x && currentPosition.x <= (position2X + 0.5f))
        {
            agent.SetDestination(new Vector3(position1X, transform.position.y, transform.position.z));
        }


       }


        if (hp <= 0)
        {
            Destroy(gameObject);
        }


    }
    private void OnTriggerEnter(Collider other){


        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("attack", true);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Great Sword Slash") || animator.GetCurrentAnimatorStateInfo(0).IsName("Great Sword Slash 0"))
            {
                if(PlayerPrefs.GetString("SelectedCharacter") == "Barbarian" ){//barbarian shield
                 BarbarianAnimatorController barbarianScript = other.gameObject.GetComponent<BarbarianAnimatorController>();
                    if(barbarianScript.shield == false){
                        gameController.Instance.healthPoints -= 10;
                        Animator player = other.gameObject.GetComponent<Animator>();
                        player.SetTrigger("hit");
                    }

                }
                else{//arisa
                gameController.Instance.healthPoints -= 10;
                 Animator player = other.gameObject.GetComponent<Animator>();
                 player.SetTrigger("hit");
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goalie Throw"))
            {
                if(PlayerPrefs.GetString("SelectedCharacter") == "Barbarian" ){//barbarian
                    BarbarianAnimatorController barbarianScript = other.gameObject.GetComponent<BarbarianAnimatorController>();
                    if(barbarianScript.shield == false){ //barbarian
                       gameController.Instance.healthPoints -= 15;
                        Animator player = other.gameObject.GetComponent<Animator>();
                        player.SetTrigger("hit");
                    }

                }
                else{//arisa
                gameController.Instance.healthPoints -= 15;
                 Animator player = other.gameObject.GetComponent<Animator>();
                 player.SetTrigger("hit");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("attack", false);
        }
        
    }




}
