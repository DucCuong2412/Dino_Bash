public interface SupersonicIAgent
{
	void start();

	void onResume();

	void onPause();

	void setAge(int age);

	void setGender(string gender);

	void initRewardedVideo(string appKey, string userId);

	void showRewardedVideo();

	bool isRewardedVideoAvailable();

	void showRewardedVideo(string placementName);

	SupersonicPlacement getPlacementInfo(string name);

	void initInterstitial(string appKey, string userId);

	void showInterstitial();

	bool isInterstitialAdAvailalbe();

	void initOfferwall(string appKey, string userId);

	void showOfferwall();

	bool isOfferwallAvailable();

	void getOfferwallCredits();
}
