using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing;

public class UnityServicesInitializer : MonoBehaviour
{
    public string environment = "production"; // Set the environment if needed (e.g., "production", "development").
    public Transform listener;
    IDetailedStoreListener purchaseListener;

    private void Start()
    {
        
    }
    void Awake()
    {
        purchaseListener = listener.GetComponent<IDetailedStoreListener>();
        InitializeUnityServices();
    }
    private async void InitializeUnityServices()
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(environment);
            await UnityServices.InitializeAsync(options);

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                Debug.Log("Unity Gaming Services initialized successfully.");
                InitializeIAP();
            }
            else
            {
                Debug.LogError("Failed to initialize Unity Gaming Services.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing Unity Gaming Services: {e.Message}");
        }
    }

    private void InitializeIAP()
    {
        // Assuming you've already set up your IAP system in Unity, you can initialize it here.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        // Add your products here, e.g., builder.AddProduct("product_id", ProductType.Consumable);
        
        UnityPurchasing.Initialize(purchaseListener, builder); 
    }
}
