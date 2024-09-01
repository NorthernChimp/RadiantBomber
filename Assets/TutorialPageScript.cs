using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class TutorialPageScript : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> tutorialPages;
    public SpriteInteractionObject thisTutorialSprite;
    public SpriteRenderer nextButton;
    public SpriteRenderer prevButton;
    public Sprite gameOverScreen;
    public int currentTutorialPage = 0;
    Counter disableTimer = new Counter(0.5f);
    void Start()
    {
        //SetupTutorialPage();
    }
    public void DisableTutorial(){gameObject.SetActive(false);}
    public void SetupTutorialPage(){
        thisTutorialSprite = new SpriteInteractionObject(transform);
        currentTutorialPage = 0;
        thisTutorialSprite.ChangeTexture(tutorialPages[currentTutorialPage]);
        prevButton.enabled = true;
        nextButton.enabled = true;
    }
    public void ChangeTutorialPage(int changeAmount)
    {
        disableTimer.ResetTimer();
        prevButton.enabled = true;
        nextButton.enabled = true;
        int nextTutorialPage = currentTutorialPage += changeAmount;
        if(nextTutorialPage < tutorialPages.Count && nextTutorialPage >= 0)
        {
            currentTutorialPage = nextTutorialPage;
            thisTutorialSprite.ChangeTexture(tutorialPages[nextTutorialPage]);
        }
    }
    public void SetupEndGame(){
        thisTutorialSprite.ChangeTexture(gameOverScreen);
        nextButton.enabled = false;
        prevButton.enabled = false;
    }
    public void SwitchToThisPage(int pageReference)
    {
        disableTimer.ResetTimer();
        if (pageReference < tutorialPages.Count && pageReference >= 0)
        {
            currentTutorialPage = pageReference;
            thisTutorialSprite.ChangeTexture(tutorialPages[pageReference]);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(disableTimer.hasFinished)
        {
            for(int i = 0; i < Touchscreen.current.touches.Count;i++)
            {
                TouchControl t = Touchscreen.current.touches[i];
                if(t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    Vector3 touchPos = Camera.main.ScreenToWorldPoint(t.position.ReadValue());
                    Vector3 diff = touchPos - nextButton.transform.position;diff.z = 0f;
                    if(diff.magnitude < MainScript.blockHeight * 2.5f)
                    {
                        ChangeTutorialPage(1);
                    }else
                    {
                        diff = touchPos - prevButton.transform.position;diff.z = 0f;
                        if(diff.magnitude < MainScript.blockHeight * 2f)
                        {
                            ChangeTutorialPage(-1);
                        }
                    }
                }
            }
        }else{disableTimer.AddTime(Time.deltaTime);}
        
    }
}
