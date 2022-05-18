using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackGrenadeScript : MonoBehaviour
{
    public UtilityAbilityInstance thisAbilityInstance;
    public float maxDistance = 1.5f;
    public float time = 3f;
	public float speed = 2.25f;
	public float decay = 0.25f;
	public float accelerationDownward = 6f;
	public float theFireDirection;
    List<MovementAffector> affecters;
    Rigidbody2D rbody;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "MovingBlock" || collision.transform.tag == "Wall")
        {
			if (thisAbilityInstance.readyToDie == false) { ApplyKnockBack(); }
			MainScript.CreateExplosionAt((Vector2)transform.position,Color.gray,MainScript.blockHeight * 2f,1f);
			
			thisAbilityInstance.readyToDie = true;
        }
    }
        public void SetDirection(Vector2 fireDirection)
    {
        affecters = new List<MovementAffector>();
        affecters.Add(new MovementAffector(fireDirection.normalized, 8f, 4.5f, 0f, MovementAffectorType.momentumBased));
        affecters.Add(new MovementAffector(Vector2.down, 0f, accelerationDownward, false,false,0f, MovementAffectorType.arbitrary,true));
		/*
		theFireDirection = fireDirection;
		thisAbilityInstance = new UtilityAbilityInstance(transform,false,true,new Counter(time));
		MainScript.allUtilityAbilities.Add(thisAbilityInstance);
		*/
    }
    public void UseUtility()
    {
        rbody = transform.GetComponent<Rigidbody2D>();
        thisAbilityInstance = new UtilityAbilityInstance(transform, false, false, new Counter(time * 3f));
        MainScript.utilityAbilities.Add(thisAbilityInstance);
		thisAbilityInstance.takesDirection = false;
        //deathCounter = new Counter(time);
        GetComponent<UtilityAbilityScript>().thisAbilityInstance = thisAbilityInstance;
        //ApplyKnockBack();
    }
    public void UpdateUtility()
    {
        Vector2 moveDirect = Vector2.zero;
        moveDirect = PhysicsObjectScript.AddUpAllMovements(affecters, Time.fixedDeltaTime, true);
        rbody.MovePosition(transform.position + (Vector3)moveDirect);
		transform.rotation = transform.rotation * Quaternion.Euler(0f, 0f, 60f * Time.fixedDeltaTime * Mathf.Sign(moveDirect.x));
		/*
		rbody.MovePosition(transform.position + ((Vector3)fireDirection * speed));
		*/
    }
	public void ApplyKnockBack()
	{
		//print(" applying knockback at " + Time.time.ToString());
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
		Vector2 direction = ((Vector2)m.theTransform.position - (Vector2)transform.position);
		if (m.hasSettledIntoPlace)
		{
			//print("has settled");
			m.currentEmptyBlock.Detach();
			m.settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity, new Counter(time)));
			m.AddMovementAffector(new MovementAffector(direction.normalized, speed, decay, true, true, time, MovementAffectorType.momentumBased));
			
			//m.EnableBlock();
		}
		else if (!m.hasSettledIntoPlace)
		{
			//print("has not settled");
			//float speedToPush = 20f;
			
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
