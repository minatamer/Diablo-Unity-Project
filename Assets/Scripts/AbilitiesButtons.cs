using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilitiesButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isMouseOver = false;

    public void unlockDefensive()
{
    gameController.Instance.abilityPoints--;
    //Debug.Log("Defensive unlocked");
    gameController.Instance.locked[1] = -1;
    Button button = gameController.Instance.buttons[1];
    changecolorOfAButton( button); 
    gameController.Instance.cooldownVal[1].text = "0";
    
}

   public void unlockWild(){

        gameController.Instance.abilityPoints --;
        //Debug.Log("wild unlocked");
        gameController.Instance.locked[2] = -1;
        Button button = gameController.Instance.buttons[2];
        changecolorOfAButton( button); 
        gameController.Instance.cooldownVal[2].text = "0";


   }

    public void unlockUltimate(){

            gameController.Instance.abilityPoints --;
            //Debug.Log("ultimate unlocked");
            gameController.Instance.locked[3]= -1;
            Button button = gameController.Instance.buttons[3];
            changecolorOfAButton( button); 
            gameController.Instance.cooldownVal[3].text = "0";

   }

   private void changecolorOfAButton(Button button){
    button.interactable = true;
    Image buttonImage = button.GetComponent<Image>();
    buttonImage.color = Color.white;  
   }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        //Debug.Log("Mouse is over the button!");
    }

    // Called when the mouse exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        //Debug.Log("Mouse is no longer over the button.");
    }
    void Update()
    {
        //Unlocks all locked abilities
        if (Input.GetKeyDown(KeyCode.U))
        {
            gameController.Instance.abilityPoints += 3;
            unlockDefensive();
            unlockWild();
            unlockUltimate();

        }

    }
}
