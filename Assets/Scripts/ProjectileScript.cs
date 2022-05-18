using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Bullet thisBullet;
	
    void Start()
    {
        
    }
    
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "MovingBlock")
        {
            //CollideWithMovingBlock(collision.transform.GetComponent<MovingBlockScript>().thisBlock);
        }
        if (collision.transform.tag == "MovingBlock")
        {
            CollideWithMovingBlock(collision.transform.GetComponent<MovingBlockScript>().thisBlock);

        }
        else if (collision.transform.tag == "Wall")
        {
            if (collision.transform.GetComponent<EmptyBlockScript>())
            {
                collision.transform.SendMessage("ChangeHealth", -1);
                thisBullet.readyToDie = true;
            }
            //thisBullet.readyToDie = true;
        }
        else if (collision.transform.tag != "Player" || collision.transform.tag != "Projectile")
        {
            //thisBullet.readyToDie = true;
        }
    }*/
    
    void Impact()
    {
        //what happens when you impact something as a bullet
        //Destroy(gameObject);
    }
    public void DestroyThisBullet()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class Bullet
{
    public bool hasImpacted;
    public TrajectoryType theType;
    public Vector2 direction;
    public float a;
    public float b;
    public float c;
    public float d;
    public float e;
    public float timeSinceSpawned = 0f;
    public bool isSpinning = false;
    public Vector2 theOriginPoint;
    public Transform theTransform;
    public bool readyToDie = false;
    public Rigidbody2D rbody;
    public CircleCollider2D collider;
    public Bullet(Vector2 theDirect, float speed, Transform bulletTransform)
    {
        direction = theDirect;
        a = speed * MainScript.blockHeight;
        theTransform = bulletTransform;
        collider = theTransform.GetComponent<CircleCollider2D>();
        rbody = theTransform.GetComponent<Rigidbody2D>();
    }
    public void Trajectory(Vector2 origin, Vector2 theDirection, TrajectoryType type, float theA, float theB, float theC, float theD, float theE, Transform thisBulletTransform)
    {
        theTransform = thisBulletTransform;
        timeSinceSpawned = 0f;
        theOriginPoint = origin;
        direction = theDirection;
        theType = type;
        a = theA;
        b = theB;
        c = theC;
        d = theD;
        e = theE;
    }
    public void DestroyBullet()
    {
        MainScript.currentGame.allBullets.Remove(this);
        theTransform.SendMessage("DestroyThisBullet");
    }
    public void Impact()
    {
        readyToDie = true;

        //any code for hitting someone with a buullet goes here
        //MainScript.currentGame.allBullets.Remove(thisBullet);
        //Destroy(gameObject);
    }
    public void UpdatePosition(float timePassed)
    {
        //theTransform.GetComponent<ProjectileScript>().UpdatePosition(timePassed);
        Vector2 completeMove = GetTrajectory(timePassed);
        //RaycastHit2D tempHit = Physics2D.CircleCast(theTransform.position, collider.radius, completeMove.normalized, completeMove.magnitude, LayerMask.GetMask("Wall", "MovingBlock"));
        Vector2 positionToMoveTo = (Vector2)theTransform.position + GetTrajectory(timePassed);
        //if (tempHit) { positionToMoveTo = tempHit.point + (tempHit.normal * collider.radius); Impact(); if (tempHit.transform.tag == "MovingBlock") { tempHit.transform.GetComponent<MovingBlockScript>().thisBlock.AddMovementAffector(new MovementAffector(direction, 5f, 2.5f, true, false, 0f, MovementAffectorType.momentumBased)); } }
        if (isSpinning) { theTransform.rotation = theTransform.rotation * Quaternion.Euler(0f, 0f, 75f * Time.fixedDeltaTime * Mathf.Sign(completeMove.x)); }
        rbody.MovePosition(positionToMoveTo);
    }
    public Vector2 GetTrajectory(float timePassed)
    {
        timeSinceSpawned += timePassed;
        switch (theType)
        {
            case TrajectoryType.straightLine:
                //a is the speed b is the acceleration/deceleration
                float decelerationAmount = (b * timeSinceSpawned);
                return direction.normalized * timePassed * (a + decelerationAmount * MainScript.blockHeight);
        }
        return Vector2.zero;
    }
}
public enum TrajectoryType
{
    straightLine, swerveLeftRight, circleOutward
}