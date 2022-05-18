using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostScript : MonoBehaviour
{
	// Start is called before the first frame update
	public float maxSpeed = 15f;
	public float acceleration = 10f;
	public float deceleration = 0f;
	public float movingBlockBounceOffSpeed = 4.5f;
	public float time = 4.5f;
    void Start()
    {
        
    }
	public void UseUtility()
	{
		PlayerScript thePlayerScript = MainScript.thePlayer.GetComponent<PlayerScript>();
		/*thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.makeInvulnerable, new Counter(time)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.breakUpDescendingPieceOnCollision, new Counter(time)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.applyMovementAffectorOnCollision, new Counter(time)));
		thePlayerScript.StopControls(time);
		thePlayerScript.SetCollisionMovementAffector(new MovementAffector(Vector2.zero, 8.5f, 0f, 1f, MovementAffectorType.getOutOfPlayersWay));
		thePlayerScript.affecters.Add(new MovementAffector(directionOfBurst, 5f, 0f, time, MovementAffectorType.momentumBased));*/
		MainScript.CreateConfetti(Color.green, time, 0.05f, 1f,0.6f);
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.makeInvulnerable, new Counter(time)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.applyMovementAffectorOnCollision, new Counter(time)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.breakUpDescendingPieceOnCollision, new Counter(time)));
		thePlayerScript.SetCollisionMovementAffector(new MovementAffector(Vector2.zero, movingBlockBounceOffSpeed, 0f, 1f, MovementAffectorType.bounceOffPlayer));
		thePlayerScript.AddControlAffector(new ControlsSettingsAffector(ControlsSettingsAffectorType.changeAll, maxSpeed, acceleration, deceleration, time));
		thePlayerScript.SetupFlashingPlayer(Color.green,time);
		//MainScript.utilityAbilities.Remove(thisReference);
		Destroy(gameObject);
		/*
		FOR SPEED BOOST
		
		*/
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
