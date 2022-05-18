using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunScript : MonoBehaviour
{
    public float range = 3f;
    public float minimumDotResult = 0.7f;
    public float maximumDistance = 4f;
    public float minimumDistance = 2f;
    public float pushBackSpeedMax = 6f;
    public float pushBackSpeedMin = 2f;
    public Vector2 fireDirection;
    public Bullet thisBullet;
    public float time = 1.25f;
    public Counter deathCounter;
    public GameObject basicWhiteBlock;
    public List<Transform> flakToPushOutward;
    public float flakSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetDirection(Vector2 theDirect)
    {
        //MainScript.CreateExplosionAt(transform.position, Color.red);
        MainScript.CreateShotgunBlastAt(transform.position, theDirect,4f,1.25f);
        for(int i = 0; i < 5; i++)
        {
            //Vector2 flakDirection = theDirect.normalized + (perpendicularDirection.normalized * (totalSpred/-2f + ((float)i + 1/5f * totalSpred)))
            //flakToPushOutward.Add(Instantiate(basicWhiteBlock, transform.position, Quaternion.identity).transform);
        }
        //Vector2 perpendicularDirection = Quaternion.Euler(0f, 0f, 90f) * theDirect;
        //Vector2 randomRecoil = ((Random.Range(0f, 1f) - 0.5f) * directionVariance * perpendicularDirection * MainScript.blockHeight);
        transform.localScale = MainScript.thePlayer.localScale;
        thisBullet = new Bullet((theDirect).normalized, 0f, transform);
        List < MovingBlock> tempList =  MainScript.GetAllBlocksFromPointWithinDistance(transform.position,maximumDistance * MainScript.blockHeight);
        List<DescendingPiece> relevantPieces = new List<DescendingPiece>();
        foreach(MovingBlock m in tempList)
        {
            Vector2 directToBlock = (Vector2)(m.theTransform.position - transform.position);
            float dotResult = Vector2.Dot(directToBlock, theDirect);
            if(dotResult >= minimumDotResult)
            {
                if (m.isPartOfDescendingPiece)
                {
                    if (!relevantPieces.Contains(m.theDescendingPiece)) { relevantPieces.Add(m.theDescendingPiece); }
                }
                else
                {
                    //float distanceFromShot = Vector2.Distance(transform.position, m.theTransform.position);
                    //float speedToPush = 
                    if (!m.DoesThisBlockHaveAnAffectorWithThisIdentifier("ShotgunBlast")) { m.AddMovementAffector(new MovementAffector(directToBlock.normalized, pushBackSpeedMax, 0f, false, true, time, MovementAffectorType.arbitrary, "ShotgunBlast")); }
                    
                }
            }
            foreach(DescendingPiece d in relevantPieces)
            {
                Vector3 averagePosition = Vector3.zero;
                foreach(MovingBlock mov in d.allMovingBlocks)
                {
                    averagePosition += mov.theTransform.position;
                }
                averagePosition = averagePosition/(float)d.allMovingBlocks.Count;
                Vector2 directToDescendingPiece = averagePosition - transform.position;
                if (!d.fixedJointHolder.DoesThisBlockHaveAnAffectorWithThisIdentifier("ShotgunBlast")) { d.fixedJointHolder.AddMovementAffector(new MovementAffector(directToDescendingPiece.normalized, pushBackSpeedMax, 0f, false, true, time, MovementAffectorType.arbitrary, "ShotgunBlast")); }
                
            }
        }
        //Destroy(gameObject);
        //thisBullet.theType = TrajectoryType.straightLine;
        //thisBullet.b = 5f;
        //MainScript.currentGame.allBullets.Add(thisBullet);
        //thisBullet.collider = GetComponent<CircleCollider2D>();
        //thisBullet.rbody = GetComponent<Rigidbody2D>();
        //GetComponent<ProjectileScript>().thisBullet = thisBullet;
    }
    public void UpdateUtility()
    {
        /*Vector2 perpendicularDirection = Quaternion.Euler(0f, 0f, 90f) * thisBullet.theDirect;
        float totalSpred = 2f;
        deathCounter = new Counter(time);
        for(int i = 0;i < flakToPushOutward.Count; i++)
        {
            Vector2 flakDirection = theDirect.normalized + (perpendicularDirection.normalized * (totalSpred / -2f + (((float)(i + 1) / (float)flakToPushOutward.Count) * totalSpred)));
            flakToPushOutward[i].transform.Translate(flakDirection.normalized * Time.fixedDeltaTime * MainScript.blockWidth * flakSpeed)
        }*/
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
