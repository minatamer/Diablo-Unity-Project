using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class gameController : MonoBehaviour
{
    private int healthPoints = 100;
    private int xp = 0;
    private int level = 1;
    private int abilityPoints = 0;
    private int healingPotions = 0;
    private int runeFragments = 0;
    public TMP_Text points_text ;
    public TMP_Text xp_text ;
    public TMP_Text level_text ;
    public TMP_Text healing_potions_text ;
    public TMP_Text ability_points_text ;
    public TMP_Text rune_fragments_count ;
    public Image healthBar;
    public Image xpBar;

    public GameObject brutePrefab;  // Assign the Brute prefab in the Inspector
    public GameObject arissaPrefab;


    // public TMP_Text[] abilitiesNames = new TMP_Text[4];
    public Button[] buttons = new Button[4];

    public TMP_Text[] cooldownVal = new TMP_Text[4];

    private int[] locked = new int[4];


    void Awake(){
           cooldownVal[0].text = "1";
           cooldownVal[1].text = "_";
           cooldownVal[2].text = "_";
           cooldownVal[3].text = "_";

          

        
       if (PlayerPrefs.GetString("SelectedCharacter")== "Sorcerer"){ 
          buttons[0].GetComponentInChildren<TMP_Text>().text = "Fireball";
          buttons[1].GetComponentInChildren<TMP_Text>().text= "Teleport";
          buttons[2].GetComponentInChildren<TMP_Text>().text= "Clone";
          buttons[3].GetComponentInChildren<TMP_Text>().text = "Inferno";
        

      }
      else{
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
            // Instantiate the selected character at position (-3, 0, -7)
            Instantiate(characterToInstantiate, new Vector3(-3, 0, -7), Quaternion.identity);
        }
        else
        {
            Debug.LogError("No character selected or character prefab not assigned.");
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
