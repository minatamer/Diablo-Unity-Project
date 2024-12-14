using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsRoll : MonoBehaviour
{
    [SerializeField] private RectTransform creditsTransform;
    [SerializeField] private RectTransform PanelTransform;
    [SerializeField] private float scrollSpeed = 50f;
    private float stopPositionY = 830f; 

    private void Update()
    {
        // Move the credits up
        Debug.Log(creditsTransform.anchoredPosition.y);
        if (creditsTransform.anchoredPosition.y < stopPositionY)
        {
            creditsTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }
        else
        {
            PanelTransform.gameObject.SetActive(false);
        }
    }
}
