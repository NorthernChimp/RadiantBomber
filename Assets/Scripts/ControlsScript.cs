using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsScript : MonoBehaviour
{
	public Vector2 movementDirect;
	public Vector2 velocity;
	public ControlsSettings defaultSettings;
	public List<ControlsSettingsAffector> affectors;
	public float maxSpeed = 1;
	public float accelerationRate = 0.75f;
	public float idleDecelerationRate = 0.4f;
	public bool activatingUtility = false;
	public Counter utilityAbilityTimer;
	public Counter bulletCounter;
	public bool leftTrigger = false;
	public static bool hasChangedTutorialPage = false;
	public static bool moveAbilityTrigger = false;
	public static bool shootAbilityTrigger = false;
	public TouchInterface moveInterface;
	public TouchInterface shootInterface;
	public TouchInterface tutorialInterface;
	public TouchInterface mainMenuInterface;
	public List<TouchInterface> allTouchInterFaces;
	public static UtilityAbility currentUtilityAbility;
	public static Sprite currentUtilityAbilitySprite;
	public static UtilityAbility currentMovementAbility;
	public static Sprite currentMovementAbilitySprite;
	public static TriggerAbility currentTriggerAbility;
	public string bulletPrefabName = "PusherBulletPrefab";
	public DisplayCrosshairScript moveDisplayCrosshair;
	public DisplayCrosshairScript shootDisplayCrosshair;
	//public DisplayCrosshair moveCrosshair;
	//public DisplayCrosshair shootCrosshair;

	// Start is called before the first frame update
	void Start()
	{
		
	}
	public static void ResetCooldowns()
    {
		currentMovementAbility.utilityCounter.ResetTimer();
		currentUtilityAbility.utilityCounter.ResetTimer();
    }
	public void SetUpPlayer()
    {
		maxSpeed *= MainScript.blockHeight;
		accelerationRate *= MainScript.blockHeight;
		idleDecelerationRate *= MainScript.blockHeight;
		leftTrigger = false;
		movementDirect = Vector2.zero;
		//currentTriggerAbility = new TriggerAbility(bulletCounter, "PusherBulletPrefab");
		//currentUtilityAbility = new UtilityAbility(UtilityActivatesOn.begin, new Counter(2f), "KnockBackExplosionPrefab",false,InterfaceType.Shoot);
		//currentMovementAbility = new UtilityAbility(UtilityActivatesOn.shoot, new Counter(2f), "MolecularBurstPrefab",true,InterfaceType.Move);
		GameObject tempObj = (GameObject)Instantiate(Resources.Load("Prefabs/DisplayCrosshairPrefab"));
		moveDisplayCrosshair = tempObj.transform.GetComponent<DisplayCrosshairScript>();
		moveDisplayCrosshair.SetupCrosshair(true, true,true);
		moveDisplayCrosshair.transform.localPosition = new Vector3(0f, 0f, 1f);
		tempObj = (GameObject)Instantiate(Resources.Load("Prefabs/DisplayCrosshairPrefab"));
		shootDisplayCrosshair = tempObj.transform.GetComponent<DisplayCrosshairScript>();
		shootDisplayCrosshair.transform.localPosition = new Vector3(0f, 0f, 1f);
		shootDisplayCrosshair.SetupCrosshair(false, false, false);

		moveInterface = new TouchInterface(TouchInterfaceAnchor.leftSide, 0f, Screen.height * 0.25f, 0f, 0f, Screen.width * 0.15f);
		moveInterface.utilityAbility = currentMovementAbility;
		//moveCrosshair = new DisplayCrosshair(moveInterface.maxSwipeDistance,false,);
		//moveInterface.relevantCrosshair = moveCrosshair;
		moveInterface.hasCrosshair = true;
		moveInterface.hasUtility = true;
		moveInterface.typeOfInterface = InterfaceType.Move;
		

		shootInterface = new TouchInterface(TouchInterfaceAnchor.rightSide, 0f, Screen.height * 0.25f, 0f, 0f, Screen.width * 0.02f);
		shootInterface.hasUtility = true;
		shootInterface.typeOfInterface = InterfaceType.Shoot;
		shootInterface.utilityAbility = currentUtilityAbility;
		shootInterface.hasCrosshair = true;

		tutorialInterface = new TouchInterface(TouchInterfaceAnchor.topCenter, 0f, Screen.height * 0.5f, 0f, 0f, Screen.width * 0.015f);
		tutorialInterface.typeOfInterface = InterfaceType.tutorial;
		mainMenuInterface = new TouchInterface(TouchInterfaceAnchor.topRight, 0f, 0f, 0f, 0f, Screen.width * 0.02f);
		mainMenuInterface.typeOfInterface = InterfaceType.Passive;
		allTouchInterFaces = new List<TouchInterface>() { moveInterface, shootInterface ,tutorialInterface,mainMenuInterface};

		//utilityAbilityTimer = new Counter(2f);
		//bulletCounter = new Counter(currentTriggerAbility.coo);

		defaultSettings = new ControlsSettings(false, maxSpeed, accelerationRate, idleDecelerationRate);
		affectors = new List<ControlsSettingsAffector>();
	}
	void IsThisAnInterface(Touch t)
	{

		if (moveInterface.isActive && t.fingerId == moveInterface.touchId)
		{
			MoveInterface(t);
		}
		else if (shootInterface.isActive && t.fingerId == shootInterface.touchId)
		{
			ShootInterface(t);
		}
		else if (tutorialInterface.isActive && t.fingerId == tutorialInterface.touchId)
		{
			TutorialInterface(t);
		}
		else if (mainMenuInterface.isActive && t.fingerId == mainMenuInterface.touchId)
		{
			MainMenuInterface(t);
		}
	}
	void TutorialInterface(Touch t)
    {
		tutorialInterface.lastDirection = (tutorialInterface.lastDirection + t.deltaPosition.normalized) / 2f;
		tutorialInterface.currentDirection += tutorialInterface.lastDirection;
		
		float distance = tutorialInterface.currentDirection.x * Mathf.Sign(tutorialInterface.currentDirection.x);
		float currentMaxSwpeDistance = tutorialInterface.maxSwipeDistance;
		float fraction = distance / currentMaxSwpeDistance;
		if (!MainScript.isChangingTutorialPage)
		{
			bool canMoveInDirection = true;
			if ((Mathf.Sign(tutorialInterface.currentDirection.x) > 0 && MainMenuScript.currentTutorialPage == 0) || Mathf.Sign(tutorialInterface.currentDirection.x) < 0 && MainMenuScript.currentTutorialPage == 6) { canMoveInDirection = false; }
			if (canMoveInDirection) {
				MainMenuScript.tutorialScreenParent.position = MainMenuScript.tutorialScreenParentOriginalPosition + new Vector3(fraction * MainScript.blockWidth * Mathf.Sign(tutorialInterface.currentDirection.x) * 3.5f, 0f, 0f);
			} 
			
		}
		
		if(fraction >= 1f && !MainScript.isChangingTutorialPage)
		{
			int pagesToChangeBy = (int)Mathf.Sign(tutorialInterface.currentDirection.x) * -1;
			int nextTutorialPage = MainMenuScript.currentTutorialPage + (pagesToChangeBy);
			if (nextTutorialPage >= 0 && nextTutorialPage < 7)
			{
				hasChangedTutorialPage = true;
				tutorialInterface.CancelTouch();
				MainScript.mainMenuObject.GetComponent<MainMenuScript>().ChangeTutorialScreenPage(pagesToChangeBy);
				
				MainScript.isChangingTutorialPage = true;
			}
		}
    }
	void MainMenuInterface(Touch t)
    {
		if (!MainScript.paused && !MainMenuScript.isOpeningMenu)
        {
			Vector2 touchPosInWorld = Camera.main.ScreenToWorldPoint(t.position);
			MainScript.mainMenuObject.transform.position = (Vector3)touchPosInWorld + new Vector3(Screen.width * 0.005f, Screen.height * 0.005f, MainScript.mainMenuObject.transform.position.z) + new Vector3(MainScript.blockHeight * 3.5f, MainScript.blockHeight * 3.5f,0f);
			if ((t.position.x < Screen.width * 0.55f || t.position.y < Screen.height * 0.55f) )
			{
				MainScript.paused = true;
				mainMenuInterface.CancelTouch();
                if (MainScript.tutorialMode) { MainScript.EndGame(); }
				MainScript.mainMenuObject.SendMessage("StartOpeningMenu");
				mainMenuInterface.isActive = false;
			}
		}
			
		
    }
	void OpenTheMenu()
    {
//		print("open the menu");
		if (!MainScript.paused && !MainMenuScript.isOpeningMenu)
		{
			MainScript.paused = true;
            if (mainMenuInterface.isActive) { mainMenuInterface.CancelTouch(); }
			if (MainScript.tutorialMode) { MainScript.EndGame(); }
			MainScript.mainMenuObject.SendMessage("StartOpeningMenu");
			mainMenuInterface.isActive = false;
		}
	}
	void ShootInterface(Touch t)
	{
		if (t.phase == TouchPhase.Stationary)
		{
			if (!shootInterface.hasStoppedMoving)
			{
				shootInterface.timeNotMoving += Time.fixedDeltaTime;
				if (shootInterface.timeNotMoving > 0.1f)
				{
					shootInterface.hasStoppedMoving = true;
				}
			}
		}
		else
		{
			shootInterface.lastDirection = (shootInterface.lastDirection + t.deltaPosition.normalized) / 2f;
			shootInterface.currentDirection += shootInterface.lastDirection;
			if (shootInterface.hasStoppedMoving && t.deltaPosition.magnitude > (Screen.width * 0.03f))
			{
				shootInterface.origin = t.position;
			}
			else { shootInterface.timeNotMoving = 0f; }
		}
		//Vector2 completeDirection = t.position - shootInterface.origin;
		//Vector2 direction = completeDirection.normalized;
		//float distance = completeDirection.magnitude;
		//float fraction = distance / shootInterface.maxSwipeDistance;
		float distance = shootInterface.currentDirection.magnitude;
		float currentMaxSwipeDistance = shootInterface.maxSwipeDistance;
		///if (moveInterface.doubleClick) { currentMaxSwipeDistance *= 10.5f; }
		float fraction = distance / currentMaxSwipeDistance;

		if (fraction > 1f) { fraction = 1f; }
		//MainScript.staticString = fraction.ToString(); ;
		
		if (!shootInterface.doubleClick)
		{
			if (fraction >= 1f) { leftTrigger = true; } else { leftTrigger = false; }
		}
		else
		{
			if ((currentUtilityAbility.activation == UtilityActivatesOn.shoot || currentUtilityAbility.activation == UtilityActivatesOn.shootThenFollowThrough)&& currentUtilityAbility.utilityCounter.hasFinished && fraction == 1f)
			{
				//MainScript.staticString = "YOU ARE ACTIVATING TEH ABILITY";
				shootInterface.ActivateAbility();
			}
			//activate UtilityAbility
		}
		if(shootInterface.hasCrosshair)
        {
			//shootDisplayCrosshair.UpdateCrosshair(shoo
			shootDisplayCrosshair.UpdateCrosshair(shootInterface);
        }
	}
	void MoveInterface(Touch t)
	{
		//movementDirect = Vector2.down;
		if(t.phase == TouchPhase.Stationary)
		{
			if(!moveInterface.hasStoppedMoving)
			{
				moveInterface.timeNotMoving += Time.fixedDeltaTime;
				if(moveInterface.timeNotMoving > 0.1f)
				{
					moveInterface.hasStoppedMoving = true;
				}
			}
		}
		else
		{
			if(moveInterface.currentDirection.magnitude > 0.1f && !moveInterface.doubleClick) { moveInterface.currentDirection *= 0.95f;}
			
			moveInterface.lastDirection = (moveInterface.lastDirection + t.deltaPosition.normalized)/2f;
			//moveInterface.currentDirection += moveInterface.lastDirection;
			//print(moveInterface.currentDirection);
			//if ( !moveInterface.doubleClick) { }
			if(t.deltaPosition.magnitude > Screen.width * 0.005f) { moveInterface.currentDirection += t.deltaPosition; }
				
			if (moveInterface.hasStoppedMoving && t.deltaPosition.magnitude > (Screen.width * 0.03f))
			{
				//moveInterface.origin = t.position;
			}
			else { moveInterface.timeNotMoving = 0f; }
		}
		Vector2 completeDirection = t.position - moveInterface.origin;
		//print(completeDirection.magnitude);
		Vector2 direction = completeDirection.normalized;
		//float distance = completeDirection.magnitude;
		float distance = moveInterface.currentDirection.magnitude;
		float currentMaxSwipeDistance = moveInterface.maxSwipeDistance;
		///if (moveInterface.doubleClick) { currentMaxSwipeDistance *= 10.5f; }
		float fraction = distance / currentMaxSwipeDistance;
		
		if (fraction > 1f) { fraction = 1f;}
		if (!moveInterface.doubleClick)
		{
			//movementDirect = direction * (fraction);
			movementDirect = moveInterface.currentDirection.normalized * moveInterface.maxSwipeDistance;
			//movementDirect = moveInterface.lastDirection.normalized * moveInterface.maxSwipeDistance;
		}
		else
		{
			//activate movement ability 
			//MainScript.staticString = "DOUBLE CLICK IS ON";
			if (currentMovementAbility.activation == UtilityActivatesOn.shoot && currentMovementAbility.utilityCounter.hasFinished && fraction == 1f)
			{
				//MainScript.staticString = "YOU ARE ACTIVATING TEH ABILITY";
				moveInterface.ActivateAbility();
			}
		}
		if(moveInterface.hasCrosshair)
		{
			moveDisplayCrosshair.UpdateCrosshair(moveInterface);
		}
		//if(moveInterface.anchor != TouchInterfaceAnchor.leftSide) { movementDirect += Vector2.up * 0.2f; }
	}
	void DoesThisStartAnInterface(Touch t)
	{
		//if(allTouchInterFaces.Count == 0) { movementDirect = Vector2.up; }
		foreach (TouchInterface touchInterface in allTouchInterFaces)
		{
			//touchInterface.SetUpTouch(t);
			
			if(touchInterface.isActive == false)
            {
				switch (touchInterface.anchor)
				{
					case TouchInterfaceAnchor.leftSide:
						//touchInterface.SetUpTouch(t);
						//movementDirect = Vector2.right;
						if (t.position.y < touchInterface.maxHeight && t.position.y > touchInterface.minHeight)
						{
							//touchInterface.SetUpTouch(t);
						}
						if (Mathf.Sign(t.position.x - (Screen.width * 0.5f)) == -1f && t.position.y < Screen.height * 0.5f)
						{
							//if (touchInterface.doubleClick && touchInterface.utilityAbility.utilityCounter.hasFinished) { print("execute the UtilityAbility"); }
							touchInterface.SetUpTouch(t);
							if (touchInterface.hasCrosshair)
							{
								if (touchInterface.typeOfInterface == InterfaceType.Move)
								{
									if (!moveDisplayCrosshair.drawOnDoubleClickOnly && !moveInterface.doubleClick || (moveDisplayCrosshair.drawOnDoubleClickOnly && moveInterface.doubleClick && currentMovementAbility.utilityCounter.hasFinished))
									{
										moveDisplayCrosshair.ChangeVisibility(true);
									}
								}
							}
						}
						break;
					case TouchInterfaceAnchor.rightSide:
						if (Mathf.Sign(t.position.x - (Screen.width * 0.5f)) == 1f && t.position.y < Screen.height *0.5f)
						{
							if (t.position.y < touchInterface.maxHeight && t.position.y > touchInterface.minHeight)
							{

							}
							touchInterface.SetUpTouch(t);
							if (touchInterface.hasCrosshair)
							{
								if (touchInterface.typeOfInterface == InterfaceType.Shoot)
								{
									if (!shootDisplayCrosshair.drawOnDoubleClickOnly && !moveInterface.doubleClick || (shootDisplayCrosshair.drawOnDoubleClickOnly && shootInterface.doubleClick))
									{
										shootDisplayCrosshair.ChangeVisibility(true);
									}
								}
							}
						}
						break;
					case TouchInterfaceAnchor.topCenter:
                        if (MainScript.tutorialMode)
                        {
							if (t.position.y > Screen.height * 0.6f && t.position.y < Screen.height * 0.9f)
							{
								if (t.position.x > Screen.width * 0.2f && t.position.x < Screen.width * 0.8f)
								{
									touchInterface.SetUpTouch(t);
								}
							}
						}
						break;
					case TouchInterfaceAnchor.topRight:
                        
						if(t.position .y > (Screen.height * 0.9f) && t.position.x > (Screen.width * 0.9f)  && !MainScript.paused)
                        {
							touchInterface.SetUpTouch(t);
                        }
						break;
				}
			}
		}

	}
	void UpdateAllTouchInterfaces()
	{
		//print("updating all touch interfaces");
		
		//if(Input.touchCount > 0) { movementDirect = Vector2.right; } 
		foreach(Touch t in Input.touches)
        {
			
		}
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch t = Input.GetTouch(i);
			switch (t.phase)
			{
				case TouchPhase.Began:
                    if (!MainMenuScript.menuIsActive) { DoesThisStartAnInterface(t); }
					break;
				case TouchPhase.Stationary:
				case TouchPhase.Moved:
					IsThisAnInterface(t);
					break;
				
				case TouchPhase.Ended:
					DoesThisEndAnInterface(t);
					break;
				case TouchPhase.Canceled:
					DoesThisEndAnInterface(t);
					break;
			}
			//MainScript.staticString = t.fingerId.ToString() + moveInterface.touchId.ToString();
			
		}
        if (moveInterface.isActive) { moveDisplayCrosshair.UpdateCrosshair(moveInterface);  }
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			print("presses escape");
			OpenTheMenu();
		}
		//if(moveInterface.hasCrosshair){if(moveInterface.relevantCrosshair.isVisible){moveInterface.relevantCrosshair.UpdateCrosshair(moveInterface);}}
		//if(shootInterface.hasCrosshair){if(shootInterface.relevantCrosshair.isVisible){shootInterface.relevantCrosshair.UpdateCrosshair(shootInterface);}}
	}
	public Vector2 Controls(Vector2 moveDirect, float timePassed)
	{
		//print(shootInterface.utilityAbility.prefabName);
		if (!shootInterface.utilityAbility.utilityCounter.hasFinished) { shootInterface.utilityAbility.utilityCounter.AddTime(timePassed); }
        if (!moveInterface.utilityAbility.utilityCounter.hasFinished) { moveInterface.utilityAbility.utilityCounter.AddTime(timePassed); }
		
		if (!currentTriggerAbility.triggerCounter.hasFinished) { currentTriggerAbility.triggerCounter.AddTime(Time.fixedDeltaTime); }
		ControlsSettings tempSettings = GetActualSettings(timePassed);
		
		//print(tempSettings.maxSpeed + " max speed");
		//print(defaultSettings.controlsPaused);
		//print(tempSettings.controlsPaused + "affectors currently " + affectors.Count);
		if (!tempSettings.controlsPaused)
		{

			if (movementDirect != Vector2.zero)
			{
				velocity += tempSettings.accelerationRate * movementDirect * Time.fixedDeltaTime;

				if (velocity.magnitude > tempSettings.maxSpeed) { velocity = velocity.normalized * tempSettings.maxSpeed; }
			}
		}
        if (!moveInterface.isActive) { movementDirect = Vector2.zero; } 
		if(!shootInterface.isActive) { leftTrigger = false; }
		if (velocity.magnitude > 0f && (movementDirect == Vector2.zero || tempSettings.controlsPaused || Vector2.Dot(movementDirect, velocity) < 0f))
		{
			float amountToDecrease = tempSettings.idleDecelerationRate * Time.fixedDeltaTime;
			if (amountToDecrease > velocity.magnitude) { velocity = Vector2.zero; } else { velocity -= (velocity.normalized) * amountToDecrease; }
		}
		if (leftTrigger && currentTriggerAbility.triggerCounter.hasFinished && !tempSettings.controlsPaused) { FireLeftTrigger(); }
		if (shootAbilityTrigger && shootInterface.utilityAbility.utilityCounter.hasFinished && !tempSettings.controlsPaused) { UseUtilityAbility(); }
		if (moveAbilityTrigger && moveInterface.utilityAbility.utilityCounter.hasFinished && !tempSettings.controlsPaused) { UseMovementAbility(); }
		moveDirect += velocity * Time.fixedDeltaTime;
		return moveDirect;
	}
	public void UseUtilityAbility()
	{
		//utilityAbilityTimer.ResetTimer();
		//utilityAbilityTimer.hasFinished = false;
		shootInterface.utilityAbility.utilityCounter.ResetTimer();
		//MainScript.staticString = "you have instantiated";
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/" + currentUtilityAbility.prefabName), transform.position, Quaternion.identity);
		temp.transform.localScale = new Vector3(MainScript.blockWidth / 0.16f, MainScript.blockWidth / 0.16f, 1f);
		if(currentUtilityAbility.takesDirection)
		{
			//Vector2 direct = shootInterface.GetRawDifference().normalized;
			temp.transform.SendMessage("SetDirection",shootInterface.currentDirection.normalized);
			
			//
			//if(shootInterface.utilityAbility.)
		}
		//MainScript.utilityAbilities.Add(new UtilityAbilityReference(temp.transform));
		temp.transform.SendMessage("UseUtility");
        if (currentUtilityAbility.takesDirection)
        {
			temp.transform.GetComponent<UtilityAbilityScript>().thisAbilityInstance.relevantTouchInterface = shootInterface;
		}
		shootAbilityTrigger = false;
		if (shootInterface.hasCrosshair) { shootDisplayCrosshair.ChangeVisibility(false); }
        if (shootInterface.utilityAbility.cancelsTouchWhenUsed) { shootInterface.CancelTouch(); } else { shootInterface.doubleClick = false; }
			
	}
	public void UseMovementAbility()
	{
		moveInterface.utilityAbility.utilityCounter.ResetTimer();
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/" + currentMovementAbility.prefabName), transform.position, Quaternion.identity);
		if (currentMovementAbility.takesDirection)
		{
			Vector2 direct = moveInterface.GetRawDifference().normalized;
			//temp.transform.SendMessage("SetDirection", direct);\
			temp.transform.SendMessage("SetDirection", moveInterface.currentDirection.normalized);
		}
		
		temp.transform.SendMessage("UseUtility");
        if (moveInterface.hasCrosshair) { moveDisplayCrosshair.ChangeVisibility(false); }
		//moveInterface.CancelTouch();
		/*utilityAbilityTimer.ResetTimer();
		utilityAbilityTimer.hasFinished = false;
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/" + currentUtilityAbility.prefabName), transform.position, Quaternion.identity);
		temp.transform.SendMessage("ApplyKnockBack");*/
		if (moveInterface.utilityAbility.cancelsTouchWhenUsed) { moveInterface.CancelTouch(); } else { moveInterface.doubleClick = false; }
		moveAbilityTrigger = false;
	}
	void DoesThisEndAnInterface(Touch t)
	{
		foreach (TouchInterface touchInterface in allTouchInterFaces)
		{
			if (touchInterface.isActive && touchInterface.touchId == t.fingerId) 
			{
				if(touchInterface.hasCrosshair)
				{
					if(touchInterface.typeOfInterface == InterfaceType.Move){moveDisplayCrosshair.ChangeVisibility(false);}
				}
				touchInterface.CancelTouch();
				DisplayCrosshairScript temp;
				if(touchInterface == mainMenuInterface) { MainScript.mainMenuObject.transform.position = MainScript.mainMenuObject.GetComponent<MainMenuScript>().menuClosedPosition; }
				if(touchInterface.typeOfInterface == InterfaceType.Move)
				{temp = moveDisplayCrosshair;}else{temp = shootDisplayCrosshair;}
				temp.ChangeVisibility(false);
				//movementDirect = Vector2.right;
			}
		}
	}
	public void BounceOff(Vector2 theNormal, Vector2 theVelocity)
	{

		//velocity = new Vector2(velocity.x - (velocity.x * (theNormal.x * Mathf.Sign(theNormal.x))),velocity.y);
		float xValue = 0f;
		float yValue = 0f;
		if (theNormal.x == 0f) { xValue = velocity.x; }
		if (theNormal.y == 0f) { yValue = velocity.y; }
		velocity = new Vector2(xValue, yValue);
		//velocity = new Vector2(velocity.x - (velocity.x * theNormal.x ),velocity.y - (velocity.y * theNormal.x));
	}
	public ControlsSettings GetActualSettings(float timePassed)
	{
		ControlsSettings temp = new ControlsSettings(defaultSettings.controlsPaused, defaultSettings.maxSpeed, defaultSettings.accelerationRate, defaultSettings.idleDecelerationRate);
		List<ControlsSettingsAffector> affectorsToThrowOut = new List<ControlsSettingsAffector>();
		foreach (ControlsSettingsAffector a in affectors)
		{
			switch (a.theType)
			{
				case ControlsSettingsAffectorType.pauseControls:
					temp.controlsPaused = true;
					break;
				case ControlsSettingsAffectorType.changeMaxSpeed:
					temp.maxSpeed = a.a * MainScript.blockHeight;
					break;
				case ControlsSettingsAffectorType.changeAcceleration:
					temp.accelerationRate = a.a * MainScript.blockHeight;
					break;
				case ControlsSettingsAffectorType.changeDeceleration:
					temp.idleDecelerationRate = a.a * MainScript.blockHeight;
					break;
				case ControlsSettingsAffectorType.changeAll:
					temp.maxSpeed = a.a * MainScript.blockHeight;
					temp.accelerationRate = a.b * MainScript.blockHeight;
					temp.idleDecelerationRate = a.c * MainScript.blockHeight;
					break;
				default:
					break;

			}
			if (a.AddTime(timePassed))
			{
				affectorsToThrowOut.Add(a);
			}
		}
		while (affectorsToThrowOut.Count > 0)
		{
			ControlsSettingsAffector tempAffector = affectorsToThrowOut[0];
			affectors.Remove(tempAffector);
			affectorsToThrowOut.Remove(tempAffector);
		}
		return temp;
	}
	void FireLeftTrigger()
	{
		//print("firing left trigger");
		
		/*Vector2 mousePosition = Camera.main.ScreenToWorldPoint((Vector3)Input.mousePosition);
		Vector2 direction = mousePosition - (Vector2)transform.position;*/
		//Vector2 rawDifference = shootInterface.GetRawDifference();
		Vector2 direction = shootInterface.currentDirection;
		if (direction == Vector2.zero) { }
		currentTriggerAbility.triggerCounter.ResetTimer();
		//bulletCounter.ResetTimer();
		//bulletCounter.hasFinished = false;
		GameObject bullet = (GameObject)Instantiate(Resources.Load("Prefabs/" + currentTriggerAbility.prefabName), transform.position, Quaternion.identity);
		bullet.transform.localScale = new Vector3(MainScript.blockWidth / 0.16f, MainScript.blockWidth / 0.16f, 1f);
		bullet.SendMessage("SetDirection", direction.normalized);
	}
	// Update is called once per frame
	void Update()
	{
		UpdateAllTouchInterfaces();
        
		/*if (Input.GetKeyDown(KeyCode.W)) { movementDirect += Vector2.up; }
        if (Input.GetKeyDown(KeyCode.A)) { movementDirect += Vector2.left; }
        if (Input.GetKeyDown(KeyCode.S)) { movementDirect += Vector2.down; }
        if (Input.GetKeyDown(KeyCode.D)) { movementDirect += Vector2.right; }
        if (Input.GetKeyUp(KeyCode.W)) { movementDirect -= Vector2.up; }
        if (Input.GetKeyUp(KeyCode.A)) { movementDirect -= Vector2.left; }
        if (Input.GetKeyUp(KeyCode.S)) { movementDirect -= Vector2.down; }
        if (Input.GetKeyUp(KeyCode.D)) { movementDirect -= Vector2.right; }
        if (Input.GetKeyDown(KeyCode.F) && utilityAbilityTimer.hasFinished) { activatingUtility = true; }
        if (Input.GetKeyUp(KeyCode.F)) { activatingUtility = false; }
        if (Input.GetMouseButtonDown(0) && bulletCounter.hasFinished) { leftTrigger = true; }
        if (Input.GetMouseButtonUp(0)) { leftTrigger = false; }
		//if (Input.GetMouseButtonDown(0)) { shootAbilityTrigger = true; }
		//if (Input.GetMouseButtonDown(0) && bulletCounter.hasFinished) { leftTrigger = true; } else { leftTrigger = false; }
		//print(movementDirect);
		if (activatingUtility && utilityAbilityTimer.hasFinished)
		{
			//UseUtilityAbility();
		}
		//if (leftTrigger && bulletCounter.hasFinished) { FireLeftTrigger(); }*/
	}
}
public class ControlsSettings
{
	public bool controlsPaused = false;
	public float maxSpeed;
	public float accelerationRate;
	public float idleDecelerationRate;
	public ControlsSettings(bool controlled, float theMax, float theAccel, float theDecel)
	{
		controlsPaused = controlled;
		maxSpeed = theMax;
		accelerationRate = theAccel;
		idleDecelerationRate = theDecel;
	}
}
public class ControlsSettingsAffector
{
	public ControlsSettingsAffectorType theType;
	public float a;
	public float b;
	public float c;
	public bool ends;
	public bool readyToEnd = false;
	public float timeBeforeEnd;
	public float currentTime = 0f;
	public ControlsSettingsAffector(ControlsSettingsAffectorType applyType, float theA, float theB, float theC, float theTimeBeforeEnd)
	{
		theType = applyType;
		a = theA;
		b = theB;
		c = theC;
		if (theTimeBeforeEnd > 0f) { ends = true; timeBeforeEnd = theTimeBeforeEnd; }
	}
	public bool AddTime(float timeToAdd)
	{
		bool tempBool = false;
		currentTime += timeToAdd;
		if (currentTime > timeBeforeEnd) { tempBool = true; }
		return tempBool;
	}
}
public class TouchInterface
{
	public float timeNotMoving = 0f;
	public bool hasStoppedMoving = false;
	public float lastTimeTouchBegan = 0f;
	public bool isActive = false;
	public bool doubleClick = false;
	public Touch currentTouch;
	public int touchId;
	public TouchInterfaceAnchor anchor;
	public float minHeight;
	public float maxHeight;
	public float distance;
	public bool hasCrosshair = false;
	//public DisplayCrosshair relevantCrosshair;
	public Vector2 origin;
	public Vector2 lastDirection;
	public Vector2 currentDirection = Vector2.zero;
	public float minSwipeDistance;
	public float maxSwipeDistance;
	public UtilityAbility utilityAbility;
	public bool hasUtility = false;
	public InterfaceType typeOfInterface = InterfaceType.Passive;
	public TouchInterface(TouchInterfaceAnchor theAnchor, float theMinHeight, float theMaxHeight, float theDistance, float theMinSwipeDistance, float theMaxSwipeDistance)
	{
		anchor = theAnchor;
		minHeight = theMinHeight;
		maxHeight = theMaxHeight;
		distance = theDistance;
		minSwipeDistance = theMinSwipeDistance;
		maxSwipeDistance = theMaxSwipeDistance;

	}
	public Touch GetTouch()
    {
		for(int i = 0;i < Input.touchCount; i++)
        {
			Touch temp = Input.GetTouch(i);
			if(temp.fingerId == touchId)
            {
				return temp;
            }
        }
		return new Touch();
    }
	
	public Vector2 GetRawDifference()
	{
		//int i = 0;
		Touch temp;
		foreach(Touch t in Input.touches)
        {
			if(t.fingerId == touchId)
            {
				Vector2 difference = t.position - origin;
				return difference;
			}
        }
		//CancelTouch();
		return Vector2.zero;
		/*while (temp.fingerId != touchId && i < Input.touchCount)
		{
			i++;
			temp = Input.GetTouch(i);
		}*/
		
	}
	public Vector2 GetDirection()
	{
		int i = 0;
		Touch temp = Input.GetTouch(i);
		while(temp.fingerId != touchId && i < Input.touchCount)
		{
			i++;
			temp = Input.GetTouch(i);
		}
		Vector2 difference = temp.position - origin;
		return difference.normalized;
	}
	public float GetDistance()
	{
		int i = 0;
		Touch temp = Input.GetTouch(i);
		while(temp.fingerId != touchId && i < Input.touchCount)
		{
			i++;
			temp = Input.GetTouch(i);
		}
		Vector2 difference = temp.position - origin;
		return difference.magnitude;
	}
	public float GetFraction()
	{
		int i = 0;
		Touch temp = Input.GetTouch(i);
		while(temp.fingerId != touchId && i < Input.touchCount)
		{
			i++;
			temp = Input.GetTouch(i);
		}
		Vector2 difference = temp.position - origin;
		if(difference.magnitude > 1f){difference = difference.normalized;}
		return difference.magnitude;
	}
	public TouchInterfaceHighlightPanel GetRelevantPanel()
    {
		int referenceInt = 0;
		if (typeOfInterface == InterfaceType.Shoot) { referenceInt = 1; }
		return (MainScript.interfaceHighlightPanels[referenceInt]);
	}
	public void SetUpTouch(Touch t)
	{
		currentDirection = Vector2.zero;
		currentTouch = t;
		isActive = true;
		touchId = t.fingerId;
		origin = t.position;
		
		//if(hasCrosshair){relevantCrosshair.isVisible = true;}
		float timeDifference = MainScript.currentGame.timeSinceStart - lastTimeTouchBegan;
		//MainScript.staticString = timeDifference.ToString();
		doubleClick = false;
		//if(timeDifference < 0.25f){doubleClick = true;}
		if(typeOfInterface != InterfaceType.Passive && typeOfInterface != InterfaceType.tutorial)
        {
			if (t.tapCount > 1 && utilityAbility.utilityCounter.hasFinished) { doubleClick = true; }
			TouchInterfaceHighlightPanel tempPanel = GetRelevantPanel();
			if (doubleClick)
			{
				tempPanel.ChangeColor(Color.green);
			}
			else
			{
				tempPanel.ChangeColor(Color.blue);
			}
			//doubleClick = true;
			//MainScript.staticString = timeDifference.ToString();
			lastTimeTouchBegan = MainScript.currentGame.timeSinceStart;
			/*if (!hasUtility) { MainScript.staticString = "HAS UTILITY IS PROBLEM"; }
			if (!doubleClick) { MainScript.staticString = "DOUBLE CLICK IS PROBLEM"; }
			if (!utilityAbility.utilityCounter.hasFinished) { MainScript.staticString = "IT IS UTILITY ABILITYT HAS FINISHED"; }*/
			if (hasUtility) { if (doubleClick && utilityAbility.utilityCounter.hasFinished && utilityAbility.activation == UtilityActivatesOn.begin) { ActivateAbility(); } }
		}
		
	}
	public void ActivateAbility()
	{
		if(typeOfInterface == InterfaceType.Move){ControlsScript.moveAbilityTrigger = true;}
		if(typeOfInterface == InterfaceType.Shoot){ControlsScript.shootAbilityTrigger = true; }
	}
	public void CancelTouch()
	{
		isActive = false;
		if(typeOfInterface != InterfaceType.Passive) { GetRelevantPanel().ChangeColor(Color.gray); if (hasUtility) { if (doubleClick && utilityAbility.utilityCounter.hasFinished && utilityAbility.activation == UtilityActivatesOn.ends) { ActivateAbility(); } } }
		if(typeOfInterface == InterfaceType.tutorial)
		{
            if (ControlsScript.hasChangedTutorialPage)
            {
				ControlsScript.hasChangedTutorialPage = false;
            }
            else
            {
				MainMenuScript.tutorialScreenParent.position = MainMenuScript.tutorialScreenParentOriginalPosition;
			}
			
			
			
		}
		//touchId = 0;
		//origin = Vector2.zero;
		//MainScript.staticString = "gets to cancel Touch";
		///if(hasCrosshair){relevantCrosshair.isVisible = false;}
		
	}
}
public enum TouchInterfaceAnchor { upperLeft, rightSide, leftSide ,topCenter,topRight}
public enum ControlsSettingsAffectorType { pauseControls, changeMaxSpeed, changeAcceleration, changeDeceleration, changeAll }
public class AbilityReference
{
	public UtilityActivatesOn activation;
	public float coolDown;
	public string prefabName;
	public bool takesDirection;
	public AbilityIcon thisAbilityIcon;

	public AbilityReference(UtilityActivatesOn theActivation, float theCooldown, string theName,AbilityIcon theIcon,bool takingDirection)
	{
		activation = theActivation;
		coolDown = theCooldown;
		prefabName = theName;
		thisAbilityIcon = theIcon;
		takesDirection = takingDirection;
	}
}
public class UtilityAbility
{
	public string abilityName = "none";
	public UtilityActivatesOn activation;
	public Counter utilityCounter;
	public Transform currentUtility;
	public InterfaceType theTypeOfAbility;
	public bool takesDirection = false;
	public string prefabName;
	public bool cancelsTouchWhenUsed = false;
	public UtilityAbility(UtilityActivatesOn theActivation, Counter theCounter,string theName, bool takesADirection,InterfaceType theType)
	{
		theTypeOfAbility = theType;
		takesDirection = takesADirection;
		activation = theActivation;
		utilityCounter = theCounter;
		prefabName = theName;
	}
	public UtilityAbility(UtilityAbilityReference reference)
    {
		abilityName = reference.nameOfAbility;
		theTypeOfAbility = reference.abilityType;
		takesDirection = reference.takesDirection;
		activation = reference.activation;
		utilityCounter = new Counter(reference.cooldownTime);
		prefabName = reference.prefabName;
		cancelsTouchWhenUsed = reference.cancelsTouchInterface;
    }

}
public class UtilityAbilityReference
{
	public string nameOfAbility;
	public InterfaceType abilityType;
	public UtilityActivatesOn activation;
	public float cooldownTime;
	public bool takesDirection = false;
	public string prefabName;
	public Sprite iconSprite;
	public AbilityIcon thisReferenceIcon;
	public bool cancelsTouchInterface = false;
	public UtilityAbilityReference(UtilityActivatesOn theActivation,float theCooldownTime,string theNameOfPrefab,bool willTakeDirection,Sprite theIcon,InterfaceType theType,bool cancelsTheTouchInterface,string theName)
    {
		nameOfAbility = theName;
		cancelsTouchInterface = cancelsTheTouchInterface;
		abilityType = theType;
		iconSprite = theIcon;
		cooldownTime = theCooldownTime;
		prefabName = theNameOfPrefab;
		activation = theActivation;
		takesDirection = willTakeDirection;
    }
}
public class TriggerAbility
{
	public Counter triggerCounter;
	public string prefabName;
	public string abilityName = "none";
	public TriggerAbility(Counter theCounter,string theName)
	{
		triggerCounter = theCounter;
		prefabName = theName;
	}
	public TriggerAbility(TriggerAbilityReference reference)
    {
		abilityName = reference.nameOfAbility;
		triggerCounter = new Counter(reference.coolDown);
		prefabName = reference.prefabName;
    }
}	
public class TriggerAbilityReference
{
	public float coolDown;
	public string prefabName;
	public Sprite triggerAbilityIcon;
	public AbilityIcon thisReferenceIcon;
	public string nameOfAbility;
	public InterfaceType theType = InterfaceType.Trigger;
	public TriggerAbilityReference(float theCooldown,string thePrefabName,Sprite abilityIcon,string nameForTrigger)
    {
		nameOfAbility = nameForTrigger;
		triggerAbilityIcon = abilityIcon;
		coolDown = theCooldown;
		prefabName = thePrefabName;
    }
}
public class TouchInterfaceHighlightPanel
{
	public SpriteRenderer renderer;
	public Counter currentAbilityTimer;
	public SpriteInteractionObject redCooldownBar;
	public SpriteInteractionObject greenCooldownBar;
	public AbilityIcon currentInterfaceAbilityIcon;
	public Transform cooldownBarFrame;
	public Vector3 directTowardsCenterScreen;
	public InterfaceType theTypeOfinterface;
	public float localScaleMaxWidth = 0f;
	public TouchInterfaceHighlightPanel(SpriteRenderer theRender,Transform redBar,Transform greenBar,Transform cooldownFrame,Transform AbilityIcon,InterfaceType panelType)
    {
		renderer = theRender;
		float localScale = Screen.width * 0.0275f;
		float blockWidth = (Screen.width * 0.01f) / 20f;
		//localScale = localScale / 0.16f;
		Vector3 transformLocalScale = new Vector3(localScale, localScale, 1f);
		redCooldownBar = new SpriteInteractionObject(redBar);
		//redCooldownBar.renderer.transform.localScale = transformLocalScale;
		redCooldownBar.ChangeColor(Color.red);
		greenCooldownBar = new SpriteInteractionObject(greenBar);
		//greenCooldownBar.renderer.transform.localScale = transformLocalScale;
		directTowardsCenterScreen = Vector3.zero;
		if (panelType == InterfaceType.Move)
		{
			currentInterfaceAbilityIcon = new AbilityIcon(AbilityIcon, ControlsScript.currentMovementAbilitySprite, panelType);
			directTowardsCenterScreen = Vector3.right;
		}
		else if (panelType == InterfaceType.Shoot)
		{
			directTowardsCenterScreen = Vector3.left;
			currentInterfaceAbilityIcon = new AbilityIcon(AbilityIcon, ControlsScript.currentUtilityAbilitySprite, panelType);
		}

		greenCooldownBar.ChangeColor(Color.green);
		localScaleMaxWidth = (Screen.width * 0.004f) / 0.16f;
		cooldownBarFrame = cooldownFrame;
		//cooldownBarFrame.localScale = new Vector3(1f,1f,1f);
		//cooldownBarFrame.localScale = new Vector3((Screen.width * 0.004f) / 0.32f, (blockWidth * 2f) / 0.32f, 1f);
		//cooldownBarFrame.localScale = new Vector3((Screen.width * 0.004f) / 0.64f, (blockWidth * 2f) / 0.64f, 1f);
		
		redCooldownBar.renderer.transform.localScale = new Vector3((Screen.width * 0.004f) / 0.16f, (blockWidth * 2f)/ 0.16f, 1f);
		redCooldownBar.renderer.transform.Translate(directTowardsCenterScreen * blockWidth);
		greenCooldownBar.renderer.transform.localScale = new Vector3((Screen.width * 0.004f) / 0.16f, (blockWidth * 2f) / 0.16f, 1f);
		greenCooldownBar.renderer.transform.Translate(directTowardsCenterScreen * blockWidth);
		cooldownBarFrame = cooldownFrame;
		cooldownBarFrame.localScale = new Vector3((Screen.width * 0.004f) / 0.64f, (Screen.width * 0.004f) / 0.64f, 1f);
		//cooldownBarFrame.transform.Translate(directTowardsCenterScreen * blockWidth);
		cooldownBarFrame.Translate(directTowardsCenterScreen * blockWidth);
		theTypeOfinterface = panelType;
		
		currentInterfaceAbilityIcon.theTransform.Translate(directTowardsCenterScreen * -4f * blockWidth);
		currentInterfaceAbilityIcon.theTransform.position = new Vector3(currentInterfaceAbilityIcon.theTransform.position.x, currentInterfaceAbilityIcon.theTransform.position.y, -0.5f);
		//currentInterfaceAbilityIcon.render.renderer.cha
		//
		//cooldownBarFrame.localScale = new Vector3(MainScript.blockWidth * 8f, MainScript.blockWidth * 8f, 1f);
	}
	public void ChangeColor(Color theColor)
    {
		renderer.material.color = theColor;
    }
	public void UpdateHighlightPanel()
    {
		UtilityAbility relevantAbility;
		if(theTypeOfinterface == InterfaceType.Move)
        {
			relevantAbility = ControlsScript.currentMovementAbility;
        }
        else
        {
			relevantAbility = ControlsScript.currentUtilityAbility;
        }
		float remainingTime = relevantAbility.utilityCounter.currentTime / relevantAbility.utilityCounter.expiryTime;
        if (relevantAbility.utilityCounter.hasFinished) { remainingTime = 1f; }
		float currentRedBarWidth = localScaleMaxWidth - (remainingTime * localScaleMaxWidth);
		float missingRedBarWidth = localScaleMaxWidth - currentRedBarWidth;
		redCooldownBar.renderer.transform.localScale = new Vector3(currentRedBarWidth, redCooldownBar.renderer.transform.localScale.y, redCooldownBar.renderer.transform.localScale.z);
		redCooldownBar.renderer.transform.position = new Vector3(greenCooldownBar.renderer.transform.position.x - (missingRedBarWidth * (0.5f ) * 0.16f * directTowardsCenterScreen.x),greenCooldownBar.renderer.transform.position.y,-2f);

		//float finishedTime = (relevantAbility.utilityCounter.current)
		//redCooldownBar.
    }
}
public enum UtilityActivatesOn { doubleClick, doubleClickMinSwipeDistance, doubleClickMaxSwipeDistance, passive ,begin,ends,followsThrough,shoot,shootThenFollowThrough}
public enum InterfaceType {Move,Shoot,Passive,Trigger,tutorial}
