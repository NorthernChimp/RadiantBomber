using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManagerScript : MonoBehaviour,IUnityAdsListener 
{
    
    private string gameId = "4071619";
    bool testMode = false;

    void Start()
    {
        // Initialize the Ads service:
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
        
    }

    public static void ShowInterstitialAd()
    {
        // Check if UnityAds ready before calling Show method:
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
        else
        {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
            MainScript.isShowingAd = false;
            MainScript.mainMenuObject.SendMessage("StartOpeningMenu");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
	// Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish (string surfacingId, ShowResult showResult) {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished) {
            // Reward the user for watching the ad to completion.
        } else if (showResult == ShowResult.Skipped) {
            // Do not reward the user for skipping the ad.
        } else if (showResult == ShowResult.Failed) {
            Debug.LogWarning ("The ad did not finish due to an error.");
        }
		MainScript.isShowingAd = false;
        MainScript.mainMenuObject.SendMessage("StartOpeningMenu");
    }

    public void OnUnityAdsReady (string surfacingId) {
        // If the ready Ad Unit or legacy Placement is rewarded, show the ad:
        /*if (surfacingId == mySurfacingId) {
            // Optional actions to take when theAd Unit or legacy Placement becomes ready (for example, enable the rewarded ads button)
        }*/
    }

    public void OnUnityAdsDidError (string message) {
        // Log the error.
    }

    public void OnUnityAdsDidStart (string surfacingId) {
        // Optional actions to take when the end-users triggers an ad.
    } 

    // When the object that subscribes to ad events is destroyed, remove the listener:
    public void OnDestroy() {
        Advertisement.RemoveListener(this);
    }
}
