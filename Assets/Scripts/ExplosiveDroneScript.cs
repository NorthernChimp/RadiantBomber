using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveDroneScript : MonoBehaviour
{
    Rigidbody2D rbody;
    public Vector2 theFireDirection;
	public float timeBeforeDeath = 5f;
    public float speed = 0.15f;
	public float time = 2f;
	public float maxDistance = 3.5f;
    public UtilityAbilityInstance thisAbilityInstance;
	public TouchInterface directingInterface;
	public List<Transform> propellers;
	public Counter deathCounter;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "MovingBlock" || collision.transform.tag == "Wall")
        {
			if (!thisAbilityInstance.readyToDie) { ApplyKnockBack(); }
            MainScript.CreateExplosionAt((Vector2)transform.position, Color.gray,MainScript.blockHeight * 3f,1f);

            thisAbilityInstance.readyToDie = true;
        }
    }
    public void SetDirection(Vector2 fireDirection)
    {
		
		theFireDirection = fireDirection;
		
		//MainScript.allU.Add(thisAbilityInstance);
	}
    public void UpdateUtility()
    {
		deathCounter.AddTime(Time.fixedDeltaTime);
		if (deathCounter.hasFinished)
		{
			ApplyKnockBack();
			MainScript.CreateExplosionAt((Vector2)transform.position, Color.gray, MainScript.blockHeight * 3f, 1f);

			thisAbilityInstance.readyToDie = true;
        }
        else
        {
			float rotationSpeed = 180f;
			foreach (Transform t in propellers)
			{
				t.rotation = t.rotation * Quaternion.Euler(0f, 0f, rotationSpeed * Time.fixedDeltaTime);
			}
			//theFireDirection = directingInterface.currentDirection.normalized;
			rbody.MovePosition(transform.position + ((Vector3)theFireDirection * speed * Time.fixedDeltaTime));
		}
    }
	public void UseUtility()
    {
		rbody = GetComponent<Rigidbody2D>();
		thisAbilityInstance = new UtilityAbilityInstance(transform,false,true,new Counter(time),true);
		GetComponent<UtilityAbilityScript>().thisAbilityInstance = thisAbilityInstance;
		deathCounter = new Counter(timeBeforeDeath);
		MainScript.utilityAbilities.Add(thisAbilityInstance);
		directingInterface = MainScript.thePlayer.GetComponent<ControlsScript>().shootInterface;
		thisAbilityInstance.takesDirection = false;
	}
	public void ApplyKnockBack()
	{
		//print("we are starting" + MainScript.currentGame.allMovingBlocksInGame.Count);
		//float minDistance = 0.5f;
		//float speedToPush = 20f;
		Vector2 position = (Vector2)transform.position;
		List<MovingBlock> tempList = MainScript.GetAllBlocksFromPointWithinDistance(position, maxDistance * MainScript.blockHeight);
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
			m.AddMovementAffector(new MovementAffector(direction.normalized, speed, 0f, false, true, time, MovementAffectorType.momentumBased));
			
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
