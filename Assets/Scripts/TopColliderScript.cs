using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopColliderScript : MonoBehaviour
{
    public static MovementAffector pushDownAffector;
    public static List<MovingBlock> allMovingBlocksInside;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public static bool IsColliderEmpty()
    {
        if (allMovingBlocksInside.Count > 0) { return true; }else { return false; }
    }
    public static void AddMovingBlock(MovingBlock m)
    {
        if (!allMovingBlocksInside.Contains(m)) { allMovingBlocksInside.Add(m); }
        m.isInTopCollider = true;
    }
    public static void SetUpAffector()
    {
        allMovingBlocksInside = new List<MovingBlock>();
        //pushDownAffector = new MovementAffector(Vector2.down, 0f, -2f, 0f, MovementAffectorType.arbitrary,"TopCollider");
        //ushDownAffector = new MovementAffector(Vector2.down, 5f, 0f, false, false, 100f, MovementAffectorType.arbitrary, "TopCollider");
        pushDownAffector = new MovementAffector(Vector2.down, 1.5f, 0f, 0f, MovementAffectorType.arbitrary, "TopCollider");
    }
    static MovementAffector GetNewPushDownAffector()
    {
        return new MovementAffector(pushDownAffector.directionOfAffector, pushDownAffector.speed, pushDownAffector.decayAmount, pushDownAffector.endTime,pushDownAffector.theType, pushDownAffector.identifierName);
    }
    static MovementAffector GetNewPushDownAffectorForPlayer()
    {
        return new MovementAffector(pushDownAffector.directionOfAffector, pushDownAffector.speed * 30f, pushDownAffector.decayAmount, pushDownAffector.endTime, pushDownAffector.theType, pushDownAffector.identifierName);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "MovingBlock")
        {
            MovingBlock temp = collision.transform.GetComponent<MovingBlockScript>().thisBlock;
            if (!allMovingBlocksInside.Contains(temp)) 
            { 
                if (temp.isPartOfDescendingPiece)
                {
                    //temp = temp.theDescendingPiece.fixedJointHolder; 
                    if (!temp.theDescendingPiece.AreWeInTopCollider())
                    {
                        if (!temp.DoesThisBlockHaveAnAffectorWithThisIdentifier("TopCollider")) { temp.AddMovementAffector(GetNewPushDownAffector()); }
                    }
                    AddMovingBlock(temp);
                }
                else
                {
                    AddMovingBlock(temp);
                    if (!temp.DoesThisBlockHaveAnAffectorWithThisIdentifier("TopCollider")) { temp.AddMovementAffector(GetNewPushDownAffector()); }
                }
            }
            //if (!temp.DoesThisBlockHaveThisMovementAffector(pushDownAffector)) { temp.AddMovementAffector(pushDownAffector); }
            temp.BounceOff(Vector2.down);
            
        }else if(collision.tag == "Player")
        {
            PlayerScript tempScript = MainScript.thePlayer.GetComponent<PlayerScript>();
            tempScript.BounceOff(Vector2.down, Vector2.zero);
            tempScript.affecters.Add(GetNewPushDownAffectorForPlayer());
        }else if(collision.tag == "Projectile")
        {
            //collision.transform.GetComponent<ProjectileScript>().thisBullet.readyToDie = true ;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.tag == "MovingBlock")
        {
            MovingBlock temp = collision.transform.GetComponent<MovingBlockScript>().thisBlock;
            if (allMovingBlocksInside.Contains(temp))
            {
                allMovingBlocksInside.Remove(temp);
                temp.isInTopCollider = false;
                if (temp.isPartOfDescendingPiece)
                {
                    if (!temp.theDescendingPiece.AreWeInTopCollider())
                    {
                        //if (temp.DoesThisBlockHaveAnAffectorWithThisIdentifier("TopCollider")) { temp.affecters.Remove(temp.GetAffectorWithIdentifier("TopCollider")); }
                    }
                }
                else
                {
                    //if (temp.DoesThisBlockHaveAnAffectorWithThisIdentifier("TopCollider")) { temp.affecters.Remove(temp.GetAffectorWithIdentifier("TopCollider")); }
                }
                
            }
            
        }
        else if (collision.tag == "Player")
        {
            PlayerScript tempScript = MainScript.thePlayer.GetComponent<PlayerScript>();
            tempScript.affecters.Remove(tempScript.GetAffectorWithId("TopCollider"));
        }
    }
    /*
     * 
     */
    // Update is called once per frame
    void Update()
    {
        
    }
}
