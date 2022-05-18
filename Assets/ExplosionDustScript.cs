using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDustScript : MonoBehaviour
{
    // Start is called before the first frame update
    public ExplosionDust thisExplosionDust;
    void Start()
    {
        
    }
    public void DestroyThisDust()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class ExplosionDust 
{
    public Vector3 directionToFace;
    public Transform theTransform;
    public float timeAlive = 0f;
    public bool readyToDie = false;
    public ExplosionDust(Transform explosionTransform, Vector3 directionToBlow)
    {
        directionToFace = directionToBlow;
        theTransform = explosionTransform;
    }
}


