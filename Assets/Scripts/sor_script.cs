using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sor_script : MonoBehaviour
{
    Animator animator ;
    NavMeshAgent navMeshAgent;
    private Vector3 hitPoint;
    private Vector3 targetPosition;
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
        Destroy(clonedInstance, 5f);
        yield return new WaitForSeconds(5f); 
        cloneLastAbilityTime = Time.time; 
    }
    void Update()
    {

       if (Input.GetMouseButtonDown(1) && waitingForRightClick == false && waitingForRightUltimate == false && waitingForRightDefensive == false) 
        {
            animator.SetBool("throwing", true);
            currentFireball = Instantiate(fireball , shootingPoint.transform.position  , Quaternion.identity);
            currentFireball.transform.parent =  shootingPoint;

            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;
                Debug.Log("Hit point: " + hitPoint);
                targetPosition = hitPoint;
            }
            else{                
                targetPosition =  mousePos;
            }
            }


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Throwing") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {    
             if(fireBallDestroyed == false && currentFireball != null){ // target reached or not 

                animator.SetBool("throwing", false);
                currentFireball.transform.parent = null;
                FireInStraightLine(targetPosition);

             }
        
        }

        if(fireBallDestroyed == true ){
            fireBallDestroyed = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && UseAbility(infernoLastAbilityTime , 15) == true)
        {
            Debug.Log("E key pressed. Waiting for right mouse click...");
            waitingForRightUltimate = true; 
            infernoLastAbilityTime  = Time.time ;
        }
      
        if (waitingForRightUltimate && Input.GetMouseButtonDown(1))
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

       
        if (Input.GetKeyDown(KeyCode.W) && UseAbility(teleportLastAbilityTime , 10) == true)
        {
            Debug.Log("W key pressed. Waiting for right mouse click...");
            waitingForRightDefensive = true; 
            teleportLastAbilityTime = Time.time;
        }
      
        if (waitingForRightDefensive && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right mouse button clicked after pressing W!");
            Vector3 spawnPosition = GetMouseWorldPosition();
            navMeshAgent.transform.position = spawnPosition;
            waitingForRightDefensive = false; 
        }       
       
       if (Input.GetKeyDown(KeyCode.Q) && UseAbility(cloneLastAbilityTime , 10) == true)
        {
            Debug.Log("Q key pressed. Waiting for right mouse click...");
            waitingForRightClick = true; 
        }
        
        if (waitingForRightClick && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right mouse button clicked after pressing Q!");
            waitingForRightClick = false;
            Vector3 spawnPosition = GetMouseWorldPosition();
             if (spawnPosition != Vector3.zero) // Ensure a valid position was returned
            {
                 clonedinstance = Instantiate(sorcererClone, spawnPosition, Quaternion.identity);
                 Animator cloneAnimator = clonedinstance.GetComponent<Animator>();
                 AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
                 cloneAnimator.runtimeAnimatorController = overrideController;
                 clonedinstance.transform.localScale = transform.localScale;
                 clonedinstance.transform.localRotation = transform.localRotation;
                 Debug.Log("Sorcerer clone created at: " + spawnPosition);
                 //clone created in a place where enemy can track him 
                 //enemy run towards the clone 
                 //clone explodes   damage to the enemies surrounding the area
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
        
       
    
}
