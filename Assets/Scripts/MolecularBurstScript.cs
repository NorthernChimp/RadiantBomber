using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolecularBurstScript : MonoBehaviour
{
	public Vector2 directionOfBurst = Vector2.zero;
	public UtilityAbilityReference thisReference;
	public float time = 1.25f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
	public void SetDirection(Vector2 theDirect)
	{
		directionOfBurst = theDirect;
	}
	public void UseUtility()
	{
		MainScript.CreateConfetti(Color.green, time, 0.035f, 2f,0.5f);
		PlayerScript thePlayerScript = MainScript.thePlayer.GetComponent<PlayerScript>();
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.makeInvulnerable,new Counter(time * 1.125f)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.breakUpDescendingPieceOnCollision,new Counter(time)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.applyMovementAffectorOnCollision,new Counter(time)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.suspendGraityOnCollision, new Counter(time)));
		thePlayerScript.StopControls(time);
		thePlayerScript.SetCollisionMovementAffector(new MovementAffector(Vector2.zero,10f,0f,1f,MovementAffectorType.getOutOfPlayersWay));
		thePlayerScript.affecters.Add(new MovementAffector(directionOfBurst, 10.5f, 0f, time, MovementAffectorType.momentumBased));
		thePlayerScript.SetupFlashingPlayer(Color.green,time);

		//MainScript.utilityAbilities.Remove(thisReference);
		Destroy(gameObject);
		/*
		FOR SPEED BOOST
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.makeInvulnerable,new Counter(2.5f)));
		thePlayerScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.applyMovementAffectorOnCollision,new Counter(2.5f)));
		thePlayerScript.SetCollisionMovementAffector(new MovementAffector(Vector2.zero,6f,0f,1f,MovementAffectorType.bounceOffPlayer));
		thePlayerScript.AddControlAffector(new ControlsSettingsAffector(ControlsSettingsAffectorType.changeAll,25f,8,0f,2.5f));
		*/
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
