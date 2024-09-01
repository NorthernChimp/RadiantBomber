using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener,IUnityAdsLoadListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    InterstitialAdExample adLoader;
    private string _gameId;

    void Awake()
    {
        InitializeAds();
        adLoader = GetComponent<InterstitialAdExample>();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }


    public void OnInitializationComplete()
    {
        //Debug.Log("Unity Ads initialization complete.");
        //Advertisement.Load(_gameId,this);
        adLoader.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
       // Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log(error + " and " + message);
        throw new System.NotImplementedException();
    }
}