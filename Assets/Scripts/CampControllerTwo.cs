using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CampControllerTwo : MonoBehaviour
{
    private List<GameObject> minions = new List<GameObject>();
    private List<Vector3> originalPositions = new List<Vector3>();

    private List<GameObject> aggressiveMinions = new List<GameObject>();
    private List<Vector3> aggressiveMinionsOriginalPositions = new List<Vector3>();

    private List<GameObject> demons = new List<GameObject>();
    private List<Vector3> originalPositionsDemons = new List<Vector3>();

    private List<GameObject> aggressiveDemons = new List<GameObject>();
    private List<Vector3> aggressiveDemonsOriginalPositions = new List<Vector3>();
    private bool runeInstantiated = false;

    void Update(){
         GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion2");
        GameObject[] allDemons = GameObject.FindGameObjectsWithTag("Demon");

         if(allMinions.Length == 0  && allDemons.Length ==0 && runeInstantiated == false){
              
                GameObject rune=  Instantiate(  gameController.Instance.RuneFragment, new Vector3(transform.position.x, -0.03f, transform.position.z), Quaternion.identity);
                rune.tag = "Rune";
                runeInstantiated = true;
       

         }
                
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion2");
            GameObject[] allDemons = GameObject.FindGameObjectsWithTag("Demon");


            foreach (GameObject minion in allMinions)
            {
                if (!minions.Contains(minion))
                {
                    minions.Add(minion);
                    originalPositions.Add(minion.transform.position);
                }
            }

            foreach (GameObject demon in allDemons)
            {
                if (!demons.Contains(demon))
                {
                    demons.Add(demon);
                    originalPositionsDemons.Add(demon.transform.position);
                }
            }


            // Ensure we don't exceed the number of available minions
            int minionCountToSelect = Mathf.Min(minions.Count, 5);
            int demonCountToSelect = Mathf.Min(demons.Count, 1);


            HashSet<int> selectedIndicesMinions = new HashSet<int>();
            HashSet<int> selectedIndicesDemons = new HashSet<int>();
            System.Random rnd = new System.Random();

            while (selectedIndicesMinions.Count < minionCountToSelect)
            {
                int randomIndex = rnd.Next(0, minions.Count);
                if (!selectedIndicesMinions.Contains(randomIndex))
                {
                    selectedIndicesMinions.Add(randomIndex);
                }
            }

            while (selectedIndicesDemons.Count < demonCountToSelect)
            {
                    
                int randomIndex = rnd.Next(0, demons.Count);
                if (!selectedIndicesDemons.Contains(randomIndex))
                {
                    selectedIndicesDemons.Add(randomIndex);
                }
            }

            // Process the selected minions
            foreach (int index in selectedIndicesMinions)
            {
                GameObject selectedMinion = minions[index];
                aggressiveMinions.Add(selectedMinion);
                aggressiveMinionsOriginalPositions.Add(originalPositions[index]);

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

            // Process the selected demons
            foreach (int index in selectedIndicesDemons)
            {
                GameObject selectedDemon = demons[index];
                aggressiveDemons.Add(selectedDemon);
                aggressiveDemonsOriginalPositions.Add(originalPositionsDemons[index]);

                NavMeshAgent agent = selectedDemon.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.SetDestination(other.transform.position);
                    agent.speed = 5.0f;
                }

            }

            // Remove selected minions from the original lists in reverse order to avoid index shifting
            List<int> indicesToRemove = new List<int>(selectedIndicesMinions);
            indicesToRemove.Sort((a, b) => b.CompareTo(a)); // Sort in descending order

            foreach (int index in indicesToRemove)
            {
                minions.RemoveAt(index);
                originalPositions.RemoveAt(index);
            }

            // Remove selected demons from the original lists in reverse order to avoid index shifting
            List<int> indicesToRemoveDemons = new List<int>(selectedIndicesDemons);
            indicesToRemoveDemons.Sort((a, b) => b.CompareTo(a)); // Sort in descending order

            foreach (int index in indicesToRemoveDemons)
            {
                demons.RemoveAt(index);
                originalPositionsDemons.RemoveAt(index);
            }


        }
    }

    private void OnTriggerStay(Collider other)
    {
        string tag = other.tag == "clonedPlayer" ? "clonedPlayer" : "Player";
        if (other.CompareTag(tag))
        {

            foreach (GameObject minion in aggressiveMinions)
            {

                if (minion != null)
                {
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
            }

            foreach (GameObject demon in aggressiveDemons)
            {
                if (demon != null)
                {
                    NavMeshAgent agent = demon.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        if (agent.destination != other.transform.position)
                        {

                            agent.SetDestination(other.transform.position);
                            agent.stoppingDistance = 2.5f;
                        }
                    }
                   
                }
            }


            int size = aggressiveMinions.Count;

            for (int j = size - 1; j >= 0; j--)
            {
                if (aggressiveMinions[j] == null)
                {
                    aggressiveMinions.RemoveAt(j);
                    aggressiveMinionsOriginalPositions.RemoveAt(j);
                    if (minions.Count > 0)
                    {
                        aggressiveMinions.Add(minions[0]);
                        minions.RemoveAt(0);
                        aggressiveMinionsOriginalPositions.Add(originalPositions[0]);
                        originalPositions.RemoveAt(0);

                    }

                }

            }

            int sizeDemons = aggressiveDemons.Count;

            for (int j = sizeDemons - 1; j >= 0; j--)
            {
                if (aggressiveDemons[j] == null)
                {
                    aggressiveDemons.RemoveAt(j);
                    aggressiveDemonsOriginalPositions.RemoveAt(j);
                    if (demons.Count > 0)
                    {
                        aggressiveDemons.Add(demons[0]);
                        demons.RemoveAt(0);
                        aggressiveDemonsOriginalPositions.Add(originalPositionsDemons[0]);
                        originalPositionsDemons.RemoveAt(0);

                    }

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
            for (int i = 0; i < aggressiveDemons.Count; i++)
            {
                NavMeshAgent agent = aggressiveDemons[i].GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.SetDestination(aggressiveDemonsOriginalPositions[i]);
                }
            }
            aggressiveMinions.Clear();
            aggressiveMinionsOriginalPositions.Clear();

            aggressiveDemons.Clear();
            aggressiveDemonsOriginalPositions.Clear();
        }
    }
}
