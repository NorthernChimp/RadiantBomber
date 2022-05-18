using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class MainMenuScript : MonoBehaviour
{
    public static bool isBuyingRemoveAds = false;

    public Sprite molecularBurstSprite;
    public Sprite pusherBulletSprite;
    public Sprite shurikenSprite;
    public Sprite speedBoostSprite;
    public Sprite timeBombSprite;
    public Sprite knockBackGrenadeSprite;
    public Sprite blunderbussSprite;
    public Sprite slingShotJumpSprite;
    public Sprite microMissileSprite;
    public Sprite shotgunSprite;
    public Sprite explosiveDroneSprite;
    public Sprite gravityFieldSprite;
    public Sprite gameOverScreen;

    public Vector3 menuOpenPosition;
    public Vector3 menuClosedPosition;
    public static int currentTutorialPage = 0;
    public static bool isOpeningMenu = false;
    public float width;
    public float height;
    public static bool menuIsActive = false;
    public Vector3 directionToMoveTutorialScreen = Vector3.zero;
    public Vector3 lowerLeftCorner;
    public Vector3 headerArea;
    public Transform menuIcon;
    public GameObject menuBorderBlockPrefab;
    public GameObject basicWhiteBlockPrefab;
    public GameObject playGameButtonPrefab;
    public GameObject highScoreButtonPrefab;
    public GameObject tutorialButtonPrefab;
    public GameObject removeAdsButtonPrefab;
    public GameObject loadoutButtonPrefab;
    public GameObject loadoutIconPrefab;
    public GameObject iconSelectorPrefab;
    public GameObject tapToGoBackPrefab;
    public GameObject firstPlacePrefab;
    public GameObject secondPlacePrefab;
    public GameObject thirdPlacePrefab;
    public GameObject titlePrefab;
    public GameObject tutorialPagePrefab;
    public GameObject menuIconPrefab;

    //public static Button removeAdsButton;
    public static Transform iapTransform;
    MyIAPManager iapManager;

    public static Transform tutorialScreenParent;
    public Transform tutorialScreenParentHolder;
    public static Vector3 tutorialScreenParentOriginalPosition;
    public Transform playGameButton;
    public Transform loadoutButton;
    public Transform HighScoreButton;
    public Transform tutorialButton;
    public Transform removeAdsButton;

    public Counter disableControlsCounter;
    public List<AbilityIcon> allIcons;
    public List<SpriteInteractionObject> allRenders;
    public List<SpriteInteractionObject> highScorePlacers;
    public List<SpriteInteractionObject> tutorialScreenRenders;
    public List<SpriteInteractionObject> tutorialPageRenders;
    public SpriteInteractionObject tapToGobackButton;
    public SpriteInteractionObject titleScreen;
    public MenuState currentState = MenuState.hidden;
    public Transform backgroundSprite;
    public IconSelecter[] iconSelectors;
    
    public List<MenuButton> allButtons;
    public List<UtilityAbilityReference> allUtilityAbilities;
    public List<TriggerAbilityReference> allTriggerAbilities;
    // Start is called before the first frame update
    void Start()
    {
        //SetUpMenu();   
    }
    public void SetupDefaultAbilities()
    {
        SelectIcon(allTriggerAbilities[0].thisReferenceIcon);
        SelectIcon(allUtilityAbilities[1].thisReferenceIcon);
        SelectIcon(allUtilityAbilities[0].thisReferenceIcon);
    }
    public void SetUpMenu()
    {
        iapManager = iapTransform.GetComponent<MyIAPManager>();
        //removeAdsButton.transform.localScale = new Vector3((0.5f * Screen.width )/ 124f, (0.5f *Screen.width )/ 124f, 1f);
        menuOpenPosition = transform.position;
        menuClosedPosition = menuOpenPosition + new Vector3(Screen.width * 0.01f, Screen.height * 0.01f, 0f);
        tutorialScreenParent = tutorialScreenParentHolder;
        tutorialScreenParent.parent = null;
        tutorialScreenParent.position = new Vector3(0f, (Screen.height * 0.005f) - (Screen.width * 0.005f), -3f);
        tutorialScreenParentOriginalPosition = tutorialScreenParent.position;
        disableControlsCounter = new Counter(0.1f);
        iconSelectors = new IconSelecter[3];
        for(int i = 0;i < 3; i++)
        {
            iconSelectors[i] = new IconSelecter(Instantiate(iconSelectorPrefab, new Vector3(transform.position.x, transform.position.y,-3f), Quaternion.identity).transform);
            iconSelectors[i].theTransform.localScale = new Vector3(MainScript.blockLocalScale, MainScript.blockLocalScale, 1f);
            iconSelectors[i].theTransform.parent = transform;
        }
        allIcons = new List<AbilityIcon>();
        allTriggerAbilities = new List<TriggerAbilityReference>() 
        { new TriggerAbilityReference(0.25f, "PusherBulletPrefab",pusherBulletSprite,"Pusher Bullet"),
            new TriggerAbilityReference(1f,"ShurikenPrefab",shurikenSprite,"Shuriken"),
            new TriggerAbilityReference(0.35f,"MicroMissilePrefab",microMissileSprite,"Micro Missiles"),
            new TriggerAbilityReference(1.2f,"ShotgunPrefab",shotgunSprite, "Shotgun") };
        allUtilityAbilities = new List<UtilityAbilityReference>
        {
            new UtilityAbilityReference(UtilityActivatesOn.begin, 2.5f, "KnockBackExplosionPrefab", false,timeBombSprite,InterfaceType.Shoot,true,"Time Bomb"),
            new UtilityAbilityReference(UtilityActivatesOn.shoot, 4f, "MolecularBurstPrefab", true,molecularBurstSprite,InterfaceType.Move,true,"Molecular Burst"),
            new UtilityAbilityReference(UtilityActivatesOn.begin,6.5f,"SpeedBoostPrefab",false,speedBoostSprite,InterfaceType.Move,false,"SpeedBoost") ,
            new UtilityAbilityReference(UtilityActivatesOn.shoot, 3.25f, "KnockBackGrenadePrefab", true, knockBackGrenadeSprite, InterfaceType.Shoot,true,"Knockback Grenade") ,
            new UtilityAbilityReference(UtilityActivatesOn.shoot,3.5f,"BlunderBussPrefab",true,blunderbussSprite,InterfaceType.Shoot,true,"Sonic Blunderbuss"),
            new UtilityAbilityReference(UtilityActivatesOn.shoot,5.75f,"SlingShotJumpPrefab",true,slingShotJumpSprite,InterfaceType.Move,false,"Slingshot Jump"),
            new UtilityAbilityReference(UtilityActivatesOn.shootThenFollowThrough,2.5f,"ExplosiveDronePrefab",true,explosiveDroneSprite,InterfaceType.Shoot,false,"Explosive Drone"),
            new UtilityAbilityReference(UtilityActivatesOn.begin,5.5f,"GravityFieldPrefab",false, gravityFieldSprite,InterfaceType.Move,false,"Gravity Field")
        };
        
        for (int i = 0; i < allUtilityAbilities.Count; i++) 
        {
            UtilityAbilityReference currentReference = allUtilityAbilities[i];
            AbilityIcon newIcon = new AbilityIcon(Instantiate(loadoutIconPrefab, transform.position, Quaternion.identity).transform, currentReference.iconSprite,currentReference.abilityType);
            currentReference.thisReferenceIcon = newIcon;
            newIcon.theTransform.parent = transform;
            allIcons.Add(newIcon);
        }
        for (int i = 0; i < allTriggerAbilities.Count; i++)
        {
            TriggerAbilityReference currentReference = allTriggerAbilities[i];
            AbilityIcon newIcon = new AbilityIcon(Instantiate(loadoutIconPrefab, transform.position, Quaternion.identity).transform, currentReference.triggerAbilityIcon, InterfaceType.Trigger);
            currentReference.thisReferenceIcon = newIcon;
            newIcon.theTransform.parent = transform;
            allIcons.Add(newIcon);
        }
        allButtons = new List<MenuButton>();
        menuIsActive = true;
        backgroundSprite = Instantiate( basicWhiteBlockPrefab, transform.position + new Vector3(MainScript.blockWidth * -0.5f, MainScript.blockWidth * -0.5f,0f), Quaternion.identity).transform;
        backgroundSprite.transform.parent = transform;
        backgroundSprite.localScale = new Vector3(MainScript.blockLocalScale * 14f, MainScript.blockLocalScale * 28f, 1f);
        allRenders = new List<SpriteInteractionObject>() { new SpriteInteractionObject(backgroundSprite) };
        allRenders[0].ChangeColor(Color.black);
        allRenders[0].ChangeAlpha(0.85f);
        tapToGobackButton = new SpriteInteractionObject(Instantiate(tapToGoBackPrefab, transform.position, Quaternion.identity).transform);
        tapToGobackButton.ChangeVisibility(false);
        tapToGobackButton.renderer.transform.parent = transform;
        highScorePlacers = new List<SpriteInteractionObject>() { new SpriteInteractionObject(Instantiate(firstPlacePrefab, transform.position, Quaternion.identity).transform), new SpriteInteractionObject(Instantiate(secondPlacePrefab, transform.position, Quaternion.identity).transform), new SpriteInteractionObject(Instantiate(thirdPlacePrefab, transform.position, Quaternion.identity).transform) };
        foreach(SpriteInteractionObject s in highScorePlacers) { s.renderer.transform.parent = transform; }
        foreach(SpriteInteractionObject spr in highScorePlacers)
        {
            spr.renderer.transform.localScale = new Vector3(MainScript.blockLocalScale, MainScript.blockLocalScale, 1f);
            spr.ChangeVisibility(false);
            //allRenders.Add(spr);
        }

        lowerLeftCorner = transform.position + new Vector3(MainScript.blockWidth * -8f, MainScript.blockWidth * -15f, 0f);
        float blockWidth = (Screen.width / 20f) * 0.01f;
        Vector3 menuIconLowerLeftCorner = transform.position + new Vector3(MainScript.blockWidth * -11.5f, (Screen.height * -0.005f) + (blockWidth * -1.5f), 0f);
        menuIcon = Instantiate(menuIconPrefab, menuIconLowerLeftCorner,Quaternion.identity).transform;
        menuIcon.parent = transform;
        menuIcon.localScale = new Vector3(MainScript.blockLocalScale, MainScript.blockLocalScale, 1f);
        headerArea = lowerLeftCorner + new Vector3(MainScript.blockWidth * 8f, MainScript.blockWidth * 22f, 0f) + new Vector3(MainScript.blockWidth * -0.5f, MainScript.blockWidth * -0.5f,-3f);
        titleScreen = new SpriteInteractionObject(Instantiate(titlePrefab, headerArea, Quaternion.identity).transform);
        titleScreen.renderer.transform.parent = transform;
        titleScreen.renderer.transform.localScale = new Vector3(MainScript.blockLocalScale, MainScript.blockLocalScale, 1f);
        for(int x = 0;x < 16; x++)
        {
            for(int y = 0;y < 30;y++)
            {
                if(y != 0 && y != 29 && x != 0 && x != 15)
                { 
                } else
                {
                    Vector3 posToInstantiate = lowerLeftCorner + new Vector3(x * MainScript.blockWidth, y * MainScript.blockWidth, 0f);
                    Transform temp = Instantiate(menuBorderBlockPrefab, posToInstantiate, Quaternion.identity).transform;
                    temp.parent = transform;
                    temp.localScale = new Vector3(MainScript.blockLocalScale, MainScript.blockLocalScale, 1f);
                    allRenders.Add(new SpriteInteractionObject(temp));
                    temp.parent = transform;
                }
            }
        }
        //Vector3 tutorialScreenLowerLeftCorner = lowerLeftCorner + (Vector3.up * MainScript.blockWidth * 22.5f);
        Vector3 tutorialScreenLowerLeftCorner = tutorialScreenParentOriginalPosition + new Vector3(MainScript.blockWidth * -7.5f,MainScript.blockWidth * -0.5f,0f);
        
        Vector3 tutorialScreenPositionOfCenter = tutorialScreenLowerLeftCorner + new Vector3(MainScript.blockWidth * 7.5f, MainScript.blockWidth * 4.5f, 0f);
        tutorialScreenRenders = new List<SpriteInteractionObject>();
        tutorialPageRenders = new List<SpriteInteractionObject>();
        for (int i = 0;i < 2; i++)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (y != 0 && y != 9 && x != 0 && x != 15)
                    {
                    }
                    else
                    {
                        Vector3 posToInstantiate = tutorialScreenLowerLeftCorner + new Vector3(x * MainScript.blockWidth, y * MainScript.blockWidth, 0f) + (Vector3.right * Screen.width * 0.01f * i);
                        Transform temp = Instantiate(menuBorderBlockPrefab, posToInstantiate, Quaternion.identity).transform;
                        temp.parent = tutorialScreenParent;
                        temp.localScale = new Vector3(MainScript.blockLocalScale, MainScript.blockLocalScale, 1f);
                        SpriteInteractionObject tempSpriteObj = new SpriteInteractionObject(temp);
                        tutorialScreenRenders.Add(tempSpriteObj);
                    }
                }
            }
            tutorialPageRenders.Add(new SpriteInteractionObject(Instantiate(tutorialPagePrefab, tutorialScreenPositionOfCenter + (Vector3.right * Screen.width * 0.01f * i) + new Vector3(0f,0f,-0.1f), Quaternion.identity).transform));
            tutorialPageRenders[i].renderer.transform.parent = tutorialScreenParent;
            tutorialPageRenders[i].renderer.transform.localScale = new Vector3(MainScript.blockLocalScale * 0.85f, MainScript.blockLocalScale * 0.85f, 1f);
            tutorialPageRenders[i].ChangeVisibility(false);
            tutorialScreenRenders.Add(tutorialPageRenders[i]);
            SpriteInteractionObject tutorialScreenBackground = new SpriteInteractionObject(Instantiate(basicWhiteBlockPrefab, tutorialScreenPositionOfCenter + (Vector3.right * Screen.width * 0.01f * i), Quaternion.identity).transform);
            tutorialScreenBackground.renderer.transform.parent = tutorialScreenParent;
            tutorialScreenBackground.ChangeColor(Color.black);
            tutorialScreenRenders.Add(tutorialScreenBackground);
            tutorialScreenBackground.renderer.transform.localScale = new Vector3(MainScript.blockLocalScale * 14f, MainScript.blockLocalScale * 8f, 1f);
            transform.position = menuOpenPosition;
        }
        Vector3 halfABlock = new Vector3(MainScript.blockWidth * 0.5f, MainScript.blockWidth * 0.5f, -1f);
        Vector3 bottomCenter = lowerLeftCorner + new Vector3(7f * MainScript.blockWidth, 0f, 0f);
        playGameButton = Instantiate(playGameButtonPrefab, bottomCenter + (Vector3.up * MainScript.blockWidth * 16f) + halfABlock, Quaternion.identity).transform;
        playGameButton.localScale = new Vector3(MainScript.blockWidth * 8f, MainScript.blockWidth * 8f, 1f);
        playGameButton.parent = transform;
        tutorialButton = Instantiate(tutorialButtonPrefab, bottomCenter + (Vector3.up * MainScript.blockWidth * 8f) + halfABlock, Quaternion.identity).transform;
        tutorialButton.localScale = new Vector3(MainScript.blockWidth * 8f, MainScript.blockWidth * 8f, 1f);
        tutorialButton.parent = transform;
        HighScoreButton = Instantiate(highScoreButtonPrefab, bottomCenter + (Vector3.up * MainScript.blockWidth * 4f) + halfABlock, Quaternion.identity).transform;
        HighScoreButton.localScale = new Vector3(MainScript.blockWidth * 8f, MainScript.blockWidth * 8f, 1f);
        HighScoreButton.parent = transform;
        loadoutButton = Instantiate(loadoutButtonPrefab, bottomCenter + (Vector3.up * MainScript.blockWidth * 12f) + halfABlock, Quaternion.identity).transform;
        loadoutButton.localScale = new Vector3(MainScript.blockWidth * 8f, MainScript.blockWidth * 8f, 1f);
        loadoutButton.parent = transform;
        removeAdsButton = Instantiate(removeAdsButtonPrefab, bottomCenter + (Vector3.up * MainScript.blockWidth * 16f) + halfABlock, Quaternion.identity).transform;
        removeAdsButton.localScale = new Vector3(MainScript.blockWidth * 8f, MainScript.blockWidth * 8f, 1f);
        removeAdsButton.parent = transform;
        allButtons.Add(new MenuButton(playGameButton, MenuButtonType.PlayGame));
        allButtons.Add(new MenuButton(tutorialButton, MenuButtonType.Tutorial));
        allButtons.Add(new MenuButton(HighScoreButton, MenuButtonType.HighScore));
        allButtons.Add(new MenuButton(loadoutButton, MenuButtonType.Loadout));
        allButtons.Add(new MenuButton(removeAdsButton, MenuButtonType.removeAds));
        foreach(MenuButton m in allButtons)
        {
            allRenders.Add(m.render);
        }
        SetupDefaultAbilities();
        SetUpMainMenu();
    }
    
    void ChangeVisibility(bool visible)
    {
        foreach(SpriteInteractionObject sprite in allRenders)
        {
            sprite.ChangeVisibility(visible);
        }
    }
    public void ChangeTutorialScreenPage(int pagesToChangeBy)
    {
        currentTutorialPage += pagesToChangeBy;//it has already been verified by controlscript before running this function. changing to a page outside the array will run an error
        directionToMoveTutorialScreen = new Vector3(Mathf.Sign(pagesToChangeBy) * -1f, 0f, 0f);
        if (directionToMoveTutorialScreen.x == -1f)
        {
            //tutorialScreenParent.Translate(directionToMoveTutorialScreen.normalized * Screen.width * -0.01f);
            //tutorialScreenParent.position = tutorialScreenParentOriginalPosition;
            //tutorialScreenParent.position = tutorialScreenParentHolder.position + (directionToMoveTutorialScreen.normalized * Screen.width * -0.01f);
            tutorialPageRenders[1].renderer.transform.GetComponent<TutorialPageScript>().SwitchToThisPage(currentTutorialPage);
        }
        else
        {
            tutorialScreenParent.position = tutorialScreenParent.position + (Vector3.left * Screen.width * 0.01f);;
            //tutorialScreenParent.Translate(directionToMoveTutorialScreen.normalized * Screen.width * 0.02f);
            //tutorialScreenParent.position = tutorialScreenParentHolder.position + (directionToMoveTutorialScreen.normalized * Screen.width * -0.01f);
            tutorialPageRenders[0].renderer.transform.GetComponent<TutorialPageScript>().SwitchToThisPage(currentTutorialPage);
        }
        //tutorialScreenRenders[0].renderer.transform.position = tutorialScreenParentOriginalPosition + (directionToMoveTutorialScreen.normalized * Screen.width * -1f);
        //tutorialScreenRenders[1].renderer.transform.position = tutorialScreenParentOriginalPosition;
    }
    public void UpdateTutorialScreenPage()
    {
        
        float speedOfTutorialScreen = Screen.width * 0.0115f;
        float distanceToMove = speedOfTutorialScreen * Time.fixedDeltaTime;
        
        //Vector3 objectivePosition = tutorialScreenParentOriginalPosition  + (directionToMoveTutorialScreen * Screen.width * 0.01f);
        Vector3 destinationPosition = tutorialScreenParentOriginalPosition;
        if(directionToMoveTutorialScreen.x < 0f) { destinationPosition += Vector3.left * Screen.width * 0.01f; }
        
        float currentDistance = Vector2.Distance(destinationPosition, tutorialScreenParent.position);
        //print(directionToMoveTutorialScreen.normalized.x + " and " + currentDistance);
        if(distanceToMove > currentDistance)
        {
            tutorialScreenParent.position = tutorialScreenParentOriginalPosition;
            MainScript.isChangingTutorialPage = false;
            foreach (SpriteInteractionObject s in tutorialPageRenders)
            {
                s.renderer.transform.GetComponent<TutorialPageScript>().SwitchToThisPage(currentTutorialPage);
            }
        }
        else 
        {
            tutorialScreenParent.Translate(directionToMoveTutorialScreen.normalized * distanceToMove);
            if (directionToMoveTutorialScreen.normalized.x == 1f) { }
        }
    }
    public void StartOpeningMenu()
    {
        MainScript.LockInMovingBlocks();
        
        isOpeningMenu = true;
        SetUpMainMenu();
    }
    void ChangeHighScoreVisiblity(bool visible)
    {
        foreach(SpriteInteractionObject temp in highScorePlacers) { temp.ChangeVisibility(visible); }
    }
    void ChangeIconVisibility(bool visible) { foreach(AbilityIcon icon in allIcons) { icon.render.ChangeVisibility(visible); }foreach (IconSelecter selecter in iconSelectors) { selecter.render.ChangeVisibility(visible); } }
    public void OpeningMenu()
    {
        float speedToMove = Screen.height * 0.02f;
        float distanceToOpen = Vector2.Distance(transform.position, menuOpenPosition);
        float distanceToMove = speedToMove * Time.fixedDeltaTime;
        if (distanceToMove >= distanceToOpen)
        {
            transform.position = menuOpenPosition;
            if(PlayerPrefs.GetInt("removeAds",0) == 0)
            {
                ActivateRemoveAdsButton();
            }
            isOpeningMenu = false;
        }else
        {
            Vector2 directionToMove = (Vector2)( transform.position - menuOpenPosition) * -1f;
            transform.Translate(directionToMove.normalized * distanceToMove);
        }
    }
    
        
    
    public void SetUpMainMenu()
    {
        //transform.position = menuOpenPosition;
        titleScreen.ChangeVisibility(true);
        ChangeHighScoreVisiblity(false);
        ChangeTutorialScreenVisibility(false);
        disableControlsCounter.ResetTimer();
        tapToGobackButton.ChangeVisibility(false);
        ChangeIconVisibility(false);
        MainScript.ChangeMenuTextVisibility(false);
        currentState = MenuState.mainMenu;
        menuIsActive = true;
        ChangeVisibility(true);
        Vector3 halfABlock = new Vector3(MainScript.blockWidth * 0.5f, MainScript.blockWidth * 0.5f, -1f);
        Vector3 tempLowerLeftCorner = transform.position + new Vector3(MainScript.blockWidth * -8f, MainScript.blockWidth * -15f, 0f);
        Vector3 bottomCenter = tempLowerLeftCorner + new Vector3(7f * MainScript.blockWidth, 0f, 0f);
        float heightDiffFromMiddle = Screen.width * 0.55f;
        float heightFromBottom = (Screen.height * 0.5f) - heightDiffFromMiddle;
        removeAdsButton.transform.position = new Vector3(removeAdsButton.transform.position.x, heightFromBottom, removeAdsButton.transform.position.z);
        float additionalYPush = 0f;
        //PlayerPrefs.SetInt("removeAds", 0);
        if (PlayerPrefs.GetInt("removeAds", 0) != 0)
        {
            DeactivateRemoveAdsButton();
            //removeAdsButton.
        }
        else
        {
            if (!isOpeningMenu)
            {
                ActivateRemoveAdsButton();
            }
            additionalYPush += 2f;
            allButtons[4].theTransform.position = bottomCenter + (Vector3.up * MainScript.blockWidth * (1f + additionalYPush)) + halfABlock;
        }
        allButtons[0].theTransform.position = bottomCenter + (Vector3.up * MainScript.blockWidth * (15f + additionalYPush)) + halfABlock;
        allButtons[1].theTransform.position = bottomCenter + (Vector3.up * MainScript.blockWidth * (11.5f + additionalYPush)) + halfABlock;
        if(MainScript.gameHasStarted && MainScript.tutorialMode) { allButtons[1].render.ChangeColor(Color.green); }else if(MainScript.gameHasStarted && !MainScript.tutorialMode) { allButtons[1].render.ChangeColor(Color.red); } else
        {
            allButtons[1].render.ChangeColor(Color.white);
        }
        //
        allButtons[2].theTransform.position = bottomCenter + (Vector3.up * MainScript.blockWidth * (8f + additionalYPush)) + halfABlock;
        allButtons[3].theTransform.position = bottomCenter + (Vector3.up * MainScript.blockWidth * (4.5f + additionalYPush)) + halfABlock;
        
        
        if (MainScript.gameHasStarted ) { allButtons[3].render.ChangeColor(Color.red); }
        else
        {
            allButtons[3].render.ChangeColor(Color.white);
        }
    }
    UtilityAbilityReference GetUtilityReferenceFromIcon(AbilityIcon theIcon)
    {
        foreach (UtilityAbilityReference reference in allUtilityAbilities)
        {
            if (reference.thisReferenceIcon == theIcon) { return reference; }
        }
        return (allUtilityAbilities[0]);
    }
    TriggerAbilityReference GetTriggerReferenceFromIcon(AbilityIcon theIcon)
    {
        foreach (TriggerAbilityReference reference in allTriggerAbilities)
        {
            if (reference.thisReferenceIcon == theIcon) { return reference; }
        }
        return (allTriggerAbilities[0]);
    }
    public void SetUpLoadout()
    {
        titleScreen.ChangeVisibility(false);
        disableControlsCounter.ResetTimer();
        tapToGobackButton.ChangeVisibility(false);
        ChangeIconVisibility(true);
        currentState = MenuState.loadOut;
        //loadOutButton.position = headerArea;
        MenuButton loadButton = GetButtonWithThisType(MenuButtonType.Loadout);
        foreach(MenuButton m in allButtons) { m.render.ChangeVisibility(false); }
        loadButton.theTransform.position = new Vector3(headerArea.x, headerArea.y, loadButton.theTransform.position.z) ;
        loadButton.render.ChangeVisibility(true);
        MainScript.ChangeMenuTextVisibility(true);

        Vector3 topRowCenterPosition = lowerLeftCorner + new Vector3(MainScript.blockWidth * 7f, MainScript.blockWidth * 19f, 0f) + new Vector3(MainScript.blockHeight * 0.5f, MainScript.blockHeight * -0.5f,0f);
        List<UtilityAbilityReference> allUtilityAbilityReferences = new List<UtilityAbilityReference>();
        List<UtilityAbilityReference> allMovementAbilityReferences = new List<UtilityAbilityReference>();

        foreach(UtilityAbilityReference reference in allUtilityAbilities){
            if (reference.abilityType == InterfaceType.Shoot) { allUtilityAbilityReferences.Add(reference); }
            else if(reference.abilityType == InterfaceType.Move) { allMovementAbilityReferences.Add(reference); }
        }

        float totalWidthOfColumn = ((float)(allMovementAbilityReferences.Count - 1)* MainScript.blockWidth *3f);
        Vector3 originPoint = topRowCenterPosition + new Vector3(totalWidthOfColumn * -0.5f, 0f, -3f);
        for(int i = 0;i < allMovementAbilityReferences.Count; i++)
        {
            UtilityAbilityReference currentAbility = allMovementAbilityReferences[i];
            Vector3 iconPosition = originPoint + new Vector3(i * MainScript.blockWidth * 3f, 0f, -1f);
            currentAbility.thisReferenceIcon.theTransform.position = new Vector3(iconPosition.x, iconPosition.y, -3.1f);
            if(currentAbility.prefabName == ControlsScript.currentMovementAbility.prefabName) { iconSelectors[0].theTransform.position =  new Vector3(currentAbility.thisReferenceIcon.theTransform.position.x, currentAbility.thisReferenceIcon.theTransform.position.y, -3.11f); }
        }
        totalWidthOfColumn = ((float)(allUtilityAbilityReferences.Count - 1) * MainScript.blockWidth * 3f);
        originPoint = topRowCenterPosition + new Vector3(totalWidthOfColumn * -0.5f, 0f, 0f);
        originPoint += Vector3.down * MainScript.blockWidth * 5f;
        for (int i = 0; i < allUtilityAbilityReferences.Count; i++)
        {
            UtilityAbilityReference currentAbility = allUtilityAbilityReferences[i];
            Vector3 iconPosition = originPoint + new Vector3(i * MainScript.blockWidth * 3f, 0f, -1f);
            currentAbility.thisReferenceIcon.theTransform.position = new Vector3(iconPosition.x, iconPosition.y, -3.1f);
            if (currentAbility.prefabName == ControlsScript.currentUtilityAbility.prefabName) { iconSelectors[1].theTransform.position = new Vector3(currentAbility.thisReferenceIcon.theTransform.position.x, currentAbility.thisReferenceIcon.theTransform.position.y, -3.11f); }
        }

        
        totalWidthOfColumn = ((float)(allTriggerAbilities.Count - 1) * MainScript.blockWidth * 3f);
        originPoint = topRowCenterPosition + new Vector3(totalWidthOfColumn * -0.5f, 0f, 0f);
        originPoint += Vector3.down * MainScript.blockWidth * 10f;
        for (int i = 0; i < allTriggerAbilities.Count; i++)
        {
            
            TriggerAbilityReference currentAbility = allTriggerAbilities[i];
            Vector3 iconPosition = originPoint + new Vector3(i * MainScript.blockWidth * 3f, 0f, -1f);
            currentAbility.thisReferenceIcon.theTransform.position = new Vector3(iconPosition.x, iconPosition.y, -3.1f);
            if (currentAbility.prefabName == ControlsScript.currentTriggerAbility.prefabName) { iconSelectors[2].theTransform.position = new Vector3(currentAbility.thisReferenceIcon.theTransform.position.x, currentAbility.thisReferenceIcon.theTransform.position.y, -3.11f); }
        }
        MainScript.mainMenuTextBoxes[0].text =  ControlsScript.currentMovementAbility.abilityName;
        MainScript.mainMenuTextBoxes[1].text =  ControlsScript.currentUtilityAbility.abilityName;
        MainScript.mainMenuTextBoxes[2].text =  ControlsScript.currentTriggerAbility.prefabName;
        /*for(int i = 0;i < allIcons.Count; i++)
        {
            AbilityIcon icon = allIcons[i];
            icon.theTransform.position = topRowCenterPosition + (Vector3.down * i * MainScript.blockHeight * 5f);
            icon.render.ChangeVisibility(true);
        }*/
    }
    public MenuButton GetButtonWithThisType(MenuButtonType typeToFind) { foreach(MenuButton m in allButtons) { if(m.theType == typeToFind) { return m; } }return allButtons[0]; }
    public void CloseMenu()
    {
        MainScript.UnlockMovingBlocks();
        MainScript.paused = false;
        isOpeningMenu = false;
        transform.position = menuClosedPosition;
        currentState = MenuState.hidden;
        menuIsActive = false;
        ChangeVisibility(false);
    }
    void CheckForTouchInputs()
    {
        //List<Touch> allTouchesThisFrame = Input.Get
    }
    void DoesThisTouchPushAButton(Touch t)
    {
        if(currentState == MenuState.mainMenu)
        {
            foreach (MenuButton b in allButtons)
            {
                if (b.IsPointInsideButton(t.position))
                {
                    PressButton(b);
                }
            }
        }else if(currentState == MenuState.loadOut)
        {
            if(GetButtonWithThisType(MenuButtonType.Loadout).IsPointInsideButton(t.position))
            {
                SetUpMainMenu();
            }
            foreach(AbilityIcon icon in allIcons)
            {
                if (icon.IsPointInsideButton(t.position))
                {
                    SelectIcon(icon);
                }
            }
        }else if(currentState == MenuState.highScore)
        {
            if (GetButtonWithThisType(MenuButtonType.HighScore).IsPointInsideButton(t.position))
            {
                SetUpMainMenu();
            }
        }
        
    }
    public void ChangeTutorialScreenVisibility(bool visibilityState)
    {
        foreach(SpriteInteractionObject temp in tutorialScreenRenders)
        {
            temp.ChangeVisibility(visibilityState);
        }
    }
    void SelectIcon(AbilityIcon theIcon)
    {
        if(theIcon.theType != InterfaceType.Trigger)
        {
            UtilityAbilityReference currentReference = GetUtilityReferenceFromIcon(theIcon);
            switch (currentReference.abilityType)
            {
                case InterfaceType.Move:

                    iconSelectors[0].theTransform.position = new Vector3(theIcon.theTransform.position.x, theIcon.theTransform.position.y, -3.11f);
                    ControlsScript.currentMovementAbility = new UtilityAbility(currentReference);
                    ControlsScript.currentMovementAbilitySprite = currentReference.iconSprite;
                    MainScript.mainMenuTextBoxes[0].text =  currentReference.nameOfAbility;
                    MainScript.interfaceHighlightPanels[0].currentInterfaceAbilityIcon.render.ChangeTexture(currentReference.iconSprite);
                    break;
                case InterfaceType.Shoot:

                    iconSelectors[1].theTransform.position = new Vector3(theIcon.theTransform.position.x, theIcon.theTransform.position.y, -3.11f);
                    ControlsScript.currentUtilityAbility = new UtilityAbility(currentReference);
                    ControlsScript.currentUtilityAbilitySprite = currentReference.iconSprite;
                    MainScript.mainMenuTextBoxes[1].text =  currentReference.nameOfAbility;
                    MainScript.interfaceHighlightPanels[1].currentInterfaceAbilityIcon.render.ChangeTexture(currentReference.iconSprite);
                    break;
                case InterfaceType.Trigger:
                    
                    //ControlsScript.currentTriggerAbility = new TriggerAbility(currentReference)
                    break;
            }
        }
        else
        {
            iconSelectors[2].theTransform.position = new Vector3(theIcon.theTransform.position.x, theIcon.theTransform.position.y, -3.11f);
            ControlsScript.currentTriggerAbility = new TriggerAbility(GetTriggerReferenceFromIcon(theIcon));
            MainScript.mainMenuTextBoxes[2].text =  ControlsScript.currentTriggerAbility.abilityName;
        }
    }
    public void SetupEndGameScreen()
    {
        tutorialScreenParent.position = tutorialScreenParentOriginalPosition;
        ChangeTutorialScreenVisibility(true);
        tutorialPageRenders[0].ChangeTexture(gameOverScreen);
    }
    void SetUpHighScore()
    {
        titleScreen.ChangeVisibility(false);
        disableControlsCounter.ResetTimer();
        tapToGobackButton.ChangeVisibility(false);
        ChangeIconVisibility(false);
        currentState = MenuState.highScore;
        //loadOutButton.position = headerArea;
        MenuButton highScoreButton = GetButtonWithThisType(MenuButtonType.HighScore);
        foreach (MenuButton m in allButtons) { m.render.ChangeVisibility(false); }
        highScoreButton.theTransform.position = new Vector3(headerArea.x, headerArea.y, highScoreButton.theTransform.position.z);
        highScoreButton.render.ChangeVisibility(true);
        MainScript.ChangeMenuTextVisibility(true);
        Vector3 topRowCenterPosition = lowerLeftCorner + new Vector3(MainScript.blockWidth * 7f, MainScript.blockWidth * 19f, 0f) + new Vector3(MainScript.blockHeight * 0.5f, MainScript.blockHeight * -0.5f, -3f);
        highScorePlacers[0].renderer.transform.position = topRowCenterPosition;
        MainScript.mainMenuTextBoxes[0].text = PlayerPrefs.GetInt("1stHighScore", 0).ToString();
        Vector3 middleRowCenterPosition = topRowCenterPosition + new Vector3(0f, MainScript.blockWidth * -5f, -3f);
        highScorePlacers[1].renderer.transform.position = middleRowCenterPosition;
        MainScript.mainMenuTextBoxes[1].text = PlayerPrefs.GetInt("2ndHighScore", 0).ToString();
        Vector3 bottomRowCenterPosition = topRowCenterPosition + new Vector3(0f, MainScript.blockWidth * -10f, -3f);
        MainScript.mainMenuTextBoxes[2].text = PlayerPrefs.GetInt("3rdHighScore", 0).ToString();
        highScorePlacers[2].renderer.transform.position = bottomRowCenterPosition;
    }
    public void DeactivateRemoveAdsButton()
    {
        removeAdsButton.gameObject.SetActive(false);
    }
    void ActivateRemoveAdsButton()
    {
        removeAdsButton.gameObject.SetActive(true); ;
    }
    void PressButton(MenuButton b)
    {
        if (!isBuyingRemoveAds)
        {
            switch (b.theType)
            {
                case MenuButtonType.PlayGame:

                    if (currentState == MenuState.mainMenu)
                    {
                        DeactivateRemoveAdsButton();

                        if (!MainScript.gameHasStarted || MainScript.tutorialMode)
                        {
                            MainScript temp = Camera.main.GetComponent<MainScript>();
                            if (MainScript.gameHasStarted) { MainScript.EndGame(); }
                            MainScript.tutorialMode = false;
                            titleScreen.ChangeVisibility(false);
                            CloseMenu();
                            temp.StartTheGame();
                            menuIsActive = false;

                        }
                        else
                        {
                            CloseMenu();
                            menuIsActive = false;
                        }

                    }
                    break;
                case MenuButtonType.Loadout:
                    if (currentState == MenuState.mainMenu && !MainScript.gameHasStarted)
                    {
                        DeactivateRemoveAdsButton();
                        SetUpLoadout();
                    }
                    break;
                case MenuButtonType.HighScore:
                    if (currentState == MenuState.mainMenu)
                    {
                        DeactivateRemoveAdsButton();
                        ChangeHighScoreVisiblity(true);
                        SetUpHighScore();
                    }
                    break;
                case MenuButtonType.Tutorial:
                    if (currentState == MenuState.mainMenu)
                    {
                        if (!MainScript.gameHasStarted)
                        {
                            DeactivateRemoveAdsButton();
                            SetupDefaultAbilities();
                            titleScreen.ChangeVisibility(false);
                            currentTutorialPage = 0;
                            tutorialPageRenders[0].renderer.transform.GetComponent<TutorialPageScript>().SwitchToThisPage(currentTutorialPage);
                            CloseMenu();
                            MainScript.tutorialMode = true;
                            tutorialScreenParent.position = tutorialScreenParentOriginalPosition;
                            Camera.main.GetComponent<MainScript>().StartTheGame();
                            menuIsActive = false;
                            ChangeTutorialScreenVisibility(true);
                        }
                        else if (MainScript.tutorialMode)
                        {
                            CloseMenu();
                            DeactivateRemoveAdsButton();
                            menuIsActive = false;
                        }
                    }
                    break;
                case MenuButtonType.removeAds:
                    if(currentState == MenuState.mainMenu && PlayerPrefs.GetInt("removeAds",0) == 0)
                    {

                        MainMenuScript.isBuyingRemoveAds = true;
                        iapManager.BuyRemoveAds();
                    }
                    break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (MainScript.paused) { if (!disableControlsCounter.hasFinished) { disableControlsCounter.AddTime(Time.deltaTime); } }
        

        if (MainScript.paused && menuIsActive && disableControlsCounter.hasFinished)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                
                Touch t = Input.GetTouch(i);
                if(t.phase == TouchPhase.Began)
                {
                    DoesThisTouchPushAButton(t);
                }
                
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (currentState)
                {
                    case MenuState.loadOut:
                    case MenuState.highScore:
                    case MenuState.scoreScreen:
                        SetUpMainMenu();
                        break;
                    case MenuState.mainMenu:
                        Application.Quit();
                        break;
                }
            }
        }
    }
}
public class MenuButton
{
    public BoxCollider2D menuCollider;
    public SpriteInteractionObject render;
    public Transform theTransform;
    public MenuButtonType theType;
    public MenuButton(Transform t, MenuButtonType typeOfButton)
    {
        theType = typeOfButton;
        theTransform = t;
        render = new SpriteInteractionObject(t);
        menuCollider = t.GetComponent<BoxCollider2D>();
    }
    public bool IsPointInsideButton(Vector2 point)
    {
        bool tempBool = false;
        Vector3 tempVect = Camera.main.ScreenToWorldPoint(point);
        if (menuCollider.bounds.Contains(new Vector3(tempVect.x,tempVect.y,theTransform.position.z))) { tempBool = true; }
        return tempBool;
    }
}
public enum MenuButtonType { PlayGame,Tutorial,HighScore,Loadout,removeAds}
public enum MenuState { hidden,mainMenu,scoreScreen,loadOut,highScore}
public class AbilityIcon
{
    public SpriteInteractionObject render;
    public string nameOfTexture;
    public Transform theTransform;
    public BoxCollider2D collider;
    public InterfaceType theType;
    public AbilityIcon(Transform t, Sprite texture,InterfaceType typeToImplement)
    {
        theType = typeToImplement;
        theTransform = t;
        //nameOfTexture = theNameOfTexture;
        collider = theTransform.GetComponent<BoxCollider2D>();
        t.localScale = new Vector3(MainScript.blockLocalScale , MainScript.blockLocalScale , 1f);
        render = new SpriteInteractionObject(t);
        render.ChangeTexture(texture);
    }
    public bool IsPointInsideButton(Vector2 point)
    {
        bool tempBool = false;
        Vector3 tempVect = Camera.main.ScreenToWorldPoint(point);
        if (collider.bounds.Contains(new Vector3(tempVect.x, tempVect.y, theTransform.position.z))) { tempBool = true; }
        return tempBool;
    }
}
public class IconSelecter
{
    public Transform theTransform;
    public SpriteInteractionObject render;
    public IconSelecter(Transform selecterTransform)
    {
        theTransform = selecterTransform;
        render = new SpriteInteractionObject(theTransform);
    }
}