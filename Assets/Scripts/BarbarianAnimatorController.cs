using System.Collections; // Required for IEnumerator and Coroutines
using UnityEngine;
using UnityEngine.AI;

public class BarbarianAnimatorController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public GameObject shieldAura; // Reference to the Shield Aura GameObject
    private Vector3 hitPoint;

    // Cooldown durations (in seconds)
    private float bashCooldown = 1f;
    private float ironMaelstromCooldown = 5f;
    private float shieldCooldown = 10f;
    private float chargeCooldown = 10f;

    // Cooldown flags
    private bool isBashOnCooldown = false;
    private bool isIronMaelstromOnCooldown = false;
    private bool isShieldOnCooldown = false;
    private bool isChargeOnCooldown = false;

    //collision
    private bool enemyCollision = false;

    private GameObject enemy;

    public bool shield = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        shieldAura = transform.Find("ShieldAura").gameObject; // Locate the Shield Aura child object
        shieldAura.SetActive(false); // Ensure it's disabled initially
    }

    void Update()
    {
        // Handle Shield ability with cooldown
        if (Input.GetKeyDown(KeyCode.W) && !isShieldOnCooldown)
        {
            ActivateShield();
        }


        // Mouse-based movement (run to destination)
       if (Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)){
                 hitPoint = hit.point;
                 Debug.Log(hitPoint);
                navMeshAgent.destination = hit.point;

                animator.SetBool("walking", true);
                
            }
        }
        if( Vector3.Distance(navMeshAgent.transform.position, hitPoint)<= 0.8f){

            animator.SetBool("walking", false);
           navMeshAgent.velocity = Vector3.zero;
        }
       

        if (navMeshAgent.remainingDistance >7.0f)
        { 
           
            
            animator.SetBool("isRunning", true);
            navMeshAgent.speed = 10.0f; 


        }
        if (navMeshAgent.remainingDistance > 0.8f && navMeshAgent.remainingDistance <7.0)
        {
          
            animator.SetBool("isRunning", false);
            animator.SetBool("walking", true);
            navMeshAgent.speed = 3.5f;
            
        }


        // Stop running when reaching destination
        // if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        // {
        //     if (animator.GetBool("isRunning"))
        //     {
        //         animator.SetBool("isRunning", false);
        //     }
        //     navMeshAgent.isStopped = true;
        // }

        // Handle Bash ability with cooldown
        if (Input.GetMouseButtonDown(1) && !isBashOnCooldown)
        {
            Vector3 posClicked = GetMouseWorldPosition();
            float distanceFromBarbarian = Vector3.Distance(posClicked, transform.position);
            if(distanceFromBarbarian <= 5f){
                animator.SetTrigger("Bash");
                StartCoroutine(BashCooldown());
            }
            else {
                Debug.Log("far from barbarian");
            }
        }
        //  if (animator.GetCurrentAnimatorStateInfo(0).IsName("IronMaelstrom") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01f)
        // {
        //    // Debug.Log("EDRAB TANY 3AYEZ ATOOB");
        //     if (enemyCollision == true)
        //     {
        //         Debug.Log("lefi bina denia");
        //         MinionController minionController = enemy.gameObject.GetComponent<MinionController>();
        //         minionController.hp -= 10;
        //         enemyCollision = false;
        //     }

        // }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bash") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01f)
        {
           // Debug.Log("EDRAB TANY 3AYEZ ATOOB");
            if (enemyCollision == true)
            {
                Debug.Log("EDRAB TANY 3AYEZ ATOOB");
                MinionController minionController = enemy.gameObject.GetComponent<MinionController>();
                minionController.hp -= 5;
                enemyCollision = false;
            }

        }

        // Handle Charge ability with cooldown
        if (Input.GetKeyDown(KeyCode.E) && !isChargeOnCooldown)
        {
            animator.SetTrigger("Charge");
            StartCoroutine(ChargeCooldown());
            StartCoroutine(ResetToIdleAfterCharge());
        }

        // Handle Iron Maelstrom ability with cooldown
        if (Input.GetKeyDown(KeyCode.Q) && !isIronMaelstromOnCooldown)
        {

            animator.SetTrigger("IronMaelstrom");
            StartCoroutine(IronMaelstromCooldown());
        }
    }
Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }

        Debug.LogWarning("Mouse click did not hit a valid position.");
        return Vector3.zero; // Return a default invalid position
    }
    private void ActivateShield()
    {
        Debug.Log("Shield activated!");
        shield  = true;
        shieldAura.SetActive(true);
        StartCoroutine(DisableShieldAfterTime(3f)); // Shield lasts for 3 seconds
        StartCoroutine(ShieldCooldown()); // Start the shield cooldown
     
    }

    private IEnumerator DisableShieldAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        shieldAura.SetActive(false);
        shield  = false;
    }

    // Bash cooldown logic
    private IEnumerator BashCooldown()
    {
        isBashOnCooldown = true;
        yield return new WaitForSeconds(bashCooldown); // Wait for cooldown duration
        isBashOnCooldown = false;
    }

    // Iron Maelstrom cooldown logic
    private IEnumerator IronMaelstromCooldown()
    {
        isIronMaelstromOnCooldown = true;
        yield return new WaitForSeconds(ironMaelstromCooldown); // Wait for cooldown duration
        isIronMaelstromOnCooldown = false;
    }

    // Shield cooldown logic
    private IEnumerator ShieldCooldown()
    {
        isShieldOnCooldown = true;
        yield return new WaitForSeconds(shieldCooldown); // Wait for cooldown duration
        isShieldOnCooldown = false;
    }

    // Charge cooldown logic
    private IEnumerator ChargeCooldown()
    {
        isChargeOnCooldown = true;
        yield return new WaitForSeconds(chargeCooldown); // Wait for cooldown duration
        isChargeOnCooldown = false;
    }

    // Coroutine to reset to Idle after Charge
    private IEnumerator ResetToIdleAfterCharge()
    {
        yield return new WaitForSeconds(1.0f); // Adjust duration to match Charge animation length
        animator.ResetTrigger("Charge");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Minion"))
        {
            
            Debug.Log("7asal collision");
            enemy = collision.gameObject;
            enemyCollision = true;
        }
    }
}
