using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotJumpScript : MonoBehaviour
{
	// Start is called before the first frame update
	public UtilityAbilityInstance thisAbilityInstance;
	public float maxDistance = 3f;
	public float maxSpeed = 20f;
	public float acceleration = 15f;
	public float deceleration = 10f;
	public float movingBlockBounceOffSpeed = 10.5f;
	public float time = 3f;
	public float speed = 4.25f;
	public float speedDecay = 0.25f;
	public float firstSegmentFraction = 0.15f;
	public Counter KnockbackCounter;
	public float firstHalfSpeed = 2.25f;
	public Vector2 directionOfJump;
	void Start()
	{

	}
	public void SetDirection(Vector2 theDirect)
	{
		directionOfJump = theDirect;
	}
	public void UseUtility()
	{
		transform.parent = MainScript.thePlayer;
		thisAbilityInstance = new UtilityAbilityInstance(transform, false, false, new Counter(time),false);
		GetComponent<UtilityAbilityScript>().thisAbilityInstance = thisAbilityInstance;
		PlayerScript thePlayerScript = MainScript.thePlayer.GetComponent<PlayerScript>();
		//thePlayerScript.playerFlashingColorCounter = new Counter()
		thePlayerScript.SetupFlashingPlayer(Color.green,time * 0.85f);
		/*thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.makeInvulnerable, new Counter(time)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.breakUpDescendingPieceOnCollision, new Counter(time)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.applyMovementAffectorOnCollision, new Counter(time)));
		
		thePlayerScript.SetCollisionMovementAffector(new MovementAffector(Vector2.zero, 8.5f, 0f, 1f, MovementAffectorType.getOutOfPlayersWay));
		thePlayerScript.affecters.Add(new MovementAffector(directionOfBurst, 5f, 0f, time, MovementAffectorType.momentumBased));*/

		//thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.makeInvulnerable, new Counter(time)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.applyMovementAffectorOnCollision, new Counter(time * firstSegmentFraction)));
		thePlayerScript.SetCollisionMovementAffector(new MovementAffector(Vector2.zero, movingBlockBounceOffSpeed, 0f, 1f, MovementAffectorType.bounceOffPlayer));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.suspendGraityOnCollision, new Counter(time * firstSegmentFraction)));
		thePlayerScript.StopControls(time * firstSegmentFraction);
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.makeInvulnerable, new Counter(time * 0.95f)));
		thePlayerScript.affecters.Add(new MovementAffector(directionOfJump, 3f, 0f, time * firstSegmentFraction, MovementAffectorType.momentumBased));
		thePlayerScript.affecters.Add(new MovementAffector(directionOfJump, speed, speedDecay, time, MovementAffectorType.momentumBased));
		//thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.breakUpDescendingPieceOnCollision, new Counter(time)));
		thePlayerScript.SetCollisionMovementAffector(new MovementAffector(Vector2.zero, movingBlockBounceOffSpeed, 0f, 1f, MovementAffectorType.bounceOffPlayer));
		//thePlayerScript.AddControlAffector(new ControlsSettingsAffector(ControlsSettingsAffectorType.changeMaxSpeed, maxSpeed,0f,0f,time));
		//thePlayerScript.AddControlAffector(new ControlsSettingsAffector(ControlsSettingsAffectorType.changeAcceleration, 5f, 0f, 0f, time));
		//MainScript.utilityAbilities.Remove(thisReference);
		//Destroy(gameObject);
		MainScript.utilityAbilities.Add(thisAbilityInstance);
		/*
		FOR SPEED BOOST
		
		*/
	}
	public void UpdateUtility()
    {
		thisAbilityInstance.deathTimer.AddTime(Time.fixedDeltaTime);
		if(thisAbilityInstance.deathTimer.currentTime >= time * firstSegmentFraction) { thisAbilityInstance.readyToDie = true;ApplyKnockBack();MainScript.CreateExplosionAt(transform.position, Color.black,MainScript.blockHeight * 1.25f,1f); }
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
		Vector2 velocity = PhysicsObjectScript.AddUpAllMovements(MainScript.thePlayer.GetComponent<PlayerScript>().affecters, Time.fixedDeltaTime, false);
		while (affectedMovingBlocks.Count > 0)
		{
			MovingBlock m = affectedMovingBlocks[0];
			ApplyAffect(m,velocity);
			affectedMovingBlocks.Remove(m);
		}
		//Destroy(gameObject, 3f);
	}
	void ApplyAffect(MovingBlock m,Vector2 velocity)
	{
		//Vector2 velocity = PhysicsObjectScript.AddUpAllMovements(MainScript.thePlayer.GetComponent<PlayerScript>().affecters, Time.fixedDeltaTime, false);
		//Vector2 direction = ((Vector2)m.theTransform.position - (Vector2)transform.position);
		if (m.hasSettledIntoPlace)
		{
			//print("has settled");

			m.currentEmptyBlock.Detach();
			
			//m.settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity, new Counter(time)));
			m.SuspendGravity(time);
			m.AddMovementAffector(new MovementAffector(velocity.normalized, speed, 0f, false, true, time, MovementAffectorType.momentumBased));
			
			//m.EnableBlock();
		}
		else if (!m.hasSettledIntoPlace)
		{
			//print("has not settled");
			//float speedToPush = 20f;
			m.SuspendGravity(time);
			//print("direction is : " + direction.normalized);
			//m.settingsAffectors.Add(new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity, new Counter(time)));
			//m.AddMovementAffector(new MovementAffector(((Vector2)m.theTransform.position - (Vector2)transform.position).normalized, 20f, 0f, false, true, 3f, MovementAffectorType.momentumBased));
			MovementAffector temp = new MovementAffector(velocity.normalized, speed, 0f, time, MovementAffectorType.momentumBased);
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
