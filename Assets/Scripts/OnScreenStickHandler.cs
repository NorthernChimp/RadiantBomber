using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnScreenStickHandler : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    // Start is called before the first frame update
    public static bool movementDoubleClick = false;
    public static bool utilityDoubleClick = false;
    public bool isLeftSide = true;
    private const float doubleTapTime = 0.3f;
    private int tapCount = 0;
    private float lastTapTime = 0f;
    bool inUse = false;
    bool doubleClick = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        // Call your tap detection logic
        HandleTap();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        UnTap();
    }
    private void UnTap(){
        inUse = false;
        if(isLeftSide){movementDoubleClick = false;MainScript.interfaceHighlightPanels[0].ChangeColor(Color.grey);}
        else{utilityDoubleClick = false;MainScript.interfaceHighlightPanels[1].ChangeColor(Color.grey);}
    }
    private void HandleTap()
    {
        inUse = true;
        if (Time.time - lastTapTime < doubleTapTime)
        {
            tapCount++;

            if (tapCount == 2)
            {
                //Debug.Log("Double Tap Detected is Left : " + isLeftSide);
                // Handle double-tap logic here
                doubleClick = true;
                if(isLeftSide){movementDoubleClick = true;MainScript.interfaceHighlightPanels[0].ChangeColor(Color.green);}
                else{utilityDoubleClick = true;MainScript.interfaceHighlightPanels[1].ChangeColor(Color.green);}
                tapCount = 0;
            }else
            {
                if(isLeftSide){movementDoubleClick = false;MainScript.interfaceHighlightPanels[0].ChangeColor(Color.blue);}
                else{utilityDoubleClick = false;MainScript.interfaceHighlightPanels[1].ChangeColor(Color.blue);}
                doubleClick = false;
            }
        }
        else
        {
            if(isLeftSide){movementDoubleClick = false;MainScript.interfaceHighlightPanels[0].ChangeColor(Color.blue);}
            else{utilityDoubleClick = false;MainScript.interfaceHighlightPanels[1].ChangeColor(Color.blue);}
            doubleClick = false;
            tapCount = 1;
        }
        lastTapTime = Time.time;
    }
}
