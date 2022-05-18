using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroMissileScript : MonoBehaviour
{
    public Bullet thisBullet;
    public float range = 0.75f;
    public float time = 0.5f;
    public float directionVariance = 0.25f;
    //public Vector2 fireDirection;
    public float pushBackSpeed = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetDirection(Vector2 theDirect)
    {
        Vector2 perpendicularDirection = Quaternion.Euler(0f, 0f, 90f) * theDirect;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector3)theDirect);
        Vector2 randomRecoil = ((Random.Range(0f, 1f) - 0.5f)* directionVariance * perpendicularDirection * MainScript.blockHeight);
        transform.localScale = MainScript.thePlayer.localScale;
        thisBullet = new Bullet((theDirect + randomRecoil).normalized, 2.5f, transform);
        thisBullet.theType = TrajectoryType.straightLine;
        thisBullet.b = 5f;
        MainScript.currentGame.allBullets.Add(thisBullet);
        thisBullet.collider = GetComponent<CircleCollider2D>();
        thisBullet.rbody = GetComponent<Rigidbody2D>();
        GetComponent<ProjectileScript>().thisBullet = thisBullet;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "MovingBlock" || collision.transform.tag == "Wall")
        {
            //CollideWithMovingBlock(collision.transform.GetComponent<MovingBlockScript>().thisBlock);
            if (!thisBullet.readyToDie) { Explode(); }
        }
    }
    public void Explode()
    {
        MainScript.CreateExplosionAt(transform.position, Color.green,MainScript.blockHeight * 4f,0.35f);
        thisBullet.readyToDie = true;
        List<MovingBlock> blocksWithinrange = MainScript.GetAllBlocksFromPointWithinDistance(transform.position, range);
        foreach (MovingBlock m in blocksWithinrange)
        {
            if (m.hasSettledIntoPlace)
            {
                m.currentEmptyBlock.Detach();
            }
            else
            {
                //if (m.isPartOfDescendingPiece) { m.theDescendingPiece.BreakApartDescendingPiece(); }
            }
            Vector2 awayFromExplosion = (Vector2)(m.theTransform.position - transform.position);
            //if (m.isPartOfDescendingPiece) { awayFromExplosion }
            
            if (!m.DoesThisBlockHaveAnAffectorWithThisIdentifier("MicroMissiles")) { m.AddMovementAffector(new MovementAffector(awayFromExplosion.normalized, pushBackSpeed, 0f, false, true, time, MovementAffectorType.momentumBased, "MicroMissiles")); m.SuspendGravity(time); };
            
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
