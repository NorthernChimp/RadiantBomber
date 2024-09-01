using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFieldScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 3f;
    public float range = 2.5f;
    public float decayAmount = 0f;
    public float endTime = 2f;

    public float maxSpeedOfPlayer = 11f;
    public float acceleration = 11f;
    public float deceleration = 11f;
    public Counter deathCounter;
    public UtilityAbilityInstance thisAbilityInstance;
    public List<MovingBlock> affectedMovingBlocks;
    public MovingBlockSettingsAffector affectorToApply;
    public CircleCollider2D collider;
    void Start()
    {
        
    }
    public void UseUtility()
    {
        affectorToApply = new MovingBlockSettingsAffector(MovingBlockSettingsAffectorType.suspendGravity, new Counter(Mathf.Infinity));
        collider = transform.GetComponent<CircleCollider2D>();
        //collider.radius = range * MainScript.blockHeight *0.01f;
        transform.localScale = new Vector3(MainScript.blockLocalScale * range * 2f, MainScript.blockLocalScale * range * 2f, 1f);
        //transform.localScale = new Vector3((MainScript.blockWidth/0.16f) * range * 1f, (MainScript.blockWidth / 0.16f) * range * 1f , 1f);
        transform.parent = MainScript.thePlayer;
        //transform.localScale = new Vector3(2f, 2f, 2f);
        PlayerScript tempScript = MainScript.thePlayer.GetComponent<PlayerScript>();
        tempScript.AddControlAffector(new ControlsSettingsAffector(ControlsSettingsAffectorType.changeAll, maxSpeedOfPlayer, acceleration, deceleration, endTime));
        tempScript.AddSettingAffector(new PlayerSettingsAffector(PlayerSettingsAffectorType.makeInvulnerable, new Counter(endTime * 1.125f)));
        affectedMovingBlocks = new List<MovingBlock>();
        deathCounter = new Counter(endTime);
        thisAbilityInstance = new UtilityAbilityInstance(transform, false, false, deathCounter,false);
        GetComponent<UtilityAbilityScript>().thisAbilityInstance = thisAbilityInstance;
        MainScript.utilityAbilities.Add(thisAbilityInstance);
    }
    public void UpdateUtility()
    {
        deathCounter.AddTime(Time.fixedDeltaTime);
        if (deathCounter.hasFinished)
        {
            thisAbilityInstance.readyToDie = true;
            EmptyAffectedMovingBlocks();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MovingBlock")
        {
            MovingBlock tempBlock = collision.transform.GetComponent<MovingBlockScript>().thisBlock;
            if (tempBlock.isPartOfDescendingPiece) 
            {
                //tempBlock = tempBlock.theDescendingPiece.fixedJointHolder; 
                tempBlock.theDescendingPiece.BreakApartDescendingPiece();
            }
            if (!tempBlock.settingsAffectors.Contains(affectorToApply)) { tempBlock.settingsAffectors.Add(affectorToApply); }
            if (!tempBlock.DoesThisBlockHaveAnAffectorWithThisIdentifier("moveTowardsPlayer")){ tempBlock.AddMovementAffector(MainScript.GetMovementAffectorTowardsPlayer(tempBlock, speed, decayAmount, endTime)); affectedMovingBlocks.Add(tempBlock); }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "MovingBlock")
        {
            MovingBlock tempBlock = collision.transform.GetComponent<MovingBlockScript>().thisBlock;
            if (tempBlock.isPartOfDescendingPiece) { tempBlock = tempBlock.theDescendingPiece.fixedJointHolder; }
            if (tempBlock.settingsAffectors.Contains(affectorToApply)) { tempBlock.settingsAffectors.Remove(affectorToApply); }
            if (tempBlock.DoesThisBlockHaveAnAffectorWithThisIdentifier("moveTowardsPlayer")) { tempBlock.affecters.Remove(tempBlock.GetAffectorWithIdentifier("moveTowardsPlayer")); if (affectedMovingBlocks.Contains(tempBlock)) { affectedMovingBlocks.Remove(tempBlock); } }
        }
    }
    public void EmptyAffectedMovingBlocks()
    {
        foreach(MovingBlock m in affectedMovingBlocks)
        {
            //MovingBlock tempBlock = collision.transform.GetComponent<MovingBlockScript>().thisBlock;
            MovingBlock tempBlock = m;
            if (tempBlock.isPartOfDescendingPiece) { tempBlock = tempBlock.theDescendingPiece.fixedJointHolder; }
            if (tempBlock.DoesThisBlockHaveAnAffectorWithThisIdentifier("moveTowardsPlayer")) { tempBlock.affecters.Remove(tempBlock.GetAffectorWithIdentifier("moveTowardsPlayer"));  }
            if (tempBlock.settingsAffectors.Contains(affectorToApply)) { tempBlock.settingsAffectors.Remove(affectorToApply); }
        }
    }
            // Update is called once per frame
            void Update()
    {
        
    }
}
