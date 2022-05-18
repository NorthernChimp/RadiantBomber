using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPrefabScript : MonoBehaviour
{
    Vector3 directionToMoveIn;
    public Counter deathCounter;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetDirection(Vector3 direction)
    {
        deathCounter = new Counter(1f);
        directionToMoveIn = direction;
    }
    private void FixedUpdate()
    {
        transform.Translate(directionToMoveIn.normalized * MainScript.blockHeight * 3.5f * Time.fixedDeltaTime,Space.World);
        transform.Rotate(new Vector3(0f, 0f, Time.fixedDeltaTime * 360f));
        if (!deathCounter.hasFinished) { deathCounter.AddTime(Time.fixedDeltaTime); }
        if (deathCounter.hasFinished) { Destroy(gameObject); }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
