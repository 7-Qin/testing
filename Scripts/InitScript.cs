using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Text;

using System.Collections.Generic;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

#if CHARTBOOST_ADS
using ChartboostSDK;
#endif
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#if FACEBOOK
using Facebook.Unity;
#endif

// Import the Thirdweb SDK namespace
using Thirdweb;

public enum Target
{
    SCORE,
    COLLECT,
    INGREDIENT,
    BLOCKS
}

public enum LIMIT
{
    MOVES,
    TIME
}

public enum Ingredients
{
    None = 0,
    Ingredient1,
    Ingredient2
}

public enum CollectItems
{
    None = 0,
    Item1,
    Item2,
    Item3,
    Item4,
    Item5,
    Item6
}

public enum RewardedAdsType
{
    GetLifes,
    GetGems,
    GetGoOn
}

[System.Serializable]
public class PointHistory
{
    public int point;
    public long date;
}


[System.Serializable]
public class AccountData
{
    public string _id;
    public string publicKey;
    public int levelReached;
    public int[] levelHistory;
    public int pointRemain;
    public PointHistory[] pointHistory;
    public int mccRemain;
    public int[] mccHistory;
    public int bnbPaid;
    public int[] bnbPaymentHistory;
    public int healthRemain;
    public int[] healthHistory;
    public string[] items;
    public string[] itemshistory;
    public string referalCode;
    public string referalLink;
    public string[] referalList;
    public string lastAuthentication;
    public int pointsEarned;
    public int referralBonus;
}

[System.Serializable]
public class ResponseData
{
    public int code;
    public string message;
    public AccountData data;
}

public class InitScript : MonoBehaviour
{

    // Create a Thirdweb SDK instance to use throughout this class
    private ThirdwebSDK sdk;


    public static InitScript Instance;
    public static int openLevel;


    public static float RestLifeTimer;
    public static string DateOfExit;
    public static DateTime today;
    public static DateTime DateOfRestLife;
    public static string timeForReps;
    private static int Lifes;

    bool loginForSharing;

    public RewardedAdsType currentReward;

    public static int lifes
    {
        get
        {
            return InitScript.Lifes;
        }
        set
        {
            InitScript.Lifes = value;
        }
    }

    public int CapOfLife = 5;
    public float TotalTimeForRestLifeHours = 0;
    public float TotalTimeForRestLifeMin = 15;
    public float TotalTimeForRestLifeSec = 60;

    // 先获取积分是多少（不用了）
    public int FirstGems = 20;

    // 创建一个静态变量来现在有的Gems/MCC
    public static int Gems;
    //显示MCC余额的变量
    public static string DisplayBalance;
    // 创建一个静态变量来存储用户可以去兑换的MCC积分余额
    public static int MCCPointBalance;

    public static int waitedPurchaseGems;
    private int BoostExtraMoves;
    private int BoostPackages;
    private int BoostStripes;
    private int BoostExtraTime;
    private int BoostBomb;
    private int BoostColorful_bomb;
    private int BoostHand;
    private int BoostRandom_color;
    public List<AdEvents> adsEvents = new List<AdEvents>();

    public static bool sound = false;
    public static bool music = false;
    private bool adsReady;
    public bool enableUnityAds;
    public bool enableGoogleMobileAds;
    public bool enableChartboostAds;
    public string rewardedVideoZone;
    public string nonRewardedVideoZone;
    public int ShowChartboostAdsEveryLevel;
    public int ShowAdmobAdsEveryLevel;
    public int dailyRewardedFrequency;//2.2.2
    public RewardedAdsTime dailyRewardedFrequencyTime;//2.2.3
    public int[] dailyRewardedShown;
    public DateTime[] dailyRewardedShownDate;
    private bool leftControl;
#if GOOGLE_MOBILE_ADS
    private InterstitialAd interstitial;
    private AdRequest requestAdmob;
#endif
    public string admobUIDAndroid;
    public string admobUIDIOS;
    public string admobRewardedUIDAndroid;
    public string admobRewardedUIDIOS;
    public bool LoginEnable;

    public int ShowRateEvery;
    public string RateURL;
    private GameObject rate;
    public int rewardedGems = 5;
    public bool losingLifeEveryGame;
    public static Sprite profilePic;
    public GameObject facebookButton;
    private string lastResponse = string.Empty;

    protected string LastResponse
    {
        get
        {
            return this.lastResponse;
        }

        set
        {
            this.lastResponse = value;
        }
    }

    private string status = "Ready";
    public string RateURLIOS; //2.1.5

    protected string Status
    {
        get
        {
            return this.status;
        }

        set
        {
            this.status = value;
        }
    }

    // Use this for initialization
    void Awake()
    {
        // When the app starts, set up the Thirdweb SDK
        // Here, we're setting up a read-only instance on the "goerli" test network.

        //ChuciQin
        sdk = new ThirdwebSDK("binance-testnet");
        // Debug.Log("Wallet Public Key is（in initScript）: " + StartScreenScript.WalletPublicKey);
        // StartCoroutine(PostPublicKey());
        // GetBalance();
        // StartCoroutine(GetUserInfo());

        Application.targetFrameRate = 60;
        Instance = this;
        RestLifeTimer = PlayerPrefs.GetFloat("RestLifeTimer");
        DateOfExit = PlayerPrefs.GetString("DateOfExit", "");
        print(DateOfExit);

        //ChuciQin
        //调用GameScreenScript中的GetBalance()方法来获取到用户MCC的余额
        // GameScreenScript.GetBalance();
        //让Gems等于用户的MCC余额,GameScreenScript.DisplayBalance也要变成数字,向下取整
        // Gems = int.Parse(GameScreenScript.DisplayBalance);
        // Gems = GameScreenScript.DisplayBalance;
        Gems = PlayerPrefs.GetInt("Gems");

        lifes = PlayerPrefs.GetInt("Lifes");
        {//2.2.2 rewarded limit
            dailyRewardedShown = new int[Enum.GetValues(typeof(RewardedAdsType)).Length];
            dailyRewardedShownDate = new DateTime[Enum.GetValues(typeof(RewardedAdsType)).Length];
            for (int i = 0; i < dailyRewardedShown.Length; i++)
            {
                dailyRewardedShown[i] = PlayerPrefs.GetInt(((RewardedAdsType)i).ToString());
                dailyRewardedShownDate[i] = DateTimeManager.GetLastDateTime(((RewardedAdsType)i).ToString());
            }
        }

        //ChuciQin
        //TODO：估计要去掉这个。从服务器中读区是否为第一次加载
        if (PlayerPrefs.GetInt("Lauched") == 0)
        {    //First lauching
            lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", lifes);

            //ChuciQin
            Gems = FirstGems;
            //调用GameScreenScript中的GetBalance()方法来获取到用户MCC的余额
            // TODO


            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.SetInt("Music", 1);
            PlayerPrefs.SetInt("Sound", 1);

            PlayerPrefs.SetInt("Lauched", 1);
            PlayerPrefs.Save();
        }
        rate = Instantiate(Resources.Load("Prefabs/Rate")) as GameObject;
        rate.SetActive(false);
        rate.transform.SetParent(GameObject.Find("CanvasGlobal").transform);
        rate.transform.localPosition = Vector3.zero;
        rate.GetComponent<RectTransform>().anchoredPosition = (Resources.Load("Prefabs/Rate") as GameObject).GetComponent<RectTransform>().anchoredPosition;
        rate.transform.localScale = Vector3.one;

        if (gameObject.GetComponent<AspectCamera>() == null) gameObject.AddComponent<AspectCamera>().map = FindObjectOfType<LevelsMap>().transform.Find("map_background_01").GetComponent<SpriteRenderer>().sprite; //gameObject.AddComponent<AspectCamera>().topPanel = GetComponent<LevelManager>().Level.transform.Find("Canvas/Panel/Panel/Panel").GetComponent<RectTransform>();//2.2.2

        GameObject.Find("Music").GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Music");
        SoundBase.Instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Sound");

#if UNITY_ADS//2.1.1
        enableUnityAds = true;
#else
        enableUnityAds = false;
#endif
#if CHARTBOOST_ADS//1.6.1
        enableChartboostAds = true;
#else
        enableChartboostAds = false;
#endif

#if FACEBOOK
        FacebookManager fbManager = gameObject.AddComponent<FacebookManager>();
        fbManager.facebookButton = facebookButton;
#endif


#if GOOGLE_MOBILE_ADS
        enableGoogleMobileAds = true;//1.6.1
#if UNITY_ANDROID
        MobileAds.Initialize(admobUIDAndroid);//2.1.6
        interstitial = new InterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
        MobileAds.Initialize(admobUIDIOS);//2.1.6
        interstitial = new InterstitialAd(admobUIDIOS);
#else
        MobileAds.Initialize(admobUIDAndroid);//2.1.6
		interstitial = new InterstitialAd (admobUIDAndroid);
#endif

        // Create an empty ad request.
        requestAdmob = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(requestAdmob);
        interstitial.OnAdLoaded += HandleInterstitialLoaded;
        interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
#else
        enableGoogleMobileAds = false;//1.6.1
#endif
        Transform canvas = GameObject.Find("CanvasGlobal").transform;
        foreach (Transform item in canvas)
        {
            item.gameObject.SetActive(false);
        }
    }

    //     IEnumerator PostPublicKey()
    // {
    //     // 创建一个新的UnityWebRequest，并设置为POST请求
    //     UnityWebRequest www = new UnityWebRequest("https://nodejs.meme-crush.com/register", "POST");

    //     // 创建一个新的JSON对象来存储公钥
    //     string json = JsonUtility.ToJson(new { publicKey = StartScreenScript.WalletPublicKey });

    //     // 将JSON对象转换为字节数组
    //     byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

    //     // 设置UnityWebRequest的body为字节数组
    //     www.uploadHandler = new UploadHandlerRaw(bodyRaw);

    //     // 设置UnityWebRequest的header为application/json
    //     www.SetRequestHeader("Content-Type", "application/json");

    //     // 发送UnityWebRequest
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log(www.error);
    //     }
    //     else
    //     {
    //         Debug.Log("Public key sent successfully in initScript");
    //     }
    // }

    // public async void GetBalance()
    // {
    //     // 创建一个新的合约对象
    //     Contract contract = sdk.GetContract("0x73F23f7825cB4f692c817556Ff03C9ed3B6719A5");

    //     // 使用合约对象来获取用户的余额
    //     var data = await contract.ERC20.BalanceOf(StartScreenScript.WalletPublicKey);

    //     // 将余额存储在 DisplayBalance 变量中
    //     DisplayBalance = data.displayValue;

    //     // 输出余额到控制台
    //     Debug.Log("Balance in initscript is: " + DisplayBalance);
    // }

    //https://nodejs.meme-crush.com/account/updatePoint
    // {
    //     "publicKey" : "0x6c123d4cA3678b9AD5987D393266f0036bd9D022",
    //     "pointGained" : 200
    // }

    // public async void UpdateMCCPoints(int MCCPointBalance)
    // {
    //     // 创建一个新的UnityWebRequest，并设置为PUT请求
    //     UnityWebRequest www = new UnityWebRequest("https://nodejs.meme-crush.com/account/updatePoint", "PUT");

    //     // 创建一个新的JSON对象来存储公钥
    //     string json = JsonUtility.ToJson(new { publicKey = StartScreenScript.WalletPublicKey, point = MCCPointBalance });

    //     // 将JSON对象转换为字节数组
    //     byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

    //     // 设置UnityWebRequest的body为字节数组
    //     www.uploadHandler = new UploadHandlerRaw(bodyRaw);

    //     // 设置UnityWebRequest的header为application/json
    //     www.SetRequestHeader("Content-Type", "application/json");

    //     // 发送UnityWebRequest
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log(www.error);
    //     }
    //     else
    //     {
    //         Debug.Log("Update MCC Points" + MCCPointBalance);
    //     }
    // }

    // public async void GetUserInfo()
    // public IEnumerator GetUserInfo()
    // {
    //     // 创建一个新的UnityWebRequest，并设置为POST请求
    //     UnityWebRequest www = new UnityWebRequest("https://nodejs.meme-crush.com/register", "POST");

    //     // 创建一个新的JSON对象来存储公钥
    //     string json = JsonUtility.ToJson(new { publicKey = StartScreenScript.WalletPublicKey });

    //     // 将JSON对象转换为字节数组
    //     byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

    //     // 设置UnityWebRequest的body为字节数组
    //     www.uploadHandler = new UploadHandlerRaw(bodyRaw);

    //     // 设置UnityWebRequest的header为application/json
    //     www.SetRequestHeader("Content-Type", "application/json");

    //     // 发送UnityWebRequest
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log(www.error);
    //     }
    //     else
    //     {
    //         //拿到用户的信息            
    //         string responseJson = www.downloadHandler.text;
    //         Debug.Log("Response (IEnumerator GetUserInfo): " + responseJson);

    //         // 创建一个新的ResponseData实例来存储解析的数据
    //         ResponseData response = new ResponseData();
            
    //         // 使用JsonUtility.FromJsonOverwrite方法将JSON数据解析到ResponseData实例中
    //         JsonUtility.FromJsonOverwrite(responseJson, response);
            
    //         // 打印或使用解析后的数据
    //         Debug.Log("Response code: " + response.code);
    //         Debug.Log("Response message: " + response.message);
    //         Debug.Log("Public Key: " + response.data.publicKey);
    //         Debug.Log("Point Remain: " + response.data.pointRemain);

    //         // response.data.pointRemain
    //         MCCPointBalance = response.data.pointRemain;
            
    //         foreach (PointHistory ph in response.data.pointHistory)
    //         {
    //             Debug.Log("Point: " + ph.point + ", Date: " + ph.date);
    //         }
    //     }
    // }


#if GOOGLE_MOBILE_ADS

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        print("HandleInterstitialLoaded event received.");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }
#endif


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            leftControl = true;
        if (Input.GetKeyUp(KeyCode.LeftControl))
            leftControl = false;

        if (Input.GetKeyUp(KeyCode.U))
        {
            print("11");
            for (int i = 1; i < GameObject.Find("Levels").transform.childCount; i++)
            {
                SaveLevelStarsCount(i, 1);
                Debug.Log("InitScript Update SaveLevelStarsCount");
            }

        }
    }

    public void SaveLevelStarsCount(int level, int starsCount)
    {
        Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
        PlayerPrefs.SetInt(GetLevelKey(level), starsCount);

    }

    private string GetLevelKey(int number)
    {
        return string.Format("Level.{0:000}.StarsCount", number);
    }

    public bool GetRewardedUnityAdsReady()
    {
#if UNITY_ADS

        rewardedVideoZone = "rewardedVideo";
        if (Advertisement.IsReady(rewardedVideoZone))
        {
            return true;
        }
        else
        {
            rewardedVideoZone = "rewardedVideoZone";
            if (Advertisement.IsReady(rewardedVideoZone))
            {
                return true;
            }
        }
#endif

        return false;
    }

    public void ShowRewardedAds()
    {
#if UNITY_ADS
        Debug.Log("show Unity Rewarded ads video in " + LevelManager.THIS.gameStatus);

        if (GetRewardedUnityAdsReady())
        {
            Advertisement.Show(rewardedVideoZone, new ShowOptions
            {
                resultCallback = result =>
                {
                    if (result == ShowResult.Finished)
                    {
                        CheckRewardedAds();
                    }
                }
            });
        }

#elif GOOGLE_MOBILE_ADS//2.2
        bool stillShow = true;
#if UNITY_ADS
        stillShow = !GetRewardedUnityAdsReady ();
#endif
        if(stillShow)
        {
            Debug.Log("show Admob Rewarded ads video in " + LevelManager.THIS.gameStatus);
            RewAdmobManager.THIS.ShowRewardedAd(CheckRewardedAds);
        }
#endif
    }

    public bool RewardedReachedLimit(RewardedAdsType type)//2.2.2
    {
        if (dailyRewardedFrequency == 0) return false;
        dailyRewardedShown[(int)type] = PlayerPrefs.GetInt(type.ToString());
        if (!DateTimeManager.IsPeriodPassed(type.ToString())) return true;
        if (dailyRewardedFrequency > 0 && dailyRewardedShown[(int)type] >= dailyRewardedFrequency) return true;
        // if (Random.Range(0, 5) > 0) return true;
        dailyRewardedShown[(int)type]++;
        PlayerPrefs.SetInt(type.ToString(), dailyRewardedShown[(int)type]);
        if (dailyRewardedShown[(int)type] >= dailyRewardedFrequency) DateTimeManager.SetDateTimeNow(type.ToString());
        PlayerPrefs.Save();

        return false;
    }

    public void CheckAdsEvents(GameState state)
    {    //1.4 added
        foreach (AdEvents item in adsEvents)
        {
            if (item.gameEvent == state)
            {
                //1.5   1.6.1
                //				if ((LevelManager.THIS.gameStatus == GameState.GameOver || LevelManager.THIS.gameStatus == GameState.Pause ||
                //				    LevelManager.THIS.gameStatus == GameState.Playing || LevelManager.THIS.gameStatus == GameState.PrepareGame || LevelManager.THIS.gameStatus == GameState.PreWinAnimations ||
                //				    LevelManager.THIS.gameStatus == GameState.RegenLevel || LevelManager.THIS.gameStatus == GameState.Win)) {

                item.calls++;  //1.6
                if (item.calls % item.everyLevel == 0)
                    ShowAdByType(item.adType);
                //				} else {
                //					ShowAdByType (item.adType);
                //
                //				}
            }
        }
    }

    void ShowAdByType(AdType adType)
    { //1.4 added
        if (adType == AdType.AdmobInterstitial && enableGoogleMobileAds)
            ShowAds(false);
        else if (adType == AdType.UnityAdsVideo && enableUnityAds)
            ShowVideo();
        else if (adType == AdType.ChartboostInterstitial && enableChartboostAds)
            ShowAds(true);

    }

    public void ShowVideo()
    {  //1.4 added
#if UNITY_ADS
        Debug.Log("show Unity ads video in " + LevelManager.THIS.gameStatus);

        if (Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
        }
        else
        {
            if (Advertisement.IsReady("defaultZone"))
            {
                Advertisement.Show("defaultZone");
            }
        }
#endif
    }


    public void ShowAds(bool chartboost = true)
    {
        if (chartboost)
        {
#if CHARTBOOST_ADS
            Debug.Log("show Chartboost Interstitial in " + LevelManager.THIS.gameStatus);

            Chartboost.showInterstitial(CBLocation.Default);
            Chartboost.cacheInterstitial(CBLocation.Default);
#endif
        }
        else
        {
#if GOOGLE_MOBILE_ADS
            Debug.Log("show Google mobile ads Interstitial in " + LevelManager.THIS.gameStatus);
            if (interstitial.IsLoaded())
            {
                interstitial.Show();
#if UNITY_ANDROID
                interstitial = new InterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
                interstitial = new InterstitialAd(admobUIDIOS);
#else
				interstitial = new InterstitialAd (admobUIDAndroid);
#endif

                // Create an empty ad request.
                requestAdmob = new AdRequest.Builder().Build();
                // Load the interstitial with the request.
                interstitial.LoadAd(requestAdmob);
            }
#endif
        }
    }

    public void ShowRate()
    {
        // ChuciQin
        // rate.SetActive(true);
        rate.SetActive(false);
    }


    void CheckRewardedAds()
    {
        RewardIcon reward = GameObject.Find("CanvasGlobal").transform.Find("Reward").GetComponent<RewardIcon>();
        if (currentReward == RewardedAdsType.GetGems)
        {
            reward.SetIconSprite(0);

            reward.gameObject.SetActive(true);
            AddGems(rewardedGems);
            GameObject.Find("CanvasGlobal").transform.Find("GemsShop").GetComponent<AnimationManager>().CloseMenu();
        }
        else if (currentReward == RewardedAdsType.GetLifes)
        {
            reward.SetIconSprite(1);
            reward.gameObject.SetActive(true);
            RestoreLifes();
            GameObject.Find("CanvasGlobal").transform.Find("LiveShop").GetComponent<AnimationManager>().CloseMenu();
        }
        else if (currentReward == RewardedAdsType.GetGoOn)
        {
            GameObject.Find("CanvasGlobal").transform.Find("PreFailed").GetComponent<AnimationManager>().GoOnFailed();
        }

    }

    public void SetGems(int count)
    {
        Gems = count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
    }


    public void AddGems(int count)
    {
        Gems += count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
        NetworkManager.currencyManager.IncBalance(count);
#endif

    }

    public void SpendGems(int count)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.cash);
        Gems -= count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
        NetworkManager.currencyManager.DecBalance(count);
#endif

    }


    public void RestoreLifes()
    {
        lifes = CapOfLife;
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.Save();
    }

    public void AddLife(int count)
    {
        lifes += count;
        if (lifes > CapOfLife)
            lifes = CapOfLife;
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.Save();
    }

    public int GetLife()
    {
        if (lifes > CapOfLife)
        {
            lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.Save();
        }
        return lifes;
    }

    public void PurchaseSucceded()
    {
        AddGems(waitedPurchaseGems);
        waitedPurchaseGems = 0;
    }

    public void SpendLife(int count)
    {
        if (lifes > 0)
        {
            lifes -= count;
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.Save();
        }
        //else
        //{
        //    GameObject.Find("Canvas").transform.Find("RestoreLifes").gameObject.SetActive(true);
        //}
    }

    public void BuyBoost(BoostType boostType, int price, int count)
    {
        PlayerPrefs.SetInt("" + boostType, count);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
        NetworkManager.dataManager.SetBoosterData();
#endif

        //   ReloadBoosts();
    }

    public void SpendBoost(BoostType boostType)
    {
        PlayerPrefs.SetInt("" + boostType, PlayerPrefs.GetInt("" + boostType) - 1);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
        NetworkManager.dataManager.SetBoosterData();
#endif
    }
    //void ReloadBoosts()
    //{
    //    BoostExtraMoves = PlayerPrefs.GetInt("" + BoostType.ExtraMoves);
    //    BoostPackages = PlayerPrefs.GetInt("" + BoostType.Packages);
    //    BoostStripes = PlayerPrefs.GetInt("" + BoostType.Stripes);
    //    BoostExtraTime = PlayerPrefs.GetInt("" + BoostType.ExtraTime);
    //    BoostBomb = PlayerPrefs.GetInt("" + BoostType.Bomb);
    //    BoostColorful_bomb = PlayerPrefs.GetInt("" + BoostType.Colorful_bomb);
    //    BoostHand = PlayerPrefs.GetInt("" + BoostType.Hand);
    //    BoostRandom_color = PlayerPrefs.GetInt("" + BoostType.Random_color);

    //}
    //public void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
    //{
    //    PurchaseSucceded();
    //}

    //private void OnApplicationFocus(bool focus)//2.1.5 need to test music on
    //{
    //	var music = GameObject.Find("Music");
    //	if (music != null) music.GetComponent<AudioSource>().Play();
    //}

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (RestLifeTimer > 0)
            {
                PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
            }
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
    }

    void OnApplicationQuit()
    {   //1.4  added 
        if (RestLifeTimer > 0)
        {
            PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
        }
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
        PlayerPrefs.Save();
        //print(RestLifeTimer)
    }

    public void OnLevelClicked(object sender, LevelReachedEventArgs args)
    {
        if (EventSystem.current.IsPointerOverGameObject(-1))
            return;
        if (!GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.activeSelf && !GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.activeSelf && !GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.activeSelf && !GameObject.Find("CanvasGlobal").transform.Find("Settings").gameObject.activeSelf)  //2.1.6
        {
            //chuciqin
            //收门票
            //如果是第10关的话，要先让用户购买一些MCC，并且让用户花费（burn）60000个MCC
            if (args.Number == 10
            || args.Number == 20
            || args.Number == 30
            || args.Number == 40
            || args.Number == 50
            || args.Number == 60
            || args.Number == 70
            || args.Number == 80
            || args.Number == 90
            ){
                PlayerPrefs.SetInt("OpenLevel", args.Number);
                PlayerPrefs.Save();
                LevelManager.THIS.MenuPlayEvent();
                LevelManager.THIS.LoadLevel();
                openLevel = args.Number;
                //  currentTarget = targets[args.Number];
                GameObject.Find("CanvasGlobal").transform.Find("MenuPlayFirstPage").gameObject.SetActive(true);
                GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
            }else{
                PlayerPrefs.SetInt("OpenLevel", args.Number);
                PlayerPrefs.Save();
                LevelManager.THIS.MenuPlayEvent();
                LevelManager.THIS.LoadLevel();
                openLevel = args.Number;
                //  currentTarget = targets[args.Number];
                GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
            }

        }
    }

    void OnEnable()
    {
        LevelsMap.LevelSelected += OnLevelClicked;
    }

    void OnDisable()
    {
        LevelsMap.LevelSelected -= OnLevelClicked;

        //		if(RestLifeTimer>0){
        PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
        //		}
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
        PlayerPrefs.Save();
#if GOOGLE_MOBILE_ADS
        interstitial.OnAdLoaded -= HandleInterstitialLoaded;
        interstitial.OnAdFailedToLoad -= HandleInterstitialFailedToLoad;
#endif

    }

    #region FaceBook

#if FACEBOOK
    //	public void CallFBInit () {
    //		FB.Init (OnInitComplete, OnHideUnity);
    //
    //	}
    //
    //	private void OnInitComplete () {
    //		Debug.Log ("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
    //
    //	}
    //
    //	private void OnHideUnity (bool isGameShown) {
    //		Debug.Log ("Is game showing? " + isGameShown);
    //	}
    //
    //	void OnGUI () {
    //		if (LoginEnable) {
    //			InitScript.Instance.CallFBLogin ();
    //			LoginEnable = false;
    //		}
    //	}
    //
    //
    //	public void CallFBLogin () {
    //		FB.LogInWithReadPermissions (new List<string> () { "public_profile", "email", "user_friends" }, this.HandleResult);
    //	}
    //
    //	public void CallFBLoginForPublish () {
    //		// It is generally good behavior to split asking for read and publish
    //		// permissions rather than ask for them all at once.
    //		//
    //		// In your own game, consider postponing this call until the moment
    //		// you actually need it.
    //		FB.LogInWithPublishPermissions (new List<string> () { "publish_actions" }, this.HandleResult);
    //	}
    //
    //	void LoginCallback (IPayResult result) {
    //
    //		if (result.Error != null)
    //			lastResponse = "Error Response:\n" + result.Error;
    //		else if (!FB.IsLoggedIn) {
    //			lastResponse = "Login cancelled by Player";
    //		} else {
    //			lastResponse = "Login was successful!";
    //			if (loginForSharing) {
    //				loginForSharing = false;
    //				Share ();
    //			}
    //		}
    //		Debug.Log (lastResponse);
    //	}
    //
    //	private void CallFBLogout () {
    //		FB.LogOut ();
    //	}
    //
    //	public void Share () {
    //		if (!FB.IsLoggedIn) {
    //			loginForSharing = true;
    //			LoginEnable = true;
    //			Debug.Log ("not logged, logging");
    //		} else {
    //			FB.FeedShare (
    //				link: new Uri ("http://apps.facebook.com/" + FB.AppId + "/?challenge_brag=" + (FB.IsLoggedIn ? AccessToken.CurrentAccessToken.UserId : "guest")),
    //				linkName: FacebookSettings.AppLabels [0],
    //				linkCaption: "I just scored " + LevelManager.Score + " points! Try to beat me!"
    //            //picture: "https://fbexternal-a.akamaihd.net/safe_image.php?d=AQCzlvjob906zmGv&w=128&h=128&url=https%3A%2F%2Ffbcdn-photos-h-a.akamaihd.net%2Fhphotos-ak-xtp1%2Ft39.2081-0%2F11891368_513258735497916_1832270581_n.png&cfs=1"
    //			);
    //		}
    //	}
    //
    //	protected void HandleResult (IResult result) {
    //		if (result == null) {
    //			this.LastResponse = "Null Response\n";
    //			Debug.Log (this.LastResponse);
    //			return;
    //		}
    //
    //		//     this.LastResponseTexture = null;
    //
    //		// Some platforms return the empty string instead of null.
    //		if (!string.IsNullOrEmpty (result.Error)) {
    //			this.Status = "Error - Check log for details";
    //			this.LastResponse = "Error Response:\n" + result.Error;
    //			Debug.Log (result.Error);
    //		} else if (result.Cancelled) {
    //			this.Status = "Cancelled - Check log for details";
    //			this.LastResponse = "Cancelled Response:\n" + result.RawResult;
    //			Debug.Log (result.RawResult);
    //		} else if (!string.IsNullOrEmpty (result.RawResult)) {
    //			this.Status = "Success - Check log for details";
    //			this.LastResponse = "Success Response:\n" + result.RawResult;
    //			if (loginForSharing) {
    //				loginForSharing = false;
    //				Share ();
    //			}
    //
    //			Debug.Log (result.RawResult);
    //		} else {
    //			this.LastResponse = "Empty Response\n";
    //			Debug.Log (this.LastResponse);
    //		}
    //	}
#endif
    #endregion

}
