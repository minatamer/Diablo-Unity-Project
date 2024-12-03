using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CampControllerTwo : MonoBehaviour
{
    private List<GameObject> minions = new List<GameObject>();
    private List<Vector3> originalPositions = new List<Vector3>();
    private int aggressiveCount = 0;

    private List<GameObject> aggressiveMinions = new List<GameObject>();
    private List<Vector3> aggressiveMinionsOriginalPositions = new List<Vector3>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion2");

            foreach (GameObject minion in allMinions)
            {
                if (!minions.Contains(minion))
                {
                    minions.Add(minion);
                    originalPositions.Add(minion.transform.position);
                }
            }

            // Select random minions to become aggressive
            for (int i = minions.Count - 1; i >= 0 && aggressiveCount < 5; i--)
            {
                if (Random.value < 0.5f)
                {
                    GameObject selectedMinion = minions[i];
                    aggressiveMinions.Add(selectedMinion);
                    aggressiveMinionsOriginalPositions.Add(originalPositions[i]);

                    // Remove the minion from the non-aggressive list
                    minions.RemoveAt(i);
                    originalPositions.RemoveAt(i);

                    aggressiveCount++;

                    // Set NavMeshAgent to target the player
                    NavMeshAgent agent = selectedMinion.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                       

                        agent.SetDestination(other.transform.position);
                           agent.speed = 5.0f; 
                    }

                    // Trigger the "Run" animation
                    Animator animator = selectedMinion.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetBool("Run", true);
                    }
                }
            }
        }
        Debug.Log(minions.Count);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int i =0;
            foreach (GameObject minion in aggressiveMinions)
            {
               
                if(minion!=null){
                NavMeshAgent agent = minion.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    if (agent.destination != other.transform.position)
                    {

                        agent.SetDestination(other.transform.position);
                        agent.stoppingDistance = 2.5f;
                    }
                }
                Animator animator = minion.GetComponent<Animator>();
                if (animator != null && !animator.GetBool("Run"))
                {
                    animator.SetBool("Run", true);
                }
                }
                
              
                
                 i ++;
            }
            int size = aggressiveMinions.Count;
           
            for(int j =size -1;j>=0;j--){
            if(aggressiveMinions[j] == null){
                aggressiveMinions.RemoveAt(j);
                aggressiveMinionsOriginalPositions.RemoveAt(j);
           }

           }
        }
       

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < aggressiveMinions.Count; i++)
            {
                NavMeshAgent agent = aggressiveMinions[i].GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.SetDestination(aggressiveMinionsOriginalPositions[i]);
                }

                Animator animator = aggressiveMinions[i].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("Run", false);
                }
            }
            aggressiveCount = 0;
            aggressiveMinions.Clear();
            aggressiveMinionsOriginalPositions.Clear();
        }
    }
}
