using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescendingPieceScript : MonoBehaviour
{
	// Start is called before the first frame update
	public DescendingPiece thisPiece;
    void Start()
    {
        
    }
	public static void RemoveAllFixedJoints(MovingBlock theFixedJointHolder)
    {
        if (theFixedJointHolder.theTransform)
        {
			FixedJoint2D[] allJointsArray = theFixedJointHolder.theTransform.GetComponents<FixedJoint2D>();
			List<FixedJoint2D> allJoints = new List<FixedJoint2D>();
			allJoints.AddRange(allJointsArray);
			int counter = 0;
			bool countCheck = false;
			while (allJoints.Count > 0 && !countCheck)
			{
				counter++;
				FixedJoint2D temp = allJoints[0];
				allJoints.Remove(temp);
				Destroy(temp);

				if (counter == 100)
				{
					//print("count check fukt up");
					countCheck = true;
				}
			}
		}
	}
	public static bool[,] RandomlyRearrangeDescendingPieceLayour(bool[,] layout)
    {
		bool[,] tempBoolList = new bool[4, 4];
		
		int randomIntX = (int)Mathf.Sign(Random.Range(-0.5f, 0.5f));
		int randomIntY = (int)Mathf.Sign(Random.Range(-0.5f, 0.5f));
		int randomSwitch = (int)Mathf.Sign(Random.Range(-0.5f, 0.5f));
		//print("random x and y is " + randomIntX + randomIntY + "and random switch is " + randomSwitch);
		for(int i = 0; i < layout.GetLength(0); i++)
        {
			int xRef = i;
			if(randomIntX == -1f) { xRef = 3 - i; }
			for(int o = 0; o < layout.GetLength(1); o++)
            {
				int yRef = o;
				if(randomIntY == -1f) { yRef = 3 - o; }
				if(randomSwitch != 1f) { tempBoolList[xRef, yRef] = layout[i, o]; } else { tempBoolList[yRef, xRef] = layout[i, o]; }
				
            }
        }
		return (tempBoolList);
    }
	public static bool[,] GetRandomDescendingPieceLayout()
	{
		int totalRandomLayouts = 18;
		int randomInt = (int)Random.Range(0f,totalRandomLayouts);
		//print("current descending piece is " + randomInt.ToString());
		switch(randomInt)
		{
			case 0:
				return new bool[4,4]{{false,false,false,false},{false,true,true,false},{false,true,true,false},{false,false,false,false}};
			case 1:
				return new bool[4,4]{{false,false,true,false},{false,true,true,false},{false,false,true,false},{false,false,false,false}};
			case 2:
				return new bool[4, 4] { { true, false, false, false }, { true, false, false, false }, { true, false, false, false }, { true, false, false, false } };
			case 3:
				return new bool[4, 4] { { false, false, false, false }, { false, true, true, true }, { false, false, true, false }, { false, false, true, false } };
			case 4:
				return new bool[4, 4] { { false, false, false, false }, { true, true, true, true }, { false, false, false, true }, { false, false, false, true } };
			case 5:
				return new bool[4, 4] { { true, true, true, false }, { true, false, true, false }, { false, false, false, false }, { false, false, false, false } };
			case 6:
				return new bool[4, 4] { { true, true, true, true }, { true, false, false, true}, { true, false, false, true}, { true, true, true, true } };
			case 7:
				return new bool[4, 4] { { true, false, false, false }, { false, true, false, false }, { false, false, true, false }, { false, false, false, true} };
			case 8:
				return new bool[4, 4] { { false, true, true, false }, { false, false, true, true }, { false, false, true, true}, { false, false, false, false } };
			case 9:
				return new bool[4, 4] { { false, true, false, false }, { false, true, true, false }, { false, true, true, false }, { false, false, true, false } };
			case 10:
				return new bool[4, 4] { { false, false, false, false }, { false, true, false, false }, { false, true, false, false }, { false, true, true, true} };
			case 11:
				return new bool[4, 4] { { true, true, false, false }, { false, true, false, false }, { false, true, false, false }, { false, true, true, false } };
			case 12:
				return new bool[4, 4] { { false, false, true, true}, { false, false, true, true}, { false, false, false, true}, { false, false, false, true} };
			case 13:
				return new bool[4, 4] { { false, true, true, true}, { false, true, false, false }, { false, true, false, false }, { false, false, false, false } };
			case 14:
				return new bool[4, 4] { { false, false, true, false }, { false, true, true, false }, { false, true, false, false }, { false, false, false, false } };
			case 15:
				return new bool[4, 4] { { false, false, false, false }, { false, true, true, true}, { true, true, false, true}, { true, false, false, false } };
			case 16:
				return new bool[4, 4] { { false, false, false, false }, { false, true, true, true }, { false, true, true, true }, { false, false, false, false } };
			case 17:
				return new bool[4, 4] { { true, true, false, false }, { false, true, false, false }, { false, true, true, false}, { false, false, false, false } };
		}
		return new bool[4,4]{{false,false,false,true},{false,true,true,false},{false,true,true,false},{false,false,false,true}};
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class DescendingPiece
{
	public Transform thisPieceTransform;
	public List<Transform> allPieces;
	public List<MovingBlock> allMovingBlocks;
	public MovingBlock fixedJointHolder;
	public Transform[,] piecesGrid;
	public MovingBlock[,] movingPiecesGrid;
	public Color colorOfDescendingPiece = Color.white;
	public bool[,] gridMap;
	public bool readyToDie = false;
	public DescendingPiece()
	{
		/*
		List<Transform> thePieces,MovingBlock[,] thePiecesGrid,bool[,] theGridMap
		
		allMovingBlocks = new List<MovingBlock>();
		allPieces = thePieces;
		foreach(Transform t in thePieces)
		{
			allMovingBlocks.Add(t.GetComponent<MovingBlockScript>().thisBlock);
		}
		gridMap = theGridMap;
		movingPiecesGrid = thePiecesGrid;*/
	}
	
	public bool DoesThisDescendingPieceContainThisMovingBlock(MovingBlock m)
    {
		bool tempBool = false;
		foreach(MovingBlock mblock in allMovingBlocks)
        {
			if(mblock.theTransform.position == m.theTransform.position)
            {
				tempBool = true;
            }
        }
		return tempBool;
    }
	public bool AreWeInTopCollider()
	{
		bool tempBool = false;
		foreach(MovingBlock m in allMovingBlocks)
		{
			if(m.isInTopCollider){tempBool = true;;}
		}
		return tempBool;
	}
	public void BreakApartDescendingPiece()
    {
		foreach (MovingBlock m in allMovingBlocks)
		{
			m.affecters = new List<MovementAffector>();
			//MovingBlockScript tempScript = m.theTransform.GetComponent<MovingBlockScript>();
			//tempScript.EnableBlock();
			//tempScript.thisBlock.isPartOfDescendingPiece = false;
			m.isPartOfDescendingPiece = false;
		}
		DescendingPieceScript.RemoveAllFixedJoints(fixedJointHolder);
		readyToDie = true;
	}
	public List<MovingBlock> GetBlocksFacingDirection(Vector2 direction)
	{
		int xDirect = (int)Mathf.Sign(direction.x);
		int yDirect = (int)Mathf.Sign(direction.y);
		List<MovingBlock> tempList = new List<MovingBlock>();
		//print(thisPiece.allMovingBlocks.Count);
		//print(thePiece.gridMap.GetLength(0));
		if (xDirect != 0 || yDirect != 0)
		{
			for (int x = 0; x < gridMap.GetLength(0); x++)
			{
				for (int y = 0; y < gridMap.GetLength(1); y++)
				{
					if (gridMap[x, y])
					{
						int xAfterAddingxDirect = x + xDirect;
						int yAfterAddingyDirect = y + yDirect;
						//print(y
						//print("the direction is " + direction + " X is " + x + "and the xAfter adding is " + xAfterAddingxDirect);
						//print("y is " + y + "yafteradding is " + yAfterAddingyDirect);
						if (direction.x != 0f)
						{
							if (xAfterAddingxDirect == -1 || xAfterAddingxDirect == gridMap.GetLength(0))
							{
								tempList.Add(movingPiecesGrid[x, y]);
							}
							else if (xAfterAddingxDirect < gridMap.GetLength(0) && xAfterAddingxDirect >= 0)
							{
								if (!(gridMap[xAfterAddingxDirect, y]))
								{
									tempList.Add(movingPiecesGrid[x, y]);
								}
							}
						}
						if (direction.y != 0f)
						{
							if (yAfterAddingyDirect == -1 || yAfterAddingyDirect == gridMap.GetLength(1))
							{
								tempList.Add(movingPiecesGrid[x, y]);
							}
							else if (yAfterAddingyDirect < gridMap.GetLength(1) && yAfterAddingyDirect >= 0)
							{
								if (!(gridMap[x, yAfterAddingyDirect]))
								{
									tempList.Add(movingPiecesGrid[x, y]);
								}
							}
						}

					}
				}
			}
		}
		return tempList;
	}
	public Color GetRandomColor()
	{
		//Random.seed = System.DateTime.Now.Millisecond;
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
	public void AddAffector(MovementAffector m)
	{
		fixedJointHolder.affecters.Add(m) ;
		/*foreach (MovingBlock moveBlock in allMovingBlocks)
		{
			//if (m.theType == MovementAffectorType.bouncingOffBlock) { if (moveBlock.isBouncingBack) { moveBlock.affecters.Remove(moveBlock.GetMovingBackAffector()); } }
			moveBlock.affecters.Add(m);
			moveBlock.isBouncingBack = true;
			//moveBlock.theTransform.GetComponent<PhysicsObjectScript>().affecters.Add(m);
		}*/
	}
}