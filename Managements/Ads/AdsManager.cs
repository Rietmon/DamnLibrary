#if ENABLE_IRONSOURCE
using System;
using DamnLibrary.Debugging;

namespace DamnLibrary.Management.Ads
{
    public static class AdsManager
    {
        public static event Action<IronSourceError,IronSourceAdInfo> OnRewardedAdShowFailedEvent
        {
            add => IronSourceRewardedVideoEvents.onAdShowFailedEvent += value;
            remove => IronSourceRewardedVideoEvents.onAdShowFailedEvent -= value;
        }
        public static event Action <IronSourceAdInfo> OnRewardedAdOpenedEvent
        {
            add => IronSourceRewardedVideoEvents.onAdOpenedEvent += value;
            remove => IronSourceRewardedVideoEvents.onAdOpenedEvent -= value;
        }
        public static event Action <IronSourceAdInfo> OnRewardedAdClosedEvent
        {
            add => IronSourceRewardedVideoEvents.onAdClosedEvent += value;
            remove => IronSourceRewardedVideoEvents.onAdClosedEvent -= value;
        }
        public static event Action<IronSourcePlacement,IronSourceAdInfo> OnRewardedAdRewardedEvent
        {
            add => IronSourceRewardedVideoEvents.onAdRewardedEvent += value;
            remove => IronSourceRewardedVideoEvents.onAdRewardedEvent -= value;
        }
        public static event Action<IronSourcePlacement,IronSourceAdInfo> OnRewardedAdClickedEvent
        {
            add => IronSourceRewardedVideoEvents.onAdClickedEvent += value;
            remove => IronSourceRewardedVideoEvents.onAdClickedEvent -= value;
        }
        public static event Action<IronSourceAdInfo> OnRewardedAdAvailableEvent
        {
            add => IronSourceRewardedVideoEvents.onAdAvailableEvent += value;
            remove => IronSourceRewardedVideoEvents.onAdAvailableEvent -= value;
        }
        public static event Action OnRewardedAdUnavailableEvent
        {
            add => IronSourceRewardedVideoEvents.onAdUnavailableEvent += value;
            remove => IronSourceRewardedVideoEvents.onAdUnavailableEvent -= value;
        }
        public static event Action<IronSourceError> OnRewardedAdLoadFailedEvent
        {
            add => IronSourceRewardedVideoEvents.onAdLoadFailedEvent += value;
            remove => IronSourceRewardedVideoEvents.onAdLoadFailedEvent -= value;
        }
        public static event Action<IronSourceAdInfo> OnRewardedAdReadyEvent
        {
            add => IronSourceRewardedVideoEvents.onAdReadyEvent += value;
            remove => IronSourceRewardedVideoEvents.onAdReadyEvent -= value;
        }
        
        public static event Action<IronSourceAdInfo> OnBannerAdLoadedEvent
        {
            add => IronSourceBannerEvents.onAdLoadedEvent += value;
            remove => IronSourceBannerEvents.onAdLoadedEvent += value;
        }
        public static event Action<IronSourceAdInfo> OnBannerAdLeftApplicationEvent
        {
            add => IronSourceBannerEvents.onAdLeftApplicationEvent += value;
            remove => IronSourceBannerEvents.onAdLeftApplicationEvent += value;
        }
        public static event Action<IronSourceAdInfo> OnBannerAdScreenDismissedEvent
        {
            add => IronSourceBannerEvents.onAdScreenDismissedEvent += value;
            remove => IronSourceBannerEvents.onAdScreenDismissedEvent += value;
        }
        public static event Action<IronSourceAdInfo> OnBannerAdScreenPresentedEvent
        {
            add => IronSourceBannerEvents.onAdScreenPresentedEvent += value;
            remove => IronSourceBannerEvents.onAdScreenPresentedEvent += value;
        }
        public static event Action<IronSourceAdInfo> OnBannerAdClickedEvent
        {
            add => IronSourceBannerEvents.onAdClickedEvent += value;
            remove => IronSourceBannerEvents.onAdClickedEvent += value;
        }
        public static event Action<IronSourceError> OnBannerAdLoadFailedEvent
        {
            add => IronSourceBannerEvents.onAdLoadFailedEvent += value;
            remove => IronSourceBannerEvents.onAdLoadFailedEvent += value;
        }
        
        public static event Action<IronSourceAdInfo> OnInterstitialAdReadyEvent
        {
            add => IronSourceInterstitialEvents.onAdReadyEvent += value;
            remove => IronSourceInterstitialEvents.onAdReadyEvent += value;
        }
        public static event Action<IronSourceError> OnInterstitialAdLoadFailedEvent
        {
            add => IronSourceInterstitialEvents.onAdLoadFailedEvent += value;
            remove => IronSourceInterstitialEvents.onAdLoadFailedEvent += value;
        }
        public static event Action<IronSourceAdInfo> OnInterstitialAdOpenedEvent
        {
            add => IronSourceInterstitialEvents.onAdOpenedEvent += value;
            remove => IronSourceInterstitialEvents.onAdOpenedEvent += value;
        }
        public static event Action<IronSourceAdInfo> OnInterstitialAdClosedEvent
        {
            add => IronSourceInterstitialEvents.onAdClosedEvent += value;
            remove => IronSourceInterstitialEvents.onAdClosedEvent += value;
        }
        public static event Action<IronSourceAdInfo> OnInterstitialAdShowSucceededEvent
        {
            add => IronSourceInterstitialEvents.onAdShowSucceededEvent += value;
            remove => IronSourceInterstitialEvents.onAdShowSucceededEvent += value;
        }
        public static event Action<IronSourceError, IronSourceAdInfo> OnInterstitialAdShowFailedEvent
        {
            add => IronSourceInterstitialEvents.onAdShowFailedEvent += value;
            remove => IronSourceInterstitialEvents.onAdShowFailedEvent += value;
        }
        public static event Action<IronSourceAdInfo> OnInterstitialAdClickedEvent
        {
            add => IronSourceInterstitialEvents.onAdClickedEvent += value;
            remove => IronSourceInterstitialEvents.onAdClickedEvent += value;
        }
        
        public static void Initialize(string key)
        {
            IronSource.Agent.validateIntegration();
            IronSource.Agent.init(key);
            UniversalDebugger.Log($"[{nameof(AdsManager)}] ({nameof(Initialize)}) Initialized for id={IronSource.Agent.getAdvertiserId()}");
        }

        public static bool IsRewardedVideoAvailable() => 
            IronSource.Agent.isRewardedVideoAvailable();

        public static void LoadRewardedVideo() => 
            IronSource.Agent.loadRewardedVideo();

        public static void ShowRewardedVideo() => 
            IronSource.Agent.showRewardedVideo();

        public static void ShowRewardedVideo(string placementName) => 
            IronSource.Agent.showRewardedVideo(placementName);

        public static void LoadInterstitial() => 
            IronSource.Agent.loadInterstitial();

        public static void ShowInterstitial() => 
            IronSource.Agent.showInterstitial();

        public static void ShowInterstitial(string placementName) =>
            IronSource.Agent.showInterstitial(placementName);

        public static bool IsInterstitialReady() =>
            IronSource.Agent.isInterstitialReady();

        public static void LoadBanner(IronSourceBannerSize size, IronSourceBannerPosition position) =>
            IronSource.Agent.loadBanner(size, position);

        public static void LoadBanner(IronSourceBannerSize size, IronSourceBannerPosition position, string placementName) => 
            IronSource.Agent.loadBanner(size, position, placementName);

        public static void DestroyBanner() => 
            IronSource.Agent.destroyBanner();

        public static void DisplayBanner() => 
            IronSource.Agent.displayBanner();

        public static void HideBanner() => 
            IronSource.Agent.hideBanner();
    }
}
#endif