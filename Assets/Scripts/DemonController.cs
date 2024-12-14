using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


public class DemonController : MonoBehaviour
{
     public int hp = 40;
     private Animator animator ;
     private NavMeshAgent agent ;
    public GameObject grenade;
    public Transform shootingPoint;

    private GameObject currentGrenade;
    private Vector3 grenadeTarget;
    private float range;

    private float grenadeTime;

    public Image healthBarImage;
    private AudioSource audioSource;
    public AudioClip explosionSound;
    public AudioClip deathSound;

    // Start is called before the first frame update
    void Start()
    {
         animator = GetComponent<Animator>();
         agent = GetComponent<NavMeshAgent>();
        audioSource = gameObject.AddComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.Instance.healthPoints <= 0)
        {
            animator.SetBool("attack", false);
        }



        if (gameObject.tag == "Demon"){


            //float position1Z = 187f;
            //float position2Z = 160f;
            float position1Z = 256.5f;
            float position2Z = 227f;
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

            //float position1Z = 98.4f;
            //float position2Z = 62f;
            float position1Z = 168f;
            float position2Z = 136.4f;
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


            //float position1X = 215f;
            //float position2X = 255f;
            float position1X = 310f;
            float position2X = 340f;
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
            agent.SetDestination(agent.transform.position);
            animator.SetTrigger("Die");
            if (deathSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(deathSound);
            }            //Destroy(gameObject);
        }


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.70f)
        {
            gameController.Instance.xp += 30;
            Destroy(gameObject);
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
            healthBarImage.fillAmount =  ((float)hp / 40);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player") && gameController.Instance.healthPoints > 0)
        {
            GameObject campTwo = GameObject.FindWithTag("CampTwo");
            GameObject campThree = GameObject.FindWithTag("CampThree");

            CampControllerTwo campTwoController = campTwo.gameObject.GetComponent<CampControllerTwo>();
            CampControllerThree campThreeController = campThree.gameObject.GetComponent<CampControllerThree>();

            if (campTwoController.aggressiveDemons.Contains(gameObject) || campThreeController.aggressiveDemons.Contains(gameObject))
            {
                animator.SetBool("attack", true);
            }



        }



    }

    void FireInStraightLine(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - currentGrenade.transform.position).normalized;
        Rigidbody rb = currentGrenade.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Vector3 force = new Vector3(direction.x, direction.y, direction.z) * 2f;
        rb.AddForce(force, ForceMode.VelocityChange);

    }

    //private IEnumerator DeleteGrenade()
    //{

    //}

    private void OnTriggerStay(Collider other){


        if (other.gameObject.CompareTag("Player"))
        {

            GameObject campTwo = GameObject.FindWithTag("CampTwo");
            GameObject campThree = GameObject.FindWithTag("CampThree");

            CampControllerTwo campTwoController = campTwo.gameObject.GetComponent<CampControllerTwo>();
            CampControllerThree campThreeController = campThree.gameObject.GetComponent<CampControllerThree>();

            if (campTwoController.aggressiveDemons.Contains(gameObject) || campThreeController.aggressiveDemons.Contains(gameObject))
            {
                //animator.SetBool("attack", true);

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Great Sword Slash") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.90f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.92f)
                {
                    if (PlayerPrefs.GetString("SelectedCharacter") == "Barbarian")
                    {//barbarian shield
                        BarbarianAnimatorController barbarianScript = other.gameObject.GetComponent<BarbarianAnimatorController>();
                        if (barbarianScript.shield == false)
                        {
                            if(gameController.Instance.invincibility == false)
                            {
                                gameController.Instance.healthPoints -= 10;
                                Animator player = other.gameObject.GetComponent<Animator>();
                                player.SetTrigger("hit");
                            }
                            
                        }

                    }
                    else
                    {//arisa
                        if (gameController.Instance.invincibility == false)
                        {
                            gameController.Instance.healthPoints -= 10;
                            Animator player = other.gameObject.GetComponent<Animator>();
                            player.SetTrigger("hit");
                        }
                            
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Great Sword Slash 0") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.90f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.92f)
                {
                    if (PlayerPrefs.GetString("SelectedCharacter") == "Barbarian")
                    {//barbarian shield
                        BarbarianAnimatorController barbarianScript = other.gameObject.GetComponent<BarbarianAnimatorController>();
                        if (barbarianScript.shield == false)
                        {
                            if (gameController.Instance.invincibility == false)
                            {
                                gameController.Instance.healthPoints -= 10;
                                Animator player = other.gameObject.GetComponent<Animator>();
                                player.SetTrigger("hit");
                            }
                                
                        }

                    }
                    else
                    {//arisa
                        if (gameController.Instance.invincibility == false)
                        {
                            gameController.Instance.healthPoints -= 10;
                            Animator player = other.gameObject.GetComponent<Animator>();
                            player.SetTrigger("hit");
                        }
                            
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goalie Throw"))
                {
                    if(currentGrenade == null)
                    {
                        Debug.Log("grenade instantiated");
                        currentGrenade = Instantiate(grenade, shootingPoint.transform.position, Quaternion.identity);
                        currentGrenade.transform.parent = shootingPoint;
                    }
                    
                }
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goalie Throw") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.40f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.41f)
                {
                    //throw and apply explosion
                    GameObject target = GameObject.FindWithTag("clonedPlayer");
                    if (target == null)
                    {
                        target = GameObject.FindWithTag("Player");
                    }
                    grenadeTarget = target.transform.position;

                    currentGrenade.transform.parent = null;
                    range = Vector3.Distance(target.transform.position, currentGrenade.transform.position);
                    grenadeTime = Time.time;

                    FireInStraightLine(grenadeTarget);
                }

                if(currentGrenade != null)
                {
                    GameObject target = GameObject.FindWithTag("clonedPlayer");
                    if (target == null)
                    {
                        target = GameObject.FindWithTag("Player");
                    }
                    float distance = Vector3.Distance(target.transform.position, currentGrenade.transform.position);
                    if (Time.time - grenadeTime > 0.5f)
                    {
                        Destroy(currentGrenade);
                        currentGrenade = null;
                        if (explosionSound != null && audioSource != null)
                        {
                            //audioSource.PlayOneShot(explosionSound);
                        }
                    }
                    else
                    {
                        FireInStraightLine(grenadeTarget);
                    }
                    
                }

                //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goalie Throw") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.855f)
                //{

                


                //    if (PlayerPrefs.GetString("SelectedCharacter") == "Barbarian")
                //    {//barbarian
                //        BarbarianAnimatorController barbarianScript = other.gameObject.GetComponent<BarbarianAnimatorController>();
                //        if (barbarianScript.shield == false)
                //        { //barbarian
                //            gameController.Instance.healthPoints -= 15;
                //            Animator player = other.gameObject.GetComponent<Animator>();
                //            player.SetTrigger("hit");
                //        }

                //    }
                //    else
                //    {//arisa
                //        gameController.Instance.healthPoints -= 15;
                //        Animator player = other.gameObject.GetComponent<Animator>();
                //        player.SetTrigger("hit");
                //    }
                //}
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
