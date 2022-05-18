using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPageScript : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> tutorialPages;
    public SpriteInteractionObject thisTutorialSprite;
    public int currentTutorialPage = 0;
    void Start()
    {
        thisTutorialSprite = new SpriteInteractionObject(transform);
    }
    public void ChangeTutorialPage(int changeAmount)
    {
        int nextTutorialPage = currentTutorialPage += changeAmount;
        if(nextTutorialPage < tutorialPages.Count && nextTutorialPage >= 0)
        {
            currentTutorialPage = nextTutorialPage;
            thisTutorialSprite.ChangeTexture(tutorialPages[nextTutorialPage]);
        }
    }
    public void SwitchToThisPage(int pageReference)
    {
        if (pageReference < tutorialPages.Count && pageReference >= 0)
        {
            currentTutorialPage = pageReference;
            thisTutorialSprite.ChangeTexture(tutorialPages[pageReference]);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
