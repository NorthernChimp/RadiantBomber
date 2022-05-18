using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericExplosionPrefabScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject explosionDot;
    public GenericExplosion thisExplosion;
    List<SpriteInteractionObject> allSprites;
    public GameObject explosionDustPrefab;
    void Start()
    {
        
    }
    public void EndExplosion()
    {
        if(thisExplosion.theType != TypeOfExplosion.confetti)
        {
            foreach (Transform t in thisExplosion.allDots)
            {
                Destroy(t.gameObject);
            }

            Destroy(gameObject);
        }
        else
        {
            foreach(ExplosionDust dust in thisExplosion.allDust)
            {
                Destroy(dust.theTransform.gameObject);
            }
            Destroy(gameObject);
        }
        
    }
    public static void CreateConfetti(Vector2 position,GenericExplosion theExplosion)
    {
        //theExplosion.allDots.Add(Instantiate((GameObject)Resources.Load("Prefabs/BasicWhiteBlockPrefab"), position, Quaternion.identity).transform);
        theExplosion.allDust.Add(new ExplosionDust(Instantiate((GameObject)Resources.Load("Prefabs/ExplosionDustPrefab"), position, Quaternion.identity).transform,MainScript.playerDirection));
        //theExplosion.allDust.Add(new ExplosionDust(Instantiate((GameObject)Resources.Load("Prefabs/ExplosionDustPrefab"), position, Quaternion.identity).transform,MainScript.playerDirection));
    }
    public void SetUpExplosionFromReference(GenericExplosion reference)
    {
        allSprites = new List<SpriteInteractionObject>();
        thisExplosion = reference;
        //thisExplosion.explosionLength = Screen.width * 0.001f;
        //thisExplosion.deathCounter = new Counter(2f);
        //thisExplosion.allDots = new List<Transform>();
        //thisExplosion.
        float widthScale = (Screen.width / 20f) * 0.01f;
        for (int i = 0; i < thisExplosion.numberOfDots; i++)
        {
            GameObject temp = Instantiate(explosionDot, transform.position, Quaternion.identity);
            allSprites.Add(new SpriteInteractionObject(temp.transform));
            temp.transform.localScale = new Vector3(widthScale / 0.16f, widthScale / 0.16f, 1f);
            temp.transform.parent = transform;
            thisExplosion.allDots.Add(temp.transform);
        }
        foreach (SpriteInteractionObject render in allSprites)
        {
            render.ChangeColor(thisExplosion.colorOfExplosion);
        }
    }
    public void SetUpExplosion(Color colorToExplode,float explosionLength,float explosionTime)
    {
        allSprites = new List<SpriteInteractionObject>();
        
        thisExplosion = new GenericExplosion(16,transform, Screen.width * 0.001f,explosionTime,colorToExplode);
        float widthScale = MainScript.blockWidth / 0.16f;
        //thisExplosion.deathCounter = new Counter(explosionTime);
        //thisExplosion.explosionLength = Screen.width * 0.001f;
        //thisExplosion.deathCounter = new Counter(2f);
        //thisExplosion.allDots = new List<Transform>();
        //thisExplosion.

        //float widthScale = (Screen.width / 20f) * 0.01f;
        for (int i = 0; i < thisExplosion.numberOfDots; i++)
        {
            GameObject temp = Instantiate(explosionDot, transform.position, Quaternion.identity);
            allSprites.Add(new SpriteInteractionObject(temp.transform));
            temp.transform.localScale = new Vector3(widthScale  *0.35f, widthScale * 0.35f, 1f);
            temp.transform.parent = transform;
            thisExplosion.allDots.Add(temp.transform);
        }
        foreach(SpriteInteractionObject render in allSprites)
        {
            render.ChangeColor(colorToExplode);
        }
    }
    public void SetUpConfetti(Color colorOfConfetti, float confettiTimeAlive, float totalTime,float confettiSpeed,Vector2 directionOfConfetti,float timeBetweenTheConfetti)
    {
        thisExplosion = new GenericExplosion(0, transform, confettiSpeed, totalTime, colorOfConfetti);
        thisExplosion.explosionDirection = directionOfConfetti;
        thisExplosion.timeBetweenConfetti = timeBetweenTheConfetti;
        thisExplosion.confettiTimeAlive = confettiTimeAlive;
        thisExplosion.theType = TypeOfExplosion.confetti;
        thisExplosion.allDust = new List<ExplosionDust>();
        //thisExplosion.deathCounter = new Counter(totalTime);
    }
    public void SetUpShotgunBlast(Color colorToBlast,float thetotalSpread,int flakAmount,Vector2 direction,float flakTimeAlive,float flakSpeed)
    {
        
        allSprites = new List<SpriteInteractionObject>();
        thisExplosion = new GenericExplosion(flakAmount, transform, 12f, 2f, colorToBlast);
        thisExplosion.explosionDirection = direction;
        thisExplosion.totalSpread = thetotalSpread;
        thisExplosion.theType = TypeOfExplosion.shotgunBlast;
        thisExplosion.deathCounter = new Counter(flakTimeAlive);
        float widthScale = MainScript.blockWidth / 0.16f;
        //float widthScale = (Screen.width / 20f) * 0.01f;
        for (int i = 0; i < flakAmount; i++)
        {
            GameObject temp = Instantiate(explosionDot, transform.position, Quaternion.identity);
            allSprites.Add(new SpriteInteractionObject(temp.transform));
            temp.transform.localScale = new Vector3(widthScale * 0.35f, widthScale * 0.35f, 1f);
            temp.transform.parent = transform;
            thisExplosion.allDots.Add(temp.transform);
        }
        foreach(SpriteInteractionObject spr in allSprites)
        {
            spr.ChangeColor(colorToBlast);
        }
    }
    public void UpdateExplosion()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class GenericExplosion
{
    public Transform thisTransform;
    public int numberOfDots = 12;
    public List<Transform> allDots;
    public float explosionLength;
    public float totalSpread;
    public Color colorOfExplosion;
    public Counter deathCounter;
    public bool ReadyToDie;
    public bool isAddingSinWave = false;
    public float waveMultiplier;
    public float speedMultiplier;
    public float confettiTimeAlive = 0f;
    public bool addRotation = false;
    public float rotationSpeed;
    public bool accelerateSpeed = false;
    public float accelerationSpeed;
    public Vector2 explosionDirection;
    public float timeSinceLastConfetti = 0f;
    public float timeBetweenConfetti = 0f;
    public TypeOfExplosion theType = TypeOfExplosion.regular;
    public List<ExplosionDust> allDust;
    
    public GenericExplosion(int dotNumber, Transform theTransform, float lengthOfExplosion, float timeBeforeDeath,Color explosionColor)
    {
        colorOfExplosion = explosionColor;
        allDots = new List<Transform>();
        deathCounter = new Counter(timeBeforeDeath);
        numberOfDots = dotNumber;
        explosionLength = lengthOfExplosion;
        thisTransform = theTransform;
    }
    public void AddSinWaveToSpeed(float wave,float speed)
    {
        isAddingSinWave = true;
        waveMultiplier = wave;
        speedMultiplier = speed;
    }
    public void AddRotation(float rotation)
    {
        addRotation = true;
        rotationSpeed = rotation;
    }
    public void AddAcceleration(float speedofAccel)
    {
        accelerateSpeed = true;
        accelerationSpeed = speedofAccel;
    }
    public void UpdateExplosion()
    {
        deathCounter.AddTime(Time.fixedDeltaTime);
        if(theType == TypeOfExplosion.regular)
        {
            for (int i = 0; i < allDots.Count; i++)
            {
                float fraction = (float)i / (float)allDots.Count;
                float currentAngle = fraction * Mathf.PI * 2f;
                Vector2 direction = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle));
                float fractionFinished = deathCounter.currentTime / deathCounter.expiryTime;
                //allDots[i].Tra((Vector3)direction.normalized * Time.fixedDeltaTime * explosionLength);
                float finalExplosionLength = explosionLength;
                if (accelerateSpeed) { finalExplosionLength += deathCounter.currentTime * accelerationSpeed; }
                if (isAddingSinWave) { finalExplosionLength += Mathf.Sin(deathCounter.currentTime * speedMultiplier) * waveMultiplier * Time.fixedDeltaTime * MainScript.blockWidth; }
                if (addRotation) { direction = Quaternion.Euler(0f, 0f, rotationSpeed * Time.fixedDeltaTime) * direction; }
                allDots[i].Translate(direction.normalized * Time.fixedDeltaTime * finalExplosionLength);
            }
        }else if(theType == TypeOfExplosion.shotgunBlast)
        {
            Vector2 perpendicularDirection = Quaternion.Euler(0f, 0f, 90f) * explosionDirection;
            for (int i = 0; i < allDots.Count; i++)
            {
                Vector2 flakDirection = explosionDirection.normalized + (perpendicularDirection.normalized * (totalSpread / -2f + (((float)(i + 1) / (float)allDots.Count) * totalSpread)));
                allDots[i].transform.Translate(flakDirection.normalized * Time.fixedDeltaTime * MainScript.blockWidth * explosionLength); ;
            }
        }else if(theType == TypeOfExplosion.confetti)
        {
            timeSinceLastConfetti += Time.fixedDeltaTime;
            if(timeSinceLastConfetti > timeBetweenConfetti) 
            {
                //float widthScale = (Screen.width / 20f) * 0.05f;
                timeSinceLastConfetti -= timeBetweenConfetti;
                Vector3 positionToInstantiate = thisTransform.position;
                Vector3 randomizedVector = new Vector3(Random.Range(MainScript.blockHeight * -0.2f, MainScript.blockWidth * 0.4f), Random.Range(MainScript.blockHeight * -0.2f, MainScript.blockWidth * 0.4f), 0f);
                //allDots.Add(Instantiate((GameObject)Resources.Load("Prefabs/BasicWhiteBlockPrefab", positionToInstantiate, Quaternion.identity).transform));
                GenericExplosionPrefabScript.CreateConfetti(positionToInstantiate + randomizedVector, this);
                allDust[allDust.Count - 1].theTransform.GetComponent<SpriteRenderer>().material.color = colorOfExplosion;
                float widthScale = MainScript.blockWidth / 0.16f;
                allDust[allDust.Count - 1].theTransform.localScale = new Vector3(widthScale * 0.45f, widthScale * 0.45f, 1f);
                //allDots[allDots.Count - 1].GetComponent<SpriteRenderer>().material.color = colorOfExplosion;

            }
            List<ExplosionDust> dustToRemove = new List<ExplosionDust>();
            foreach(ExplosionDust dust in allDust)
            {
                dust.timeAlive += Time.fixedDeltaTime;
                if(dust.timeAlive >= confettiTimeAlive)
                {
                    dustToRemove.Add(dust);
                }
                else
                {
                    dust.theTransform.Translate(dust.directionToFace.normalized * Time.fixedDeltaTime * MainScript.blockHeight * explosionLength * -1f);
                }
            }
            while (dustToRemove.Count > 0)
            {
                ExplosionDust tempDust = dustToRemove[0];
                allDust.Remove(tempDust);
                dustToRemove.Remove(tempDust);
                tempDust.theTransform.SendMessage("DestroyThisDust");
            }
            /*foreach(Transform t in allDots)
            {
                t.Translate(MainScript.playerDirection.normalized * Time.fixedDeltaTime * MainScript.blockWidth * explosionLength * -1f);
            }*/
        }
        
        if (deathCounter.hasFinished) { ReadyToDie = true;  }
    }
}public enum TypeOfExplosion { regular,shotgunBlast,confetti}