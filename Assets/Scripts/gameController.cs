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
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public class gameController : MonoBehaviour
{
    public static gameController Instance { get; private set; }
    public int healthPoints = 100;
    public int xp = 0;
    public int level = 1;
    public int abilityPoints = 0;
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

    public bool bossLevel = false;

    // public TMP_Text[] abilitiesNames = new TMP_Text[4];
    public Button[] buttons = new Button[4];

    public TMP_Text[] cooldownVal = new TMP_Text[4];

    public int[] locked = new int[4];

    public Camera mainCamera;

    public bool invincibility = false;
    public bool slowMotion = false;
    public bool coolDownCheat = false;

    public GameObject leftGate;
    public GameObject rightGate;

    public Transform leftPivotPoint;
    public Transform rightPivotPoint;

    public bool openGate = true;

    private bool deathAnimation = false;

    public bool pause = false;

    public AudioSource audioSource;

    public AudioClip dieSound;


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button clicked!");
    }

    private void changecolorOfAButton(Button button)
    {
        button.interactable = true;
        Image buttonImage = button.GetComponent<Image>();
        buttonImage.color = Color.white;
    }

    void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure there's only one instance
            return;
        }
        Instance = this;
        cooldownVal[0].text = "0";
        cooldownVal[1].text = "_";
        cooldownVal[2].text = "_";
        cooldownVal[3].text = "_";


        locked[0] = -1;

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
        
        initializeButtonAbilities(buttons[1]);
        initializeButtonAbilities(buttons[2]);
        initializeButtonAbilities(buttons[3]);


        if (bossLevel == true && PlayerPrefs.HasKey("AbilityDefensive"))
        {
            locked[1] = PlayerPrefs.GetInt("AbilityDefensive");
            locked[2] = PlayerPrefs.GetInt("AbilityWild");
            locked[3] = PlayerPrefs.GetInt("AbilityUltimate");
            if (locked[1] == -1)
            {
                changecolorOfAButton(buttons[1]);
            }
            if (locked[2] == -1)
            {
                changecolorOfAButton(buttons[2]);
            }
            if (locked[3] == -1)
            {
                changecolorOfAButton(buttons[3]);
            }
        }


    }
    // Start is called before the first frame update
    void Start()
    {
        if (bossLevel == true && PlayerPrefs.HasKey("healthPoints"))
        {
            healingPotions = PlayerPrefs.GetInt("healingPotions");
            level = PlayerPrefs.GetInt("level");
            xp = PlayerPrefs.GetInt("xp");
            healthPoints = PlayerPrefs.GetInt("healthPoints");
            runeFragments= PlayerPrefs.GetInt("runeFragments");
            abilityPoints = PlayerPrefs.GetInt("abilityPoints");
        }

        else if (bossLevel == true)
        {
            level = 4;
            healthPoints = 400;
            abilityPoints = 3;
        }

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
                //newObject = Instantiate(characterToInstantiate, new Vector3(227.13f, 3.67f, 135.12f), Quaternion.identity);
                newObject = Instantiate(characterToInstantiate, new Vector3(230.8f, 3.67f, 17.6f), Quaternion.identity);
            }
                
            if (bossLevel == false)
            {
                initializeCamp(campOne, 10, 0, 1);
            }


            if (bossLevel == false)
            {
                initializeCamp(campTwo, 14, 2, 2);
            }

            if (bossLevel == false){
                initializeCamp(campThree, 18, 4, 3);
            }


            //initializePotions(-196f, 386.1f, 320.4f, -94f, 20);
            initializePotions(136.6f, 271.6f, 264.5f, 83.6f, 20);

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
        pause = true;
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
    private void initializeButtonAbilities(Button button){
        ColorBlock colorBlock = button.colors;
        colorBlock.disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); 
        button.colors = colorBlock;
        button.interactable = false ;
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
            //GameObject demon1 = Instantiate(DemonPrefab, new Vector3(260f, 3.67f, 187f), Quaternion.identity);
            //GameObject demon2 = Instantiate(DemonPrefab, new Vector3(230f, 3.67f, 187f), Quaternion.identity);
            GameObject demon1 = Instantiate(DemonPrefab, new Vector3(225f, 3.67f, 256.5f), Quaternion.identity);
            GameObject demon2 = Instantiate(DemonPrefab, new Vector3(192f, 3.67f, 256.5f), Quaternion.identity);
            demon1.tag = "Demon";
            demon2.tag = "Demon";
        }

        if(campNum == 3){
            //GameObject demon1 = Instantiate(DemonPrefab, new Vector3(260f, 3.67f, 92.4f), Quaternion.identity);
            //GameObject demon2 = Instantiate(DemonPrefab, new Vector3(215f, 3.67f, 61.4f), Quaternion.identity);
            //GameObject demon3 = Instantiate(DemonPrefab, new Vector3(215f, 3.67f, 92.4f), Quaternion.identity);
            //GameObject demon4 = Instantiate(DemonPrefab, new Vector3(212f, 3.67f, 92.4f), Quaternion.identity);
            GameObject demon1 = Instantiate(DemonPrefab, new Vector3(341.6f, 3.67f, 167f), Quaternion.identity);
            GameObject demon2 = Instantiate(DemonPrefab, new Vector3(309.8f, 3.67f, 135.4f), Quaternion.identity);
            GameObject demon3 = Instantiate(DemonPrefab, new Vector3(309.8f, 3.67f, 167f), Quaternion.identity);
            GameObject demon4 = Instantiate(DemonPrefab, new Vector3(307.5f, 3.67f, 167f), Quaternion.identity);
            demon1.tag = "Demon11";
            demon2.tag = "Demon12";
            demon3.tag = "Demon12";
            demon4.tag = "Demon11";
        }
               

    }
    
  
    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (healthPoints <= 0)
        {
            //Time.timeScale = 0f;
            healthPoints = 0;
            Animator animator = player.GetComponent<Animator>();
            if(deathAnimation == false)
            {
                animator.SetTrigger("die");
                audioSource.PlayOneShot(dieSound);
                deathAnimation = true;
            }


            // Start the coroutine to load the Game Over scene after a short delay
            StartCoroutine(LoadGameOverScene());
        }
        //Test if player has 3 runes and if within certain distance, start opening gate
        
        if(player != null)
        {
            float distance = Vector3.Distance(leftGate.transform.position, player.transform.position);
            if (runeFragments == 3 && distance < 20f)
            {
                Debug.Log("you are near gate");
                openGate = true;
            }
        }

        if (openGate == true)
        {
            if(leftGate.transform.eulerAngles.y < 90f)
            {
                Quaternion rotation = Quaternion.AngleAxis(20f * Time.deltaTime, Vector3.up);
                leftGate.transform.position = rotation * (leftGate.transform.position - leftPivotPoint.position) + leftPivotPoint.position;
                leftGate.transform.rotation = rotation * leftGate.transform.rotation;

                Quaternion rotationright = Quaternion.AngleAxis(-20f * Time.deltaTime, Vector3.up); // Negate the angle
                rightGate.transform.position = rotationright * (rightGate.transform.position - rightPivotPoint.position) + rightPivotPoint.position;
                rightGate.transform.rotation = rotationright * rightGate.transform.rotation;
            }


        }
       
            for(int i = 1; i<=3; i++){
                 if(abilityPoints > 0 ){
                     buttons[i].interactable = true;
                 }
                else if(locked[i] != -1){
                       buttons[i].interactable = false;
                 }
            }
            
       
         if(Input.GetKeyDown(KeyCode.F)){
            Debug.Log("pressed f");
         if(healthPoints != 100*level && healingPotions !=0){
                 Debug.Log("pressed f");
                 int increment = (int) (0.5*level*100);
                 healthPoints +=increment;
                 healthPoints = Math.Min(100*level,healthPoints);
                 healingPotions --;
                 if(PlayerPrefs.GetString("SelectedCharacter") == "Sorcerer"){
                     player = GameObject.FindWithTag("Player");
                     GameObject originalPlayer = GameObject.FindWithTag("OriginalPlayer");
                     sor_script playerScript;

                      if(player!= null){

                       playerScript = player.GetComponent<sor_script>();

                      }
                      else {
                       playerScript = originalPlayer.GetComponent<sor_script>();
                      }
                      playerScript.drink();
                      

                 }

                 else{//barbarian drink animation
                    player = GameObject.FindWithTag("Player");
                    GameObject originalPlayer = GameObject.FindWithTag("OriginalPlayer");
                    BarbarianAnimatorController playerScript;

                    if (player != null)
                    {

                        playerScript = player.GetComponent<BarbarianAnimatorController>();

                    }
                    else
                    {
                        playerScript = originalPlayer.GetComponent<BarbarianAnimatorController>();
                    }
                    playerScript.drink();
                }
                  

        }
        }
       
        if(xp >= 100*level && level!=4)
        {
            int overflow = xp - (100 * level) ;
            level++;
            abilityPoints++;
            xp = overflow;
            healthPoints = 100 * level;
        }


        ability_points_text.text = "Ability Points: " + abilityPoints+"| ";
        rune_fragments_count.text = "RF: " + runeFragments;
        healing_potions_text.text = "Healing Potions: "+ healingPotions+"| ";
        level_text.text = "|"+level.ToString()+ "|";
        points_text.text = healthPoints + "/" + (level * 100);
        xp_text.text = xp + "/"+  (level*100);
        healthBar.fillAmount = (float)healthPoints/(100*level);
        xpBar.fillAmount =  (float)xp/(100*level);


        //Cheats
        //Heal by 20
        if (Input.GetKeyDown(KeyCode.H))
        {
            healthPoints += 20;
            if(healthPoints > 100 * level)
            {
                healthPoints = 100 * level;
            }
        }
        //Decrement health by 20
        if (Input.GetKeyDown(KeyCode.D))
        {
            healthPoints -= 20;
            if (healthPoints < 0)
            {
                healthPoints = 0;
            }
        }
        //be invincible to damage
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (invincibility == false)
            {
                invincibility = true;
            }
            else
            {
                invincibility = false;
            }
        }

        //Makes the gameplay in half speed when enabled
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Toggle slow motion state
            slowMotion = !slowMotion;

            // Apply time scale
            Time.timeScale = slowMotion ? 0.5f : 1f;

            // Adjust fixed delta time to maintain consistent physics
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        //Sets the cool down time for all the abilities to 0 HANDELED IN BARBARIAN AND SCORCERER SCRIPTS
        //if (Input.GetKeyDown(KeyCode.C))
        //{

        //}

        //Unlocks all locked abilities HANDELED IN ABILITY BUTTONS SCRIPT
        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    locked[1] = -1;
        //    locked[2] = -1;
        //    locked[3] = -1;
        //}

        //Gain 1 ability point
        if (Input.GetKeyDown(KeyCode.A))
        {
            abilityPoints++;
        }

        //Increment XP by 100
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (level != 4)
            {
                xp += 100;
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(pause == false)
            {
                PauseGame();
            }
            else
            {
                PauseScene.Instance.Resume();
            }
            
        }




    }
    IEnumerator LoadGameOverScene()
    {
        yield return new WaitForSecondsRealtime(4f);
        SceneManager.LoadScene("GameOver");
    }




}
