using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	ControlsScript thisControlsScript;
	Rigidbody2D rbody;
	public List<MovementAffector> affecters;
	List<PlayerSettingsAffector> settingsAffectors;
	MovementAffector affectorToApplyOnCollisions;
	public List<Sprite> eyeBallSprites;
	int health = 3;
	public float maxSpeed = 15f;
	public Counter explosionCounter;
	public Counter regenCounter;
	public Counter playerWoundedCounter;
	public GameObject starPrefab;
	public Transform eyeSocketTransform;
	public Transform eyeBallTransform;
	public SpriteInteractionObject playerSprite;
	public SpriteInteractionObject eyeSockets;
	public SpriteInteractionObject eyeBalls;
	public Counter playerFlashingColorCounter;
	public Color colorPlayerIsFlashing;
	bool hasPushedAwayBeforeBecomingVulnerable = false;
	public bool playerIsFlashingColor = false;
	//public SpriteInteractionObject playerSprite;

	// Start is called before the first frame update
	void Start()
    {
		playerFlashingColorCounter = new Counter(1f);
		playerSprite = new SpriteInteractionObject(transform);
		playerWoundedCounter = new Counter(1f);
		playerWoundedCounter.hasFinished = true;
		eyeSockets = new SpriteInteractionObject(eyeSocketTransform);
		eyeBalls = new SpriteInteractionObject(eyeBallTransform);
		explosionCounter = new Counter(1f);
		regenCounter = new Counter(5f);
		maxSpeed *= MainScript.blockHeight;
		rbody = GetComponent<Rigidbody2D>();
		affecters = new List<MovementAffector>();
		thisControlsScript = GetComponent<ControlsScript>();
		//thisPhysicsObjectScript = GetComponent<PhysicsObjectScript>();
		//thisPhysicsObjectScript.affecters = new List<MovementAffector>();
		//print("we get here");
		float widthScale = MainScript.blockWidth / 0.16f;
		float heightScale = MainScript.blockHeight / 0.16f;
		//transform.localScale = new Vector3(widthScale, heightScale, transform.localScale.z);
		//thisControlsScript = GetComponent<ControlsScript>();
    }
	public void SetupFlashingPlayer(Color colorToFlash,float time)
    {
		playerIsFlashingColor = true;
		colorPlayerIsFlashing = colorToFlash;
		playerFlashingColorCounter = new Counter(time);
    }
	public MovementAffector GetAffectorWithId(string theIdentifier)
	{
		foreach (MovementAffector m in affecters)
		{
			if (m.identifierName == theIdentifier) { return m; }
		}
		return null;
	}
	public  Vector2 Rotate(Vector2 v, float degrees)
	{
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}
	public void BounceOff(Vector2 theNormal, Vector2 theMoveDirect)
	{
		float bounceAmount = 1f;
		foreach (MovementAffector m in affecters)
		{
			//Vector2 completeDirection = m.directionOfAffector * m.speed;
			if (m.theType == MovementAffectorType.momentumBased && Vector2.Dot(m.directionOfAffector, theNormal) < 0f)
			{
				m.directionOfAffector = Vector2.Reflect(m.directionOfAffector, theNormal);
				m.speed *= bounceAmount;
				//MainScript.staticString = "I think we hit something somehow";
			}
		}
		if (theNormal.y > 0f)
		{
			//it should probably counteract gravity
		}
	}
	public void CollidedWithMovingBlock(MovingBlock block)
	{
		PlayerSettings currentPlayerSettings = GetCurrentSettings(false);
		if(currentPlayerSettings.vulnerable)
		{
			SetUpDazedAnimation();
			Vector2 completeDirection = transform.position - block.theTransform.position;
			Vector2 direction = completeDirection.normalized;
			thisControlsScript.affectors.Add(new ControlsSettingsAffector(ControlsSettingsAffectorType.pauseControls, 1f, 1f, 1f,1f));
			settingsAffectors.Add(new PlayerSettingsAffector(PlayerSettingsAffectorType.makeInvulnerable,new Counter(1f)));
			affecters.Add(new MovementAffector(direction,4f,0f,1f,MovementAffectorType.momentumBased));
			block.AddMovementAffector(new MovementAffector(direction * -1f,1.5f,0f,1f,MovementAffectorType.momentumBased));
			block.SuspendGravity(1f);
			ChangeHealth(-1);
			float randomAngleRadians = Random.Range(0f, Mathf.PI * 2f);
			SetupFlashingPlayer(Color.red,1f);
			for (int i = 0; i < 5; i++)
			{
				float currentAngleRadians = randomAngleRadians + (((float)i / 5) * Mathf.PI * 2f);
				Vector2 currentAngle = new Vector2(Mathf.Sin(currentAngleRadians), Mathf.Cos(currentAngleRadians));
				//Vector2 currentAngle = (Vector3)originAngle * Quaternion.Euler(0f, 0f, (float)(i / 3) * (Mathf.PI * 2f));
				GameObject temp = Instantiate(starPrefab, transform.position, Quaternion.LookRotation(Vector3.forward, (Vector3)currentAngle));
				temp.transform.localScale = new Vector3(MainScript.blockLocalScale, MainScript.blockLocalScale, 1f);
				temp.SendMessage("SetDirection", (Vector3)currentAngle);
			}
		}
		else
		{

            //MovingBlock collidedMovingBlock = block.theTransform.GetComponent<MovingBlockScript>().thisBlock;
            if (block.isActive)
            {
				MovingBlock collidedMovingBlock = block;
				List<MovingBlock> allMovingBlocksInCollision = new List<MovingBlock>() { collidedMovingBlock };
				if (collidedMovingBlock.isPartOfDescendingPiece && currentPlayerSettings.breakUpDescendingPieceOnCollision)
				{
					foreach (MovingBlock m in collidedMovingBlock.theDescendingPiece.allMovingBlocks)
					{
						if (m.theTransform.position != collidedMovingBlock.theTransform.position)
						{
							if (m.theTransform.position != collidedMovingBlock.theTransform.position)
							{
								allMovingBlocksInCollision.Add(m);
							}

						}
					}
					collidedMovingBlock.theDescendingPiece.BreakApartDescendingPiece();
				}

				if (currentPlayerSettings.applyMovementAffectorOnCollision)
				{
					if (explosionCounter.hasFinished)
					{

						MainScript.CreateExplosionAt(transform.position, Color.green, MainScript.blockHeight * 3f, 0.55f);
						explosionCounter.ResetTimer();
					}

					if (affectorToApplyOnCollisions.theType == MovementAffectorType.getOutOfPlayersWay)
					{
						Vector2 currentVelocity = PhysicsObjectScript.AddUpAllMovements(affecters, Time.fixedDeltaTime, false);
						if (currentVelocity != Vector2.zero)
						{
							//you cannot move Out of the way if the player is not moving
							Vector2 velocityDirection = currentVelocity.normalized;
							Vector2 rotatedVelocity = Rotate(velocityDirection, 90f);
							foreach (MovingBlock m in allMovingBlocksInCollision)
							{
								Vector2 completeDirection = transform.position - m.theTransform.position;
								Vector2 direction = completeDirection.normalized;
								Vector2 finalDirection = rotatedVelocity;
								if (Vector2.Dot(direction, rotatedVelocity) < 0f) { finalDirection *= -1f; }
								//m.AddMovementAffector(new MovementAffector(finalDirection * -1f, affectorToApplyOnCollisions.speed, affectorToApplyOnCollisions.decayAmount, affectorToApplyOnCollisions.endTime, MovementAffectorType.bounceOffPlayer));
								if (!m.DoesThisBlockHaveAnAffectorWithThisIdentifier("PlayerMade"))
								{
									if (currentPlayerSettings.suspendGravityOnCollision)
									{
										m.SuspendGravity(affectorToApplyOnCollisions.endTime);
									}
									m.AddMovementAffector(new MovementAffector(finalDirection * -1, affectorToApplyOnCollisions.speed, 0f, affectorToApplyOnCollisions.endTime, MovementAffectorType.momentumBased, "PlayerMade"));
									m.SuspendGravity(0.75f);
								}


								//Vector2 rawVelocity = PhysicsObjectScript.AddUpAllMovements(affecters, Time.fixedDeltaTime, false);
								//if (Vector2.Dot(direction, velocityDirection) > 0.85f) { PhysicsObjectScript.BounceOff(direction * -1f,affecters); }
							}

						}
					}
					else if (affectorToApplyOnCollisions.theType == MovementAffectorType.bounceOffPlayer)
					{
						if (explosionCounter.hasFinished)
						{
							MainScript.CreateExplosionAt(transform.position, Color.green, MainScript.blockHeight * 3f, 0.55f);
							explosionCounter.ResetTimer();
						}
						foreach (MovingBlock m in allMovingBlocksInCollision)
						{
							if (!m.DoesThisBlockHaveAnAffectorWithThisIdentifier("PlayerMade"))
							{
								Vector2 completeDirection = transform.position - block.theTransform.position;
								Vector2 direction = completeDirection.normalized;
								m.AddMovementAffector(new MovementAffector(direction * -1f, affectorToApplyOnCollisions.speed, affectorToApplyOnCollisions.decayAmount, affectorToApplyOnCollisions.endTime, MovementAffectorType.bounceOffPlayer, "PlayerMade"));
							}

						}


					}
					else
					{
						collidedMovingBlock.AddMovementAffector(new MovementAffector(affectorToApplyOnCollisions.directionOfAffector, affectorToApplyOnCollisions.speed, affectorToApplyOnCollisions.decayAmount, affectorToApplyOnCollisions.endTime, affectorToApplyOnCollisions.theType));
					}
				}
			
			}
            
		}
	}
	
	void OnCollisionEnter2D(Collision2D col)
	{
		
		if(col.transform.tag == "MovingBlock")
		{
			//PlayerSettings currentPlayerSettings = GetCurrentSettings(false);
			/*MovingBlockScript tempScript = col.transform.GetComponent<MovingBlockScript>();
            if (tempScript)
            {
				CollidedWithMovingBlock(col.transform.GetComponent<MovingBlockScript>().thisBlock);
			}*/
			
			//thisBlock.BounceOffMovingBlock(col.transform.GetComponent<MovingBlockScript>().thisBlock);
		}else if (col.transform.tag == "Wall")
        {
			//col.no
			//Vector2 positionToMoveTo = col.contacts[0].point + (col.contacts[0].normal * MainScript.blockHeight * 0.5f);
				//Vector2 positionToMoveTo = .point + (temp.normal * MainScript.blockHeight * 0.5f);
			//rbody.MovePosition(positionToMoveTo);
				//BounceOff(temp.normal, moveDirect);
				//moveDirect = new Vector2(0f, moveDirect.y);
		}
	}
	public void SetCollisionMovementAffector(MovementAffector affector)
	{
		affectorToApplyOnCollisions = affector;
	}
	public void AddSettingAffector(PlayerSettingsAffector theAffector)
	{
		settingsAffectors.Add(theAffector);
	}
	public void ChangeHealth(int changeAmount)
	{
		health += changeAmount;
		if(changeAmount < 0) { regenCounter.ResetTimer(); }
		if(health <= 0)
		{
			MainScript.readyToEnd = true;
		}
	}
	public void KillPlayer()
	{
		Destroy(gameObject);
		//print(" player is ded");
	}
	public PlayerSettings GetDefaultSettings()
	{
		PlayerSettings temp = new PlayerSettings();
		temp.vulnerable = true;
		return temp;
	}
	public void AddControlAffector(ControlsSettingsAffector affector)
	{
		thisControlsScript.affectors.Add(affector);
	}
	void PushBlocksAwayBeforeBecomingVulnerable()
	{
		hasPushedAwayBeforeBecomingVulnerable = true;
		List<MovingBlock> tempList = new List<MovingBlock>();
		List<DescendingPiece> allPieces = new List<DescendingPiece>();
		foreach (MovingBlock m in MainScript.allMovingBlocks) 
		{
			float distance = Vector2.Distance((Vector2)transform.position , (Vector2)m.theTransform.position);
			float minDistanceToPushAway = 2f * MainScript.blockWidth;
			if (distance < minDistanceToPushAway)
			{
                if (!m.isPartOfDescendingPiece)
                {
					Vector2 directionToPushAway = (Vector2)(m.theTransform.position - transform.position);
					float speedToPushAway = 6.5f;
					m.AddMovementAffector(new MovementAffector(directionToPushAway.normalized, speedToPushAway, 0f, 0.1f, MovementAffectorType.arbitrary));
					m.settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity, new Counter(0.1f)));
                }
                else
                {
                    if (!allPieces.Contains(m.theDescendingPiece)) { allPieces.Add(m.theDescendingPiece); }
                }
				
			}
		}
		foreach (DescendingPiece d in allPieces)
		{
			Vector2 averagePosition = Vector2.zero;
			foreach(MovingBlock mov in d.allMovingBlocks)
            {
				averagePosition += (Vector2)mov.theTransform.position;
            }
			averagePosition /= (float)d.allMovingBlocks.Count;
			Vector2 directionToPushAway = (Vector2)(averagePosition - (Vector2)transform.position);
			float speedToPushAway = 5f;
			MovingBlock m = d.fixedJointHolder;
			m.AddMovementAffector(new MovementAffector(directionToPushAway.normalized, speedToPushAway, 0f, 0.1f, MovementAffectorType.arbitrary));
			m.settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity, new Counter(0.1f)));
		}
	}
	public PlayerSettings GetCurrentSettings(bool updateTime)
	{
		PlayerSettings tempSettings = GetDefaultSettings();
		List<PlayerSettingsAffector> affectorsToRemove = new List<PlayerSettingsAffector>();
		
		foreach(PlayerSettingsAffector pAffect in settingsAffectors)
		{
			if(!pAffect.affectorCounter.hasFinished && updateTime){
				pAffect.affectorCounter.AddTime(Time.fixedDeltaTime); 
				if (pAffect.theType == PlayerSettingsAffectorType.makeInvulnerable) 
				{
					Counter temp = new Counter(1f);
					float timeLeft = pAffect.affectorCounter.expiryTime - pAffect.affectorCounter.currentTime;
					if(timeLeft < 0.25f && !hasPushedAwayBeforeBecomingVulnerable)
                    {
						//the player is about to become vulnerable so push blocks away
						PushBlocksAwayBeforeBecomingVulnerable();
                    }
                    
				} 
			}
			switch(pAffect.theType)
			{
				case PlayerSettingsAffectorType.makeInvulnerable:
					tempSettings.vulnerable = false;
					break;
				case PlayerSettingsAffectorType.applyMovementAffectorOnCollision:
					tempSettings.applyMovementAffectorOnCollision = true;
					break;
				case PlayerSettingsAffectorType.breakUpDescendingPieceOnCollision:
					tempSettings.breakUpDescendingPieceOnCollision = true;
					break;
				case PlayerSettingsAffectorType.suspendGraityOnCollision:
					tempSettings.suspendGravityOnCollision = true;
					break;
			}
			if(pAffect.affectorCounter.hasFinished){affectorsToRemove.Add(pAffect);}
		}
		while(affectorsToRemove.Count > 0)
		{
			if (affectorsToRemove[0].theType == PlayerSettingsAffectorType.makeInvulnerable)
			{
				hasPushedAwayBeforeBecomingVulnerable = false;
			}
			settingsAffectors.Remove(affectorsToRemove[0]);
			affectorsToRemove.Remove(affectorsToRemove[0]);
		}
		return tempSettings;
	}
	public void SetUpPlayer()
    {
		settingsAffectors = new List<PlayerSettingsAffector>();
		rbody = GetComponent<Rigidbody2D>();
		affecters = new List<MovementAffector>();
		thisControlsScript = GetComponent<ControlsScript>();
		//thisPhysicsObjectScript = GetComponent<PhysicsObjectScript>();
		//thisPhysicsObjectScript.affecters = new List<MovementAffector>();
		//print("we get here");
		float widthScale = MainScript.blockWidth / 0.16f;
		float heightScale = MainScript.blockHeight / 0.16f;
		transform.localScale = new Vector3(widthScale, heightScale, transform.localScale.z);
	}
	public Vector2 CheckForCollisions(Vector2 moveDirect)
	{
		if (moveDirect.magnitude != 0f)
		{
			//RaycastHit2D tempHit = Physics2D.CircleCast(transform.position, MainScript.blockWidth / 2f, moveDirect, MainScript.blockWidth * 0.55f, LayerMask.GetMask("Wall", "MovingBlock"));
			if (moveDirect.x != 0f)
			{
				RaycastHit2D horizontalCheck = Physics2D.CircleCast(transform.position, MainScript.blockHeight / 2f, new Vector2(Mathf.Sign(moveDirect.x), 0f), MainScript.blockWidth * 0.55f, LayerMask.GetMask("Wall"));
				if (horizontalCheck)
				{
					//print("got a hit");
					if (horizontalCheck.transform.tag == "Wall")
					{

					}
					else if (horizontalCheck.transform.tag == "MovingBlock")
					{
					}
					Vector2 pointToMoveTo = horizontalCheck.point + (horizontalCheck.normal * (MainScript.blockWidth * 0.5f));
					GetComponent<Rigidbody2D>().MovePosition(pointToMoveTo);
					//thisPhysicsObjectScript.BounceOff(horizontalCheck.normal, moveDirect);
					moveDirect = new Vector2(0f, moveDirect.y);
					thisControlsScript.BounceOff(horizontalCheck.normal, moveDirect);
				}
			}
			if (moveDirect.y != 0f)
			{
				RaycastHit2D verticalCheck = Physics2D.CircleCast(transform.position, MainScript.blockHeight / 2f, new Vector2(0f, Mathf.Sign(moveDirect.y)), MainScript.blockHeight * 0.005f, LayerMask.GetMask("Wall"));
				if (verticalCheck)
				{
					//print("got a hit");
					float yPosition = verticalCheck.point.y;
					if (verticalCheck.transform.tag == "Wall")
					{
						yPosition = verticalCheck.transform.position.y + (MainScript.blockHeight * (Mathf.Sign(verticalCheck.normal.y)) * 1.15f);
					}
					else if (verticalCheck.transform.tag == "MovingBlock")
					{
						//yPosition = verticalCheck.transform.position.y + (MainScript.blockHeight * Mathf.Sign(verticalCheck.normal.y) * 1.15f);
					}
					//Vector2 pointToMoveTo = verticalCheck.point + (verticalCheck.normal * (MainScript.blockHeight * 0.5f));
					Vector2 pointToMoveTo = new Vector2(transform.position.x, yPosition);
					GetComponent<Rigidbody2D>().MovePosition(pointToMoveTo);
					//thisPhysicsObjectScript.BounceOff(verticalCheck.normal, moveDirect);
					moveDirect = new Vector2(moveDirect.x, 0f);
					thisControlsScript.BounceOff(verticalCheck.normal, moveDirect);
				}
			}
		}
		return moveDirect;
	}
	public void StopControls(float timeBeforeEnd)
	{
		thisControlsScript.affectors.Add(new ControlsSettingsAffector(ControlsSettingsAffectorType.pauseControls, 0f, 0f, 0f, timeBeforeEnd));
	}
	public void ChangeMaxSpeed(float newMaxSpeed,float timeBeforeEnd)
	{
		thisControlsScript.affectors.Add(new ControlsSettingsAffector(ControlsSettingsAffectorType.changeMaxSpeed, newMaxSpeed, 0f, 0f, timeBeforeEnd));
	}
	public void SetUpDazedAnimation()
    {
		//playerSprite.ChangeColor(Color.grey);
		eyeBalls.ChangeTexture(eyeBallSprites[1]);
		eyeSockets.ChangeVisibility(false);
		playerWoundedCounter.ResetTimer();
    }
	public void SetUpNormalAnimation()
    {
		//playerSprite.ChangeColor(Color.white);
		eyeBalls.ChangeTexture(eyeBallSprites[0]);
		eyeSockets.ChangeVisibility(true);
    }
	public void UpdatePosition(float timePassed)
	{
        if (playerIsFlashingColor)
        {
			playerFlashingColorCounter.AddTime(Time.fixedDeltaTime);
            if (playerFlashingColorCounter.hasFinished) { playerIsFlashingColor = false;playerSprite.ChangeColor(Color.white); } else
            {
				float tempFloat = Mathf.Cos(playerFlashingColorCounter.currentTime * Mathf.PI * 16f);
				Color appropriateColor = Color.white;
				if(tempFloat > 0f) { appropriateColor = colorPlayerIsFlashing; }
				if(playerSprite.renderer.material.color != appropriateColor) { playerSprite.ChangeColor(appropriateColor); }
			}
			
        }
        if (!explosionCounter.hasFinished) { explosionCounter.AddTime(Time.fixedDeltaTime); }
		PlayerSettings currentSettings = GetCurrentSettings(true);
		rbody.velocity = Vector2.zero;
		Vector2 moveDirect = Vector2.zero;
		moveDirect += PhysicsObjectScript.AddUpAllMovements(affecters,timePassed,true);
		moveDirect += thisControlsScript.Controls(moveDirect,timePassed);
		moveDirect = PhysicsObjectScript.CheckForWalls(moveDirect,transform,true,true,affecters);
		for(int i =0; i < MainScript.heartDisplayIcons.Count;i++)
		{
			HeartIcon currentIcon = MainScript.heartDisplayIcons[i];
			bool isFullCheck = true;
			if(i + 1 > health) { isFullCheck = false; }
			if(currentIcon.isFull != isFullCheck) {currentIcon.ChangeState(isFullCheck);}
		}
        //CheckForCollisions(moveDirect);
        //if (moveDirect.magnitude > (maxSpeed)) { moveDirect = moveDirect.normalized * maxSpeed; }
        if (playerWoundedCounter.hasFinished)
        {
			MainScript.playerDirection = moveDirect;
			Vector2 eyeBallDirection = moveDirect.magnitude * MainScript.blockHeight * moveDirect.normalized * 1.35f;

			eyeSockets.renderer.transform.localPosition = new Vector3(eyeBallDirection.x, eyeBallDirection.y, eyeSockets.renderer.transform.localPosition.z);
			eyeBalls.renderer.transform.localPosition = new Vector3(eyeBallDirection.x * 1.25f, eyeBallDirection.y * 1.25f, eyeBalls.renderer.transform.localPosition.z);
        }
        else
        {
			playerWoundedCounter.AddTime(Time.fixedDeltaTime);
			if (playerWoundedCounter.hasFinished) 
			{
				SetUpNormalAnimation();
            }
            else
            {
				float currentAngleInRadians = playerWoundedCounter.currentTime * Mathf.PI * 3.25f;
				Vector2 currentAngle = new Vector2(Mathf.Sin(currentAngleInRadians), Mathf.Cos(currentAngleInRadians));
				eyeBalls.renderer.transform.localPosition =  new Vector3(currentAngle.normalized.x * MainScript.blockHeight * 0.15f , currentAngle.normalized.y * MainScript.blockHeight * 0.15f, eyeBalls.renderer.transform.localPosition.z);
            }
		}
		
		if (moveDirect.magnitude > 0f) { rbody.MovePosition((Vector2)transform.position + moveDirect); }
		if(health < 3)
		{
			
			if(regenCounter.hasFinished)
			{
				ChangeHealth(1);
				regenCounter.ResetTimer();
            }
            else
            {
				regenCounter.AddTime(Time.fixedDeltaTime);
			}
		}
		List<MovementAffector> tempList = new List<MovementAffector>();
		foreach(MovementAffector m in affecters) { if (m.readyToEnd) { tempList.Add(m); } }
		while(tempList.Count > 0)
        {
			MovementAffector temp = tempList[0];
			affecters.Remove(temp);
			tempList.Remove(temp);
        }
	}
	// Update is called once per frame
	void Update()
    {

    }
}
public class PlayerSettings
{
	public bool vulnerable;
	public bool applyMovementAffectorOnCollision = false;
	public bool suspendGravityOnCollision = false;
	public MovementAffector affectorToApplyOnCollision;
	public bool breakUpDescendingPieceOnCollision = false;
}
public class PlayerSettingsAffector
{
	public PlayerSettingsAffectorType theType;
	public Counter affectorCounter;
	public PlayerSettingsAffector(PlayerSettingsAffectorType derType,Counter theCounter)
	{
		theType = derType;
		affectorCounter = theCounter;
	}
}
public enum PlayerSettingsAffectorType{ makeInvulnerable, doubleSpeed,applyMovementAffectorOnCollision,breakUpDescendingPieceOnCollision,pauseControls,suspendGraityOnCollision}