using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlockScript : MonoBehaviour
{
	// Start is called before the first frame update
	public MovingBlock thisBlock;
    void Start()
    {
		//COLOR CHANGE CODE
		//GetComponent<Renderer>(). material. color = new Color(0, 255, 0);   
    }
	public static List<DirectionAndDistance> GetAllDirectionDistanceFromPoint(Vector3 thePoint,int numberOfDirections)
    {
		List<DirectionAndDistance> tempList = new List<DirectionAndDistance>();
		for(int i = 0; i < numberOfDirections; i++)
        {
			float percentage = (float)i / (float)numberOfDirections;
			Vector2 currentDirection = new Vector2(Mathf.Sin(percentage * Mathf.PI * 2f), Mathf.Cos(percentage * Mathf.PI * 2f));

			RaycastHit2D tempHit = Physics2D.Raycast(thePoint + ((Vector3)currentDirection * MainScript.blockWidth * 0.61f), currentDirection, MainScript.blockWidth * 0.5f, LayerMask.GetMask("Wall", "MovingBlock"));
            if (tempHit) { tempList.Add(new DirectionAndDistance(currentDirection, tempHit.distance)); } else
            {
				tempList.Add(new DirectionAndDistance(currentDirection, MainScript.blockWidth * 0.55f));
            }
        }
		return tempList;
    }
	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.transform.tag == "MovingBlock")
		{
			if(thisBlock.GetActualSettings(0f).vulnerable)
			{
				//we take the hit
			}else
			{
				//thisBlock.BounceOffMovingBlock(col.transform.GetComponent<MovingBlockScript>().thisBlock);
			}
		}else if(col.transform.tag == "Player")
		{
			//col.transform.GetComponent<PlayerScript>().CollidedWithMovingBlock(thisBlock);
		}else if (col.transform.tag == "Projectile")
		{
			col.transform.SendMessage("CollideWithMovingBlock",(thisBlock));
		}else if (col.transform.tag == "Wall")
		{
			Vector3 directToWall = transform.position - col.transform.position;
			Vector3 directToWallHorizontal = new Vector3(directToWall.x,directToWall.y,0f);
			thisBlock.settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity,new Counter(Time.fixedDeltaTime * 1f)));
			thisBlock.AddMovementAffector(new MovementAffector(directToWallHorizontal.normalized, 10f, 0f, Time.fixedDeltaTime * 1f, MovementAffectorType.momentumBased));
		}
		
	}
    private void OnTriggerEnter2D(Collider2D col)
    {
	}
    public void DestroyThisMovingBlock()
	{
		Destroy(gameObject);
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class MovingBlock 
{
	public MovingBlockSettings defaultSettings;
	public List<MovingBlockSettingsAffector> settingsAffectors;
	public List<BounceOffMovingBackCounter> movingBlocksToBounceOff;
	public bool isInTopCollider = false;
	public bool isActive = false;
	public bool isPartOfDescendingPiece = false;
	public bool hasSettledIntoPlace = false;
	public bool isFixedJointHolder = false;
	public bool isSettlingIntoPlace = false;
	public int numberOfBlocksInside = 0;
	public Transform theDescendingPieceTransform;
	public DescendingPiece theDescendingPiece;
	public EmptyBlock currentEmptyBlock;
	public CircleCollider2D collider;
	public Color blockColor = Color.white;
	public int descendingPieceX = 0;
	public int descendingPieceY = 0;
	public Transform theTransform;
	public MovementAffector moveAwayFromThisBlock;
	public MovementAffector latestBounceOffMovingBlock;
	public List<MovementAffector> affecters;
	public Rigidbody2D rbody;
	public SpriteInteractionObject spriteInteractionObject;
	public bool affectedByGravity;
	public bool isBouncingBack = false;
	public DirectionAndDistance lastBestDirectionDistance;
	public Counter bounceBackCounter;
	public MovingBlock(int xValue, int yValue, bool partOfDescending, DescendingPiece thisDescendingPiece, Transform thisTransform)
	{
		movingBlocksToBounceOff = new List<BounceOffMovingBackCounter>();
		bounceBackCounter = new Counter(1.5f);
		latestBounceOffMovingBlock = new MovementAffector(Vector2.zero, 3.5f, 0f, false, false, 0f, MovementAffectorType.bouncingOffBlock);
		affecters = new List<MovementAffector>();
		defaultSettings = new MovingBlockSettings(true);
		settingsAffectors = new List<MovingBlockSettingsAffector>();
		descendingPieceX = xValue;
		descendingPieceY = yValue;
		isPartOfDescendingPiece = true;
		//theDescendingPieceTransform = thePieceTransform;
		theDescendingPiece = thisDescendingPiece;
		theDescendingPieceTransform = thisDescendingPiece.thisPieceTransform;
		theTransform = thisTransform;
		spriteInteractionObject = new SpriteInteractionObject(theTransform);
        //if (!isPartOfDescendingPiece) { spriteInteractionObject.ChangeColor(GetRandomColor()); }
			


		collider = theTransform.GetComponent<CircleCollider2D>();
		rbody = theTransform.GetComponent<Rigidbody2D>();
		moveAwayFromThisBlock = new MovementAffector(Vector2.zero, 3f, 0f, false, true, 0.5f, MovementAffectorType.moveAwayFromObject, theTransform);
		moveAwayFromThisBlock.owner = this;
		//moveAwayFromThisBlock.objectPosition = thisTransform;
	}
	public void UpdatePosition(float timePassed)
	{
		if (isActive)
		{

		}
		if (!isPartOfDescendingPiece || isFixedJointHolder)
		{
			rbody.velocity = Vector2.zero;
			Vector2 moveDirect = Vector2.zero;
			MovingBlockSettings currentSettings = GetActualSettings(timePassed);
			moveDirect = PhysicsObjectScript.AddUpAllMovements(affecters,timePassed,true);
			//print("move direct is " + moveDirect);
			List<MovementAffector> tempList = new List<MovementAffector>();
			foreach (MovementAffector m in affecters) { if (m.readyToEnd) { tempList.Add(m); } }
			while (tempList.Count > 0) { MovementAffector tempAffect = tempList[0];  affecters.Remove(tempAffect); tempList.Remove(tempAffect); }
			if (currentSettings.isAffectedByGravity) { moveDirect = PhysicsObjectScript.ApplyAffector(MainScript.gravity, moveDirect,timePassed,false); }
			if (movingBlocksToBounceOff.Count > 0)
			{
				moveDirect = ApplyBounceBack(moveDirect,timePassed);
			}
			bool shouldApplyTopColliderPushDown = false;
            if (isPartOfDescendingPiece)
            {
                if (theDescendingPiece.AreWeInTopCollider()) { shouldApplyTopColliderPushDown = true; }
            }
            else
            {
                if (isInTopCollider) { shouldApplyTopColliderPushDown = true; }
            }
            if (shouldApplyTopColliderPushDown)
            {
				moveDirect = PhysicsObjectScript.ApplyAffector(TopColliderScript.pushDownAffector,moveDirect,timePassed,false);
            }
			/*if (isBouncingBack)
			{
				bounceBackCounter.AddTime(timePassed);
				if (bounceBackCounter.hasFinished) { isBouncingBack = false; } else { moveDirect = PhysicsObjectScript.ApplyAffector(latestBounceOffMovingBlock, moveDirect,timePassed); }
			}*/
			
			
			moveDirect = CheckForCollision(moveDirect, currentSettings);
			moveDirect = AdjustToFitColumns(moveDirect, timePassed);
			rbody.MovePosition((Vector2)theTransform.position + moveDirect);
			//if (!isPartOfDescendingPiece) { PushAllBlocksInsideOutward(); }
			PushAllBlocksInsideOutward();
			if(movingBlocksToBounceOff.Count > 2)
            {
				//MainScript.blocksToGetOutOfTheWay.Add(this);
				lastBestDirectionDistance = MoveOutOfTheWay();
				Vector2 move = MoveOutOfTheWay().direction * Time.fixedDeltaTime;
				rbody.MovePosition((Vector2)theTransform.position + move);
			}
		}
		//if (!MainScript.blocksToRemove.Contains(this))
		bool tempBool = true;
		if(hasSettledIntoPlace)
        {
            if (MainScript.blocksToRemove.Contains(currentEmptyBlock))
            {
				tempBool = false;
            }
        }
		if(tempBool)
        {
			float distanceToPlayer = Vector2.Distance((Vector2)MainScript.thePlayer.position, (Vector2)theTransform.position);
			if (distanceToPlayer < MainScript.blockHeight * 0.8f)
			{
				MainScript.thePlayer.GetComponent<PlayerScript>().CollidedWithMovingBlock(this);
			}
		}
        //if (isPartOfDescendingPiece) { foreach (MovingBlock m in theDescendingPiece.allMovingBlocks) { m.rbody.velocity = Vector2.zero; } }
	}
	public void StopMovingBlock()
    {
		rbody.velocity = Vector2.zero;
    }
	public Color GetRandomColor()
    {
		//int randomInt = (int)Random.Range(0f, 6f);
		int randomInt = (int)Random.Range(0f, (float)MainScript.allPossibleBlockColors.Count);
		Color tempColor = Color.white;
		/*switch (randomInt)
		{
			case 0:
				tempColor = Color.red;
				break;
			case 1:
				tempColor = Color.cyan;
				break;
			case 2:
				tempColor = Color.yellow;
				break;
			case 3:
				tempColor = Color.green;
				break;
			case 4:
				tempColor = Color.blue;
				break;
			case 5:
				tempColor = Color.magenta;
				break;
			case 6:
				tempColor = Color.gray;
				break;
		}*/

		return MainScript.allPossibleBlockColors[randomInt];
    }
	Vector2 ApplyBounceBack(Vector2 moveDirect, float timePassed)
	{
		//MainScript.staticString = "applying BOUNCE BACK";
		Vector2 bounceOffDirection = Vector2.zero;
		List<BounceOffMovingBackCounter> countersToDelete = new List<BounceOffMovingBackCounter>();
		Vector2 positionOrigin =  theTransform.position;
		if(isPartOfDescendingPiece)
		{
			Vector2 tempVector = Vector2.zero;
			for(int i = 0;i < theDescendingPiece.allMovingBlocks.Count;i++)
			{
				tempVector += (Vector2)theDescendingPiece.allMovingBlocks[i].theTransform.position;
			}
			positionOrigin = tempVector/(float)theDescendingPiece.allMovingBlocks.Count;
		}
		foreach(BounceOffMovingBackCounter bCount in movingBlocksToBounceOff)
		{
			if(!bCount.bounceOffCounter.hasFinished){bCount.bounceOffCounter.AddTime(timePassed);}
			Vector2 directionAwayFromBlock = (Vector2)(positionOrigin - (Vector2)bCount.positionToBounceFrom);
			bounceOffDirection += directionAwayFromBlock.normalized;
			if(bCount.bounceOffCounter.hasFinished){bCount.readyToDie = true; countersToDelete.Add(bCount);}
		}
		while(countersToDelete.Count > 0f)
		{
			movingBlocksToBounceOff.Remove(countersToDelete[0]);
			countersToDelete.Remove(countersToDelete[0]);
		}
		latestBounceOffMovingBlock.directionOfAffector = bounceOffDirection.normalized;
		moveDirect = PhysicsObjectScript.ApplyAffector(latestBounceOffMovingBlock, moveDirect,timePassed,false);
		return moveDirect;
	}
	public void SuspendGravity(float time)
	{
		settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity,new Counter(time)));
	}
	public RaycastHit2D CheckForContact(Vector2 directionToCheck)
	{
		return Physics2D.Raycast(theTransform.position, directionToCheck, MainScript.blockHeight * 0.56f, LayerMask.GetMask("Wall"));
	}
	public void BounceOff(Vector2 theNormal)
	{
		float bounceAmount = 1f;
		foreach (MovementAffector m in affecters)
		{
			//Vector2 completeDirection = m.directionOfAffector * m.speed;
			
			if (IsThisMovementAffectorTypeMomentumBased(m.theType) && Vector2.Dot(m.directionOfAffector, theNormal) < 0f)
			{
				m.directionOfAffector = Vector2.Reflect(m.directionOfAffector, theNormal);
				m.speed *= bounceAmount;
				//MainScript.staticString = "I think we hit something somehow";
			}
		}
		bool playerMustBounce = IsThisBlockNearThePlayer() ;
		List<MovingBlock> allNearbyBlocks = MainScript.GetAllBlocksFromPointWithinDistance(theTransform.position, MainScript.blockWidth * 0.53f);
		foreach(MovingBlock m in allNearbyBlocks) { playerMustBounce = m.IsThisBlockNearThePlayer(); }
        if (playerMustBounce) { MainScript.thePlayer.GetComponent<PlayerScript>().BounceOff(theNormal, Vector2.zero); }
	}
	public bool IsThisBlockNearThePlayer()
    {
		bool IsNearPlayer = false;
		float distanceToPlayer = Vector2.Distance((Vector2)MainScript.thePlayer.position, (Vector2)theTransform.position);
		if (distanceToPlayer < MainScript.blockWidth * 0.93f) { IsNearPlayer = true; }
		return IsNearPlayer;
	}
	public bool IsThisMovementAffectorTypeMomentumBased(MovementAffectorType theType)
	{
		bool tempBool = false;
		if(theType == MovementAffectorType.momentumBased || theType == MovementAffectorType.moveTowardsPoint || theType ==  MovementAffectorType.moveAwayFromObject|| theType == MovementAffectorType.bounceOffPlayer || theType == MovementAffectorType.getOutOfPlayersWay)
		{
			tempBool = true;
		}
		return tempBool;
	}
	public Vector2 CheckForCollision(Vector2 moveDirect, MovingBlockSettings currentSettings)
	{
		if (moveDirect.magnitude != 0f)
		{
			//basically check if you can move horizontally in the moveDirect direction
			if (moveDirect.x != 0f)
			{
				Vector2 horizontalDirection = new Vector2(Mathf.Sign(moveDirect.x), 0f);
				List<MovingBlock> blocksMakingHorizontalCheck = new List<MovingBlock>();
				if (isPartOfDescendingPiece)
				{
					blocksMakingHorizontalCheck.AddRange(theDescendingPiece.GetBlocksFacingDirection(horizontalDirection));

				}
				else { blocksMakingHorizontalCheck.Add(this); }
				List<RaycastHit2D> allHorizontalRaycastHits = new List<RaycastHit2D>();
				foreach (MovingBlock m in blocksMakingHorizontalCheck)
				{
					RaycastHit2D temp = m.CheckForContact(horizontalDirection);
					if (temp) { allHorizontalRaycastHits.Add(temp); }
				}
				if (allHorizontalRaycastHits.Count > 0)
				{
					//horizontal raycast hit something
					RaycastHit2D temp = allHorizontalRaycastHits[0];
					for (int i = 1; i < allHorizontalRaycastHits.Count; i++)
					{
						if (allHorizontalRaycastHits[i].distance < temp.distance) { temp = allHorizontalRaycastHits[i]; }
					}
					if (temp.transform.tag == "Wall")
					{
						//Vector2 positionToMoveTo = temp.point + (temp.normal * MainScript.blockHeight * 0.5f);
						//rbody.MovePosition(positionToMoveTo);
						
						BounceOff(temp.normal);
						moveDirect = new Vector2(0f, moveDirect.y);
					}
				}
			}
			if (moveDirect.y != 0f)
			{
				Vector2 direction = new Vector2(0f, Mathf.Sign(moveDirect.y));
				List<MovingBlock> blocksMakingVerticalCheck = new List<MovingBlock>();
				if (isPartOfDescendingPiece)
				{
					blocksMakingVerticalCheck.AddRange(theDescendingPiece.GetBlocksFacingDirection(direction));
				}
				else { blocksMakingVerticalCheck.Add(this); }
				List<RaycastHit2D> allVerticalRaycastHits = new List<RaycastHit2D>();
				foreach (MovingBlock m in blocksMakingVerticalCheck)
				{
					RaycastHit2D temp = m.CheckForContact(direction);
					if (temp) { allVerticalRaycastHits.Add(temp); }
				}
				if (allVerticalRaycastHits.Count > 0)
				{
					RaycastHit2D temp = allVerticalRaycastHits[0];
					for (int i = 1; i < allVerticalRaycastHits.Count; i++)
					{
						if (allVerticalRaycastHits[i].distance < temp.distance) { temp = allVerticalRaycastHits[i]; }
					}
					Vector2 positionToMoveTo = temp.point + (temp.normal * MainScript.blockHeight * 0.5f);
					//rbody.MovePosition(positionToMoveTo);
					BounceOff(temp.normal);
					moveDirect = new Vector2(moveDirect.x, 0f);
					if (direction == Vector2.down && currentSettings.isAffectedByGravity && !isSettlingIntoPlace && !hasSettledIntoPlace)
					{
						if (isPartOfDescendingPiece)
						{
							foreach (MovingBlock m in theDescendingPiece.allMovingBlocks)
							{
								//MovingBlockScript tempScript = m.theTransform.GetComponent<MovingBlockScript>();
								m.FindClosestOpenBlock();
								m.isSettlingIntoPlace = true;
							}
							theDescendingPiece.BreakApartDescendingPiece();

						}
						else
						{
							FindClosestOpenBlock();
						}

					}
				}
			}
			//if the wall check has not reduced movedirect to zero do a full move collision check
			//if (moveDirect.magnitude > 0f)
			if(moveDirect.x != 0f && moveDirect.y != 0f)
			{
				//theTransform.SendMessage("Speak", "doing circleCast");
				Vector2 diagonalDirect = new Vector2(Mathf.Sign(moveDirect.x), Mathf.Sign(moveDirect.y)).normalized;
				List<MovingBlock> blocksMakingRaycast = new List<MovingBlock>();
				if (isPartOfDescendingPiece)
                {
					blocksMakingRaycast.AddRange(theDescendingPiece.GetBlocksFacingDirection(diagonalDirect));
                }
                else
                {
					blocksMakingRaycast.Add(this);
                }
				//RaycastHit2D fullHit = Physics2D.CircleCast((Vector2)theTransform.position, 0.06f, moveDirect.normalized, MainScript.blockHeight * 0.6f, LayerMask.GetMask("Wall", "MovingBlock")); ;
				//RaycastHit2D fullHit = Physics2D.Raycast((Vector2)theTransform.position, moveDirect.normalized, MainScript.blockWidth * 0.53f, LayerMask.GetMask("Wall", "MovingBlock"));
				foreach(MovingBlock m in blocksMakingRaycast)
                {
					RaycastHit2D fullHit = Physics2D.Raycast((Vector2)m.theTransform.position, new Vector3(diagonalDirect.x,diagonalDirect.y,0f), MainScript.blockWidth * 0.53f, LayerMask.GetMask("Wall", "MovingBlock"));
					if (fullHit)
					{
						if (fullHit.transform.tag == "MovingBlock")
						{
							//MovingBlock struckMovingBlock = fullHit.transform.GetComponent<MovingBlockScript>().thisBlock;
							//BounceOffMovingBlock(struckMovingBlock);
						}
						else if (fullHit.transform.tag == "Wall")
						{
							if (fullHit.normal.x != 0f) { moveDirect.x = 0f; }
							if (fullHit.normal.y != 0f) { moveDirect.y = 0f; }
							Vector3 directToWall = theTransform.position - (Vector3)fullHit.point;
							Vector3 directToWallHorizontal = new Vector3(directToWall.x, directToWall.y, 0f);
							moveDirect = Vector2.zero;
							settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity, new Counter(Time.fixedDeltaTime * 1f)));
							AddMovementAffector(new MovementAffector(directToWallHorizontal.normalized, 10f, 0f, Time.fixedDeltaTime * 1f, MovementAffectorType.momentumBased));
							//Vector2 positionToMoveTo = fullHit.point + (fullHit.normal * MainScript.blockHeight * 0.5f);
							//rbody.MovePosition(positionToMoveTo);
							BounceOff(fullHit.normal);
						}
					}
				}
				
			}
		}
		return moveDirect;
	}
	public bool DoesThisBlockBounceOffTransform(Transform t)
	{
		bool temp = false;
		foreach(BounceOffMovingBackCounter bCount in movingBlocksToBounceOff)
		{
			float distanceFromBouncePosition = Vector2.Distance((Vector2)t.position,(Vector2)bCount.positionToBounceFrom);
			if(distanceFromBouncePosition < MainScript.blockWidth * 0.5f)
			{
				temp = true;
			}
		}
		return temp;
	}
	public MovingBlock GetRelevantBlock()
    {
        if (isPartOfDescendingPiece)
        {
			return (theDescendingPiece.fixedJointHolder);
        }
        else { return this; }
    }
	public void BounceOffMovingBlock(MovingBlock struckMovingBlock)
	{
        //are we already bouncing off this block?
        if (struckMovingBlock.isActive)
        {
			if (!DoesThisBlockBounceOffTransform(struckMovingBlock.theTransform))
			{
				//no add it to the list
				Vector2 rawVelocity = PhysicsObjectScript.AddUpAllMovements(affecters,Time.fixedDeltaTime,false);
				Vector2 directionToMovingBlock = (Vector2)(theTransform.position - struckMovingBlock.theTransform.position);
				float dotProduct = Vector2.Dot(rawVelocity,directionToMovingBlock);
					if (struckMovingBlock.isPartOfDescendingPiece)
					{
						struckMovingBlock.theDescendingPiece.fixedJointHolder.SuspendGravity(Time.fixedDeltaTime);
						if(dotProduct > 0f)
						{
							struckMovingBlock.theDescendingPiece.fixedJointHolder.AddMovementAffector(new MovementAffector(rawVelocity.normalized, (rawVelocity.magnitude / MainScript.blockWidth) * 0.5f, 0f, 0.35f, MovementAffectorType.momentumBased));
							BounceOff((directionToMovingBlock * -1f).normalized);
						}
						struckMovingBlock.theDescendingPiece.fixedJointHolder.movingBlocksToBounceOff.Add(new BounceOffMovingBackCounter(theTransform, new Counter(0.15f)));
					}
					else
					{
						struckMovingBlock.SuspendGravity(Time.fixedDeltaTime);
						if(dotProduct > 0f)
						{
							struckMovingBlock.AddMovementAffector(new MovementAffector(rawVelocity.normalized,(rawVelocity.magnitude/MainScript.blockWidth) * 0.5f,0f,0.25f,MovementAffectorType.momentumBased));
							BounceOff((directionToMovingBlock * -1f).normalized);
						}
						struckMovingBlock.movingBlocksToBounceOff.Add(new BounceOffMovingBackCounter(theTransform, new Counter(0.15f)));
					}
				
				if (struckMovingBlock.isPartOfDescendingPiece)
				{
					//struckMovingBlock.theDescendingPiece.fixedJointHolder.SuspendGravity(Time.fixedDeltaTime);
					//struckMovingBlock.theDescendingPiece.fixedJointHolder.AddMovementAffector(new MovementAffector(rawVelocity.normalized, (rawVelocity.magnitude / MainScript.blockWidth) * 0.5f, 0f, 0.25f, MovementAffectorType.momentumBased));
					//struckMovingBlock.theDescendingPiece.fixedJointHolder.movingBlocksToBounceOff.Add(new BounceOffMovingBackCounter(theTransform, new Counter(0.15f)));
				}
				else
				{
					//struckMovingBlock.SuspendGravity(Time.fixedDeltaTime);
					//struckMovingBlock.AddMovementAffector(new MovementAffector(rawVelocity.normalized,(rawVelocity.magnitude/MainScript.blockWidth) * 0.5f,0f,0.25f,MovementAffectorType.momentumBased));
					//struckMovingBlock.movingBlocksToBounceOff.Add(new BounceOffMovingBackCounter(theTransform, new Counter(0.15f)));
				}
			
				
				movingBlocksToBounceOff.Add(new BounceOffMovingBackCounter(struckMovingBlock.theTransform, new Counter(0.15f)));
				
			}
		}
	}
	public MovingBlockSettings GetActualSettings(float timePassed)
	{
		MovingBlockSettings temp = new MovingBlockSettings(defaultSettings.isAffectedByGravity);
		List<MovingBlockSettingsAffector> tempList = new List<MovingBlockSettingsAffector>();
		foreach (MovingBlockSettingsAffector mAffector in settingsAffectors)
		{
			switch (mAffector.typeOfAffector)
			{
				case MovingBlockSettingsAffectorType.suspendGravity:
					temp.isAffectedByGravity = false;
					if (!MainScript.paused)
					{
						if (mAffector.theCounter.AddTime(timePassed))
						{
							//mAffector.readyToEnd = true;
							tempList.Add(mAffector);
						}
					}
					break;
			}
		}
		while (tempList.Count > 0) { settingsAffectors.Remove(tempList[0]); tempList.Remove(tempList[0]); }
		return temp;
	}
	public void EnableBlock()
	{
		//theTransform.GetComponent<MovingBlockScript>().EnableBlock();
		affecters = new List<MovementAffector>();
		isActive = true;
		hasSettledIntoPlace = false;
		isSettlingIntoPlace = false;
		//Rigidbody2D rbody = GetComponent<Rigidbody2D>();
		//theTransform.GetComponent<SpriteRenderer>().material.color = Color.green;
		affectedByGravity = true;
		collider.enabled = true;
		rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
		//thisPhysicsObject.affecters.Add(MainScript.gravity);
	}
	public void DisableBlock()
	{
		isActive = false;
		hasSettledIntoPlace = true;
		//thisPhysicsObject.affecters.Remove(MainScript.gravity);
		//theTransform.GetComponent<SpriteRenderer>().material.color = Color.gray;
		rbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
		collider.enabled = false; ;
		//GetComponent<BoxCollider2D>().enabled = false;
		//theTransform.GetComponent<CircleCollider2D>().enabled = false;

		//Vector2 tempV = GetXandYReference();
		//MainScript.currentMap.settledSquares[(int)tempV.x,(int)tempV.y] = thisBlock;
	}
	public void FindClosestOpenBlock()
	{
		currentEmptyBlock = MainScript.GetClosestBlockToPoint((Vector2)theTransform.position);
		currentEmptyBlock.Attach(this);
		/*currentEmptyBlock.isUntaken = false;
		isSettlingIntoPlace = true;
		currentEmptyBlock.currentMovingBlock = this;
		affecters.Add(currentEmptyBlock.moveToThisBlock);
		currentEmptyBlock.moveToThisBlock.owner = this;*/
	}
	public void PushAllBlocksInsideOutward()
	{
		numberOfBlocksInside = 0;
		if(isPartOfDescendingPiece)
        {
			foreach(MovingBlock m in theDescendingPiece.allMovingBlocks)
            {
				List<MovingBlock> allMovingBlocksInRange = MainScript.GetAllBlocksWithinBlockDistance((Vector2)theTransform.position);
				foreach(MovingBlock mBlock in allMovingBlocksInRange)
                {
					if (!theDescendingPiece.DoesThisDescendingPieceContainThisMovingBlock(mBlock))
					{
						BounceOffMovingBlock(mBlock);
					}
				}
				
			}
			
		}
        else
        {
			List<MovingBlock> allMovingBlocksInRange = MainScript.GetAllBlocksWithinBlockDistance((Vector2)theTransform.position);
			foreach (MovingBlock m in allMovingBlocksInRange)
			{
				if (m.theTransform.position == theTransform.position)
				{
					//its this block, it doesn't count
				}
				else
				{
					BounceOffMovingBlock(m);
					numberOfBlocksInside++;
					//if (!isBlockMovingAwayFromThis(m)) { m.AddMovementAffector(new MovementAffector(Vector2.zero,moveAwayFromThisBlock.speed,moveAwayFromThisBlock.decayAmount,moveAwayFromThisBlock.decays,moveAwayFromThisBlock.ends,moveAwayFromThisBlock.endTime,moveAwayFromThisBlock.theType,theTransform)); }
					//MovementAffector temp = 
					/*bool tempBool = true;
					foreach (MovementAffector mAffect in m.affecters) { if (mAffect.theType == MovementAffectorType.moveAwayFromObject && mAffect.objectPosition == theTransform) { tempBool = false; } }

					if (tempBool) { m.AddMovementAffector(new MovementAffector(Vector2.zero, moveAwayFromThisBlock.speed, moveAwayFromThisBlock.decayAmount, moveAwayFromThisBlock.decays, moveAwayFromThisBlock.ends, moveAwayFromThisBlock.endTime, moveAwayFromThisBlock.theType, theTransform, m)); }*/
				}
			}
		}
		
	}
	public DirectionAndDistance MoveOutOfTheWay()
    {
		float getOutOfTheWaySpeed = 1.5f * Time.fixedDeltaTime * MainScript.blockWidth;
		List<DirectionAndDistance> temp = MovingBlockScript.GetAllDirectionDistanceFromPoint(theTransform.position, 8);
		List<DirectionAndDistance> highestYet = new List<DirectionAndDistance>();
		float highestDistance = 0f;
		for (int i = 0; i < temp.Count; i++)
		{
			if (temp[i].distance > highestDistance)
			{
				highestYet = new List<DirectionAndDistance>() { temp[i] };
				highestDistance = temp[i].distance;
			}
			else if (temp[i].distance == highestDistance)
			{
				highestYet.Add(temp[i]);
			}
		}
		DirectionAndDistance mostUnobstructedDirection = highestYet[0];
		float bestDotProductSoFar = 1f;
		foreach (DirectionAndDistance tempDirectDistance in highestYet)
		{
			float totalDotProduct = 0f;
			foreach (BounceOffMovingBackCounter bounce in movingBlocksToBounceOff)
			{
				Vector2 directionToBounce = bounce.positionToBounceFrom - theTransform.position;
				float currentDotProduct = Vector2.Dot(directionToBounce, tempDirectDistance.direction);
				totalDotProduct += currentDotProduct;
			}
			totalDotProduct = totalDotProduct / (float)movingBlocksToBounceOff.Count;
			if (totalDotProduct < bestDotProductSoFar)
			{
				bestDotProductSoFar = totalDotProduct;
				mostUnobstructedDirection = tempDirectDistance;
			}
		}
		lastBestDirectionDistance = mostUnobstructedDirection;
		int randomInt = (int)Random.Range(0f, (float)temp.Count);
		//Vector2 direction = highestYet[randomInt].direction;
		//moveDirect += direction * getOutOfTheWaySpeed;
		Vector2 moveDirect = mostUnobstructedDirection.direction * getOutOfTheWaySpeed;
		return mostUnobstructedDirection;
	}
	public Vector2 AdjustToFitColumns(Vector2 moveDirect,float timePassed)
	{
		Vector2 eventualPosition = (Vector2)theTransform.position + moveDirect;
		float differenceBetweenNearestColumn = GetDirectionToNearestColumn(eventualPosition);
		if (differenceBetweenNearestColumn != 0f)
		{
			float moveTowardNearestColumnSpeed = 0.15f * timePassed * MainScript.blockWidth;
			if (moveTowardNearestColumnSpeed > (differenceBetweenNearestColumn * Mathf.Sign(differenceBetweenNearestColumn))) { moveTowardNearestColumnSpeed = differenceBetweenNearestColumn; }
			Vector2 direction = new Vector2(Mathf.Sign(differenceBetweenNearestColumn), 0f);
			moveDirect += direction * moveTowardNearestColumnSpeed;
		}

		//if (numberOfBlocksInside < 2)
		if (movingBlocksToBounceOff.Count < 2)
        {
			
		}
        else
        {
			
		}
		
		return moveDirect;
	}
	float GetDirectionToNearestColumn(Vector2 thePoint)
	{
		float leftCornerX = MainScript.currentMap.lowerLeftCorner.x;
		float lowestDistance = Mathf.Infinity;
		float lowestXPos = 0f;
		for (int i = 0; i < MainScript.currentMap.width; i++)
		{
			float currentXpos = leftCornerX + (i * MainScript.blockWidth);
			float distance = Vector2.Distance(new Vector2(theTransform.position.x, 0f), new Vector2(currentXpos, 0f));
			if (distance < lowestDistance) { lowestDistance = distance; lowestXPos = currentXpos; }
		}
		float xDifference = lowestXPos - theTransform.position.x;
		return xDifference;
	}
	bool DoWeHaveABouncingBackAffector()
	{
		foreach (MovementAffector mAffect in affecters) { if (mAffect.theType == MovementAffectorType.bouncingOffBlock) { return true; } }
		return false;
	}
	public MovementAffector GetMovingBackAffector()
	{
		MovementAffector temp = affecters[0];
		foreach (MovementAffector m in affecters) { if (m.theType == MovementAffectorType.bouncingOffBlock) { temp = m; } }
		return temp;
	}
	public MovementAffector GetAffectorWithIdentifier(string theIdentifier)
    {
		foreach(MovementAffector m in affecters)
        {
			if(m.identifierName == theIdentifier) { return m; }
        }
		return null;
    }
	public void RemoveMovementAffector(MovementAffector m)
	{
		affecters.Remove(m);
	}
	public void AddMovementAffector(MovementAffector m)
	{
		//Vector2 directToThisMovingBlock = (Vector2)m.theTransform.position - (Vector2)transform.position;

		if (isPartOfDescendingPiece)
		{
			theDescendingPiece.AddAffector(m);
		}
		else
		{
			if (m.theType == MovementAffectorType.bouncingOffBlock)
			{
				if (isBouncingBack && DoWeHaveABouncingBackAffector())
				{
					affecters.Remove(GetMovingBackAffector());
				}
				else { isBouncingBack = true; }
			}
			
			affecters.Add(m);
		}

	}
	public Vector2 GetVelocity()
	{
		return PhysicsObjectScript.AddUpAllMovements(affecters,Time.fixedDeltaTime,false);
	}
	public bool DoesThisBlockHaveThisMovementAffector(MovementAffector mAffector)
	{
		return (affecters.Contains(mAffector));
	}
	public bool DoesThisBlockHaveAnAffectorWithThisIdentifier(string theIdentifier)
	{
		
        if (isPartOfDescendingPiece && !isFixedJointHolder)
        {
			return (theDescendingPiece.fixedJointHolder.DoesThisBlockHaveAnAffectorWithThisIdentifier(theIdentifier));
        }
        else
        {
			bool tempBool = false;
			foreach (MovementAffector m in affecters)
			{
				if (m.identifierName == theIdentifier)
				{
					tempBool = true;
				}
			}
			return tempBool;
		}
		
	}
	public bool DoesThisBlockHaveThisTypeMovementAffector(MovementAffectorType mAffectorType)
	{
		bool tempBool = false;
		foreach(MovementAffector mAffect in affecters)
		{
			if(mAffect.theType == mAffectorType)
			{
				tempBool = true;
			}
		}
		return (tempBool);
	}
	public MovingBlock(Transform thisTransform)
	{
		spriteInteractionObject = new SpriteInteractionObject(thisTransform);
		blockColor = GetRandomColor();
		spriteInteractionObject.ChangeColor(blockColor);
		movingBlocksToBounceOff = new List<BounceOffMovingBackCounter>();
		bounceBackCounter = new Counter(1.5f);
		latestBounceOffMovingBlock = new MovementAffector(Vector2.zero, 2.5f, 0f, false, false, 0f, MovementAffectorType.bouncingOffBlock);
		affecters = new List<MovementAffector>();
		defaultSettings = new MovingBlockSettings(true);
		settingsAffectors = new List<MovingBlockSettingsAffector>();
		//descendingPieceX = xValue;
		//descendingPieceY = yValue;
		isPartOfDescendingPiece = false;
		//theDescendingPieceTransform = thePieceTransform;
		//theDescendingPiece = thisDescendingPiece;
		//theDescendingPieceTransform = thisDescendingPiece.thisPieceTransform;
		theTransform = thisTransform;
		collider = theTransform.GetComponent<CircleCollider2D>();
		rbody = theTransform.GetComponent<Rigidbody2D>();
		moveAwayFromThisBlock = new MovementAffector(Vector2.zero, 3f, 0f, false, true, 0.5f, MovementAffectorType.moveAwayFromObject, theTransform);
		moveAwayFromThisBlock.owner = this;
	}
}
public class MovingBlockSettings
{
	public float maxSpeed = 12.5f;
	public bool isAffectedByGravity = true;
	public bool vulnerable = true;
	public MovingBlockSettings(bool gravityAffected)
	{
		isAffectedByGravity = gravityAffected;
	}
}
public class MovingBlockSettingsAffector
{
	public float a;
	public float b;
	public float c;
	public bool ends;
	public bool readyToEnd = false;
	public Counter theCounter;
	public Vector2 direction;
	public MovingBlockSettingsAffectorType typeOfAffector;
	public MovingBlockSettingsAffector(MovingBlockSettingsAffectorType theType, Counter count)
	{
		typeOfAffector = theType;
		theCounter = count;
	}
}
public enum MovingBlockSettingsAffectorType { suspendGravity, redirectGravity }
public class BounceOffMovingBackCounter
{
	public Transform movingBlockToBounceOff;
	public Vector3 positionToBounceFrom;
	public Counter bounceOffCounter;
	public bool readyToDie = false;
	public BounceOffMovingBackCounter(Transform theBlock,Counter theCounter)
	{
		positionToBounceFrom = theBlock.position;
		movingBlockToBounceOff = theBlock;
		bounceOffCounter = theCounter;
	}
}
public class DirectionAndDistance
{
	public Vector2 direction;
	public float distance;
		public DirectionAndDistance(Vector2 theDirection,float theDistance)
    {
		distance = theDistance;
		direction = theDirection;
    }
}