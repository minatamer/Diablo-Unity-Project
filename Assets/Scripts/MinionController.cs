using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class MinionController : MonoBehaviour
{
    // Start is called before the first frame update
    public int hp;
    Animator animator ;
    public Image healthBarImage;
    public NavMeshAgent agent;
    private AudioSource audioSource;
    public AudioClip deathSound;

    void Start()
    {
        hp = 20;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not already attached
        }
    }


    private void OnTriggerEnter(Collider other){

        if (other.gameObject.CompareTag("Player") && gameController.Instance.healthPoints > 0)
        {
            if (gameController.Instance.bossLevel == false)
            {
                GameObject campOne = GameObject.FindWithTag("CampOne");
                GameObject campTwo = GameObject.FindWithTag("CampTwo");
                GameObject campThree = GameObject.FindWithTag("CampThree");

                CampController campController = campOne.gameObject.GetComponent<CampController>();
                CampControllerTwo campTwoController = campTwo.gameObject.GetComponent<CampControllerTwo>();
                CampControllerThree campThreeController = campThree.gameObject.GetComponent<CampControllerThree>();

                if (campController.aggressiveMinions.Contains(gameObject) || campTwoController.aggressiveMinions.Contains(gameObject) || campThreeController.aggressiveMinions.Contains(gameObject))
                {
                    animator.SetBool("Punch", true);
                }
            }
            else
            {
                animator.SetBool("Punch", true);
            }
                

                

        }



    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            if (gameController.Instance.bossLevel == false)
            {
                GameObject campOne = GameObject.FindWithTag("CampOne");
                GameObject campTwo = GameObject.FindWithTag("CampTwo");
                GameObject campThree = GameObject.FindWithTag("CampThree");

                CampController campController = campOne.gameObject.GetComponent<CampController>();
                CampControllerTwo campTwoController = campTwo.gameObject.GetComponent<CampControllerTwo>();
                CampControllerThree campThreeController = campThree.gameObject.GetComponent<CampControllerThree>();

                if (campController.aggressiveMinions.Contains(gameObject) || campTwoController.aggressiveMinions.Contains(gameObject) || campThreeController.aggressiveMinions.Contains(gameObject))
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Mutant Punch") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.90f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.92f)
                    {
                        if (PlayerPrefs.GetString("SelectedCharacter") == "Barbarian")
                        {//barbarian


                            BarbarianAnimatorController barbarianScript = other.gameObject.GetComponent<BarbarianAnimatorController>();
                            if (barbarianScript.shield == false)
                            {
                                if (gameController.Instance.invincibility == false)
                                {
                                    gameController.Instance.healthPoints -= 5;
                                barbarianScript.getHit();

                                }
                            }
                        }
                        else
                        {//arisa
                            if (gameController.Instance.invincibility == false)
                            {
                                gameController.Instance.healthPoints -= 5;
                            sor_script SorScript = other.gameObject.GetComponent<sor_script>();
                            SorScript.getHit();

                            }
                        }
                    }
                }
            }
            else
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Mutant Punch") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.90f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.92f)
                {
                    if (PlayerPrefs.GetString("SelectedCharacter") == "Barbarian")
                    {//barbarian


                        BarbarianAnimatorController barbarianScript = other.gameObject.GetComponent<BarbarianAnimatorController>();
                        if (barbarianScript.shield == false)
                        {
                            gameController.Instance.healthPoints -= 5;
                            //Animator player = other.gameObject.GetComponent<Animator>();
                            //player.SetTrigger("hit");
                            barbarianScript.getHit();
                        }
                    }
                    else
                    {//arisa
                        gameController.Instance.healthPoints -= 5;
                        sor_script SorScript = other.gameObject.GetComponent<sor_script>();
                        //Animator player = other.gameObject.GetComponent<Animator>();
                        //player.SetTrigger("hit");
                        SorScript.getHit();
                    }
                }
            }


            

                



        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("Punch", false);

        }


    }
    public void getHit()
    {
        if (!animator.GetCurrentAnimatorStateInfo(1).IsName("Hurt"))
        {
            animator.SetTrigger("Hit");
            //Debug.Log("hurt animation");
        }
        
    }
    public void UpdateHealthBar()
    {
            healthBarImage.fillAmount =  ((float)hp / 20);
    }
    // Update is called once per frame
    void Update()
    {

        if (gameController.Instance.healthPoints <= 0)
        {
            animator.SetBool("Punch", false);
        }

        if (hp <= 0)
        {

            agent.SetDestination(agent.transform.position);
            animator.SetTrigger("Die");
           
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {

            if (deathSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(deathSound);

                //Destroy(gameObject);
            }
        }

        if (agent.remainingDistance <= 2f && !agent.pathPending)
        {
            // Agent has reached the target
            animator.SetBool("Run", false);
            animator.SetBool("Walk", false);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.70f)
        {
            gameController.Instance.xp += 10;
            Destroy(gameObject);
        }





    }

}
