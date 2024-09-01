using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackExplosionScript : MonoBehaviour
{
	// Start is called before the first frame update
	public float maxDistance = 2.0f;
	public float speed = 3f;
	public float time = 1.75f;
	public UtilityAbilityInstance thisAbilityInstance;
	public Counter deathCounter;


    void Start()
    {
        
    }
	public void UseUtility()
    {
		thisAbilityInstance = new UtilityAbilityInstance(transform,false,false, new Counter(time * 3f),false);
		MainScript.utilityAbilities.Add(thisAbilityInstance);
		deathCounter = new Counter(time);
		GetComponent<UtilityAbilityScript>().thisAbilityInstance = thisAbilityInstance;
		//ApplyKnockBack();
    }
	public void UpdateUtility()
    {
		deathCounter.AddTime(Time.fixedDeltaTime);
        if (deathCounter.hasFinished) 
		{ 
			ApplyKnockBack();
			MainScript.CreateExplosionAt((Vector2)transform.position,Color.blue,MainScript.blockHeight * 1.5f,0.85f);
			//MainScript.utilityAbilities.Remove(this); ; Destroy(gameObject); }
			//MainScript.utilityAbilities.Remove(transform);
			//DestroyThisUtility();
			thisAbilityInstance.readyToDie = true;
		}
    }
	public void DestroyThisUtility()
	{
		Destroy(gameObject);
	}
	public void ApplyKnockBack()
	{
		//print("we are starting" + MainScript.currentGame.allMovingBlocksInGame.Count);
		//float minDistance = 0.5f;
		//float speedToPush = 20f;
		Vector2 position = (Vector2)transform.position;
		List<MovingBlock> tempList = MainScript.GetAllBlocksFromPointWithinDistance(position, maxDistance);
		List<DescendingPiece> descendingPiecesToBreakApart = new List<DescendingPiece>();
		List<MovingBlock> affectedMovingBlocks = new List<MovingBlock>();
		foreach (MovingBlock m in tempList)
		{
			if (m.isPartOfDescendingPiece)
			{
				if (!descendingPiecesToBreakApart.Contains(m.theDescendingPiece))
				{
					descendingPiecesToBreakApart.Add(m.theDescendingPiece);
				}
			}
			else
			{
				//ApplyAffect(m);
				//print("adding here no descending piece");
				affectedMovingBlocks.Add(m);
			}
		}
		foreach (DescendingPiece d in descendingPiecesToBreakApart)
		{
			foreach (MovingBlock m in d.allMovingBlocks)
			{
				//m.isPartOfDescendingPiece = false;
				//ApplyAffect(m);
				//print("adding here descending piece");
				affectedMovingBlocks.Add(m);
			}
			d.BreakApartDescendingPiece();
		}
		//foreach(MovingBlock m in affectedMovingBlocks)
		while (affectedMovingBlocks.Count > 0)
		{
			MovingBlock m = affectedMovingBlocks[0];
			ApplyAffect(m);
			affectedMovingBlocks.Remove(m);
		}
		//Destroy(gameObject, 3f);
	}
	void ApplyAffect(MovingBlock m)
	{
		if (m.hasSettledIntoPlace)
		{
			//print("has settled");
			
			m.currentEmptyBlock.Detach();
			//m.EnableBlock();
			m.settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity, new Counter(time)));
			m.AddMovementAffector(new MovementAffector(Vector2.up, speed, 0f, false, true, time, MovementAffectorType.momentumBased));
		}
		else if (!m.hasSettledIntoPlace)
		{
			//print("has not settled");
			//float speedToPush = 20f;
			Vector2 direction = ((Vector2)m.theTransform.position - (Vector2)transform.position);
			//print("direction is : " + direction.normalized);
			m.settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity, new Counter(time)));
			//m.AddMovementAffector(new MovementAffector(((Vector2)m.theTransform.position - (Vector2)transform.position).normalized, 20f, 0f, false, true, 3f, MovementAffectorType.momentumBased));
			MovementAffector temp = new MovementAffector(direction.normalized, speed, 0f, time, MovementAffectorType.momentumBased);
			m.AddMovementAffector(temp);
			//m.AddMovementAffector(new MovementAffector(direction.normalized,10f,0f,3f,MovementAffectorType.momentumBased));
			//m.AddMovementAffector(new MovementAffector(((Vector2)m.theTransform.position - (Vector2)transform.position).normalized,10f,0f,3f,MovementAffectorType.momentumBased));
			//m.EnableBlock(); 
			//ApplyAffect(m);
		}
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
