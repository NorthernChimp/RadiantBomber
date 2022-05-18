using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyBlockScript : MonoBehaviour
{
	// Start is called before the first frame update
	public EmptyBlock thisEmptyBlock;
    void Start()
    {
        
    }
	public void ChangeHealth(int difference)
	{
		//thisEmptyBlock.ChangeHealth(difference);
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class EmptyBlock
{
	public int xPos;
	public int yPos;
	public int health = 3;
	public int maxHealth = 3;
	public bool isUntaken = true;
	public MovingBlock currentMovingBlock;
	public MovementAffector moveToThisBlock;
	public Transform thisTransform;
	public BoxCollider2D thisBoxCollider;
	public Counter regenCounter;
	public EmptyBlock(int x, int y, Transform theTransform)
	{
		regenCounter = new Counter(3f);
		isUntaken = true;
		xPos = x;
		yPos = y;
		thisTransform = theTransform;
		thisBoxCollider = thisTransform.GetComponent<BoxCollider2D>();
		moveToThisBlock = new MovementAffector(thisTransform.position, 5f, 0f, false, false, 0f, MovementAffectorType.moveTowardsEmptyBlock);
	}
	public void UpdateEmptyBlock(float timePass)
	{
		if(health < maxHealth)
        {
			regenCounter.AddTime(timePass);
            if (regenCounter.hasFinished) { ChangeHealth(1);regenCounter.ResetTimer(); }
        }
	}
	public void ChangeHealth(int difference)
	{
		if(!isUntaken)
		{
			health += difference;
			if(health <= 0)
			{
				DetachAndDestroy();
			}
		}
	}
	public void Attach(MovingBlock m)
	{
		//thisTransform.GetComponent<EmptyBlockScript>().Attach(m);
		thisTransform.GetComponent<SpriteRenderer>().material.color = Color.blue;
		//print("we are attaching");
		currentMovingBlock = m;
		isUntaken = false;
		//blockToAttach.theTransform.GetComponent<MovingBlockScript>().DisableBlock();
		m.DisableBlock();
		//MainScript.allMovingBlocks.Remove(m);
		m.theTransform.position = new Vector3(thisTransform.position.x, thisTransform.position.y, m.theTransform.position.z); ;
		thisBoxCollider.enabled = true;
	}
	public void DetachAndDestroy()
	{
		thisTransform.GetComponent<SpriteRenderer>().material.color = Color.white;
		MainScript.allMovingBlocks.Remove(currentMovingBlock);
		//MainScript.CreateExplosionAt(transform.position,currentMovingBlock.block)
		//MainScript.currentGame.allMovingBlocksInGame.Remove(currentMovingBlock);
		currentMovingBlock.theTransform.SendMessage("DestroyThisMovingBlock");
		currentMovingBlock = null;
		isUntaken = true;
		thisBoxCollider.enabled = false;
	}
	public void Detach()
	{
        if (!MainScript.blocksToRemove.Contains(this))
        {
			thisTransform.GetComponent<SpriteRenderer>().material.color = Color.white;
			//PhysicsObjectScript tempScript = thisEmptyBlock.currentMovingBlock.theTransform.GetComponent<PhysicsObjectScript>();
			currentMovingBlock.affecters.Remove(moveToThisBlock);
			currentMovingBlock.EnableBlock();
			//currentMovingBlock.theTransform.GetComponent<PhysicsObjectScript>().affectedByGravity = true;
			//tempScript.affecters.Add(MainScript.gravity);
			isUntaken = true;
			thisBoxCollider.enabled = false;
		}
		//thisTransform.GetComponent<EmptyBlockScript>().Detach();
		
	}
}
/*public class UtilityAbilityReference
{
	public Transform abilityTransform;
	public bool readyToDie = false;
	public UtilityAbilityReference(Transform t)
	{
		abilityTransform = t;
	}
}*/