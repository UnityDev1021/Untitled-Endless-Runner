//#define TEST_MODE

using GoogleMobileAds.Api;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class RewardedAdsManager : MonoBehaviour
    {
        [Header("Local References")]
        [SerializeField] private GameLogic localGameLogic;

        [Space]
        [SerializeField] private UnityEngine.UI.Button showAdsBt, showAdsBt_BuyHearts;

        // Start is called before the first frame update
        private void Start()
        {
            // When true all events raised by GoogleMobileAds will be raised
            // on the Unity main thread. The default value is false.
            MobileAds.RaiseAdEventsOnUnityMainThread = true;

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(initStatus =>
            {
                //Debug.Log($"Rewarded Initialization Status : {initStatus.ToString()}");
                LoadRewardedAd();
            });
        }

        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";        //Test ID
        //private string _adUnitId = "ca-app-pub-6547462235936410/9801503599";            //Live ID
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";              //notconfigured now
#else
  private string _adUnitId = "unused";
#endif

        private RewardedAd rewardedAd;

        /// <summary>
        /// Loads the rewarded ad.
        /// </summary>
        public void LoadRewardedAd()
        {
            // Clean up the old ad before loading a new one.
            DestroyAd();

            //Debug.Log("Loading the rewarded ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest.Builder().Build();

            // send the request to load the ad.
            RewardedAd.Load(_adUnitId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.Log("Rewarded ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    //Debug.Log("Rewarded ad loaded with response : "
                    //          + ad.GetResponseInfo());

                    rewardedAd = ad;
                    showAdsBt.interactable = true;
                    showAdsBt_BuyHearts.interactable = true;

                    RegisterEventHandlers(rewardedAd);
                    RegisterReloadHandler(rewardedAd);
                });
        }

        //Should be on the button to show rewards/ get a reward by watching an ad.
        public void ShowRewardedAd()
        {
            //const string rewardMsg =
            //    "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    // TODO: Reward the user.
                    //Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
#if !TEST_MODE
                    GameManager.instance.totalDiamonds += 5;
                    PlayerPrefs.SetInt("DIAMONDS_AMOUNT", GameManager.instance.totalDiamonds);
                    localGameLogic.OnAdsRewarded?.Invoke();
                    showAdsBt.interactable = true;
                    showAdsBt_BuyHearts.interactable = true;
#endif
                });
            }
        }

        private void RegisterEventHandlers(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                //Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                //    adValue.Value,
                //    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                //Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                //Debug.Log("Rewarded ad was clicked.");
            };

            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                //Debug.Log("Rewarded ad full screen content opened.");
            };

            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                //Debug.Log("Rewarded ad full screen content closed.");
                DestroyAd();
            };

            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.Log("Rewarded ad failed to open full screen content " +
                               "with error : " + error);
            };
        }

        private void RegisterReloadHandler(RewardedAd ad)
        {
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                //Debug.Log("Rewarded Ad full screen content closed.");

                // Reload the ad so that we can show another as soon as possible.
                LoadRewardedAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.Log("Rewarded ad failed to open full screen content " +
                               "with error : " + error);

                // Reload the ad so that we can show another as soon as possible.
                LoadRewardedAd();
            };
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            if (rewardedAd != null)
            {
                //Debug.Log("Destroying Rewarded ad.");
                rewardedAd.Destroy();
                rewardedAd = null;
            }
        }

    }
}