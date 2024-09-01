using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Purchasing;
using TMPro;

public class MainScript : MonoBehaviour
{
	public static TextMeshProUGUI scoreTextMesh;
	Counter menuTapCounter = new Counter(0.5f);
	public GameObject menuButtonPrefab;
	public static SpriteRenderer menuButton;
	[SerializeField]
	public static bool hasControllerInput = false;
	public TextMeshProUGUI movementAbilityTextMesh;
	public ControlsScript controlScript;
	public GameObject tutorialGameObject;
	public static TutorialPageScript tutorialMessage;
	public TextMeshProUGUI triggerAbilityTextMesh;
	public Image[] touchscreenThumbsticks;
	public TextMeshProUGUI utilityAbilityTextMesh;
	public RectTransform scoreTextTransform;
	public RectTransform movementAbilityTextTransform;
	public RectTransform triggerAbilityTextTransform;
	public RectTransform utilityAbilityTextTransform;

	public static Counter waitToOpenMenuAfterDeath;

	public static bool isShowingAd = false;
	public InterstitialAdExample advertisement;
	public Button removeAdsButton;
	public Transform iapManagerTransform;

	public static string staticString = "booboo";
	public static Vector2 playerDirection;
	public static MovementAffector gravity;
	public static float blockWidth;
	public static float blockLocalScale;
	public static float blockHeight;
	public static List<EmptyBlock> allEmptyBlocks;
	public static EmptyBlock[,] emptyBlocksByRow;
	public static List<UtilityAbilityInstance> utilityAbilities;
	public static List<GenericExplosion> explosions;
	public static List<InstantiationCheckBox> allInstantiationCheckBoxes;
	//public static List<Transform> explosions;
	public static GameMap currentMap;
	public static Game currentGame;
	public static bool paused = true;
	public static bool tutorialMode = false;
	public static bool isChangingTutorialPage = false;
	public static Transform thePlayer;
	public Transform topCollider;
	public static bool readyToEnd = false;
	public static bool gameHasStarted = false;
	public static List<Color> allPossibleBlockColors;
	public List<Color> possibleColors;
	public static bool removeAdsEnabled = true;
	public GameObject scoreScreenBackgroundPrefab;
	public GameObject topColliderPrefab;
	public GameObject tetrisBlockPrefab;
	public GameObject brickWallPrefab;
	public static GameObject mainMenuObject;
	public GameObject mainMenuPrefab;
	public GameObject playerPrefab;
	public GameObject emptyBlockPrefab;
	public GameObject explosionPrefab;
	public GameObject heartIconPrefab;
	public GameObject InstantiationCheckBoxPrefab;
	public GameObject cooldownFramePrefab;
	public GameObject TouchInterfaceHighlightPanelPrefab;
	public GameObject basicWhiteBlockPrefab;
	public GameObject abilityIconPrefab;
	
	public Counter emptyBlockRemoverCounter;
	public static List<EmptyBlock> blocksToRemove;
	public static List<MovingBlock> blocksToGetOutOfTheWay;
	public static List<HeartIcon> heartDisplayIcons;
	public static List<MovingBlock> allMovingBlocks;
	public static List<UnityEngine.UI.Text> mainMenuTextBoxes;
	public static List<TextMeshProUGUI> mainMenuTextMeshBoxes;
	//public static UnityEngine.UI.Text [] mainMenuTextBoxes;
	public static List<TouchInterfaceHighlightPanel> interfaceHighlightPanels;

	public Camera mainCamera;
	// Start is called before the first frame update
	void Start()
    {
		//PlayerPrefs.SetInt("removeAds", 0);
		//MainMenuScript.removeAdsButton = removeAdsButton;
		MainMenuScript.iapTransform = iapManagerTransform;
		allPossibleBlockColors = possibleColors; 
		waitToOpenMenuAfterDeath = new Counter(1f);
		blocksToGetOutOfTheWay = new List<MovingBlock>();
		blocksToRemove = new List<EmptyBlock>();
		emptyBlockRemoverCounter = new Counter(0.07f);
		//UnityEngine.UI.Text movementAbilityCanvasText = movementAbilityTextTransform.GetComponent<UnityEngine.UI.Text>();
		int textSize = (int)(Screen.width / 20f);
		//movementAbilityCanvasText.fontSize = textSize;
		//movementAbilityTextMesh = movementAbilityTextTransform.GetComponent<TextMeshProUGUI>();
		//Debug.Log(movementAbilityTextMesh.name);
		movementAbilityTextMesh.fontSize = textSize;
		//UnityEngine.UI.Text triggerAbilityCanvasText = triggerAbilityTextTransform.GetComponent<UnityEngine.UI.Text>();
		//triggerAbilityTextMesh = triggerAbilityTextTransform.GetComponent<TextMeshProUGUI>();
		//triggerAbilityCanvasText.fontSize = textSize;
		//UnityEngine.UI.Text utilityAbilityCanvasText = utilityAbilityTextTransform.GetComponent<UnityEngine.UI.Text>();
		//utilityAbilityTextMesh = utilityAbilityTextTransform.GetComponent<TextMeshProUGUI>();
		//utilityAbilityCanvasText.fontSize = textSize;
		scoreTextMesh = scoreTextTransform.GetComponent<TextMeshProUGUI>();
		scoreTextMesh.fontSize = (int)(Screen.width/20f);
		//mainMenuTextBoxes = new List<UnityEngine.UI.Text>() { movementAbilityCanvasText, triggerAbilityCanvasText, utilityAbilityCanvasText};
		mainMenuTextMeshBoxes = new List<TextMeshProUGUI>(){movementAbilityTextMesh,triggerAbilityTextMesh,utilityAbilityTextMesh};
		utilityAbilityTextMesh.fontSize = textSize;
		triggerAbilityTextMesh.fontSize = textSize;
		//scoreText.text = "";
		interfaceHighlightPanels = new List<TouchInterfaceHighlightPanel>();
		
		blockWidth = (Screen.width)/ 20f;
		blockWidth *= 0.01f;
		heartDisplayIcons = new List<HeartIcon>();
		Vector3 upperLeftCorner = new Vector3(transform.position.x, transform.position.y,0f) - new Vector3((Screen.width * 0.005f),Screen.height * -0.005f,0f);
		Vector3 originPosition = upperLeftCorner + new Vector3(blockWidth * 2f,blockWidth * -2f,0f);
		//Vector3 originPosition = upperLeftCorner;
		blockLocalScale = blockWidth / 0.16f;
		tutorialMessage = Instantiate(tutorialGameObject,Vector3.up * Screen.height * 0.0035f + Vector3.forward,Quaternion.identity).GetComponent<TutorialPageScript>();
		tutorialMessage.transform.localScale = new Vector3(blockLocalScale,blockLocalScale,1f) * 1f;
		tutorialMessage.gameObject.SetActive(false);
		tutorialMessage.SetupTutorialPage();

		mainMenuObject = Instantiate(mainMenuPrefab, new Vector3(0f + blockWidth * 0.5f, 0f, -3f), Quaternion.identity);
		MainMenuScript menuScript = mainMenuObject.GetComponent<MainMenuScript>();
		Vector3 menuButtonPos = upperLeftCorner + new Vector3(blockWidth * 1.25f,blockWidth * -5f,0f);
		menuButton = Instantiate(menuButtonPrefab, menuButtonPos, Quaternion.identity).GetComponent<SpriteRenderer>();
		menuButton.transform.localScale = new Vector3(blockLocalScale,blockLocalScale,1f) * 0.325f;
		

		for (int i = 0; i < 2; i++)
		{
			float remainingBottomRoom = (Screen.height - (Screen.width * 1.5f)) * 0.01f;
			Vector3 instantiationPoint = new Vector3(transform.position.x, transform.position.y, 0f) + new Vector3(Screen.width * -0.005f, Screen.height * -0.005f, 0f) + new Vector3(Screen.width * 0.0025f, remainingBottomRoom * 0.5f, 0f);
			instantiationPoint += new Vector3(Screen.width * 0.005f * i, 0f, 0f);
			Transform t = Instantiate(TouchInterfaceHighlightPanelPrefab, instantiationPoint, Quaternion.identity).transform;
			t.localScale = new Vector3((Screen.width * 0.005f) / 0.16f, remainingBottomRoom / 0.16f, 1f);
			InterfaceType tempType = InterfaceType.Passive;
			if( i == 0) { tempType = InterfaceType.Move; } else { tempType = InterfaceType.Shoot; }
			TouchInterfaceHighlightPanel tempPanel = new TouchInterfaceHighlightPanel(t.GetComponent<SpriteRenderer>(), Instantiate(basicWhiteBlockPrefab, instantiationPoint + new Vector3(0f, (remainingBottomRoom * 0.5f) - (blockWidth), 0f), Quaternion.identity).transform, Instantiate(basicWhiteBlockPrefab, instantiationPoint + new Vector3(0f, (remainingBottomRoom * 0.5f) - (blockWidth), -0.1f), Quaternion.identity).transform, Instantiate(cooldownFramePrefab, instantiationPoint + new Vector3(0f, (remainingBottomRoom * 0.5f) - blockWidth, -2.002f), Quaternion.identity).transform, Instantiate(abilityIconPrefab, instantiationPoint + new Vector3(0f, (remainingBottomRoom * 0.5f) - blockWidth, -0.05f), Quaternion.identity).transform,tempType);
			tempPanel.ChangeColor(Color.gray);
			interfaceHighlightPanels.Add(tempPanel);
		}
		
		allInstantiationCheckBoxes = new List<InstantiationCheckBox>();
		for(int i = 0; i < 3;i++)
		{
			Vector3 instantiationPlace = originPosition + new Vector3(i * (blockWidth * 1.5f),0f,-0.1f);
			Transform temp = Instantiate(heartIconPrefab,instantiationPlace,Quaternion.identity).transform;
			temp.localScale = new Vector3(blockLocalScale,blockLocalScale,1f);
			HeartIconScript tempScript = temp.GetComponent<HeartIconScript>();
			tempScript.thisHeartIcon = new HeartIcon(true, temp.GetComponent<SpriteRenderer>(),temp);
			heartDisplayIcons.Add(tempScript.thisHeartIcon);
		}
		
		//UI Elements
		scoreTextTransform.position = new Vector3(Screen.width * 0.8f, Screen.height - (Screen.width * 0.1f), 0f);
		Vector3 positionOfScoreTextBackground = transform.position + new Vector3(Screen.width * 0.005f, Screen.height * 0.005f, 9f) + new Vector3(Screen.width * -0.002f,Screen.width * -0.001f,0f);
		//positionOfScoreTextBackground += new Vector3(Screen.width * 0.0008f, (Screen.height * 0.01f) - (Screen.width * 0.0001f), 0f);
		Instantiate(scoreScreenBackgroundPrefab, positionOfScoreTextBackground, Quaternion.identity).transform.localScale = new Vector3(MainScript.blockLocalScale,MainScript.blockLocalScale,1f);

		scoreTextMesh.text = "";
		Vector3 originalPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
		/*for (int i = 0; i < mainMenuTextBoxes.Count; i++)
		{
			UnityEngine.UI.Text tempText = mainMenuTextBoxes[i];
			tempText.transform.position = originalPosition + new Vector3(0f, Screen.width * 0.1f, 0f) + new Vector3(0f, i * Screen.width * -0.25f);
			tempText.enabled = false;
		}*/
		for(int i = 0; i < mainMenuTextMeshBoxes.Count;i++){
			TextMeshProUGUI text = mainMenuTextMeshBoxes[i];
			text.transform.position = originalPosition + new Vector3(0f,Screen.width * 0.1f,0f) + new Vector3(0f,i * Screen.width * -0.25f);
			text.enabled = false;
		}
		//scoreTextTransform.rect = new Rect(scoreTextTransform.position, new Vector2(Screen.width * 0.5f, Screen.height * 0.2f));

		//scoreTextTransform.position = new Vector3(Screen.width * 0.5f, Screen.height * -0.15f, 0f);
		emptyBlocksByRow = new EmptyBlock[18, 28];
		Time.fixedDeltaTime = 1 / 60f;
		
		//blockWidth *= 0.01f;
		blockHeight = blockWidth;
		allMovingBlocks = new List<MovingBlock>();
		explosions = new List<GenericExplosion>();
		float tempFloat = Screen.height / blockWidth;
		utilityAbilities = new List<UtilityAbilityInstance>();
		float orthoSize = tempFloat * blockWidth * 0.005f;
		mainCamera = GetComponent<Camera>();
		//float orthoSize = 3.2f * Screen.height /Screen.width * 0.5f;
		mainCamera.orthographicSize = orthoSize;
		
		allEmptyBlocks = new List<EmptyBlock>();
		//blockWidth = Screen.width / 20f;
		//CreateExplosionAt(Vector2.zero);
		float topOfScreen = transform.position.y + (Screen.height * 0.005f);
		topCollider = Instantiate(topColliderPrefab, Vector2.zero, Quaternion.identity).transform;
		TopColliderScript.SetUpAffector();
		topCollider.position = new Vector3(transform.position.x,topOfScreen - (blockWidth * 2.5f),0f);
		topCollider.localScale = new Vector3(blockLocalScale * 20f,blockLocalScale * 5f,1f);
		CreateTheBoard(20, 30);
		//StartTheGame();
		menuScript.SetUpMenu();
		SetThumbsticks(false);
		thePlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).transform;
		thePlayer.GetComponent<ControlsScript>().SendMainMenuScript(mainMenuObject.GetComponent<MainMenuScript>());
		//thePlayer.GetComponent<PlayerScript>().SetUpPlayer();
		//thePlayer.GetComponent<ControlsScript>().SendMainMenuScript(main);
		thePlayer.SendMessage("SetUpPlayer");		
		controlScript = thePlayer.GetComponent<ControlsScript>();
		//thePlayer.gameObject.SetActive(false);
	}
	public void SetThumbsticks(bool enableOn){	foreach(Image img in touchscreenThumbsticks){img.gameObject.SetActive(enableOn);}}
	public void GetReadyToRemoveAdsButton()
    {
		removeAdsEnabled = false;
    }
	public static MovementAffector GetMovementAffectorTowardsPlayer(MovingBlock m,float speed, float decayAmount,float endTime)
    {
		MovementAffector temp = new MovementAffector(Vector2.zero, speed, decayAmount, endTime, MovementAffectorType.moveTowardsPlayer);
		temp.owner = m;
		temp.identifierName = "moveTowardsPlayer";
		return temp;
    }
	public void GetMovingBlocksOutOfTheWay()
    {
		List<MovingBlock> movingBlocksToRemove = new List<MovingBlock>();
		foreach(MovingBlock m in blocksToGetOutOfTheWay)
        {
            if (m.lastBestDirectionDistance.distance > blockWidth * 0.6f) 
			{
				m.rbody.MovePosition((Vector2)m.theTransform.position + (m.lastBestDirectionDistance.direction.normalized * blockWidth * 0.6f));
				movingBlocksToRemove.Add(m);
			}
        }
		while(movingBlocksToRemove.Count > 0)
        {
			MovingBlock temp = movingBlocksToRemove[0];
			blocksToGetOutOfTheWay.Remove(temp);
			movingBlocksToRemove.Remove(temp);
        }
		int countCheck = 0;
		while(countCheck < 12 && blocksToGetOutOfTheWay.Count > 0)
        {
			countCheck++;
			if(countCheck == 1000) { print("its infinite"); }
			float highestDistance = blocksToGetOutOfTheWay[0].lastBestDirectionDistance.distance;
			List<DirectionAndDistance> highestDirectionDistance = new List<DirectionAndDistance>();
			MovingBlock firstToMove = blocksToGetOutOfTheWay[0];
			foreach (MovingBlock m in blocksToGetOutOfTheWay)
			{
				m.lastBestDirectionDistance = m.MoveOutOfTheWay();
				if (m.lastBestDirectionDistance.distance > highestDistance) 
				{
					print("we have changed highestDistance from default");
					firstToMove = m;
					highestDistance = m.lastBestDirectionDistance.distance;
				}
			}
			firstToMove.rbody.MovePosition((Vector2)firstToMove.theTransform.position + (firstToMove.lastBestDirectionDistance.direction.normalized * blockHeight * 3.5f * Time.fixedDeltaTime));
			blocksToGetOutOfTheWay.Remove(firstToMove);
		}
		blocksToGetOutOfTheWay = new List<MovingBlock>();
    }
	public static void LockInMovingBlocks()
    {
		foreach (MovingBlock m in MainScript.allMovingBlocks) 
		{
            if (m.isActive)
            {
				m.rbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
			}
		}
	}
	public static void UnlockMovingBlocks()
    {
		foreach (MovingBlock m in MainScript.allMovingBlocks)
		{
			if (m.isActive)
			{
				m.rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
			}
		}
	}
	public static void EndGame()
	{
		
		scoreTextMesh.rectTransform.position = new Vector3(Screen.width * 0.8f, Screen.height - (Screen.width * 0.1f), 0f);
		scoreTextMesh.enabled = false;
		paused = true;
		gameHasStarted = false;
        if (thePlayer) { thePlayer.SendMessage("KillPlayer"); }
		//
		foreach(EmptyBlock e in allEmptyBlocks)
		{
			if(!e.isUntaken)
			{
				e.DetachAndDestroy();
			}
		}
		while(allMovingBlocks.Count > 0)
		{
			
			//Transform temp = m.theTransform;
			MovingBlock mBlock = allMovingBlocks[0];
			if(mBlock.isPartOfDescendingPiece){mBlock.theDescendingPiece.BreakApartDescendingPiece();}
			allMovingBlocks.Remove(mBlock);
			mBlock.theTransform.SendMessage("DestroyThisMovingBlock");
		}
		while(utilityAbilities.Count > 0)
        {
			UtilityAbilityInstance temp = utilityAbilities[0];
			utilityAbilities.Remove(temp);
			temp.abilityTransform.SendMessage("DestroyThisUtility");
		}
		while(currentGame.allBullets.Count > 0)
        {
			Bullet temp = currentGame.allBullets[0];
			currentGame.allBullets.Remove(temp);
			temp.theTransform.SendMessage("DestroyThisBullet");
        }
		while(explosions.Count > 0)
        {
			GenericExplosion temp = explosions[0];
			explosions.Remove(temp);
			temp.thisTransform.SendMessage("EndExplosion");
		}
		//MainScript.mainMenuObject.SendMessage("StartOpeningMenu");
	}
	public static void ChangeMenuTextVisibility(bool visibility)
    {
		foreach(TextMeshProUGUI pro in MainScript.mainMenuTextMeshBoxes){pro.enabled = visibility;}
	}
	public void OpenMenu()
	{
		
	}
	public static bool CheckRowForCompletion(int rowToCheck)
	{
		bool isComplete = true;
	
		//if(!emptyBlocksByRow[0,rowToCheck].isUntaken && !blocksToRemove.Contains(emptyBlocksByRow[0,rowToCheck]))
		for(int i =0; i < emptyBlocksByRow.GetLength(0);i++)
        {
			EmptyBlock e = emptyBlocksByRow[i, rowToCheck];
            if (blocksToRemove.Contains(e)) { isComplete = false; }
			//print(e.isUntaken);
			if (e.isUntaken)
			{
				isComplete = false;
			}
		}
		/*for(int i =0 ; i < allEmptyBlocks.Count ;i++)
		{
			EmptyBlock e = allEmptyBlocks[i];
			isComplete = false;
			if(e.yPos == rowToCheck)
			{
				if(e.isUntaken)
				{
					isComplete = false;
				}
			}
		}*/
		return isComplete;
		//return false;
	}
	public static void RemoveRowForPoints(int rowToRemove)
	{
		//print("we are removing the row");
		List<EmptyBlock> emptyBlocksToRelease = new List<EmptyBlock>();
		for (int i = 0; i < 18; i++)
		{
			EmptyBlock e = emptyBlocksByRow[i, rowToRemove];
			//emptyBlocksToRelease.Add(e);
			blocksToRemove.Add(e);
			//e.DetachAndDestroy();
		}
		/*foreach (EmptyBlock e in allEmptyBlocks)
		{
			if(e.yPos == rowToRemove)
			{
				e.DetachAndDestroy();
			}
		}*/
		currentGame.AddScore(10);
		scoreTextMesh.text = currentGame.score.ToString();
	}
	public static void CreateExplosionAt(Vector2 pointToExplode,Color explosionColor,float explosionLength,float explosionTime)
    {
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/GenericExplosionPrefab"), (Vector3)pointToExplode, Quaternion.identity);
		GenericExplosionPrefabScript tempScript = temp.transform.GetComponent<GenericExplosionPrefabScript>();
		tempScript.SetUpExplosion(explosionColor,explosionLength,explosionTime);
		//temp.transform.SendMessage("SetUpExplosion");
		explosions.Add(tempScript.thisExplosion);
    }
	public static void CreateShotgunBlastAt(Vector2 pointToBlast,Vector2 directionOfBlast,float flakSpeed,float flakTimeAlive)
    {
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/GenericExplosionPrefab"), (Vector3)pointToBlast, Quaternion.identity);
		GenericExplosionPrefabScript tempScript = temp.transform.GetComponent<GenericExplosionPrefabScript>();
		tempScript.SetUpShotgunBlast(Color.red,MainScript.blockHeight,6,directionOfBlast,flakTimeAlive,flakSpeed);
		//temp.transform.SendMessage("SetUpExplosion");
		explosions.Add(tempScript.thisExplosion);
	}
	public static void CreateConfetti(Color colorOfConfetti, float time, float  confettiCooldown,float confettiSpeed,float confettiTimeAlive)
    {
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/GenericExplosionPrefab"), thePlayer.position, Quaternion.identity);
		temp.transform.parent = thePlayer;
		GenericExplosionPrefabScript tempScript = temp.GetComponent<GenericExplosionPrefabScript>();
		tempScript.SetUpConfetti(colorOfConfetti, confettiTimeAlive, time, confettiSpeed,playerDirection,confettiCooldown);
		explosions.Add(tempScript.thisExplosion);
	}
	public static List<MovingBlock> GetAllBlocksFromPointWithinDistance(Vector2 thePoint, float theDistance)
	{
		List<MovingBlock> tempList = new List<MovingBlock>();
		foreach (MovingBlock m in allMovingBlocks)
		{
			float currentDistance = Vector2.Distance((Vector2)m.theTransform.position, thePoint);
			if (currentDistance <= theDistance)
			{
				tempList.Add(m);
			}
		}
		return (tempList);
	}
	public void StartTheGame()
	{
		SetThumbsticks(true);
		gameHasStarted = true;
		readyToEnd = false;
		//blocksToGetOutOfTheWay = new List<MovingBlock>();
		blocksToRemove = new List<EmptyBlock>();
		ControlsScript.ResetCooldowns();
		scoreTextMesh.text = "0";
        if (tutorialMode) 
		{
			tutorialMessage.gameObject.SetActive(true);
			tutorialMessage.SetupTutorialPage();
			scoreTextMesh.enabled = false; 
		} else { scoreTextMesh.enabled = true; }
		gravity = new MovementAffector(Vector2.down, 2.5f, 0f, false, false, 0f, MovementAffectorType.arbitrary);
		Vector3 lowerLeft = new Vector2(currentMap.lowerLeftCorner.x, currentMap.lowerLeftCorner.y);
		paused = false;
		currentGame = new Game();
		currentMap = new GameMap(20, 30);
		currentMap.lowerLeftCorner = lowerLeft;
		thePlayer.position = Vector3.zero;
		thePlayer.GetComponent<PlayerScript>().Revive();
		//thePlayer.gameObject.SetActive(true);
		//CreateRandomDescendingPieceAlongTheTop();
		//currentGame.allMovingBlocksInGame.Add(CreateRandomMovingBlock());
	}
	public static MovingBlock CreateIndividualMovingBlock(Vector3 positionToCreate)
	{
		//float widthScale = blockWidth / 0.16f;
		//float heightScale = blockHeight / 0.16f;
		float widthScale = blockWidth / 2f;
		float heightScale = blockWidth / 2f;
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/MovingBlockPrefab"), positionToCreate, Quaternion.identity);
		temp.transform.localScale = new Vector3(widthScale, heightScale, temp.transform.localScale.z);
		//temp.GetComponent<BoxCollider2D>().size = new Vector2(blockWidth / widthScale, blockHeight / heightScale) * 0.95f;
		float smallestSide = blockWidth;
		MovingBlockScript tempScript = temp.GetComponent<MovingBlockScript>();
		tempScript.thisBlock = new MovingBlock(0, 0, false, null, temp.transform);
		tempScript.thisBlock.EnableBlock();
		tempScript.thisBlock.spriteInteractionObject.ChangeColor(tempScript.thisBlock.GetRandomColor());
		allMovingBlocks.Add(tempScript.thisBlock);
		return tempScript.thisBlock;
	}
	public void CreateRandomDescendingPieceAlongTheTop()
	{
		int randomInt = (int)Random.Range(1f,14f);
		Vector3 lowerLeftCornerOfInstantiate = (Vector3)currentMap.lowerLeftCorner + new Vector3((float)randomInt * blockWidth,blockHeight * 24f,0f);
		Vector3 positionToInstantiate = lowerLeftCornerOfInstantiate + new Vector3(MainScript.blockWidth * 1.5f, MainScript.blockWidth * 1.5f);
		Transform instantiationCheckBox = Instantiate(InstantiationCheckBoxPrefab,positionToInstantiate,Quaternion.identity).transform;
		InstantiationCheckBoxScript tempScript = instantiationCheckBox.GetComponent<InstantiationCheckBoxScript>();
		tempScript.thisCheckBox = new InstantiationCheckBox(instantiationCheckBox,InstantiationBoxPrefab.descendingPiece);
		tempScript.thisCheckBox.SetSize(4,4);
		//currentGame.allDescendingPieces.Add(CreateDescendingPiece(positionToInstantiate,new bool[4,4]{{false,false,false,true},{false,true,true,false},{false,false,true,false},{false,false,false,true}}));
	}
	
	public static DescendingPiece CreateDescendingPiece(Vector3 positionToCreate,bool[,] blockPositions)
	{
		//List<Transform> tempList = new List<Transform>();
		//List<MovingBlock> theBlocksInDescendingPiece = new List<MovingBlock>();
		MovingBlock [,] blockGrid = new MovingBlock[blockPositions.GetLength(0),blockPositions.GetLength(1)];
		bool hasMadeFirstBlock = false;
		Transform theFixedJointHolder;
		Rigidbody2D rbodyToAddJointsTo;
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/DescendingPiecePrefab"),Vector3.zero,Quaternion.identity);
		DescendingPieceScript thePieceScript = temp.GetComponent<DescendingPieceScript>();
		//thePieceScript.thisPiece = new DescendingPiece(new List<Transform>(),new List<MovingBlock>(),blockGrid);
		thePieceScript.thisPiece = new DescendingPiece();
		thePieceScript.thisPiece.colorOfDescendingPiece = thePieceScript.thisPiece.GetRandomColor();
		thePieceScript.thisPiece.thisPieceTransform = temp.transform;
		//DescendingPiece descendingPiece = thePieceScript.thisPiece;
		thePieceScript.thisPiece.allMovingBlocks = new List<MovingBlock>();
		thePieceScript.thisPiece.movingPiecesGrid = new MovingBlock[4,4];
		thePieceScript.thisPiece.gridMap = new bool[4,4];
		for(int x = 0; x < blockPositions.GetLength(0);x++)
		{
			for(int y = 0; y < blockPositions.GetLength(1);y++)
			{
				if(blockPositions[x,y])
				{
					thePieceScript.thisPiece.gridMap[x,y] = true;
					Vector3 positionToInstantiate = positionToCreate + new Vector3(x * blockWidth,y * blockHeight,9f);
					thePieceScript.thisPiece.allMovingBlocks.Add(CreateDescendingPieceMovingBlock(positionToInstantiate,x,y,thePieceScript.thisPiece,!hasMadeFirstBlock).GetComponent<MovingBlockScript>().thisBlock);
					MovingBlock currentBlock = thePieceScript.thisPiece.allMovingBlocks[thePieceScript.thisPiece.allMovingBlocks.Count - 1];
					thePieceScript.thisPiece.movingPiecesGrid[x,y] = currentBlock;
					if(hasMadeFirstBlock)
					{
						theFixedJointHolder = thePieceScript.thisPiece.allMovingBlocks[0].theTransform;
						theFixedJointHolder.gameObject.AddComponent<FixedJoint2D>().connectedBody = currentBlock.rbody;
					}else
					{
						//tempList[tempList.Count - 1].isFixedJointHolder = true;
						thePieceScript.thisPiece.fixedJointHolder = thePieceScript.thisPiece.allMovingBlocks[0];
						theFixedJointHolder = thePieceScript.thisPiece.allMovingBlocks[0].theTransform;
						rbodyToAddJointsTo = thePieceScript.thisPiece.allMovingBlocks[0].theTransform.GetComponent<Rigidbody2D>();
						thePieceScript.thisPiece.allMovingBlocks[0].isFixedJointHolder = true;
						hasMadeFirstBlock = true;
					}
				}else{thePieceScript.thisPiece.gridMap[x,y] = false;}
			}
		}
		foreach(MovingBlock m in thePieceScript.thisPiece.allMovingBlocks)
        {
			TopColliderScript.AddMovingBlock(m);
        }
		return(thePieceScript.thisPiece);
	}
	public static Transform CreateDescendingPieceMovingBlock(Vector3 positionToCreate, int xPos, int yPos, DescendingPiece thePiece, bool isTheParentBlock)
	{
		float widthScale = blockWidth / 0.16f;
		float heightScale = blockHeight / 0.16f;
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/MovingBlockPrefab"), positionToCreate, Quaternion.identity);
		temp.transform.localScale = new Vector3(widthScale, heightScale, temp.transform.localScale.z);
		//temp.GetComponent<BoxCollider2D>().size = new Vector2(blockWidth / widthScale, blockHeight / heightScale) * 0.95f;
		//float smallestSide = blockWidth;
		MovingBlockScript tempScript = temp.GetComponent<MovingBlockScript>();
		tempScript.thisBlock = new MovingBlock(xPos, yPos, true, thePiece, temp.transform);
		tempScript.thisBlock.blockColor = thePiece.colorOfDescendingPiece;
		//TopColliderScript.allMovingBlocksInside.Add(tempScript.thisBlock);
		tempScript.thisBlock.spriteInteractionObject.ChangeColor(thePiece.colorOfDescendingPiece);
		//currentGame.allMovingBlocksInGame.Add(tempScript.thisBlock);
		tempScript.thisBlock.isFixedJointHolder = isTheParentBlock;
		tempScript.thisBlock.theDescendingPiece = thePiece;
		allMovingBlocks.Add(tempScript.thisBlock);
		//tempScript.StartUpBlock();
		tempScript.thisBlock.isPartOfDescendingPiece = true;
		tempScript.thisBlock.EnableBlock();
		//currentGame.allMovingBlocksInGame.Add(tempScript.thisBlock);
		
		//temp.GetComponent<CircleCollider2D>().radius = smallestSide * 0.205f;
		return temp.transform;
	}
	void CreateTheBoard(int width, int height)
	{
		blockWidth = Screen.width / 20f * 0.01f;
		blockHeight = blockWidth;
		//blockHeight = (((Screen.height )/(float)boardHeight) * 0.008f);
		float widthScale = blockWidth / 0.16f;
		float heightScale = blockHeight / 0.16f;
		allEmptyBlocks = new List<EmptyBlock>();
		Vector3 positionOfPlayer = transform.position + new Vector3(0f, Screen.height * 0.3f, 0f);
		currentMap = new GameMap(width, height);
		float yPosOfBottomLeft = transform.position.y - ((Screen.height * 0.5f) * 0.01f);
		float remainingSpace = (Screen.height * 0.01f) - (blockWidth * 30f);
		Vector2 bottomLeftCorner = new Vector2(transform.position.x - ((Screen.width * 0.5f) * 0.01f), yPosOfBottomLeft) + new Vector2(blockWidth * 0.5f, blockHeight * 0.5f);
		currentMap.lowerLeftCorner = bottomLeftCorner + new Vector2(0f,remainingSpace);
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if ((x == 0 || x == width - 1) || (y == 0 || y == height - 1))
				{
					GameObject temp = (GameObject)Instantiate(brickWallPrefab, (currentMap.lowerLeftCorner + (new Vector2(blockWidth * x, blockHeight * y))), Quaternion.identity);
					temp.transform.localScale = new Vector3(widthScale, heightScale, temp.transform.localScale.z);
				}
				else
				{
					GameObject temp = (GameObject)Instantiate(emptyBlockPrefab, (Vector3)currentMap.lowerLeftCorner + new Vector3(blockWidth * x, blockHeight * y, 10f), Quaternion.identity);
					temp.transform.localScale = new Vector3(widthScale, heightScale, temp.transform.localScale.z);
					EmptyBlockScript tempScript = temp.GetComponent<EmptyBlockScript>();
					tempScript.thisEmptyBlock = new EmptyBlock(x, y, temp.transform);
					emptyBlocksByRow[x - 1, y - 1] = tempScript.thisEmptyBlock;
					allEmptyBlocks.Add(tempScript.thisEmptyBlock);
				}
			}
		}
		//FillInTheBoard();
		
	}
	public static List<MovingBlock> GetAllBlocksWithinBlockDistance(Vector2 thePoint)
	{
		List<MovingBlock> tempList = new List<MovingBlock>();
		foreach (MovingBlock m in allMovingBlocks)
		{
			bool verticallyWithinRange = false;
			bool horizontallyWithinRange = false;
			float xDifference = m.theTransform.position.x - thePoint.x;
			float yDifference = m.theTransform.position.y - thePoint.y;
			float absoluteXDifference = xDifference * Mathf.Sign(xDifference);
			float absoluteYDifference = yDifference * Mathf.Sign(yDifference);
			if (absoluteXDifference < blockWidth * 0.995f) { horizontallyWithinRange = true; }
			if (absoluteYDifference < blockHeight * 0.995f) { verticallyWithinRange = true; }
			if (horizontallyWithinRange && verticallyWithinRange)
			{
				//block m must be touching this block
				tempList.Add(m);
			}
		}
		return tempList;
	}
	public static EmptyBlock GetClosestBlockToPoint(Vector2 thePoint)
	{
		EmptyBlock lowestDistanceYet = null;
		float lowestDistanceSoFar = Mathf.Infinity;
		foreach (EmptyBlock eBlock in allEmptyBlocks)
		{
			if (eBlock.isUntaken)
			{
				float currentDistance = Vector2.Distance((Vector2)eBlock.thisTransform.position, thePoint);
				if (currentDistance < lowestDistanceSoFar) { lowestDistanceYet = eBlock; lowestDistanceSoFar = currentDistance; }
			}
		}
		//print("started with point " + thePoint + " ended at " + lowestDistanceYet.thisTransform.position );
		return lowestDistanceYet;
	}
	public void CreateRandomMovingBlock()
    {
		int randomInt = (int)Random.Range(1f,17f);
		Vector3 positionToInstantiate = (Vector3)currentMap.lowerLeftCorner + new Vector3((float)randomInt * blockWidth,blockHeight * 25f,0f);
		Vector3 lowerLeftCornerOfInstantiate = (Vector3)currentMap.lowerLeftCorner + new Vector3((float)randomInt * blockWidth,blockHeight * 24f,0f);
		//Vector3 positionToInstantiate = lowerLeftCornerOfInstantiate + new Vector3(MainScript.blockWidth * 1.5f, MainScript.blockWidth * 1.5f);
		Transform instantiationCheckBox = Instantiate(InstantiationCheckBoxPrefab,positionToInstantiate,Quaternion.identity).transform;
		InstantiationCheckBoxScript tempScript = instantiationCheckBox.GetComponent<InstantiationCheckBoxScript>();
		tempScript.thisCheckBox = new InstantiationCheckBox(instantiationCheckBox,InstantiationBoxPrefab.movingBlock);
		tempScript.thisCheckBox.SetSize(1,1);
		//Vector3 instantiationPoint = transform.position + Vector3.forward;
		//return IndividualMovingBlock(positionToInstantiate);
    }
	// Update is called once per frame
	public static MovingBlock IndividualMovingBlock(Vector3 positionToCreate)
	{
		float widthScale = blockWidth / 0.16f;
		float heightScale = blockHeight / 0.16f;
		GameObject temp = (GameObject)Instantiate(Resources.Load("Prefabs/MovingBlockPrefab"), positionToCreate, Quaternion.identity);
		temp.transform.localScale = new Vector3(widthScale, heightScale, temp.transform.localScale.z);
		//temp.GetComponent<BoxCollider2D>().size = new Vector2(blockWidth / widthScale, blockHeight / heightScale) * 0.95f;
		//float smallestSide = blockWidth;
		MovingBlockScript tempScript = temp.GetComponent<MovingBlockScript>();
		tempScript.thisBlock = new MovingBlock(temp.transform);
		TopColliderScript.AddMovingBlock(tempScript.thisBlock);
		tempScript.thisBlock.theTransform = temp.transform;
		//currentGame.allMovingBlocksInGame.Add(tempScript.thisBlock);
		allMovingBlocks.Add(tempScript.thisBlock);
		//tempScript.StartUpBlock();
		tempScript.thisBlock.EnableBlock();
		return tempScript.thisBlock;
	}
    private void OnGUI()
    {
		//GUI.Box(new Rect(new Vector2(0f,0f),new Vector2(Screen.width,Screen.height * 0.05f)), staticString);
    }
    void UpdateGame(float timePassed)
	{
		
        if (tutorialMode)
        {
            if (isChangingTutorialPage)
            {
				mainMenuObject.SendMessage("UpdateTutorialScreenPage");
            }
        }
		currentGame.AddTime(timePassed);
        if (!emptyBlockRemoverCounter.hasFinished) { emptyBlockRemoverCounter.AddTime(timePassed); } else
        {
			if (blocksToRemove.Count > 0)
			{
				int randomInt = (int)Random.Range(0f, (float)blocksToRemove.Count);
				EmptyBlock temp = blocksToRemove[randomInt];
				blocksToRemove.Remove(temp);
				MainScript.CreateExplosionAt(temp.thisTransform.position + new Vector3(Random.Range(-0.5f * blockWidth,1f * blockWidth), Random.Range(-0.5f * blockWidth, 1f * blockWidth),0f), temp.currentMovingBlock.blockColor, blockWidth * 3f, 1f);
				temp.DetachAndDestroy();
				emptyBlockRemoverCounter.ResetTimer();
				CheckAboveEmptyBlocksForBlocksToRemove(temp);
				//emptyBlocksByRow
			}
		}
		foreach(TouchInterfaceHighlightPanel panel in interfaceHighlightPanels)
        {
			panel.UpdateHighlightPanel();
        }
		if(currentGame.timeSinceStart > currentGame.phaseLength * (float)currentGame.currentPhase)
		{
			currentGame.currentPhase++;
            if (!MainScript.tutorialMode)
            {
				gravity.speed += blockWidth * 0.5f;
				currentGame.defaultSettings.timeBetweenDescendingPieceSpawns -= 1f;
				currentGame.defaultSettings.timeBetweenRandomMovingBlockSpawns -= 1f;
				if (currentGame.defaultSettings.timeBetweenRandomMovingBlockSpawns < 0.5f) { currentGame.defaultSettings.timeBetweenRandomMovingBlockSpawns = 0.5f; }
				if (currentGame.defaultSettings.timeBetweenDescendingPieceSpawns < 2f) { currentGame.defaultSettings.timeBetweenDescendingPieceSpawns = 2f; }
			}
			
		}
		
		List<GenericExplosion> explosionsToRemove = new List<GenericExplosion>();
		foreach(GenericExplosion g in explosions)
        {
			g.UpdateExplosion();
			if(g.ReadyToDie)
            {
				
				explosionsToRemove.Add(g);
            }
        }
		while(explosionsToRemove.Count > 0)
        {
			GenericExplosion tempExplode = explosionsToRemove[0];
			explosions.Remove(tempExplode);
			explosionsToRemove.Remove(tempExplode);
			tempExplode.thisTransform.SendMessage("EndExplosion");
			//Destroy(tempExplode.thisTransform.gameObject);
        }
			foreach (MovingBlock m in allMovingBlocks)
		{
			if (m.isActive) {  m.UpdatePosition(timePassed); }

			//m.theTransform.GetComponent<MovingBlockScript>().UpdateBlock();
		}
		foreach (DescendingPiece d in currentGame.allDescendingPieces)
		{

		}
		if (currentGame.timeSinceLastDescendingPiece > currentGame.defaultSettings.timeBetweenDescendingPieceSpawns)
		{
			CreateRandomDescendingPieceAlongTheTop();
			currentGame.timeSinceLastDescendingPiece = 0f;
		}
		if(currentGame.timeSinceLastRandomMovingBlock > currentGame.defaultSettings.timeBetweenRandomMovingBlockSpawns)
		{
			CreateRandomMovingBlock();
			currentGame.timeSinceLastRandomMovingBlock = 0f;
		}
		List<Bullet> bulletsToRemove = new List<Bullet>();
		foreach (Bullet b in currentGame.allBullets)
		{
			b.UpdatePosition(timePassed);
			if (b.readyToDie) { bulletsToRemove.Add(b); }
		}
		List<InstantiationCheckBox> boxesToDelete = new List<InstantiationCheckBox>();
		foreach(InstantiationCheckBox checkBox in allInstantiationCheckBoxes)
		{
			checkBox.UpdateInstantiateBox();	
			if(checkBox.readyToDie)
			{
				boxesToDelete.Add(checkBox);
			}
		}
		while(boxesToDelete.Count > 0)
		{
			InstantiationCheckBox currentCheckBox= boxesToDelete[0];
			allInstantiationCheckBoxes.Remove(currentCheckBox);
			boxesToDelete.Remove(currentCheckBox);
			Destroy(currentCheckBox.theTransform.gameObject);
		}
        if (!paused)
        {
			thePlayer.GetComponent<PlayerScript>().UpdatePosition(timePassed);
			for (int i = 0; i < emptyBlocksByRow.GetLength(1); i++)
			{
				if (MainScript.CheckRowForCompletion(i))
				{
					MainScript.RemoveRowForPoints(i);
				}
			}
		}
		foreach(EmptyBlock e in allEmptyBlocks)
        {
			e.UpdateEmptyBlock(timePassed);
        }
		List<UtilityAbilityInstance> abilitiesToDestroy = new List<UtilityAbilityInstance>();
		foreach(UtilityAbilityInstance reference in utilityAbilities)
        {
			reference.abilityTransform.SendMessage("UpdateUtility");
            if (reference.requiresUpdate) { reference.abilityTransform.SendMessage("SetDirection", controlScript.GetShootDir()); }
			if(reference.readyToDie)
			{
				abilitiesToDestroy.Add(reference);
			}
			
        }

		while(abilitiesToDestroy.Count > 0){ UtilityAbilityInstance temp = abilitiesToDestroy[0];utilityAbilities.Remove(temp);abilitiesToDestroy.Remove(temp); temp.abilityTransform.SendMessage("DestroyThisUtility");}
		//while(abilitiesToDestroy.Count > 0){UtilityAbilityReference temp = abilitiesToDestroy[0];utilityAbilities.Remove(temp);abilitiesToDestroy.Remove(temp); temp.SendMessage("DestroyThisUtility");}
		while (bulletsToRemove.Count > 0) { Bullet currentBullet = bulletsToRemove[0]; bulletsToRemove.Remove(currentBullet); currentBullet.DestroyBullet();  }
		if(blocksToGetOutOfTheWay.Count > 0)
        {
			//GetMovingBlocksOutOfTheWay();

		}
		foreach(EmptyBlock e in allEmptyBlocks)
		{
			if(!e.isUntaken && e.yPos > 24)
			{
				SetThumbsticks(false);
				SetUpEndGameCleanup();
			}
		}
		
        if (readyToEnd) { SetThumbsticks(false);SetUpEndGameCleanup();  }
	}
	void CheckAboveEmptyBlocksForBlocksToRemove(EmptyBlock e)
    {
		int currentYRef = e.yPos + 1;
		if(currentYRef < 30)
        {

			EmptyBlock currentEmpty = emptyBlocksByRow[e.xPos - 1, currentYRef - 1];

			if (!currentEmpty.isUntaken && !blocksToRemove.Contains(currentEmpty))
            {
				currentEmpty.Detach();
				CheckAboveEmptyBlocksForBlocksToRemove(currentEmpty);
            }
        }
    }
	public static void CheckScoreAgainstHighScores(int score)
    {
		int currentFirst = PlayerPrefs.GetInt("1stHighScore",0);
		int currentSecond = PlayerPrefs.GetInt("2ndHighScore",0);
		int currentThird = PlayerPrefs.GetInt("3rdHighScore",0);

		if(score > currentFirst)
		{
			PlayerPrefs.SetInt("3rdHighScore", currentSecond);
			PlayerPrefs.SetInt("2ndHighScore", currentFirst);
			PlayerPrefs.SetInt("1stHighScore", score);
		}
		else if (score > currentSecond)
        {
			PlayerPrefs.SetInt("3rdHighScore", currentSecond);
			PlayerPrefs.SetInt("2ndHighScore", score);
			
		}else if (score > currentThird)
        {
			PlayerPrefs.SetInt("3rdHighScore", score);
		}

	}
	public void DisableTutorial(){tutorialMessage.gameObject.SetActive(false);}
	public static void SetUpEndGameCleanup()
    {
        if (!tutorialMode)
        {
			waitToOpenMenuAfterDeath = new Counter(1f);
			waitToOpenMenuAfterDeath.ResetTimer();
			CheckScoreAgainstHighScores(currentGame.score);
			mainMenuObject.SendMessage("SetupEndGameScreen");
			tutorialMessage.gameObject.SetActive(true);
			tutorialMessage.SetupEndGame();
			scoreTextMesh.transform.position = new Vector2(Screen.width * 0.5f, Screen.height - (Screen.width * 0.4f));
			gameHasStarted = false;
			blocksToRemove = new List<EmptyBlock>();
			paused = true;
			foreach (MovingBlock m in allMovingBlocks) { if (m.isPartOfDescendingPiece) { m.theDescendingPiece.BreakApartDescendingPiece(); } m.StopMovingBlock(); }
			readyToEnd = true;
			while (utilityAbilities.Count > 0)
			{
				UtilityAbilityInstance temp = utilityAbilities[0];
				utilityAbilities.Remove(temp);
				if (temp.abilityTransform) { temp.abilityTransform.SendMessage("DestroyThisUtility"); }
			}
			while (currentGame.allBullets.Count > 0)
			{
				Bullet temp = currentGame.allBullets[0];
				currentGame.allBullets.Remove(temp);
				if (temp.theTransform) { temp.theTransform.SendMessage("DestroyThisBullet"); }
			}
			if (thePlayer) { thePlayer.SendMessage("KillPlayer"); }
		}
        else { EndGame(); 
			//mainMenuObject.SendMessage("StartOpeningMenu"); 
		}
		
	}
	void EndGameCleanup()
    {
		if(allMovingBlocks.Count > 0)
        {
			if (!emptyBlockRemoverCounter.hasFinished) { emptyBlockRemoverCounter.AddTime(Time.fixedDeltaTime); }
			if (emptyBlockRemoverCounter.hasFinished)
			{
				emptyBlockRemoverCounter.ResetTimer();
				int randomInt = (int)Random.Range(0f, allMovingBlocks.Count);
				MovingBlock temp = allMovingBlocks[randomInt];
                if (temp.isPartOfDescendingPiece) { temp.theDescendingPiece.BreakApartDescendingPiece(); }
				if (temp.hasSettledIntoPlace)
				{
					temp.currentEmptyBlock.Detach();
				}
				allMovingBlocks.Remove(temp);
				MainScript.CreateExplosionAt(temp.theTransform.position + new Vector3(Random.Range(-0.5f * blockWidth, 1f * blockWidth), Random.Range(-0.5f * blockWidth, 1f * blockWidth), 0f), temp.blockColor, blockWidth * 3f, 1f);
				temp.theTransform.SendMessage("DestroyThisMovingBlock");
			}
			
		}
		List<GenericExplosion> explosionsToRemove = new List<GenericExplosion>();
		foreach (GenericExplosion g in explosions)
		{
			g.UpdateExplosion();
			if (g.ReadyToDie)
			{

				explosionsToRemove.Add(g);
			}
		}
		while(explosionsToRemove.Count > 0)
        {
			GenericExplosion temp = explosionsToRemove[0];
			explosions.Remove(temp);
			explosionsToRemove.Remove(temp);
			temp.thisTransform.SendMessage("EndExplosion");
        }

	}
	void CheckController(){
		bool hasController = Gamepad.current != null;
		if(hasController != hasControllerInput){hasControllerInput = hasController;SetThumbsticks(!hasControllerInput);}
	}
	void FixedUpdate()
	{
		//CheckController();
		/*if (removeAdsButton.gameObject.active == true) 
		{ 
			if (removeAdsEnabled != true) 
			{
				MainMenuScript temp = mainMenuObject.GetComponent<MainMenuScript>();
				temp.DeactivateRemoveAdsButton();
				temp.SetUpMainMenu();
			}
		}*/
		if (!paused && !readyToEnd) 
		{ 
			UpdateGame(Time.fixedDeltaTime); 
		}else if(paused && MainMenuScript.isOpeningMenu)
		{
			mainMenuObject.SendMessage("OpeningMenu"); 
		}
        else if(readyToEnd)
		{
			EndGameCleanup();
			
            //if(Input.touchCount > 0f) { mainMenuObject.SendMessage("SetUpMainMenu"); }
		}
	}
	void Update()
    {
       if(paused && readyToEnd && !gameHasStarted && !MainMenuScript.menuIsActive)
        {
            if (waitToOpenMenuAfterDeath.hasFinished)
            {
				for (int i = 0; i < Touchscreen.current.touches.Count; i++)
				{
					TouchControl touch = Touchscreen.current.touches[i];
					if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
					{
						EndGame();
						
						if(PlayerPrefs.GetInt("removeAds",0) == 0)
                        {
							isShowingAd = true;
							//AdsManagerScript.ShowInterstitialAd();//THIS IS WHERE WE SHOW TH AD
							advertisement.ShowAd();
                        }
                        else
                        {
							tutorialMessage.DisableTutorial();
							gameHasStarted = false;
							mainMenuObject.SendMessage("StartOpeningMenu");
						}
					}
				}
			}
            else
            {
				waitToOpenMenuAfterDeath.AddTime(Time.deltaTime);
            }
		}else
		{
			bool tappedMenu = false;
			if(Time.time > 0.25f)
			{
				if(menuTapCounter.hasFinished)
				{
					for (int i = 0; i < Touchscreen.current.touches.Count; i++)
					{
						TouchControl t = Touchscreen.current.touches[i];
						switch (t.phase.ReadValue())
						{
							case UnityEngine.InputSystem.TouchPhase.Began:
								Vector3 realPos = Camera.main.ScreenToWorldPoint(t.position.ReadValue());
								Vector3 diff = realPos - MainScript.menuButton.transform.position;diff.z = 0f;
								//Debug.Log(diff.magnitude + " from Menu button");
								if(diff.magnitude < MainScript.blockHeight * 2f){tappedMenu = true;menuTapCounter.ResetTimer();}
							break;
						}
					}
					if(tappedMenu)
					{
						mainMenuObject.GetComponent<MainMenuScript>().HasTappedMenuButton();
					}
				}else{menuTapCounter.AddTime(Time.deltaTime);}
				
			}
			
		}
		
	}
}
public class GameMap
{
	public int height;
	public int width;
	public MovingBlock[,] settledSquares;
	public Vector2 lowerLeftCorner;
	public GameMap(int theWidth, int theHeight)
	{
		height = theHeight;
		width = theWidth;
		settledSquares = new MovingBlock[width, height];
	}
}
public class Game
{
	public GameSettings defaultSettings;
	public float theScore;
	public float timeSinceStart = 0f;
	public int score = 0;
	public int currentPhase = 1;
	public float phaseLength = 45f;
	public bool hasStarted = false;
	public bool hasEnded = false;
	public float timeSinceLastDescendingPiece = 10f;
	public float timeSinceLastRandomMovingBlock = 0f;
	public List<MovingBlock> allMovingBlocksInGame;
	public List<DescendingPiece> allDescendingPieces;
	public List<Bullet> allBullets;
	public Game()
	{
		allBullets = new List<Bullet>();
		defaultSettings = new GameSettings();
        if (MainScript.tutorialMode) { defaultSettings.timeBetweenDescendingPieceSpawns = 10f; defaultSettings.timeBetweenRandomMovingBlockSpawns = 5f; }
		allMovingBlocksInGame = new List<MovingBlock>();
		allDescendingPieces = new List<DescendingPiece>();
	}
	public void AddScore(int scoreToAdd)
	{
		score += scoreToAdd;
	}
	public void AddTime(float timeToAdd)
	{
		timeSinceLastDescendingPiece += timeToAdd;
		timeSinceLastRandomMovingBlock += timeToAdd;
		timeSinceStart += timeToAdd;
	}
	public void AddMovingBlock(MovingBlock m)
	{
		allMovingBlocksInGame.Add(m);
	}
}
public class GameSettings
{
	public float timeBetweenDescendingPieceSpawns = 5f;
	public float timeBetweenRandomMovingBlockSpawns = 1.5f;

}
public class Counter
{
	public float currentTime = 0f;
	public float expiryTime = 0f;
	public bool hasFinished = false;
	public bool repeats = false;
	public Counter(float exp)
	{
		expiryTime = exp;
	}
	public bool AddTime(float timeToAdd)
	{
		currentTime += timeToAdd;
		if (currentTime >= expiryTime)
		{
			if (repeats) { ResetTimer(); } else { hasFinished = true; }
			return true;
		}
		else
		{
			return false;
		}
	}
	public void ResetTimer()
	{
		currentTime = 0f;
		hasFinished = false;
	}
}
public class SpriteInteractionObject
{
	public SpriteRenderer renderer;
	public float timeSinceLastInterval = 0f;
	public float totalIntervalPeriod = 0f;
	public Color colorToAdd;
	public Color normalColor;
	public bool flickering = false;
	public bool colorIsOn = false;
	public SpriteInteractionObject (Transform t)
    {
		renderer = t.GetComponent<SpriteRenderer>();
    }
	public void StartFlicker(Color colorToFlicker, float interval)
    {
		colorToAdd = colorToFlicker;
		totalIntervalPeriod = interval;
		timeSinceLastInterval = 0f;
    }
	Color GetCurrentColor()
    {
        if (colorIsOn) { return normalColor; } else { return colorToAdd; }
    }
	public void ChangeTexture(Sprite theTexture)
    {
		renderer.sprite = theTexture;
		//renderer.sprite = theTexture.to;
		//Material temp = new Material(theTexture);
	}
	public void ChangeColor(Color newColor)
    {
		renderer.material.color = newColor;
    }
	public void ChangeVisibility(bool visibilityState)
    {
		renderer.enabled = visibilityState;
    }
	public void ChangeAlpha(float newAlpha)
    {
		renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, newAlpha);
    }
	bool NormalColorCheck()
    {
		if(renderer.material.color == normalColor)
        {
			return true;
        }
        else { return false; }
    }
	public void UpdateSprite(float timePassed)
    {
        if (flickering)
        {
			timeSinceLastInterval += timePassed;
			if (timeSinceLastInterval >= totalIntervalPeriod) { colorIsOn = !colorIsOn; }
			if (colorIsOn == NormalColorCheck()) { renderer.material.color = GetCurrentColor(); }
		}
    }
}

public class MainMenu
{
	/*GUI.Box(new Rect(Screen.width * 0.2f,Screen.height *0.4f,Screen.width *0.6f,Screen.height * 0.2f),"poop");
		if(GUI.Button(new Rect(Screen.width * 0.3f,Screen.height * 0.45f,Screen.width * 0.4f,Screen.height * 0.1f),"start game"))
		{
			Camera.main.GetComponent<MainScript>().StartTheGame();
			//StartTheGame();
			Destroy(gameObject);
		}*/
}
public class UtilityAbilityInstance
{
	public Counter deathTimer;
	public Transform abilityTransform;
	public bool readyToDie = false;
	public bool takesDirection = false;
	public bool requiresUpdate = false;
	public TouchInterface relevantTouchInterface;
	public UtilityAbilityInstance(Transform theTransform,bool ifReadyToDie,bool itTakesDirection, Counter theDeathTimer,bool requiredUpdate)
    {
		deathTimer = theDeathTimer;
		readyToDie = ifReadyToDie;
		takesDirection = itTakesDirection;
		abilityTransform = theTransform;
		requiresUpdate = requiredUpdate;
    }
}