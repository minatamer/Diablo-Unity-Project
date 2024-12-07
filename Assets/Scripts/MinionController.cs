using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class MinionController : MonoBehaviour
{
    // Start is called before the first frame update
    public int hp = 20;
    Animator animator ;
    public Image healthBarImage;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other){

        if(other.gameObject.CompareTag("Player")){
                              animator.SetBool("Punch", true);

            if( PlayerPrefs.GetString("SelectedCharacter") == "Barbarian" ){//barbarian

                BarbarianAnimatorController barbarianScript = other.gameObject.GetComponent<BarbarianAnimatorController>();
               if(barbarianScript.shield == false){
                      gameController.Instance.healthPoints -= 5;
                        Animator player = other.gameObject.GetComponent<Animator>();
                         player.SetTrigger("hit"); 
               }
            }
            else{//arisa
                gameController.Instance.healthPoints -= 5;
                Animator player = other.gameObject.GetComponent<Animator>();
                player.SetTrigger("hit"); 
            }

        }
        


    }
    public void UpdateHealthBar()
    {
            healthBarImage.fillAmount =  ((float)hp / 20);
    }
    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

}
