using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class sor_script : MonoBehaviour
{
    Animator animator ;
    NavMeshAgent navMeshAgent;
    private Vector3 hitPoint;
    private Vector3 targetPosition ;
    public GameObject fireball;
    public Transform shootingPoint;
    private GameObject currentFireball; 
    private bool waitingForRightClick = false;
     private bool waitingForRightUltimate= false;
     private bool waitingForRightDefensive = false;
    public GameObject sorcererClone ;
    private GameObject clonedinstance;
     public GameObject dangArea ;
    private GameObject dangInstance;
    private bool fireBallDestroyed = false;
    private float fireBallLastAbilityTime = 0;
    private float cloneLastAbilityTime = 0;
    private float infernoLastAbilityTime = 0;
    private float teleportLastAbilityTime = 0;
    private float fireBallThrownAtTime;
    public GameObject explosionPrefab;
    public GameObject explosionDemonPrefab;


    private int fireBallCooldown = 2;
    private int cloneCooldown = 10;
    private int infernoCooldown = 15;
    private int teleportCooldown = 10;


    private bool usedInferno = false;
    private bool usedClone = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    public void SetFireBallDestroyed(bool value)
    {
        fireBallDestroyed = value;
    }
 void FireInStraightLine(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - currentFireball.transform.position).normalized;
        Rigidbody rb = currentFireball.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Vector3 force = new Vector3(direction.x, direction.y, direction.z) * 0.1f;
        rb.AddForce(force, ForceMode.VelocityChange);
        
    }
    public void SetCurrentFireball(GameObject fireball)
    {
        currentFireball = fireball;
    }
     bool UseAbility(float lastAbilityTime, int abilityCooldown )
    {
        if (Time.time - lastAbilityTime >= abilityCooldown || lastAbilityTime == 0 )
        {
            return true;

        }
        else
        {
            Debug.Log("Ability is on cooldown!");
            return false;
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
    private IEnumerator HandleInfernoCooldown(GameObject dangInstance)
    {
        Destroy(dangInstance, 5f); 
        yield return new WaitForSeconds(5f);
        usedInferno = false;
        infernoLastAbilityTime = Time.time; 
    }
    private IEnumerator HandleCloneCooldown(GameObject clonedInstance)
    {
        yield return new WaitForSeconds(5f);
        usedClone = false;
        GameObject explosion =  Instantiate(explosionPrefab, new Vector3(clonedInstance.transform.position.x,clonedInstance.transform.position.y+5f,clonedInstance.transform.position.z), Quaternion.identity);
        //cause damage to enemies around 
        string[] enemyTags = { "Minion", "Minion2", "Minion3", "Demon11", "Demon12", "Demon", "Boss" };
        List<GameObject> enemies = new List<GameObject>();

        foreach (string tag in enemyTags)
        {
            GameObject[] taggedEnemies = GameObject.FindGameObjectsWithTag(tag);
            enemies.AddRange(taggedEnemies);
        }

        GameObject[] enemyArray = enemies.ToArray();

        foreach (GameObject enemy in enemyArray)
        {
            float distance = Vector3.Distance(enemy.transform.position, clonedInstance.transform.position);
            if (distance < 5f)
            {
                if (enemy.tag.Contains("Minion"))
                {
                    MinionController enemyScript = enemy.gameObject.GetComponent<MinionController>();
                    enemyScript.hp -= 10;
                    enemyScript.getHit();
                    enemyScript.UpdateHealthBar();
                }
                if (enemy.tag.Contains("Demon"))
                {
                    DemonController enemyScript = enemy.gameObject.GetComponent<DemonController>();
                    enemyScript.hp -= 10;
                    enemyScript.getHit();
                    enemyScript.UpdateHealthBar();
                }
                if (enemy.tag.Contains("Boss"))
                {
                    BossController enemyScript = enemy.gameObject.GetComponent<BossController>();

                    if (enemyScript.minions.Count > 0)
                    {
                        Debug.Log(""); //do nothing
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
            }
        }

        Destroy(explosion, 1.0f);
        Destroy(clonedInstance);
        clonedInstance = null;
        gameObject.tag= "Player";
        cloneLastAbilityTime = Time.time; 
    }
    public void drink(){
        animator.SetTrigger("drink");
    }
    
    void Update()
    {
        UpdateAbilitiesCoolDown();

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (gameController.Instance.coolDownCheat == false)
            {
                fireBallCooldown = 0;
                cloneCooldown = 0;
                infernoCooldown = 0;
                teleportCooldown = 0;
                UpdateAbilitiesCoolDown();
                gameController.Instance.coolDownCheat = true;
            }
            else
            {

                fireBallCooldown = 2;
                cloneCooldown = 10;
                teleportCooldown = 10;
                infernoCooldown = 15;
                UpdateAbilitiesCoolDown();
                gameController.Instance.coolDownCheat = false;
            }

        }



        if ( Input.GetMouseButtonDown(1)) 
        {      
            if(waitingForRightClick == true && (UseAbility(cloneLastAbilityTime , cloneCooldown) == false ||  gameController.Instance.locked[2] != -1 )){
                waitingForRightClick = false;
            }
            else if(waitingForRightUltimate == true  && (UseAbility(infernoLastAbilityTime , infernoCooldown) == false ||  gameController.Instance.locked[3] != -1 )){
                waitingForRightUltimate = false;
            }
            else if(waitingForRightDefensive == true && (UseAbility(teleportLastAbilityTime , teleportCooldown) == false ||  gameController.Instance.locked[1] != -1 )){
                waitingForRightDefensive = false;
            }
            else if(waitingForRightClick == false && waitingForRightUltimate == false && waitingForRightDefensive == false ){

                animator.SetBool("throwing",true);
                fireBallThrownAtTime = Time.time;
                fireBallLastAbilityTime = Time.time;
                currentFireball = Instantiate(fireball , shootingPoint.transform.position  , Quaternion.identity);
                currentFireball.transform.parent =  shootingPoint;

                Vector3 mousePos = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 hitPoint = hit.point;
                    targetPosition = hitPoint;
                }
                else{                
                    targetPosition =  mousePos;
                }
            }
            }
            
        releaseBall();
       
        if(fireBallDestroyed == true ){
            fireBallDestroyed = false;
        }

        if (Input.GetKeyDown(KeyCode.E) )
        {
            Debug.Log("E key pressed. Waiting for right mouse click...");
            if (usedInferno == false)
            {
                waitingForRightUltimate = true;
            }
            
        }

       
        if (waitingForRightUltimate && Input.GetMouseButtonDown(1) && UseAbility(infernoLastAbilityTime , infernoCooldown) == true && gameController.Instance.locked[3] == -1)
        {
            Debug.Log("Right mouse button clicked after pressing E!");
            waitingForRightUltimate = false;
            Vector3 spawnPosition = GetMouseWorldPosition();
             if (spawnPosition != Vector3.zero) 
            {
                 dangInstance = Instantiate(dangArea, spawnPosition, Quaternion.Euler(0, 0, 90));
                usedInferno = true;
                 StartCoroutine(HandleInfernoCooldown(dangInstance));

            }   
        }       

       
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W key pressed. Waiting for right mouse click...");
            waitingForRightDefensive = true; 
            
        }
      
        if (waitingForRightDefensive && Input.GetMouseButtonDown(1) && UseAbility(teleportLastAbilityTime , teleportCooldown) == true && gameController.Instance.locked[1] == -1)
        {
             Debug.Log("Right mouse button clicked after pressing W!");
             Vector3 spawnPosition = GetMouseWorldPosition();

             // Validate spawn position and update
             NavMeshHit hit;
             if (NavMesh.SamplePosition(spawnPosition, out hit, 1.0f, NavMesh.AllAreas))
             {
                navMeshAgent.enabled = false; 
                navMeshAgent.transform.position = hit.position;
                navMeshAgent.enabled = true; 
                Debug.Log($"Teleported to {hit.position}");
             }
        
            waitingForRightDefensive = false;
            teleportLastAbilityTime = Time.time;
        }       
       
       if (Input.GetKeyDown(KeyCode.Q) )
        {
            Debug.Log("Q key pressed. Waiting for right mouse click...");
            if(usedClone == false)
            {
                waitingForRightClick = true;
            }
             
        }

        
        if (waitingForRightClick && Input.GetMouseButtonDown(1) && UseAbility(cloneLastAbilityTime , cloneCooldown) == true && gameController.Instance.locked[2] == -1 )
        {
            Debug.Log("Right mouse button clicked after pressing Q!");
            waitingForRightClick = false;
            Vector3 spawnPosition = GetMouseWorldPosition();
             if (spawnPosition != Vector3.zero) 
            {
                 clonedinstance = Instantiate(sorcererClone, spawnPosition, Quaternion.identity);
                usedClone = true;
                 Destroy(clonedinstance.GetComponent<sor_script>());
                 Animator cloneAnimator = clonedinstance.GetComponent<Animator>();
                 AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
                 cloneAnimator.runtimeAnimatorController = overrideController;
                 clonedinstance.transform.localScale = transform.localScale;
                 clonedinstance.transform.localRotation = transform.localRotation;
                 Debug.Log("Sorcerer clone created at: " + spawnPosition);
                 clonedinstance.tag = "clonedPlayer";
                 gameObject.tag= "OriginalPlayer";
                 StartCoroutine(HandleCloneCooldown(clonedinstance));
            }          
        }



        if (Input.GetMouseButtonDown(0) &&  gameController.Instance.buttons[1].GetComponent<AbilitiesButtons>().isMouseOver == false &&  gameController.Instance.buttons[2].GetComponent<AbilitiesButtons>().isMouseOver == false &&  gameController.Instance.buttons[3].GetComponent<AbilitiesButtons>().isMouseOver == false){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)){
                 hitPoint = hit.point;
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
          
           
            animator.SetBool("running", true);
            // animator.SetBool("walking", false);
            navMeshAgent.speed = 10.0f; 


        }
        if (navMeshAgent.remainingDistance > 0.8f && navMeshAgent.remainingDistance <7.0)
        {
            animator.SetBool("running", false);
            animator.SetBool("walking", true);
            navMeshAgent.speed = 3.5f;
            
        }

    }
    public void getHit()
    {
        if (!animator.GetCurrentAnimatorStateInfo(1).IsName("Hurt"))
        {
            animator.SetTrigger("hit");
            //Debug.Log("hurt animation");
        }

    }

    private void releaseBall(){
        if ( Time.time - fireBallThrownAtTime >= 1.0f){                
            if(fireBallDestroyed == false && currentFireball != null){ // target reached or not 
                currentFireball.transform.parent = null;
                FireInStraightLine(targetPosition);
            }
        animator.SetBool("throwing", false);
        fireBallThrownAtTime = 0.0f;
        

        }
    }
    private void UpdateAbilitiesCoolDown(){
        for(int i =0;i<=3;i++){
            if (gameController.Instance.coolDownCheat == true)
            {
                gameController.Instance.cooldownVal[i].text = "0";
            }

            else
            {
                if (gameController.Instance.locked[i] == -1)
                {
                    if (i == 0 && UseAbility(fireBallLastAbilityTime, fireBallCooldown) == false)
                    {
                        gameController.Instance.cooldownVal[i].text = Mathf.FloorToInt(fireBallCooldown - (Time.time - fireBallLastAbilityTime)).ToString();

                    }

                    if (i == 1 && UseAbility(teleportLastAbilityTime, teleportCooldown) == false)
                    {
                        gameController.Instance.cooldownVal[i].text = Mathf.FloorToInt(teleportCooldown - (Time.time - teleportLastAbilityTime)).ToString();
                    }

                    else if (i == 2 && UseAbility(cloneLastAbilityTime, cloneCooldown) == false)
                    {
                        gameController.Instance.cooldownVal[i].text = Mathf.FloorToInt(cloneCooldown - (Time.time - cloneLastAbilityTime)).ToString();
                    }

                    else if (i == 3 && UseAbility(infernoLastAbilityTime, infernoCooldown) == false)
                    {
                        gameController.Instance.cooldownVal[i].text = Mathf.FloorToInt(infernoCooldown - (Time.time - infernoLastAbilityTime)).ToString();
                    }
                }
            }



            
        }
    }
    private void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Rune"){
            gameController.Instance.runeFragments ++;
            Destroy(other.gameObject);

        }
        if(other.gameObject.tag == "Potion"){
            if(  gameController.Instance.healingPotions < 3 ){
            gameController.Instance.healingPotions ++;
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
            GameObject explosion = Instantiate(explosionDemonPrefab, new Vector3(transform.position.x , transform.position.y + 3f, transform.position.z), Quaternion.identity);

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

    }
   
}

