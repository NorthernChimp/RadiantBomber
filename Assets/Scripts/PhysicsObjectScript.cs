using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObjectScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
	public static Vector2 AddUpAllMovements(List<MovementAffector> theListOfAffectors,float timePassed,bool addTime)
	{
		Vector2 tempVector = Vector2.zero;
		List<MovementAffector> affectorsToRemove = new List<MovementAffector>();
		for(int i = 0;i < theListOfAffectors.Count; i++)
        {
			MovementAffector m = theListOfAffectors[i];
			tempVector = ApplyAffector(m, tempVector,timePassed,addTime);
			if (addTime) { m = UpdateAffector(m, timePassed); }
			//if (m.readyToEnd) { affectorsToRemove.Add(m); }
		}
		foreach (MovementAffector m in theListOfAffectors)
		{
			
		}
		int counter = 0;
		while (affectorsToRemove.Count > 0 && counter < 100)
		{
			theListOfAffectors.Remove(affectorsToRemove[0]);
			counter++;
			affectorsToRemove.Remove(affectorsToRemove[0]);
		}
		return tempVector;
	}
	public static Vector2 CheckForWalls(Vector2 moveDirect,Transform objectCheckingWalls, bool includeHorizontal, bool includeVertical,List<MovementAffector> affectors)
	{
		if (moveDirect.magnitude != 0f)
		{
			//basically check if you can move horizontally in the moveDirect direction
			if (moveDirect.x != 0f && includeHorizontal)
			{
				Vector2 horizontalDirection = new Vector2(Mathf.Sign(moveDirect.x), 0f);
				RaycastHit2D tempHit = Physics2D.Raycast(objectCheckingWalls.position,horizontalDirection,MainScript.blockHeight * 0.53f,LayerMask.GetMask("Wall"));
				if(tempHit)
				{
					Vector2 positionToMoveTo = tempHit.point + (tempHit.normal * MainScript.blockHeight * 0.5f);
					float xPosition = tempHit.transform.position.x + (horizontalDirection.x * -1f * MainScript.blockHeight);
					//rbody.MovePosition(positionToMoveTo);
					//BounceOff(temp.normal, moveDirect);
					moveDirect = new Vector2(0f, moveDirect.y);
					BounceOff(tempHit.normal,affectors);
				}
				/*if (allHorizontalRaycastHits.Count > 0)
				{
					//horizontal raycast hit something
					RaycastHit2D temp = allHorizontalRaycastHits[0];
					for (int i = 1; i < allHorizontalRaycastHits.Count; i++)
					{
						if (allHorizontalRaycastHits[i].distance < temp.distance) { temp = allHorizontalRaycastHits[i]; }
					}
					if (temp.transform.tag == "Wall")
					{
						
					}
				}*/
			}
			if (moveDirect.y != 0f && includeVertical)
			{
				Vector2 direction = new Vector2(0f, Mathf.Sign(moveDirect.y));
				RaycastHit2D tempHit = Physics2D.Raycast(objectCheckingWalls.position,direction,MainScript.blockHeight * 0.53f,LayerMask.GetMask("Wall"));
				if(tempHit)
				{
					//float yPosition = tempHit.transform.position.y + (direction.y * -1f * MainScript.blockHeight);
					//Vector2 positionToMoveTo = tempHit.point + (tempHit.normal * MainScript.blockHeight * 0.5f);
					//Vector2 positionToMoveTo = tempHit.point + (tempHit.normal * MainScript.blockHeight * 0.5f);
					//rbody.MovePosition(positionToMoveTo);
					//BounceOff(temp.normal, moveDirect);
					moveDirect = new Vector2( moveDirect.x,0f);
					BounceOff(tempHit.normal,affectors);
				}
				/*List<MovingBlock> blocksMakingVerticalCheck = new List<MovingBlock>();
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
				}*/
				
			}
			if(moveDirect.x != 0f && moveDirect.y != 0f)
			{
				Vector2 direction = moveDirect.normalized;
				RaycastHit2D tempHit = Physics2D.Raycast(objectCheckingWalls.position,direction,MainScript.blockHeight * 0.56f,LayerMask.GetMask("Wall"));
				if(tempHit)
				{
					if(tempHit.normal.x != 0f)
					{
						moveDirect = new Vector2(0f,moveDirect.y);
						BounceOff(tempHit.normal,affectors);
					}
					if(tempHit.normal.y !=0f)
					{
						//float yPosition = tempHit.transform.position.y + (direction.y * -1f * MainScript.blockHeight);moveDirect = new Vector2( moveDirect.x,0f);
						moveDirect = new Vector2(moveDirect.x,0f);
						BounceOff(tempHit.normal,affectors);
					}
				}
			}
			//if the wall check has not reduced movedirect to zero do a full move collision check
			/*if (moveDirect.magnitude > 0f)
			{
				//theTransform.SendMessage("Speak", "doing circleCast");
				//RaycastHit2D fullHit = Physics2D.CircleCast((Vector2)theTransform.position, 0.06f, moveDirect.normalized, MainScript.blockHeight * 0.6f, LayerMask.GetMask("Wall", "MovingBlock")); ;
				RaycastHit2D fullHit = Physics2D.Raycast((Vector2)theTransform.position, moveDirect.normalized, MainScript.blockWidth * 0.56f, LayerMask.GetMask("Wall", "MovingBlock"));
				if (fullHit)
				{
					if (fullHit.transform.tag == "MovingBlock")
					{
						MovingBlock struckMovingBlock = fullHit.transform.GetComponent<MovingBlockScript>().thisBlock;
						BounceOffMovingBlock(struckMovingBlock);
					}else
					{
						Vector2 positionToMoveTo = fullHit.point + (fullHit.normal * MainScript.blockHeight * 0.5f);
						rbody.MovePosition(positionToMoveTo);
						BounceOff(fullHit.normal, moveDirect);
					}
				}
			}*/
		}
		return moveDirect;
	}
	public static void BounceOff(Vector2 theNormal, List<MovementAffector> affecters)
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
	public static Vector2 ApplyAffector(MovementAffector m, Vector2 moveDirect,float timePassed,bool updateTime)
	{
		Vector2 result = Vector2.zero;
		if (m.theType == MovementAffectorType.momentumBased || m.theType == MovementAffectorType.arbitrary || m.theType == MovementAffectorType.bouncingOffBlock ||m.theType == MovementAffectorType.bounceOffPlayer || m.theType == MovementAffectorType.getOutOfPlayersWay|| m.theType == MovementAffectorType.GetOutOfTheTop)
		{
			//if (m.theType == MovementAffectorType.momentumBased) { print("direction is " + m.directionOfAffector + " and the speed is " + m.speed + " and the time is " + Time.deltaTime); }
			result = moveDirect + (m.directionOfAffector * m.speed * timePassed);
		}else if(m.theType == MovementAffectorType.moveTowardsPlayer)
        {
			Vector2 directionToMove = MainScript.thePlayer.position - m.owner.theTransform.position;
			float amountToMove = m.speed * timePassed;
			result = moveDirect + (directionToMove.normalized  * amountToMove);
        }
		else if (m.theType == MovementAffectorType.moveTowardsPlayer)
		{
			Vector2 directionToMove = ((Vector2)MainScript.thePlayer.position - (Vector2)m.owner.theTransform.position).normalized;
			float amountToMove = m.speed * timePassed;
			float distance = Vector2.Distance((Vector2)m.owner.theTransform.position, m.directionOfAffector);
			//if(amountToMove > distance)
			if (distance < MainScript.blockHeight * 0.45f)
			{
				result = moveDirect + (directionToMove * distance);
				if (m.theType == MovementAffectorType.moveTowardsEmptyBlock)
				{
					//thisTetrisSquareScript.DisableBlock();
					if (m.owner.isPartOfDescendingPiece)
					{
						// *** work on the Descending Piece Class
						m.owner.theDescendingPiece.BreakApartDescendingPiece();
						foreach (MovingBlock mov in m.owner.theDescendingPiece.allMovingBlocks)
						{
							mov.FindClosestOpenBlock();
						}
					}
					else
					{
						m.owner.currentEmptyBlock.Attach(m.owner);
					}
				}
			}
			else { result = moveDirect + (directionToMove * amountToMove); }

		}
		else if (m.theType == MovementAffectorType.moveAwayFromObject)
		{
			Vector2 directionToMove = (Vector2)m.owner.theTransform.position - (Vector2)m.objectPosition.position;
			//directionToMove = new Vector2(0f, directionToMove.y);
			result = moveDirect + (directionToMove.normalized * m.speed * timePassed);
		}
        //if (m.decays) { m.speed -= m.decayAmount * timePassed; }
		//if (m.decays && updateTime) { m.speed -= m.decayAmount * timePassed * MainScript.blockHeight; if (m.speed <= 0f) { m.readyToEnd = true; } }
		//if (m.ends && updateTime) { m.endTime -= timePassed; if (m.endTime <= 0f) { m.readyToEnd = true; } }
		return result;
	}
	public static MovementAffector UpdateAffector(MovementAffector m,float timePassed)
    {
		if (m.decays) 
		{ 
			m.speed -= m.decayAmount * timePassed * MainScript.blockHeight; 
			if (m.speed <= 0f) { m.readyToEnd = true; } 
		}else if (m.accelerates)
        {
			m.speed += m.decayAmount * timePassed * MainScript.blockHeight;
        }
		if (m.ends) 
		{ 
			m.endTime -= timePassed; if (m.endTime <= 0f) 
			{
				m.readyToEnd = true; 
			}
		}
		return m;
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}

public class MovementAffector
{
	public Vector2 directionOfAffector;
	public float speed;
	public float decayAmount;
	public bool decays;
	public bool accelerates;
	public bool ends;
	public bool readyToEnd;
	public bool isOnMovingBlock = true;
	public float endTime;
	public string identifierName = "none";
	public Transform objectPosition;
	public MovingBlock owner;
	public MovementAffectorType theType;
	public MovementAffector(Vector2 direction, float theSpeed, float decaySpeed, bool itDecays, bool itEnds, float theEndTime, MovementAffectorType typeToImplement,string theIdentifier)
	{
		identifierName = theIdentifier;
		directionOfAffector = direction;
		speed = theSpeed * MainScript.blockWidth;
		decayAmount = decaySpeed * MainScript.blockWidth;
		decays = itDecays;
		ends = itEnds;
		endTime = theEndTime;
		theType = typeToImplement;
	}
	public MovementAffector(Vector2 direction, float theSpeed, float decaySpeed, bool itDecays, bool itEnds, float theEndTime, MovementAffectorType typeToImplement, bool thisAccelerates)
	{
		accelerates = thisAccelerates;
		directionOfAffector = direction;
		speed = theSpeed * MainScript.blockWidth;
		decayAmount = decaySpeed * MainScript.blockWidth;
		decays = itDecays;
		ends = itEnds;
		endTime = theEndTime;
		theType = typeToImplement;
	}
	public MovementAffector(Vector2 direction, float theSpeed, float decaySpeed, bool itDecays, bool itEnds, float theEndTime, MovementAffectorType typeToImplement)
	{
		directionOfAffector = direction;
		speed = theSpeed * MainScript.blockWidth;
		decayAmount = decaySpeed * MainScript.blockWidth;
		decays = itDecays;
		ends = itEnds;
		endTime = theEndTime;
		theType = typeToImplement;
	}
	public MovementAffector(Vector2 direction, float theSpeed,float decaySpeed,float theEndTime,MovementAffectorType typeToImplement)
    {
		directionOfAffector = direction;
		speed = theSpeed * MainScript.blockWidth;
		decayAmount = decaySpeed * MainScript.blockWidth;
		if(decayAmount > 0f) { decays = true; }
		//decays = itDecays;
		//ends = itEnds;
		endTime = theEndTime;
		if (endTime > 0f) { ends = true; }
		theType = typeToImplement;
	}
	public MovementAffector(Vector2 direction, float theSpeed,float decaySpeed,float theEndTime,MovementAffectorType typeToImplement,string identifyingName)
    {
		identifierName = identifyingName;
		directionOfAffector = direction;
		speed = theSpeed * MainScript.blockWidth;
		decayAmount = decaySpeed * MainScript.blockWidth;
		if(decayAmount > 0f) { decays = true; }
		//decays = itDecays;
		//ends = itEnds;
		endTime = theEndTime;
		if (endTime > 0f) { ends = true; }
		theType = typeToImplement;
	}
	public MovementAffector(Vector2 direction, float theSpeed, float decaySpeed, bool itDecays, bool itEnds, float theEndTime, MovementAffectorType typeToImplement, Transform objPosition)
	{
		directionOfAffector = direction;
		speed = theSpeed * MainScript.blockWidth;
		decayAmount = decaySpeed * MainScript.blockWidth;
		decays = itDecays;
		ends = itEnds;
		endTime = theEndTime;
		theType = typeToImplement;
		objectPosition = objPosition;
	}
	public MovementAffector(Vector2 direction, float theSpeed, float decaySpeed, bool itDecays, bool itEnds, float theEndTime, MovementAffectorType typeToImplement, Transform objPosition, MovingBlock theOwner)
	{
		directionOfAffector = direction;
		speed = theSpeed * MainScript.blockWidth;
		decayAmount = decaySpeed * MainScript.blockWidth;
		decays = itDecays;
		ends = itEnds;
		endTime = theEndTime;
		theType = typeToImplement;
		objectPosition = objPosition;
		owner = theOwner;
	}
	/*public void OnTriggerEnter2D(Collider2D col)
	{
		MovementAffector getOutOfTheTop = new MovementAffector(Vector2.down,9f,0f,0f,MovementAffectorType.GetOutOfTheTop);
		if(col.transform.tag == "MovingBlock")
		{
			MovingBlock m = col.transform.GetComponent<MovingBlockScript>().thisBlock;
			if(!m.DoesThisBlockHaveThisTypeMovementAffector(MovementAffectorType.GetOutOfTheTop))
			{
				m.AddMovementAffector(getOutOfTheTop);
			}
		}
		
	}
	public void OnTriggerExit2D(Collider2D col)
	{
		if(col.transform.tag == "MovingBlock")
		{
			MovingBlock m = col.transform.GetComponent<MovingBlockScript>().thisBlock;
			if(m.DoesThisBlockHaveThisTypeMovementAffector(MovementAffectorType.GetOutOfTheTop))
			{
				m.RemoveMovementAffector(getOutOfTheTop);
			}
		}
	}*/
}
public enum MovementAffectorType
{
	momentumBased, arbitrary, moveTowardsPoint, moveTowardsEmptyBlock, moveAwayFromObject, bouncingOffBlock,bounceOffPlayer,getOutOfPlayersWay,GetOutOfTheTop,moveTowardsPlayer
}