using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlunderbussScript : MonoBehaviour
{
    List<MovementAffector> affecters;
    UtilityAbilityInstance thisAbilityInstance;
    Vector2 fireDirection;
    Rigidbody2D rbody;
    public float range = 3f;
    public float timeSinceShot = 0f;
    public float speedAcceleration = 3f;
    public float initialSpeed = 1f;
    public float time = 2f;
    public float blowbackSpeed = 6f;
    public float decaySpeed = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "MovingBlock" || collision.transform.tag == "Wall")
        {
            //ApplyKnockBack();
            if (thisAbilityInstance.readyToDie == false) { ApplyBlowBack(); }
            
            //MainScript.CreateExplosionAt((Vector2)transform.position, Color.gray);
            thisAbilityInstance.readyToDie = true;
        }
    }
    public void SetDirection(Vector2 theFireDirection)
    {
        affecters = new List<MovementAffector>();
        fireDirection = theFireDirection;
        Vector2 perpendicularDirection = Quaternion.Euler(0f, 0f, 90f * Mathf.Sign(theFireDirection.x)) * theFireDirection;
        transform.rotation = Quaternion.LookRotation(Vector3.forward,perpendicularDirection);
        //affecters.Add(new MovementAffector(fireDirection.normalized, 8f, 4.5f, 0f, MovementAffectorType.momentumBased));
        //affecters.Add(new MovementAffector(Vector2.down, 1f, 3.75f, false, false, 0f, MovementAffectorType.arbitrary, true));
    }
    void ApplyBlowBack()
    {
        MainScript.CreateExplosionAt(transform.position, Color.black, MainScript.blockHeight * 5f, 0.35f);
        List<MovingBlock> blocksWithinrange = MainScript.GetAllBlocksFromPointWithinDistance(transform.position, range);
        foreach (MovingBlock m in blocksWithinrange)
        {
            if (m.hasSettledIntoPlace)
            {
                m.currentEmptyBlock.Detach();
            }
            if (m.isPartOfDescendingPiece) { m.theDescendingPiece.BreakApartDescendingPiece(); }
            m.SuspendGravity(time);
            m.AddMovementAffector(new MovementAffector(fireDirection.normalized, blowbackSpeed, decaySpeed, true, true, time, MovementAffectorType.arbitrary, false));
        }
    }
    public void UseUtility()
    {
        rbody = transform.GetComponent<Rigidbody2D>();
        thisAbilityInstance = new UtilityAbilityInstance(transform, false, false, new Counter(time * 3f),false);
        MainScript.utilityAbilities.Add(thisAbilityInstance);
        thisAbilityInstance.takesDirection = false;
        //deathCounter = new Counter(time);
        GetComponent<UtilityAbilityScript>().thisAbilityInstance = thisAbilityInstance;
        //ApplyKnockBack();
    }
    public void UpdateUtility()
    {
        timeSinceShot += Time.fixedDeltaTime;
        float totalSpeed = (initialSpeed + (speedAcceleration * timeSinceShot) * MainScript.blockWidth);
        Vector2 moveDirect = fireDirection.normalized * totalSpeed * Time.fixedDeltaTime;
        //moveDirect = PhysicsObjectScript.AddUpAllMovements(affecters, Time.fixedDeltaTime, true);
        //Vector2 moveDirect = fireDirection * (timeSinceShot * )
        rbody.MovePosition(transform.position + (Vector3)(moveDirect));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
