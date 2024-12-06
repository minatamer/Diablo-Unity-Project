using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CampController : MonoBehaviour
{
    private List<GameObject> minions = new List<GameObject>();
    private List<Vector3> originalPositions = new List<Vector3>();

    private List<GameObject> aggressiveMinions = new List<GameObject>();
    private List<Vector3> aggressiveMinionsOriginalPositions = new List<Vector3>();
    private bool runeInstantiated = false;




    void Update(){
       
         GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
         if(allMinions.Length == 0 && runeInstantiated == false )  {
              
                GameObject rune=  Instantiate(  gameController.Instance.RuneFragment, new Vector3(transform.position.x, 0.597f, transform.position.z), Quaternion.identity);
                rune.tag = "Rune";
       
                runeInstantiated = true;
         

        }
                
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");

            foreach (GameObject minion in allMinions)
            {
                if (!minions.Contains(minion))
                {
                    minions.Add(minion);
                    originalPositions.Add(minion.transform.position);
                }
            }

           

            // Ensure we don't exceed the number of available minions
            int minionCountToSelect = Mathf.Min(minions.Count, 5);


            HashSet<int> selectedIndices = new HashSet<int>();
            System.Random rnd = new System.Random();

            while (selectedIndices.Count < minionCountToSelect)
            {
                int randomIndex = rnd.Next(0, minions.Count);
                if (!selectedIndices.Contains(randomIndex))
                {
                    selectedIndices.Add(randomIndex);
                }
            }

            // Process the selected minions
            foreach (int index in selectedIndices)
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

            // Remove selected minions from the original lists in reverse order to avoid index shifting
            List<int> indicesToRemove = new List<int>(selectedIndices);
            indicesToRemove.Sort((a, b) => b.CompareTo(a)); // Sort in descending order

            foreach (int index in indicesToRemove)
            {
                minions.RemoveAt(index);
                originalPositions.RemoveAt(index);
            }


        }
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
                    if (minions.Count > 0)
                    {
                        aggressiveMinions.Add(minions[0]);
                        minions.RemoveAt(0);
                        aggressiveMinionsOriginalPositions.Add(originalPositions[0]);
                        originalPositions.RemoveAt(0);

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
            aggressiveMinions.Clear();
            aggressiveMinionsOriginalPositions.Clear();
        }
    }
}
