using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

using GoogleMobileAds.Api;

using MoPubReward = MoPubManager.MoPubReward;

public class AdsLoader : MonoBehaviour {

	public Text debugText;

	[Header("iOS")]
	public string iOSBannerAdUnit;
	public string iOSfsAdUnit;
	public string iOSrvAdUnit;

	[Header("Android")]
	public string androidBannerAdUnit;
	public string androidfsAdUnit;
	public string androidrvAdUnit;

	private void Start() {
		#if UNITY_ANDROID
			MoPub.loadBannerPluginsForAdUnits(androidBannerAdUnit.ToArray());
			MoPub.loadInterstitialPluginsForAdUnits(androidfsAdUnit.ToArray());
			MoPub.loadRewardedVideoPluginsForAdUnits(androidrvAdUnit.ToArray());

		#elif UNITY_IOS
			MoPub.loadPluginsForAdUnits(iOSBannerAdUnit.ToArray());
			MoPub.loadPluginsForAdUnits(iOSfsAdUnit.ToArray());
			MoPub.loadPluginsForAdUnits(iOSrvAdUnit.ToArray());
		#endif

		MoPub.initializeRewardedVideo();
	}

	void OnEnable() {
		MoPubManager.onAdLoadedEvent += onAdLoadedEvent;
		MoPubManager.onAdFailedEvent += onAdFailedEvent;
		MoPubManager.onAdClickedEvent += onAdClickedEvent;
		MoPubManager.onAdExpandedEvent += onAdExpandedEvent;
		MoPubManager.onAdCollapsedEvent += onAdCollapsedEvent;

		MoPubManager.onInterstitialLoadedEvent += onInterstitialLoadedEvent;
		MoPubManager.onInterstitialFailedEvent += onInterstitialFailedEvent;
		MoPubManager.onInterstitialShownEvent += onInterstitialShownEvent;
		MoPubManager.onInterstitialClickedEvent += onInterstitialClickedEvent;
		MoPubManager.onInterstitialDismissedEvent += onInterstitialDismissedEvent;
		MoPubManager.onInterstitialExpiredEvent += onInterstitialExpiredEvent;

		MoPubManager.onRewardedVideoLoadedEvent += onRewardedVideoLoadedEvent;
		MoPubManager.onRewardedVideoFailedEvent += onRewardedVideoFailedEvent;
		MoPubManager.onRewardedVideoExpiredEvent += onRewardedVideoExpiredEvent;
		MoPubManager.onRewardedVideoShownEvent += onRewardedVideoShownEvent;
		MoPubManager.onRewardedVideoClickedEvent += onRewardedVideoClickedEvent;
		MoPubManager.onRewardedVideoFailedToPlayEvent += onRewardedVideoFailedToPlayEvent;
		MoPubManager.onRewardedVideoReceivedRewardEvent += onRewardedVideoReceivedRewardEvent;
		MoPubManager.onRewardedVideoClosedEvent += onRewardedVideoClosedEvent;
		MoPubManager.onRewardedVideoLeavingApplicationEvent += onRewardedVideoLeavingApplicationEvent;
	}

	void OnDisable ()
	{
		// Remove all event handlers
		MoPubManager.onAdLoadedEvent -= onAdLoadedEvent;
		MoPubManager.onAdFailedEvent -= onAdFailedEvent;
		MoPubManager.onAdClickedEvent -= onAdClickedEvent;
		MoPubManager.onAdExpandedEvent -= onAdExpandedEvent;
		MoPubManager.onAdCollapsedEvent -= onAdCollapsedEvent;

		MoPubManager.onInterstitialLoadedEvent -= onInterstitialLoadedEvent;
		MoPubManager.onInterstitialFailedEvent -= onInterstitialFailedEvent;
		MoPubManager.onInterstitialShownEvent -= onInterstitialShownEvent;
		MoPubManager.onInterstitialClickedEvent -= onInterstitialClickedEvent;
		MoPubManager.onInterstitialDismissedEvent -= onInterstitialDismissedEvent;
		MoPubManager.onInterstitialExpiredEvent -= onInterstitialExpiredEvent;

		MoPubManager.onRewardedVideoLoadedEvent -= onRewardedVideoLoadedEvent;
		MoPubManager.onRewardedVideoFailedEvent -= onRewardedVideoFailedEvent;
		MoPubManager.onRewardedVideoExpiredEvent -= onRewardedVideoExpiredEvent;
		MoPubManager.onRewardedVideoShownEvent -= onRewardedVideoShownEvent;
		MoPubManager.onRewardedVideoFailedToPlayEvent -= onRewardedVideoFailedToPlayEvent;
		MoPubManager.onRewardedVideoReceivedRewardEvent -= onRewardedVideoReceivedRewardEvent;
		MoPubManager.onRewardedVideoClosedEvent -= onRewardedVideoClosedEvent;
		MoPubManager.onRewardedVideoLeavingApplicationEvent -= onRewardedVideoLeavingApplicationEvent;
	}

	private string GetBannerAdUnit() {
		#if UNITY_ANDROID
			return androidBannerAdUnit;
		#elif UNITY_IOS
			return iOSBannerAdUnit;
		#endif
	}
	private string GetFSAdUnit() {
		#if UNITY_ANDROID
			return androidfsAdUnit;
		#elif UNITY_IOS
			return iOSfsAdUnit;
		#endif
	}
	private string GetRVAdUnit() {
		#if UNITY_ANDROID
			return androidrvAdUnit;
		#elif UNITY_IOS
			return iOSrvAdUnit;
		#endif
	}

	public void DisplayBanner() {
		string adUnitId = GetBannerAdUnit();
		MyLog("Requesting banner, adUnitId: " + adUnitId);
		MoPub.createBanner(adUnitId, MoPubAdPosition.BottomCenter);
	}

	public void DisplayUnityBanner() {
		string adUnitId = GetBannerAdUnit();
		BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		bannerView.LoadAd(request);
	}

	public void DisplayFS() {
		string adUnitId = GetFSAdUnit();
		MyLog("Requesting interstitial, adUnitId: " + adUnitId);
		MoPub.requestInterstitialAd(adUnitId);
	}

	public void DisplayRV() {
		string adUnitId = GetRVAdUnit();
		MyLog("Requesting video, adUnitId: " + adUnitId);
		MoPub.requestRewardedVideo(adUnitId, GetMediationSettings(), GetKeywords(), GetLatitude(), GetLongitude(), GetCustomerId());
	}

	private string GetCustomerId() {
		return "customer001";
	}

	private string GetKeywords() {
		return "rewarded, video, mopub";
	}

	private double GetLatitude() {
		return 37.7833;
	}
	private double GetLongitude() {
		return 122.4167;
	}

	private List<MoPubMediationSetting> GetMediationSettings() {
		#if UNITY_ANDROID
			var vungleSettings = new MoPubMediationSetting ("Vungle");
			vungleSettings.Add ("userId", "the-user-id");
			vungleSettings.Add ("cancelDialogBody", "Cancel Body");
			vungleSettings.Add ("cancelDialogCloseButton", "Shut it Down");
			vungleSettings.Add ("cancelDialogKeepWatchingButton", "Watch On");
			vungleSettings.Add ("cancelDialogTitle", "Cancel Title");

			var mediationSettings = new List<MoPubMediationSetting> ();
			mediationSettings.Add (vungleSettings);
		#elif UNITY_IPHONE
			var vungleSettings = new MoPubMediationSetting ("Vungle");
			vungleSettings.Add ("userIdentifier", "the-user-id");

			var mediationSettings = new List<MoPubMediationSetting> ();
			mediationSettings.Add (vungleSettings);
		#endif

		return mediationSettings;
	}

	private void onAdLoadedEvent (float height) {
		MyLog("onAdLoadedEvent. height: " + height);
		MoPub.showBanner(GetBannerAdUnit(), true);
	}

	private void onAdFailedEvent (string errorMsg) {
		MyLog("onAdFailedEvent: " + errorMsg);
	}

	private void onAdClickedEvent (string adUnitId) {
		MyLog("onAdClickedEvent: " + adUnitId);
	}

	private void onAdExpandedEvent (string adUnitId) {
		MyLog("onAdExpandedEvent: " + adUnitId);
	}

	private void onAdCollapsedEvent (string adUnitId) {
		MyLog("onAdCollapsedEvent: " + adUnitId);
	}

	// Interstitial Events

	private void onInterstitialLoadedEvent (string adUnitId) {
		MyLog("onInterstitialLoadedEvent: " + adUnitId);
		MoPub.showInterstitialAd (adUnitId);
	}

	private void onInterstitialFailedEvent (string errorMsg) {
		MyLog("onInterstitialFailedEvent: " + errorMsg);
	}

	private void onInterstitialShownEvent (string adUnitId) {
		MyLog("onInterstitialShownEvent: " + adUnitId);
	}

	private void onInterstitialClickedEvent (string adUnitId) {
		MyLog("onInterstitialClickedEvent: " + adUnitId);
	}

	private void onInterstitialDismissedEvent (string adUnitId) {
		MyLog("onInterstitialDismissedEvent: " + adUnitId);
	}

	private void onInterstitialExpiredEvent (string adUnitId) {
		MyLog("onInterstitialExpiredEvent: " + adUnitId);
	}

	// Rewarded Video Events

	private void onRewardedVideoLoadedEvent (string adUnitId) {
		MyLog("onRewardedVideoLoadedEvent: " + adUnitId);
		MoPub.showRewardedVideo(adUnitId);
	}

	private void onRewardedVideoFailedEvent (string errorMsg) {
		MyLog("onRewardedVideoFailedEvent: " + errorMsg);
	}

	private void onRewardedVideoExpiredEvent (string adUnitId) {
		MyLog("onRewardedVideoExpiredEvent: " + adUnitId);
	}

	private void onRewardedVideoShownEvent (string adUnitId) {
		MyLog("onRewardedVideoShownEvent: " + adUnitId);
	}

	private void onRewardedVideoClickedEvent (string adUnitId) {
		MyLog("onRewardedVideoClickedEvent: " + adUnitId);
	}

	private void onRewardedVideoFailedToPlayEvent (string errorMsg) {
		MyLog("onRewardedVideoFailedToPlayEvent: " + errorMsg);
	}

	private void onRewardedVideoReceivedRewardEvent (MoPubManager.RewardedVideoData rewardedVideoData) {
		MyLog("onRewardedVideoReceivedRewardEvent: " + rewardedVideoData);
	}

	private void onRewardedVideoClosedEvent (string adUnitId) {
		MyLog("onRewardedVideoClosedEvent: " + adUnitId);
	}

	private void onRewardedVideoLeavingApplicationEvent (string adUnitId) {
		MyLog("onRewardedVideoLeavingApplicationEvent: " + adUnitId);
	}

	private void MyLog(string log) {
		debugText.text += log + Environment.NewLine;
		Debug.Log(log);
	}
}

static public class Extensions {
	static public string[] ToArray(this string str) {
		return new string[] { str };
	}
}