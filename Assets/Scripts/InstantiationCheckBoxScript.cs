using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiationCheckBoxScript : MonoBehaviour
{
	// Start is called before the first frame update
	public InstantiationCheckBox thisCheckBox;
	
    void Start()
    {
		thisCheckBox.allMovingBlocksInside = new List<Transform>();
    }
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "MovingBlock")
		{
			thisCheckBox.allMovingBlocksInside.Add(collision.transform);
		}
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "MovingBlock")
		{
			thisCheckBox.allMovingBlocksInside.Remove(collision.transform);
		}
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
public class InstantiationCheckBox
{
	public List<Transform> allMovingBlocksInside;
	public BoxCollider2D collider;
	public Vector2 lowerLeftCorner;
	public Transform theTransform;
	public InstantiationBoxPrefab prefabToInstantiate;
	public bool readyToDie = false;
	public InstantiationCheckBox(Transform checkBoxTransform, InstantiationBoxPrefab thePrefab)
	{
		allMovingBlocksInside = new List<Transform>();
		collider = checkBoxTransform.GetComponent<BoxCollider2D>();
		prefabToInstantiate = thePrefab;
		theTransform = checkBoxTransform;
		MainScript.allInstantiationCheckBoxes.Add(this);
	}
	public void SetSize(int xWidth, int yHeight)
	{
		float totalWidth = xWidth * MainScript.blockWidth;
		float totalHeight = yHeight * MainScript.blockWidth;
		theTransform.localScale = new Vector3(totalWidth, totalHeight, 1f);
		//collider. = totalWidth * 0.5f;
		//collider.
		//collider.height = totalHeight * 0.5f;
		lowerLeftCorner = (Vector2)(theTransform.position + new Vector3(totalWidth * -0.5f, totalHeight * -0.5f, 0f) + new Vector3(MainScript.blockWidth * 0.5f, MainScript.blockWidth * 0.5f, 0f));
	}
	public bool CheckForBlocks()
	{
		bool canInstantiate = true;
		//if(allMovingBlocksInside.Count > 0) { canInstantiate = false; }
		if(TopColliderScript.allMovingBlocksInside.Count > 0) { canInstantiate = false; }
		return (canInstantiate);
	}
	public void UpdateInstantiateBox()
	{
		if (CheckForBlocks())
		{
			readyToDie = true;
			switch (prefabToInstantiate)
			{
				case InstantiationBoxPrefab.descendingPiece:
					MainScript.currentGame.allDescendingPieces.Add(MainScript.CreateDescendingPiece(lowerLeftCorner, DescendingPieceScript.RandomlyRearrangeDescendingPieceLayour(DescendingPieceScript.GetRandomDescendingPieceLayout())));
					break;
				case InstantiationBoxPrefab.movingBlock:
					MainScript.IndividualMovingBlock(lowerLeftCorner);
					break;
			}
		}
	}
}
public enum InstantiationBoxPrefab { movingBlock, descendingPiece }