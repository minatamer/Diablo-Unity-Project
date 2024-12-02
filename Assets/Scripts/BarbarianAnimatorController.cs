using System.Collections; // Required for IEnumerator and Coroutines
using UnityEngine;
using UnityEngine.AI;

public class BarbarianAnimatorController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public GameObject shieldAura; // Reference to the Shield Aura GameObject

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
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                navMeshAgent.SetDestination(hit.point);
                animator.SetBool("isRunning", true);
                navMeshAgent.isStopped = false;
            }
        }

        // Stop running when reaching destination
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (animator.GetBool("isRunning"))
            {
                animator.SetBool("isRunning", false);
            }
            navMeshAgent.isStopped = true;
        }

        // Handle Bash ability with cooldown
        if (Input.GetKeyDown(KeyCode.B) && !isBashOnCooldown)
        {
            animator.SetTrigger("Bash");
            StartCoroutine(BashCooldown());
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bash") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01f)
        {
            Debug.Log("EDRAB TANY 3AYEZ ATOOB");
            if (enemyCollision == true)
            {
                Debug.Log("EDRAB TANY 3AYEZ ATOOB");
                MinionController minionController = enemy.gameObject.GetComponent<MinionController>();
                minionController.hp -= 20;
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

    private void ActivateShield()
    {
        Debug.Log("Shield activated!");
        shieldAura.SetActive(true);
        StartCoroutine(DisableShieldAfterTime(3f)); // Shield lasts for 3 seconds
        StartCoroutine(ShieldCooldown()); // Start the shield cooldown
    }

    private IEnumerator DisableShieldAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        shieldAura.SetActive(false);
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
