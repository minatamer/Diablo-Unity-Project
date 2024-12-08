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
    private float cloneLastAbilityTime = 0;
    private float infernoLastAbilityTime = 0;
    private float teleportLastAbilityTime = 0;
    private float fireBallThrownAtTime;
    public GameObject explosionPrefab;
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
        infernoLastAbilityTime = Time.time; 
    }
    private IEnumerator HandleCloneCooldown(GameObject clonedInstance)
    {
        yield return new WaitForSeconds(5f);
        GameObject explosion =  Instantiate(explosionPrefab, new Vector3(clonedInstance.transform.position.x,clonedInstance.transform.position.y+5f,clonedInstance.transform.position.z), Quaternion.identity);
        Destroy(explosion, 1.0f);
        Destroy(clonedInstance);
        //cause damage to enemies around 
        gameObject.tag= "Player";
        cloneLastAbilityTime = Time.time; 
    }
    void Update()
    {
        //  int level =  gameController.Instance.level ;
       

    //    if(Input.GetKeyDown(KeyCode.F) && gameController.Instance.healthPoints != level*100 && gameController.Instance.healingPotions !=0){
    //     Debug.Log("drinkkkkkkk");
    //      animator.SetTrigger("drink");
    //     }
    

            
           if ( Input.GetMouseButtonDown(1) && waitingForRightClick == false && waitingForRightUltimate == false && waitingForRightDefensive == false) 
        {      
            if(waitingForRightClick == true && UseAbility(cloneLastAbilityTime , 10) == false ){
                waitingForRightClick = false;
                        }
            else if(waitingForRightUltimate == true  && UseAbility(infernoLastAbilityTime , 15) == false ){
                waitingForRightUltimate = false;
            }
            else if(waitingForRightDefensive == true && UseAbility(teleportLastAbilityTime , 10) == false ){
                waitingForRightDefensive = false;
            }
            else{
                animator.SetBool("throwing",true);
                fireBallThrownAtTime = Time.time;
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
            waitingForRightUltimate = true; 
          //  infernoLastAbilityTime  = Time.time ;
        }

       
        if (waitingForRightUltimate && Input.GetMouseButtonDown(1) && UseAbility(infernoLastAbilityTime , 15) == true)
        {
            Debug.Log("Right mouse button clicked after pressing E!");
            waitingForRightUltimate = false;
            Vector3 spawnPosition = GetMouseWorldPosition();
             if (spawnPosition != Vector3.zero) // Ensure a valid position was returned
            {
                 dangInstance = Instantiate(dangArea, spawnPosition, Quaternion.Euler(0, 0, 90));
                 StartCoroutine(HandleInfernoCooldown(dangInstance));

            }   
        }       

       
        if (Input.GetKeyDown(KeyCode.W) )
        {
            Debug.Log("W key pressed. Waiting for right mouse click...");
            waitingForRightDefensive = true; 
        }
      
        if (waitingForRightDefensive && Input.GetMouseButtonDown(1)&& UseAbility(teleportLastAbilityTime , 10) == true)
        {
            Debug.Log("Right mouse button clicked after pressing W!!!!!");
            Vector3 spawnPosition = GetMouseWorldPosition();
            navMeshAgent.transform.position = spawnPosition;
            waitingForRightDefensive = false; 
            teleportLastAbilityTime = Time.time;
        }       
       
       if (Input.GetKeyDown(KeyCode.Q) )
        {
            Debug.Log("Q key pressed. Waiting for right mouse click...");
            waitingForRightClick = true; 
        }
        
        if (waitingForRightClick && Input.GetMouseButtonDown(1) && UseAbility(cloneLastAbilityTime , 10) == true)
        {
            Debug.Log("Right mouse button clicked after pressing Q!");
            waitingForRightClick = false;
            Vector3 spawnPosition = GetMouseWorldPosition();
             if (spawnPosition != Vector3.zero) // Ensure a valid position was returned
            {
                 clonedinstance = Instantiate(sorcererClone, spawnPosition, Quaternion.identity);
                 Destroy(clonedinstance.GetComponent<sor_script>());
                 Animator cloneAnimator = clonedinstance.GetComponent<Animator>();
                 AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
                 cloneAnimator.runtimeAnimatorController = overrideController;
                 clonedinstance.transform.localScale = transform.localScale;
                 clonedinstance.transform.localRotation = transform.localRotation;
                 Debug.Log("Sorcerer clone created at: " + spawnPosition);
                 clonedinstance.tag = "clonedPlayer";
                 gameObject.tag= "OriginalPlayer";
                 //clone explodes  damage to the enemies surrounding the area
                StartCoroutine(HandleCloneCooldown(clonedinstance));
            }          
        }


          if (Input.GetMouseButtonDown(0)){
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
   
    private void releaseBall(){
             if ( Time.time - fireBallThrownAtTime >= 1.0f)
         {                
              if(fireBallDestroyed == false && currentFireball != null){ // target reached or not 
                 currentFireball.transform.parent = null;
                 FireInStraightLine(targetPosition);
             }
              animator.SetBool("throwing", false);
              fireBallThrownAtTime = 0.0f;  
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
            gameController.Instance.healthPoints -= 30;
        }

    }
   
}

