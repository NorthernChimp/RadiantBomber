using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherBulletScript : MonoBehaviour
{
    public Bullet thisBullet;
    public float maxDistance = 2.5f;
    public string projectileIdentifier = "PusherBullet";
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetDirection(Vector2 theDirect)
    {
        float widthScale = MainScript.blockWidth / 2f;
        float heightScale = MainScript.blockWidth / 2f;
        transform.localScale = MainScript.thePlayer.localScale;
        thisBullet = new Bullet(theDirect, 22.5f, transform);
        thisBullet.theType = TrajectoryType.straightLine;
        MainScript.currentGame.allBullets.Add(thisBullet);
        thisBullet.collider = GetComponent<CircleCollider2D>();
        thisBullet.rbody = GetComponent<Rigidbody2D>();
        GetComponent<ProjectileScript>().thisBullet = thisBullet;
        //thisBullet.tra
        //thisBullet.direction = theDirect;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!thisBullet.hasImpacted)
        {
            if (collision.transform.tag == "MovingBlock")
            {
                thisBullet.hasImpacted = true;
                Explode();
                thisBullet.readyToDie = true;
            }
            else if (collision.transform.tag == "Wall")
            {
                thisBullet.hasImpacted = true;
                //Vector3 directionFromWallToBullet = transform.position - collision.transform.position;
                //MainScript.CreateShotgunBlastAt(transform.position, directionFromWallToBullet.normalized, 3f, 0.2f);

                /*if (collision.transform.GetComponent<EmptyBlockScript>())
                {
                    //collision.transform.SendMessage("ChangeHealth", -1);
                    //thisBullet.readyToDie = true;
                }*/
                Explode();
                thisBullet.readyToDie = true;
            }
            else if (collision.transform.tag != "Player" && collision.transform.tag != "Projectile" && collision.transform.tag != "MainMenu")
            {
                print(collision.transform.tag);
                thisBullet.readyToDie = true;
            }

        }
    }
    public void Explode()
    {
        MainScript.CreateShotgunBlastAt(transform.position, thisBullet.direction.normalized * -1f, 3f, 0.2f);
        List<MovingBlock> blocksWithinRange = MainScript.GetAllBlocksFromPointWithinDistance(transform.position, maxDistance * MainScript.blockHeight);
        foreach(MovingBlock m in blocksWithinRange)
        {
            if (m.isActive)
            {
                //Vector3 directionFromBlockToBullet = transform.position - collision.transform.position;
                
                if (!m.DoesThisBlockHaveAnAffectorWithThisIdentifier(projectileIdentifier))
                {
                    m.AddMovementAffector(new MovementAffector(thisBullet.direction.normalized, 4.5f, 0f, 1f, MovementAffectorType.momentumBased, projectileIdentifier));
                }
                //CollideWithMovingBlock(m);
            }
        }
    }
    public void CollideWithMovingBlock(MovingBlock m)
    {
        //MovingBlock tempBlock = m;
        thisBullet.readyToDie = true;

        //print(m.isPartOfDescendingPiece.ToString() +  m.isFixedJointHolder.ToString()); ;
        if (!m.DoesThisBlockHaveAnAffectorWithThisIdentifier(projectileIdentifier))
        {
            
            m.AddMovementAffector(new MovementAffector(thisBullet.direction.normalized, 4.5f, 0f, 1f, MovementAffectorType.momentumBased, projectileIdentifier));
        }
        /*if (m.isPartOfDescendingPiece)
		{
			tempBlock = m.theDescendingPiece.fixedJointHolder;
		}*/

        //else { print("will not add because it has already"); }
        thisBullet.Impact();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
