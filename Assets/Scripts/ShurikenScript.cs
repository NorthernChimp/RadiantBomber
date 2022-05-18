using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenScript : MonoBehaviour
{
    public Bullet thisBullet;
    public string projectileIdentifier = "Shuriken";
    int totalHealth = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetDirection(Vector2 theDirect)
    {
        float widthScale = MainScript.blockWidth / 2f;
        float heightScale = MainScript.blockWidth / 2f;
        transform.localScale = MainScript.thePlayer.localScale;
        thisBullet = new Bullet(theDirect, 15f, transform);
        thisBullet.isSpinning = true;
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
        if (collision.transform.tag == "MovingBlock")
        {
            CollideWithMovingBlock(collision.transform.GetComponent<MovingBlockScript>().thisBlock);
        }
        else if (collision.transform.tag == "Wall")
        {
            if (collision.transform.GetComponent<EmptyBlockScript>())
            {
                //collision.transform.SendMessage("ChangeHealth", -1);
                //thisBullet.readyToDie = true;
            }
            Vector2 directionToShuriken = transform.position - collision.transform.position;
            //collision.
            //if(directionToShuriken.x * (Mathf.Sign(directionToShuriken.x)) > directionToShuriken.y * (Mathf.Sign(directionToShuriken.y))) { directionToShuriken = new Vector2(Mathf.Sign(directionToShuriken.x), 0f); } else { directionToShuriken = new Vector2(0f,Mathf.Sign(directionToShuriken.y)); }
            thisBullet.direction = Vector2.Reflect(thisBullet.direction.normalized, directionToShuriken.normalized);
            //thisBullet.readyToDie = true;
            totalHealth--;
            if (totalHealth <= 0) { thisBullet.readyToDie = true; }
        }
        else if (collision.transform.tag != "Player" && collision.transform.tag != "Projectile" && collision.transform.tag != "MainMenu")
        {
            //print(collision.transform.tag);
            //thisBullet.readyToDie = true;
        }

    }
    Vector2 GetClosestNormal(Vector2 rawNormal)
    {
        Vector2 reflectNormal = Vector2.zero;
        if(Mathf.Abs(rawNormal.x) > Mathf.Abs(rawNormal.y))
        {
            reflectNormal = new Vector2(Mathf.Sign(rawNormal.x), 0f);
        }
        else
        {
            reflectNormal = new Vector2(0f,Mathf.Sign(rawNormal.y));
        }
        return reflectNormal;
    }
    public void CollideWithMovingBlock(MovingBlock m)
    {
        //MovingBlock tempBlock = m;
        //thisBullet.readyToDie = true;

        //print(m.isPartOfDescendingPiece.ToString() +  m.isFixedJointHolder.ToString()); ;
        m.AddMovementAffector(new MovementAffector(thisBullet.direction.normalized, 4.5f, 0f, 1f, MovementAffectorType.momentumBased, projectileIdentifier));
        Vector2 directionToShuriken = transform.position - m.theTransform.position;
        thisBullet.direction = Vector2.Reflect(thisBullet.direction.normalized, GetClosestNormal(directionToShuriken.normalized));
        totalHealth--;
        if(totalHealth <= 0) { thisBullet.readyToDie = true; }
        if (!m.DoesThisBlockHaveAnAffectorWithThisIdentifier(projectileIdentifier))
        {
            
        }
        /*if (m.isPartOfDescendingPiece)
		{
			tempBlock = m.theDescendingPiece.fixedJointHolder;
		}*/

        //else { print("will not add because it has already"); }
        //thisBullet.Impact();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
