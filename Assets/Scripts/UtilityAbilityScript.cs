using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAbilityScript : MonoBehaviour
{
    public UtilityAbilityInstance thisAbilityInstance;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void DestroyThisUtility()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
