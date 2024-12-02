using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class CampController : MonoBehaviour
{
    // Start is called before the first frame update
    //NavMeshAgent agent;
    [SerializeField] GameObject[] minions;
    private Vector3[] originalPositions;
    private int aggressiveCount = 0;
    private List<int> aggressiveMinions;
    void Start()
    {
        originalPositions = new Vector3[10];
        aggressiveMinions = new List<int>();
        for (int i = 0; i < minions.Length; i++)
        {
            if (minions[i] != null) // Ensure the minion exists
            {
                originalPositions[i] = minions[i].transform.position;
                Debug.Log(minions[i].transform.position);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (aggressiveCount == 0 && minions.Length > 0)
        {
            stopRunning();
        }
        
    }

    void stopRunning()
    {
        for (int i = 0; i < minions.Length; i++)
        {
            NavMeshAgent agent = minions[i].GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                Animator animator = minions[i].GetComponent<Animator>();

                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    // Agent has reached the destination
                    if (animator != null)
                    {
                        animator.SetBool("Run", false);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    minions = GameObject.FindGameObjectsWithTag("Minion");
        //}

        if (other.gameObject.CompareTag("Player")) 
        {
            for (int i = 0; i < minions.Length; i++)
            {
                if (aggressiveCount == 5)
                    break;
                if (Random.value < 0.5f)
                {
                    aggressiveMinions.Add(i);
                    aggressiveCount++;
                    NavMeshAgent agent = minions[i].GetComponent<NavMeshAgent>();

                    if (agent != null)
                    {
                        agent.SetDestination(other.transform.position);
                        
                    }

                    Animator animator = minions[i].GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetBool("Run", true); 
                    }
                }
                
            }

            //make sure the minions that are chosen their destination keeps getting updated
            for (int i = 0; i < aggressiveMinions.Count; i++)
            {
                int index = aggressiveMinions[i];
                NavMeshAgent agent = minions[index].GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.SetDestination(other.transform.position);

                }

                //Animator animator = minions[i].GetComponent<Animator>();
                //if (animator != null)
                //{
                //    animator.SetBool("Run", true);
                //}
            }
        }
    }

        private void OnTriggerExit(Collider other)
    {    
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < minions.Length; i++)
            {
                if (minions[i] != null)
                {
                    NavMeshAgent agent = minions[i].GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        agent.SetDestination(originalPositions[i]);
                    }

                }
            }
        }
        aggressiveCount = 0;
        aggressiveMinions = new List<int>();

    }

    

}
