using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;
using Unity.VisualScripting;
using System.Runtime.InteropServices;
using UnityEditor.Rendering;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement; 
public class gameController : MonoBehaviour
{

    public static gameController Instance { get; private set; }
    public int healthPoints = 100;
    private int xp = 0;
    public int level = 1;
    private int abilityPoints = 0;
    public int healingPotions = 0;
    public int runeFragments = 0;
    public TMP_Text points_text;
    public TMP_Text xp_text;
    public TMP_Text level_text;
    public TMP_Text healing_potions_text;
    public TMP_Text ability_points_text;
    public TMP_Text rune_fragments_count;
    public Image healthBar;
    public Image xpBar;

    public GameObject brutePrefab;  // Assign the Brute prefab in the Inspector
    public GameObject arissaPrefab;

    public GameObject Camp;
    public GameObject MinionPrefab;
    public GameObject DemonPrefab;

    public GameObject RuneFragment;
    public GameObject Potion;
    public GameObject campOne;
    public GameObject campTwo;
    public GameObject campThree;
    
    public bool barbarianShield = false;

    public bool bossLevel = true;

    // public TMP_Text[] abilitiesNames = new TMP_Text[4];
    public Button[] buttons = new Button[4];

    public TMP_Text[] cooldownVal = new TMP_Text[4];

    private int[] locked = new int[4];

    public Camera mainCamera;



    void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure there's only one instance
            return;
        }
        Instance = this;
        cooldownVal[0].text = "1";
        cooldownVal[1].text = "_";
        cooldownVal[2].text = "_";
        cooldownVal[3].text = "_";




        if (PlayerPrefs.GetString("SelectedCharacter") == "Sorcerer") {
            buttons[0].GetComponentInChildren<TMP_Text>().text = "Fireball";
            buttons[1].GetComponentInChildren<TMP_Text>().text = "Teleport";
            buttons[2].GetComponentInChildren<TMP_Text>().text = "Clone";
            buttons[3].GetComponentInChildren<TMP_Text>().text = "Inferno";


        }
        else {
            buttons[0].GetComponentInChildren<TMP_Text>().text = "Bash";
            buttons[1].GetComponentInChildren<TMP_Text>().text = "Shield";
            buttons[2].GetComponentInChildren<TMP_Text>().text = "Iron Maelstrom";
            buttons[3].GetComponentInChildren<TMP_Text>().text = "Charge";
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter");
        Debug.Log($"Selected character is: {selectedCharacter}");
        GameObject characterToInstantiate = null;
        //if (pauseButton != null)
        //{
        //    pauseButton.onClick.AddListener(PauseGame);
        //}
        // Check the selected character and assign the corresponding prefab
        if (selectedCharacter == "Barbarian")
        {
            characterToInstantiate = brutePrefab;  // Assign Brute prefab
        }
        else if (selectedCharacter == "Sorcerer")
        {
            characterToInstantiate = arissaPrefab; // Assign Arissa prefab
        }

        if (characterToInstantiate != null)
        {
            GameObject newObject;
            if (bossLevel == true)
            {
                newObject = Instantiate(characterToInstantiate, new Vector3(0f, 4f, -24f), Quaternion.identity);
            }
            else
            {
                newObject = Instantiate(characterToInstantiate, new Vector3(227.13f, 3.67f, 135.12f), Quaternion.identity);
            }
                
            // GameObject camp = Instantiate(Camp, new Vector3(200.13f, -10f, 135.12f), Quaternion.identity);
            if (bossLevel == false)
            {
                initializeCamp(campOne, 10, 0, 1);
            }


            // camp.AddComponent<CampController>();
            // GameObject campTwo = Instantiate(Camp, new Vector3(244.13f, -10f, 171.12f), Quaternion.identity);
            if (bossLevel == false)
            {
                initializeCamp(campTwo, 14, 2, 2);
            }

            // campTwo.AddComponent<CampControllerTwo>();


            //GameObject campThree = Instantiate(Camp, new Vector3(244.13f, -10f,81.2f), Quaternion.identity);
            if (bossLevel == false){
                initializeCamp(campThree, 18, 4, 3);
            }

                
           // campThree.AddComponent<CampControllerThree>();


             initializePotions( -196f,  386.1f,  320.4f ,  -94f ,  20);
             
            newObject.SetActive(true);

            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.target = newObject.transform;
                cameraFollow.offset = new Vector3(0, 10, -10); // Adjust offset as needed
                cameraFollow.rotationAngle = new Vector3(45, 0, 0);
            }
        }
        else
        {
            Debug.LogError("No character selected or character prefab not assigned.");
        }
    }
    public void PauseGame()
    {
        if (SceneManager.GetSceneByName("PauseScene").isLoaded)
        {
            Debug.LogWarning("PauseScene is already loaded.");
            return;
        }

        Debug.Log("Game Paused");
        Time.timeScale = 0; // Pause the game

        // Load the Pause Scene additively
        SceneManager.LoadScene("PauseScene", LoadSceneMode.Additive);
    }

  private void initializePotions(float xMin, float xMax, float zMax , float zMin , int numberOfPotions){

    for(int i=0;i<numberOfPotions;i++){
          System.Random random = new System.Random();

    float x = (float)(random.NextDouble() * (xMax - (xMin)) + (xMin));
    float z = (float)(random.NextDouble() * (zMax - (zMin)) + (zMin));
    GameObject potion = Instantiate(Potion, new Vector3(x, 0.84f, z),  Quaternion.Euler(0, 0, 90));
    potion.tag= "Potion";

    }
  }
    private void initializeCamp(GameObject camp ,  int numberOfMinions, int numberofDemons,int campNum){
         Collider campCollider = camp.GetComponent<Collider>();
         Bounds bounds = campCollider.bounds;
         initializePotions( bounds.min.x,  bounds.max.x,   bounds.max.z ,   bounds.min.z , 2);

        float spacing = 2.0f; 
        Vector3 startPosition = camp.transform.position + new Vector3(-((numberOfMinions - 1) * spacing) / 2, 0, 0); // Start centered around camp

        for (int i = 0; i < numberOfMinions; i++)
        {
            float randomOffsetY = UnityEngine.Random.Range(-0.5f, 0.5f);
            float randomOffsetZ = UnityEngine.Random.Range(-0.5f, 0.5f);
            Vector3 minionPosition = startPosition + new Vector3(i * spacing, randomOffsetY, randomOffsetZ);
            GameObject minion = Instantiate(MinionPrefab, minionPosition, Quaternion.identity);
            if(campNum == 1){
            minion.tag = "Minion"; 
            }
             if(campNum == 2){
            minion.tag = "Minion2"; 
            }
            if(campNum == 3){
            minion.tag = "Minion3"; 
            }
            minion.transform.parent = camp.transform;
        }

        if (campNum == 2)
        {
            GameObject demon1 = Instantiate(DemonPrefab, new Vector3(260f, 3.67f, 187f), Quaternion.identity);
            GameObject demon2 = Instantiate(DemonPrefab, new Vector3(230f, 3.67f, 187f), Quaternion.identity);
            demon1.tag = "Demon";
            demon2.tag = "Demon";
        }

        if(campNum == 3){

            GameObject demon1 = Instantiate(DemonPrefab, new Vector3(260f, 3.67f, 92.4f), Quaternion.identity);
            GameObject demon2 = Instantiate(DemonPrefab, new Vector3(215f, 3.67f, 61.4f), Quaternion.identity);
            GameObject demon3 = Instantiate(DemonPrefab, new Vector3(215f, 3.67f, 92.4f), Quaternion.identity);
            GameObject demon4 = Instantiate(DemonPrefab, new Vector3(212f, 3.67f, 92.4f), Quaternion.identity);
            demon1.tag = "Demon11";
            demon2.tag = "Demon12";
             demon3.tag = "Demon12";
            demon4.tag = "Demon11";
        }
        
        

         

    }
     void OnButtonClicked(Button clickedButton)
    {
        // Get and use the clicked button's instance
        Debug.Log("Clicked Button: " + clickedButton.GetComponentInChildren<TMP_Text>().text);
    }
    public void updateAbilities()
{
          foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button)); // Pass the button instance
        }
    }
    // Update is called once per frame
    void Update()
    {
         if(Input.GetKeyDown(KeyCode.F)){
            Debug.Log("pressed f");
         if(healthPoints != 100*level && healingPotions !=0){
                 Debug.Log("pressed f");
                 int increment = (int) (0.5*level*100);

                 healthPoints +=increment;
                 healthPoints = Math.Min(100*level,healthPoints);
                 healingPotions --;
                


        }
        }
        if(level == 4 ){
            xp = 400;
        }
        updateAbilities();
        ability_points_text.text = "Ability Points: " + abilityPoints+"| ";
        rune_fragments_count.text = "RF: " + runeFragments;
        healing_potions_text.text = "Healing Potions: "+ healingPotions+"| ";
        level_text.text = "|"+level.ToString()+ "|";
        points_text.text = healthPoints + "/100";
        xp_text.text = xp + "/"+  (level*100);
        healthBar.fillAmount = (float)healthPoints/100;
        xpBar.fillAmount =  (float)xp/(100*level);

    }

   

}
