using System.Collections; // Required for IEnumerator and Coroutines
using System.Threading;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BarbarianAnimatorController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public GameObject shieldAura; // Reference to the Shield Aura GameObject
    private Vector3 hitPoint;

    // Cooldown durations (in seconds)
    private float bashCooldown = 2f;
    private float ironMaelstromCooldown = 5f;
    private float shieldCooldown = 10f;
    private float chargeCooldown = 10f;

    private float bashAbilityTime = 0;
    private float shieldAbilityTime = 0;
    private float ironMaelstromAbilityTime = 0;
    private float chargeAbilityTime = 0;

    // Cooldown flags
    private bool isBashOnCooldown = false;
    private bool isIronMaelstromOnCooldown = false;
    private bool isShieldOnCooldown = false;
    private bool isChargeOnCooldown = false;

    //collision
    private bool enemyCollision = false;

    private GameObject enemy;

    public bool shield = false;

    private bool waitingForRightClick;

    private bool bossChargeDamage = false;

    public GameObject explosionDemonPrefab;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        shieldAura = transform.Find("ShieldAura").gameObject; // Locate the Shield Aura child object
        shieldAura.SetActive(false); // Ensure it's disabled initially
    }

    public void getHit()
    {
        if (!animator.GetCurrentAnimatorStateInfo(1).IsName("Hurt"))
        {
            animator.SetTrigger("hit");
            //Debug.Log("hurt animation");
        }

    }

    private void UpdateAbilitiesCoolDown()
    {
        for (int i = 0; i <= 3; i++)
        {
            if (gameController.Instance.locked[i] == -1)
            {
                if (i == 0 && isBashOnCooldown == true)
                {
                    int cooldown = Mathf.FloorToInt(bashCooldown - (Time.time - bashAbilityTime));
                    if (cooldown < 0)
                    {
                        cooldown = 0;
                    }
                    gameController.Instance.cooldownVal[i].text = cooldown.ToString();
                }
                if (i == 1 && isShieldOnCooldown == true)
                {
                    int cooldown = Mathf.FloorToInt(shieldCooldown - (Time.time - shieldAbilityTime));
                    if (cooldown < 0)
                    {
                        cooldown = 0;
                    }
                    gameController.Instance.cooldownVal[i].text = cooldown.ToString();
                }
                if (i == 2 && isIronMaelstromOnCooldown == true)
                {
                    int cooldown = Mathf.FloorToInt(ironMaelstromCooldown - (Time.time - ironMaelstromAbilityTime));
                    if (cooldown < 0)
                    {
                        cooldown = 0;
                    }
                    gameController.Instance.cooldownVal[i].text = cooldown.ToString();
                }
                if (i == 3 && isChargeOnCooldown == true)
                {
                    int cooldown = Mathf.FloorToInt(chargeCooldown - (Time.time - chargeAbilityTime));
                    if (cooldown < 0)
                    {
                        cooldown = 0;
                    }
                    gameController.Instance.cooldownVal[i].text = cooldown.ToString();
                }

            }
        }
    }

    public void drink()
    {
        animator.SetTrigger("drink");
    }

    void Update()
    {
        UpdateAbilitiesCoolDown();

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (gameController.Instance.coolDownCheat == false)
            {
                bashCooldown = 0;
                shieldCooldown = 0;
                ironMaelstromCooldown = 0;
                chargeCooldown = 0;
                UpdateAbilitiesCoolDown();
                gameController.Instance.coolDownCheat = true;
            }
            else
            {

                bashCooldown = 2f;
                shieldCooldown = 10f;
                ironMaelstromCooldown = 5f;
                chargeCooldown = 10f;
                UpdateAbilitiesCoolDown();
                gameController.Instance.coolDownCheat = false;
            }
                
        }

            if (Input.GetMouseButtonDown(1) && waitingForRightClick == true)
        {
            if (isChargeOnCooldown == true)
            {
                waitingForRightClick = false;
            }
            else
            {
                Debug.Log("Charge Right clicked pressed");
                Vector3 position = GetMouseWorldPosition();

                
                navMeshAgent.destination = position;
                animator.SetTrigger("Charge");
                StartCoroutine(ChargeCooldown());
                StartCoroutine(ResetToIdleAfterCharge());
                waitingForRightClick = false;
            }
        }
        // Handle Shield ability with cooldown
        if (Input.GetKeyDown(KeyCode.W) && !isShieldOnCooldown && gameController.Instance.locked[1] == -1)
        {
            StartCoroutine(ActivateShield());
        }


        // Mouse-based movement (run to destination)
       if (Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)){
                 hitPoint = hit.point;
                 //Debug.Log(hitPoint);
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
        if (Input.GetMouseButtonDown(1) && !isBashOnCooldown && waitingForRightClick == false)
        {
            Debug.Log("Entered bashing");
            Vector3 posClicked = GetMouseWorldPosition();
            Debug.Log(posClicked);
            posClicked.y = 0;
            float radius = 2f;
            GameObject foundObject = null;
            Collider[] hitColliders = Physics.OverlapSphere(posClicked, radius);

            foreach (Collider collider in hitColliders)
            {
                string tag = collider.gameObject.tag;
                if (tag.Contains("Minion") || tag.Contains("Demon") || tag.Contains("Boss"))
                {
                    Debug.Log("found object");
                    foundObject = collider.gameObject;
                    break;
                }
            }

            float distanceFromBarbarian = Vector3.Distance(posClicked, transform.position);
            
            if(distanceFromBarbarian <= 3f && foundObject != null){
                Debug.Log("Bashing animation");
                animator.SetTrigger("Bash");
                StartCoroutine(BashCooldown());
            }
            else {
                //Debug.Log("far from barbarian");
            }
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("IronMaelstrom") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01f)
        {
            if (enemyCollision == true)
            {
                //Debug.Log("lefi bina denia");

                if (enemy.tag.Contains("Minion"))
                {
                    MinionController enemyScript = enemy.GetComponent<MinionController>();
                    enemyScript.hp -= 10;
                    enemyScript.getHit();
                    enemyScript.UpdateHealthBar();
                }
                if (enemy.tag.Contains("Demon"))
                {
                    DemonController enemyScript = enemy.GetComponent<DemonController>();
                    enemyScript.hp -= 10;
                    enemyScript.getHit();
                    enemyScript.UpdateHealthBar();
                }
                if (enemy.tag.Contains("Boss"))
                {
                    BossController enemyScript = enemy.gameObject.GetComponent<BossController>();//update with ennemy script name
                    if (enemyScript.minions.Count > 0)
                    {
                        return;
                    }
                    else if (enemyScript.auraActive)
                    {
                        if (gameController.Instance.invincibility == false)
                        {
                            gameController.Instance.healthPoints -= 25;
                        }
                            
                        enemyScript.auraActive = false;
                    }
                    else
                    {
                        if (enemyScript.currentShieldHealth > 0)
                        {
                            enemyScript.currentShieldHealth -= 10;
                            if (enemyScript.currentShieldHealth < 0)
                            {
                                int damage = enemyScript.currentShieldHealth;
                                enemyScript.currentShieldHealth = 0;
                                enemyScript.hp += damage;

                            }
                            enemyScript.UpdateShieldBar();
                        }
                        else
                        {
                            enemyScript.hp -= 10;
                            enemyScript.getHit();
                            enemyScript.UpdateHealthBar();
                        }
                    }

                    enemyScript.UpdateHealthBar();
                }

                enemyCollision = false;
            }


        }

        if (animator.GetCurrentAnimatorStateInfo(1).IsName("Charge"))
        {
            transform.position = animator.transform.position;
            Debug.Log("charge animation");
            if (enemyCollision == true)
            {
                if (enemy.tag.Contains("Minion"))
                {
                    MinionController enemyScript = enemy.GetComponent<MinionController>();
                    enemyScript.hp -= 50;  //DO A BIG DAMAGE THAT WILL KILL THEM 
                    enemyScript.UpdateHealthBar();
                }
                if (enemy.tag.Contains("Demon"))
                {
                    DemonController enemyScript = enemy.GetComponent<DemonController>();
                    enemyScript.hp -= 50; //DO A BIG DAMAGE THAT WILL KILL THEM 
                    enemyScript.UpdateHealthBar();
                }
                if (enemy.tag.Contains("Boss"))
                {
                    BossController enemyScript = enemy.GetComponent<BossController>();
                    if (bossChargeDamage == false)
                    {
                        bossChargeDamage = true;
                        if (enemyScript.minions.Count > 0)
                        {
                            return;
                        }
                        else if (enemyScript.auraActive)
                        {
                            if (gameController.Instance.invincibility == false)
                            {
                                gameController.Instance.healthPoints -= 35;
                            }
                                
                            enemyScript.auraActive = false;
                        }
                        else
                        {
                            if (enemyScript.currentShieldHealth > 0)
                            {
                                enemyScript.currentShieldHealth -= 20;
                                if (enemyScript.currentShieldHealth < 0)
                                {
                                    int damage = enemyScript.currentShieldHealth;
                                    enemyScript.currentShieldHealth = 0;
                                    enemyScript.hp += damage;

                                }
                                enemyScript.UpdateShieldBar();
                            }
                            else
                            {
                                enemyScript.hp -= 20;
                                enemyScript.getHit();
                                enemyScript.UpdateHealthBar();
                            }
                        }

                        enemyScript.UpdateHealthBar();
                    }
                    
                  
                }
                enemyCollision = false;
            }

        }

        if(animator.GetCurrentAnimatorStateInfo(1).IsName("New State"))
        {
            //Debug.Log("BOSS CHARGE DAMAGE RESET");
            bossChargeDamage = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bash") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01f)
        {
           // Debug.Log("EDRAB TANY 3AYEZ ATOOB");
            if (enemyCollision == true)
            {
                //Debug.Log("EDRAB TANY 3AYEZ ATOOB");
                if (enemy.tag.Contains("Minion"))
                {
                    MinionController enemyScript = enemy.GetComponent<MinionController>();
                    enemyScript.hp -= 5;
                    enemyScript.getHit();
                    enemyScript.UpdateHealthBar();
                }
                if (enemy.tag.Contains("Demon"))
                {
                    DemonController enemyScript = enemy.GetComponent<DemonController>();
                    enemyScript.hp -= 5;
                    enemyScript.getHit();
                    enemyScript.UpdateHealthBar();
                }
                if (enemy.tag.Contains("Boss"))
                {
                    BossController enemyScript = enemy.gameObject.GetComponent<BossController>();//update with ennemy script name
                    if (enemyScript.minions.Count > 0)
                    {
                        return;
                    }
                    else if (enemyScript.auraActive)
                    {
                        if (gameController.Instance.invincibility == false)
                        {
                            gameController.Instance.healthPoints -= 20;
                        }
                            
                        enemyScript.auraActive = false;
                    }
                    else
                    {
                        if (enemyScript.currentShieldHealth > 0)
                        {
                            enemyScript.currentShieldHealth -= 5;
                            if (enemyScript.currentShieldHealth < 0)
                            {
                                int damage = enemyScript.currentShieldHealth;
                                enemyScript.currentShieldHealth = 0;
                                enemyScript.hp += damage;

                            }
                            enemyScript.UpdateShieldBar();
                        }
                        else
                        {
                            enemyScript.hp -= 5;
                            enemyScript.getHit();
                            enemyScript.UpdateHealthBar();
                        }
                    }

                    enemyScript.UpdateHealthBar();
                }

                enemyCollision = false;
            }

        }

        // Handle Charge ability with cooldown
        if (Input.GetKeyDown(KeyCode.E) && !isChargeOnCooldown && gameController.Instance.locked[3] == -1)
        {
            Debug.Log("E is pressed!");
            waitingForRightClick = true;

        }

        // Handle Iron Maelstrom ability with cooldown
        if (Input.GetKeyDown(KeyCode.Q) && !isIronMaelstromOnCooldown && gameController.Instance.locked[2] == -1)
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
    private IEnumerator ActivateShield()
    {
        //Debug.Log("Shield activated!");
        shield  = true;
        shieldAura.SetActive(true);
        StartCoroutine(DisableShieldAfterTime(3f)); // Shield lasts for 3 seconds
        yield return new WaitForSeconds(3f);
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
        yield return new WaitForSeconds(1f);
        bashAbilityTime = Time.time;
        isBashOnCooldown = true;
        yield return new WaitForSeconds(bashCooldown); // Wait for cooldown duration
        isBashOnCooldown = false;
        bashAbilityTime = 0;
    }

    // Iron Maelstrom cooldown logic
    private IEnumerator IronMaelstromCooldown()
    {
        yield return new WaitForSeconds(2f);
        ironMaelstromAbilityTime = Time.time;
        isIronMaelstromOnCooldown = true;
        yield return new WaitForSeconds(ironMaelstromCooldown); // Wait for cooldown duration
        isIronMaelstromOnCooldown = false;
        ironMaelstromAbilityTime = 0;
    }

    // Shield cooldown logic
    private IEnumerator ShieldCooldown()
    {
        shieldAbilityTime = Time.time;
        isShieldOnCooldown = true;
        yield return new WaitForSeconds(shieldCooldown); // Wait for cooldown duration
        isShieldOnCooldown = false;
        shieldAbilityTime = 0;
    }

    // Charge cooldown logic
    private IEnumerator ChargeCooldown()
    {
        yield return new WaitForSeconds(2f);
        chargeAbilityTime = Time.time;
        isChargeOnCooldown = true;
        yield return new WaitForSeconds(chargeCooldown); // Wait for cooldown duration
        isChargeOnCooldown = false;
        chargeAbilityTime = 0;
    }

    // Coroutine to reset to Idle after Charge
    private IEnumerator ResetToIdleAfterCharge()
    {
        yield return new WaitForSeconds(1.0f); // Adjust duration to match Charge animation length
        animator.ResetTrigger("Charge");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Minion") || other.gameObject.tag.Contains("Demon") || other.gameObject.tag.Contains("Boss"))
        {
            Debug.Log("7asal collision");
            enemy = other.gameObject;
            enemyCollision = true;
        }


        if (other.gameObject.tag == "Rune")
        {
            gameController.Instance.runeFragments++;
            Destroy(other.gameObject);

        }
        if (other.gameObject.tag == "Potion")
        {
            if (gameController.Instance.healingPotions < 3)
            {
                gameController.Instance.healingPotions++;
                Destroy(other.gameObject);
            }
        }
        if (other.gameObject.tag == "Spike")
        {
            if (gameController.Instance.invincibility == false)
            {
                gameController.Instance.healthPoints -= 30;
                getHit();
            }
                
            //Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Grenade")
        {
            GameObject explosion = Instantiate(explosionDemonPrefab, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z), Quaternion.identity);

            if (gameController.Instance.invincibility == false)
            {
                getHit();
            }
               
            Destroy(explosion, 1.0f);
            if (gameController.Instance.invincibility == false)
            {
                gameController.Instance.healthPoints -= 15;
            }
                
            //Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Gate")
        {
            PlayerPrefs.SetInt("healingPotions", gameController.Instance.healingPotions);
            PlayerPrefs.SetInt("xp", gameController.Instance.xp);
            PlayerPrefs.SetInt("level", gameController.Instance.level);
            PlayerPrefs.SetInt("healthPoints", gameController.Instance.healthPoints);
            PlayerPrefs.SetInt("runeFragments", gameController.Instance.runeFragments);
            PlayerPrefs.SetInt("abilityPoints", gameController.Instance.abilityPoints);
            PlayerPrefs.SetInt("AbilityDefensive", gameController.Instance.locked[1]);
            PlayerPrefs.SetInt("AbilityWild", gameController.Instance.locked[2]);
            PlayerPrefs.SetInt("AbilityUltimate", gameController.Instance.locked[3]);
            gameController.Instance.bossLevel = true;
            SceneManager.LoadScene("BossScene");
        }


    }
}
