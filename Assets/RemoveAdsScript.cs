using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class RemoveAdsScript : MonoBehaviour
{
    //IAPButton thisButton;
    
    // Start is called before the first frame update
    void Start()
    {
        //thisButton = GetComponent<IAPButton>();
    }
    public void StartPurchase()
    {
        MainMenuScript.isBuyingRemoveAds = true;
    }
    public void FailPurchase()
    {
        MainMenuScript.isBuyingRemoveAds = false;
    }
    public void RemoveAds()
    {
        PlayerPrefs.SetInt("removeAds", 1);
        MainMenuScript.isBuyingRemoveAds = false;
        
        //MainScript.mainMenuObject.GetComponent<MainMenuScript>().SetUpMainMenu();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
