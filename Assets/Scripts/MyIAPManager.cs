using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using UnityEngine.Purchasing.Security;


public class MyIAPManager : MonoBehaviour, IStoreListener,IDetailedStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    private IAppleExtensions m_AppleExtensions;
    private IGooglePlayStoreExtensions m_GoogleExtensions;

    // ProductIDs
    public static string GOLD_50 = "gold50";
    public static string NONCONSUMABLE1 = "nonconsume1";
    public static string removeAds = "com.dannyconrad.radiantbomber.removeads";
    public static string WEEKLYSUB = "weeklysub";
   
    void Awake()
    {

    }

  
    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing, can use button click instead
            InitializePurchasing();
            //myText.transform.localScale = new Vector3((0.5f * Screen.width) * 0.01f, (0.5f * Screen.width)  * 0.01f, 1f);
            //myText.rectTransform.localScale = new Vector3(Screen.width, Screen.height, 1f);
            //myText.transform.localScale = new Vector3(Screen.width, Screen.height, 1f) * 0.01f;
            //ListProducts();
        }
    }

    public void MyInitialize()
    {
        InitializePurchasing();
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            Debug.Log("this stops it");
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        //builder.Configure<IGooglePlayConfiguration>().SetDeferredPurchaseListener(OnDeferredPurchase); ** you put this here, prob garbage**
        builder.AddProduct(removeAds, ProductType.NonConsumable);
        /*
        builder.AddProduct(NONCONSUMABLE1, ProductType.NonConsumable);
        builder.AddProduct(GOLD_50, ProductType.Consumable);
        builder.AddProduct(WEEKLYSUB, ProductType.Subscription);*/

        //MyDebug("Starting Initialized...");
        UnityPurchasing.Initialize(this, builder);
        

    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuySubscription()
    {
        BuyProductID(WEEKLYSUB);
    }
    public void BuyRemoveAds()
    {
        //MyDebug("we get here");
        MainMenuScript.isBuyingRemoveAds = true;
        BuyProductID(removeAds);
    }
  
    public void BuyGold50()
    {
        BuyProductID(GOLD_50);
    }

    public void BuyNonConsumable()
    {
        BuyProductID(NONCONSUMABLE1);
    }
    
    public void RestorePurchases()
    {
        /*m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(result => {
            if (result)
            {
                MyDebug("Restore purchases succeeded.");
            }
            else
            {
                MyDebug("Restore purchases failed.");
            }
         });*/
         m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((success, error) =>
    {
        if (success)
        {
            Debug.Log("Transactions restored successfully.");
        }
        else
        {
            Debug.LogError($"Failed to restore transactions. Error: {error}");
        }
    });
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"Purchase of product '{product.definition.id}' failed due to {failureDescription.reason}: {failureDescription.message}");

        // Handle different types of failure reasons
        switch (failureDescription.reason)
        {
            case PurchaseFailureReason.PurchasingUnavailable:
                Debug.LogError("Purchasing is unavailable. Check if the app store is set up correctly.");
                break;
            case PurchaseFailureReason.ExistingPurchasePending:
                Debug.LogError("An existing purchase is pending. Please wait for it to complete.");
                break;
            case PurchaseFailureReason.ProductUnavailable:
                Debug.LogError("The product is not available in the store.");
                break;
            case PurchaseFailureReason.SignatureInvalid:
                Debug.LogError("The purchase signature is invalid.");
                break;
            case PurchaseFailureReason.UserCancelled:
                Debug.Log("The user canceled the purchase.");
                break;
            case PurchaseFailureReason.PaymentDeclined:
                Debug.LogError("The payment was declined. Check payment method and try again.");
                break;
            case PurchaseFailureReason.DuplicateTransaction:
                Debug.LogWarning("The transaction is a duplicate.");
                break;
            case PurchaseFailureReason.Unknown:
            default:
                Debug.LogError("An unknown error occurred during the purchase.");
                break;
        }

        // Optionally, you can handle UI updates or other responses to the failure here.
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            UnityEngine.Purchasing.Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                MyDebug(string.Format("Purchasing product:" + product.definition.id.ToString()));
                
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                MyDebug("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            MyDebug("BuyProductID FAIL. Not initialized.");
        }
        
    }

    public void ListProducts()
    {

        foreach (UnityEngine.Purchasing.Product item in m_StoreController.products.all)
        {
            if (item.receipt != null)
            {
                MyDebug("Receipt found for Product = " + item.definition.id.ToString());
            }
        }
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        MyDebug("OnInitialized: PASS");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_GoogleExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        //IGooglePlayConfiguration test = GetComponent<IGooglePlayConfiguration>();
        //IGooglePlayConfiguration m_GoogleConfig = extensions.GetExtension<IGooglePlayConfiguration>();
        //m_GoogleExtensions?.SetDeferredPurchaseListener(OnPurchaseDeferred); ** this line was part of the code before but it was causing an error. now it seems like it works with it. may cause issues later **

        Dictionary<string, string> dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

        foreach (UnityEngine.Purchasing.Product item in controller.products.all)
        {

            if (item.receipt != null)
            {
                string intro_json = (dict == null || !dict.ContainsKey(item.definition.storeSpecificId)) ? null : dict[item.definition.storeSpecificId];

                if (item.definition.type == ProductType.Subscription)
                {
                    SubscriptionManager p = new SubscriptionManager(item, intro_json);
                    SubscriptionInfo info = p.getSubscriptionInfo();
                    MyDebug("SubInfo: " + info.getProductId().ToString());
                    MyDebug("isSubscribed: " + info.isSubscribed().ToString());
                    MyDebug("isFreeTrial: " + info.isFreeTrial().ToString());
                }
            }
        }
    }

    public void OnPurchaseDeferred(Product product)
    {
        MainMenuScript.isBuyingRemoveAds = false;
        MyDebug("Deferred product " + product.definition.id.ToString());
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        // Handle the initialization failure
        Debug.LogError($"Initialization Failed: {error.ToString()} - {message}");
        
        // You can take further action based on the error, like displaying a message to the user
        // or attempting to reinitialize the purchasing system.
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        MyDebug("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        MainMenuScript.isBuyingRemoveAds = false;
        try
        {
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            var result = validator.Validate(args.purchasedProduct.receipt);
            MyDebug("Validate = " + result.ToString());
            foreach (IPurchaseReceipt productReceipt in result)
            {
                MyDebug("Valid receipt for " + productReceipt.productID.ToString());
            }
            if (args.purchasedProduct.definition.id.ToString() == removeAds) 
            {
                CompleteRemoveAds();
            };
        }
        catch (Exception e)
        {
            MyDebug("Error is " + e.Message.ToString());
        }

        MyDebug(string.Format("ProcessPurchase: " + args.purchasedProduct.definition.id));

        return PurchaseProcessingResult.Complete;
       
     }
    void CompleteRemoveAds()
    {
        PlayerPrefs.SetInt("removeAds", 1); MainScript.mainMenuObject.GetComponent<MainMenuScript>().SetUpMainMenu();
    }

    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
    {
		MainMenuScript.isBuyingRemoveAds = false;
        if(product.definition.storeSpecificId == removeAds && failureReason == PurchaseFailureReason.DuplicateTransaction) { CompleteRemoveAds(); }
        //failureReason = PurchaseFailureReason.DuplicateTransaction
        MyDebug(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }


    private void MyDebug(string debug)
    {
        
        Debug.Log(debug);
        //myText.text += "\r\n" + debug;
    }

}
