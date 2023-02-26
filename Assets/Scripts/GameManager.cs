using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    #region Serializable Variables

    // Menus objects
    public GameObject panelMenu; //Main menu
    public GameObject panelPlay; //In-game menu
    public GameObject panelGameOver;
    public GameObject panelPackSelector;
    public GameObject packsButtons;
    
    public GameObject panelLevelSelector;
    public GameObject panelLevelSelectorBackground;
    public Sprite[] buttonImages;
    
    public GameObject panelSettings;
    public GameObject levelsGridUI;
    public GameObject levelButton;
    public float levelScale = 1f;
    public Text currentLevelNumberUIText; //In game UI level number

    // All levels and their packs
    public GameObject[] levels;
    public int[] pack1;
    public int[] pack2;
    public int[] pack3;
    public int[] pack4;

    public int[] pack5;

    //Background variables
    public Sprite[] backgroundTextures;
    public GameObject background;
    public GameObject nextBackground;
    public bool IsStuck { get; set; }
    
    //Transition mask
    public GameObject transitionMask;

    #endregion

    #region Private Variables

    // Variable with all packs inside
    private int[][] allPacks;

    //What levels are accessible
    private bool[][] levelsUnlocked;

    //Win condition fulfilled
    private bool isCurrentLevelCompleted;

    //Which level will the button "Continue" in main menu load
    private int lastLevelPlayedNumber;

    //Level selector GUI objects
    private GameObject levelsGrid;
    private int currentPack;

    //Bool for transition animation between levels
    private bool retrying;

    //Background variables
    private bool preselectedBackground;
    private bool backgroundIsNext;
    public int currentBackground = 0;
    private int lastBackground = 0;

    //All game states
    private enum GameState {
        Menu,
        PackSelector,
        LevelSelector,
        Settings,
        ContinueButtonClicked,
        Play,
        LevelCompleted,
        LoadLevel,
        GameOver
    }

    private GameState currentGameState;
    private bool unloadlevel = true;
    private bool isSwitchingState;
    private GameObject transitionLevel;

    //Which levels are currently loaded
    private GameObject currentLevel;
    private GameObject nextLevel;
    private bool loadNewLevel;
    private bool unfinishedLevel;
    private int CurrentLevelNumber { get; set; }
    
    //Transitions
    public Animator transition;
    
    // level counter for ads
    private int levelsWithoutAds = 0;

    #endregion

    #region Buttons Functions

    public void GotoMainMenu() {
        SwitchState(GameState.Menu);
    }

    public void GotoPackSelect() {
        SwitchState(GameState.PackSelector);
    }

    public void ContinueButtonClicked() {
        SwitchState(GameState.ContinueButtonClicked);
    }

    public void PackSelectorButtonClicked() {
        SwitchState(GameState.PackSelector);
    }

    public void LevelSelectorButtonClicked(int packSelected) {
        currentPack = packSelected;
        SwitchState(GameState.LevelSelector);
    }

    public void ExitLevelButtonClicked() {
        unloadlevel = true;
        currentPack = GetLevelPackPosition(CurrentLevelNumber)[0];
        unfinishedLevel = true;
        SwitchState(GameState.LevelSelector);
    }

    public void SetIsCurrentLevelCompleted(bool @bool) {
        isCurrentLevelCompleted = @bool;
    }

    public void RetryButtonClicked() {
        if (isSwitchingState || retrying) return;

        retrying = true;
        //StartCoroutine(UnlockMovement(1f));
        loadNewLevel = true;
        SwitchState(GameState.LoadLevel);
        // preselectedBackground = true;
    }

    public void DeleteAllData() {
        SaveSystem.SeriouslyDeleteAllSaveFiles();

        levelsUnlocked = new bool[allPacks.Length][];
        for (var i = 0; i < levelsUnlocked.Length; i++) {
            levelsUnlocked[i] = new bool[allPacks[i].Length];
        }

        levelsUnlocked[0][0] = true;
        lastLevelPlayedNumber = 0;
    }

    public void GotoSettings() {
        if (!isSwitchingState) {
            SwitchState(GameState.Settings);
        }
    }

    #endregion

    #region Unity Functions

    private void Start() {
        
        // Initialize ads
        IronSource.Agent.init("18ccc2fbd", IronSourceAdUnits.INTERSTITIAL);
        IronSource.Agent.validateIntegration();
        
        // Load saved data
        LoadSavedProgress();

        Instance = this;
        //Preload last played level and deactivate it.
        nextLevel = Instantiate(levels[lastLevelPlayedNumber]);
        nextLevel.SetActive(false);

        currentBackground = Random.Range(0, backgroundTextures.Length);
        lastBackground = currentBackground;

        //Debug (unlock all levels):
        //////////////////////////
        // for (int j = 0; j < levelsUnlocked.Length; j++)
        // {
        //
        //     for (int i = 0; i < levelsUnlocked[j].Length; i++)
        //     {
        //
        //         levelsUnlocked[j][i] = true;
        //     }
        // }
        ////////////////////////////

        SwitchState(GameState.Menu);
    }
    
    # region Ads

    void OnApplicationPause(bool isPaused) {                 
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void OnEnable()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
        
        // Interstitial
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;        
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent; 
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent; 
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
    
        //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
    }
    
    private void SdkInitializationCompletedEvent()
    {
         IronSource.Agent.loadInterstitial();
    }

    // Invoked when the initialization process has failed.
// @param description - string - contains information about the failure.
    void InterstitialAdLoadFailedEvent (IronSourceError error) {
    }
// Invoked when the ad fails to show.
// @param description - string - contains information about the failure.
    void InterstitialAdShowFailedEvent(IronSourceError error) {
    }
// Invoked when end user clicked on the interstitial ad
    void InterstitialAdClickedEvent () {
    }
// Invoked when the interstitial ad closed and the user goes back to the application screen.
    void InterstitialAdClosedEvent () {
    }
// Invoked when the Interstitial is Ready to shown after load function is called
    void InterstitialAdReadyEvent() {
    }
// Invoked when the Interstitial Ad Unit has opened
    void InterstitialAdOpenedEvent() {
    }
// Invoked right before the Interstitial screen is about to open.
// NOTE - This event is available only for some of the networks. 
// You should not treat this event as an interstitial impression, but rather use InterstitialAdOpenedEvent
    void InterstitialAdShowSucceededEvent() {
    }
    
    /************* Interstitial AdInfo Delegates *************/
// Invoked when the interstitial ad was loaded succesfully.
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo) {
    }
// Invoked when the initialization process has failed.
    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError) {
    }
// Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo) {
    }
// Invoked when end user clicked on the interstitial ad
    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo) {
    }
// Invoked when the ad failed to show.
    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo) {
    }
// Invoked when the interstitial ad closed and the user went back to the application screen.
    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo) {
    }
// Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
// This callback is not supported by all networks, and we recommend using it only if  
// it's supported by all networks you included in your build. 
    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo) {
    }
    # endregion


    private void Update() {
        switch (currentGameState) {
            case GameState.Menu:
                break;
            case GameState.ContinueButtonClicked:
                break;
            case GameState.Play:
                UpdatePlay();
                break;
            case GameState.LevelCompleted:
                break;
            case GameState.LoadLevel:
                break;
            case GameState.GameOver:
                UpdateGameOver();
                break;
            case GameState.PackSelector:
                break;
            case GameState.LevelSelector:
                break;
            case GameState.Settings:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    #endregion

    #region Private Functions
    private void SwitchState(GameState newGameState, float delay = 0) {
        StartCoroutine(SwitchDelay(newGameState, delay));
    }

    private IEnumerator SwitchDelay(GameState newGameState, float delay) {
        isSwitchingState = true;
        yield return new WaitForSeconds(delay);
        EndState();
        currentGameState = newGameState;
        BeginState(newGameState);
        isSwitchingState = false;
    }

    /**
     * Reset levels unlocked, set first level to unlocked, load saved data.
     */
    private void LoadSavedProgress() {
        allPacks = new[] {pack1, pack2, pack3, pack4, pack5};
        levelsUnlocked = new bool[allPacks.Length][];
        for (var i = 0; i < levelsUnlocked.Length; i++) {
            levelsUnlocked[i] = new bool[allPacks[i].Length];
        }

        levelsUnlocked[0][0] = true;

        if (SaveSystem.LoadData() == null) return;

        levelsUnlocked = SaveSystem.LoadData().packsUnlocked;
        lastLevelPlayedNumber = SaveSystem.LoadData().lastLevelPlayed;
    }
    private void UpdateGameOver() {
        if (Input.touchCount > 0) {
            
            SwitchState(GameState.Menu);
        }
    }

    private void UpdatePlay() {
        if (Vector3.Distance(transitionMask.transform.position, new Vector3(0, 0, 0)) > 5f)
        {
            transitionMask.transform.position = Vector3.MoveTowards(transitionMask.transform.position, new Vector3(0, 0, 0), 0.1f);
        }
            
        if (currentLevel != null && isCurrentLevelCompleted && !isSwitchingState) {
            SwitchState(GameState.LevelCompleted);
        }
        else if (IsStuck && !isSwitchingState && !isCurrentLevelCompleted) {
            Destroy(currentLevel);
            SwitchState(GameState.LoadLevel);
        }

    }

    /**
     *  What to do when switching to a new state
     */
    private void BeginState(GameState newGameState) {
        switch (newGameState) {
            case GameState.Menu: BeginMenu(); break;
            case GameState.PackSelector: BeginPackSelector(); break;
            case GameState.LevelSelector: BeginLevelSelector(); break;
            case GameState.ContinueButtonClicked: BeginContinueButtonClicked(); break;
            case GameState.LoadLevel: BeginLoadLevel(); break;
            case GameState.Play: BeginPlay(); break;
            case GameState.LevelCompleted: BeginLevelCompleted(); break;
            case GameState.GameOver: BeginGameOver(); break;
            case GameState.Settings: BeginSettings(); break;
            default: throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
        }
    }

    private void BeginSettings() {
        panelSettings.SetActive(true);
    }

    private void BeginGameOver() {
        panelGameOver.SetActive(true);
        
    }

    private void BeginLevelCompleted() {
        lastBackground = currentBackground;

        // panelLevelCompleted.SetActive(true);

        var levelPosition = GetLevelPackPosition(CurrentLevelNumber);
        if (allPacks[levelPosition[0]][levelPosition[1]] ==
            allPacks[^1][allPacks[^1].Length - 1]) {
            SwitchState(GameState.GameOver, 2f);
        }
        else {
            UnlockNextLevel(CurrentLevelNumber);
            SwitchState(GameState.LoadLevel);
        }

        SaveSystem.SaveData(levelsUnlocked, lastLevelPlayedNumber);
    }

    private void BeginPlay() {
        

        retrying = false;
        panelPlay.SetActive(true);
        IsStuck = false;
        isCurrentLevelCompleted = false;
        levelsWithoutAds++;
        switch (levelsWithoutAds)
        {
            case >= 3:
                if (IronSource.Agent.isInterstitialReady())
                {
                    levelsWithoutAds = 0;
                    Debug.Log("Showing Interstitial");
                    IronSource.Agent.showInterstitial();
                    IronSource.Agent.loadInterstitial();
                }
                else
                {
                    Debug.Log("Interstitial not ready");
                }
                break;
        }

        StartCoroutine(UnlockMovement(0.0f));
    }

    private void SwitchLayerMasks()
    {
        foreach (SpriteRenderer spriteRenderer in currentLevel.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
        }

        if (backgroundIsNext)
        {
            background.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            nextBackground.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
        else
        {
            background.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            nextBackground.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    private void BeginLoadLevel()
    {
        lastBackground = currentBackground;
        loadNewLevel = true;

        Vector3 winPosition = GetWinPosition(CurrentLevelNumber);
        if (currentLevel != null) {
            winPosition = GetWinPosition(currentLevel);
            foreach (Transform child in currentLevel.transform) {
                if (child.GetComponent<Player>() != null) {
                        // child.GetComponent<Player>().SetIsCosmetic(true);
                        child.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            if (transitionLevel != null) {
                Destroy(transitionLevel);
            }

            transitionLevel = currentLevel;
            currentLevel = null;
            foreach (SpriteRenderer spriteRenderer in transitionLevel.GetComponentsInChildren<SpriteRenderer>()) {
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }

            
        }

        if (retrying)
        {
            foreach (Button button in panelPlay.GetComponentsInChildren<Button>())
            {
                if (button.gameObject.name == "Retry")
                {
                    winPosition = new Vector3(0,-15,0);
                }
            }

        }
        
        
        if (loadNewLevel) {
            currentLevel = Instantiate(levels[CurrentLevelNumber]);
            Destroy(nextLevel);
        }
        else {
            currentLevel = nextLevel;
            currentLevel.SetActive(true); 
        }
        
        if (CurrentLevelNumber + 1 < levels.Length) {
            nextLevel = Instantiate(levels[CurrentLevelNumber + 1]);
            nextLevel.SetActive(false);
        }
        else {
            nextLevel = null;
        }

        foreach (SpriteRenderer spriteRenderer in currentLevel.GetComponentsInChildren<SpriteRenderer>()) {
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        
        if (nextLevel != null)
            foreach (SpriteRenderer spriteRenderer in nextLevel.GetComponentsInChildren<SpriteRenderer>()) {
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        
        
        FitToScreen(currentLevel);

        loadNewLevel = false;

        currentLevelNumberUIText.text = (GetLevelPackPosition(CurrentLevelNumber)[0] + 1) + "-" +
                                        (GetLevelPackPosition(CurrentLevelNumber)[1] + 1);
        IsStuck = false;
        lastLevelPlayedNumber = CurrentLevelNumber;


        SaveSystem.SaveData(levelsUnlocked, lastLevelPlayedNumber);
        SwitchBackgroundsTransition();
        StartCoroutine(Transition(winPosition));
    }
    
    private void FitToScreen(GameObject level) {
        var combinedBounds = new Bounds(Vector3.zero, Vector3.zero);
        foreach (SpriteRenderer spriteRenderer in level.GetComponentsInChildren<SpriteRenderer>()) {
            combinedBounds.Encapsulate(spriteRenderer.bounds);
        }
        float levelWidth = combinedBounds.size.x;
        float levelHeight = combinedBounds.size.y;
        
        var camera = Camera.main;
        if (camera == null) {
            throw new Exception("No camera found");
        }
        float cameraWidth = camera.orthographicSize * 2 * camera.aspect;
        float cameraHeight = camera.orthographicSize * 2;
        levelScale = 1;
        levelScale = cameraWidth / (levelWidth+1);

        if (levelHeight > cameraHeight) {
            levelScale = cameraHeight / levelHeight;
        }

        level.transform.localScale = new Vector3(levelScale, levelScale, 1);
        
        // Now center the level in the camera
        Vector3 levelCenter = combinedBounds.center;
        level.transform.position = new Vector3(-levelCenter.x, -levelCenter.y, 0);
        
    }
    
    private Vector3 GetWinPosition(int levelNumber) {
        Vector3 winPosition = Vector3.zero;
        foreach (Transform child in levels[levelNumber].transform) {
            if (child.name == "Win") {
                winPosition = child.position;
            }
        }

        return winPosition;
    }
    
    private Vector3 GetWinPosition(GameObject level) {
        Vector3 winPosition = Vector3.zero;
        foreach (Transform child in level.transform) {
            if (child.name == "Win") {
                winPosition = child.position;
            }
        }

        return winPosition;
    }

    private void SwitchBackgroundsTransition()
    {
        if (!preselectedBackground) {
            while (lastBackground == currentBackground) {
                currentBackground = Random.Range(0, backgroundTextures.Length);
            }
        }
        if (!backgroundIsNext)
        {
            SpriteRenderer spriteRenderer = nextBackground.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = backgroundTextures[currentBackground];
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            background.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            backgroundIsNext = true;
        }
        else
        {
            SpriteRenderer spriteRenderer = background.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = backgroundTextures[currentBackground];
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            nextBackground.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            backgroundIsNext = false;
        }
        
        preselectedBackground = false;
    }

    private IEnumerator Transition(Vector3 winPosition) {

        SetMaskVisibility(true);
        transitionMask.transform.position = winPosition;
        transition.SetTrigger("StartLevelTransition");
        SwitchState(GameState.Play);
        yield return new WaitForSeconds(1f);
        if (transitionLevel != null) { 
            Destroy(transitionLevel);
        }
        unloadlevel = false;
        SwitchLayerMasks();
        SetMaskVisibility(false);
        foreach (Button button in panelPlay.GetComponentsInChildren<Button>())
        {
            button.interactable = button.name is not ("Rewind" or "Hints");
        }
        
    }
    
    private void SetMaskVisibility(bool visible)
    {
        transitionMask.SetActive(visible);
    }
    private void BeginContinueButtonClicked() {
        CurrentLevelNumber = lastLevelPlayedNumber;
        SwitchState(GameState.LoadLevel);
    }

    private void BeginLevelSelector() {
        panelLevelSelector.SetActive(true);
        levelsGrid = levelsGridUI;
        var buttons = new GameObject[allPacks[currentPack].Length];

        for (var i = 0; i < allPacks[currentPack].Length; i++) {
            var tmp = i;
            buttons[i] = Instantiate(levelButton);
            if (!levelsUnlocked[currentPack][tmp]) {
                buttons[i].GetComponent<Button>().interactable = false;
            }

            buttons[i].transform.SetParent(levelsGrid.transform);
            buttons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText((i + 1).ToString());
            buttons[i].transform.GetComponent<Button>().onClick
                .AddListener(() => {
                    CurrentLevelNumber = allPacks[currentPack][tmp];
                    loadNewLevel = true;//When loading a different level than the preloaded one.
                    SwitchState(GameState.LoadLevel);
                });
            // change button image to a random button image
            var buttonImage = buttons[i].GetComponent<Image>();
            buttonImage.sprite = buttonImages[Random.Range(0, buttonImages.Length)];
            
        }
        
        //Set the dimensions of the background image to fit the grid + padding = half the size of a cell
        var gridLayout = levelsGrid.GetComponent<GridLayoutGroup>();
        var levelSelectorBackground = panelLevelSelectorBackground.GetComponent<RectTransform>();
        var gridLayoutWidth = gridLayout.cellSize.x * gridLayout.constraintCount + gridLayout.spacing.x * (gridLayout.constraintCount - 1);
        var gridLayoutHeight = gridLayout.cellSize.y * (Mathf.CeilToInt((float)buttons.Length / gridLayout.constraintCount) + .5f) + gridLayout.spacing.y * (Mathf.CeilToInt((float)buttons.Length / gridLayout.constraintCount) - 1);
        levelSelectorBackground.sizeDelta = new Vector2(gridLayoutWidth, gridLayoutHeight);
    }

    private void BeginPackSelector() {
        var buttons2 = new GameObject[packsButtons.transform.childCount];
        for (var i = 0; i < buttons2.Length; i++) {
            buttons2[i] = packsButtons.transform.GetChild(i).gameObject;
            buttons2[i].transform.GetComponent<Button>().interactable = levelsUnlocked[i][0];
        }

        panelPackSelector.SetActive(true);
    }

    /**
     *     In case of switching to menu do:
     *        
     *       - Destroy current level if available
     *       - Set cursor to visible
     *       - Set menu panel to visible
     */
    private void BeginMenu() {
        Cursor.visible = true;
        panelMenu.SetActive(true);
    }

    private void EndState() {
        switch (currentGameState) {
            case GameState.Menu:
                panelMenu.SetActive(false);
                break;
            case GameState.PackSelector:
                panelPackSelector.SetActive(false);
                break;
            case GameState.LevelSelector:
                panelLevelSelector.SetActive(false);
                //   Destroy(levelsGrid);
                foreach (RectTransform child in levelsGrid.transform) {
                    Destroy(child.gameObject);
                }

                break;
            case GameState.ContinueButtonClicked:
                break;
            case GameState.Play:
                EndPlay();
                break;
            case GameState.LevelCompleted:
                // panelLevelCompleted.SetActive(false);
                CurrentLevelNumber++;
                break;
            case GameState.LoadLevel:
                break;
            case GameState.GameOver:
                unloadlevel = true;
                EndPlay();
                panelGameOver.SetActive(false);
                break;
            case GameState.Settings:
                panelSettings.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void EndPlay()
    {
        foreach (Button button in panelPlay.GetComponentsInChildren<Button>())
        {
            button.interactable = false;
        }

        if (unfinishedLevel)
        {
            unfinishedLevel = false;
            Destroy(nextLevel);
            nextLevel = Instantiate(levels[CurrentLevelNumber]);
            nextLevel.SetActive(false);
        }

        if (unloadlevel)
        {
            Destroy(currentLevel);
            panelPlay.SetActive(false);
        }
    }

    private int[] GetLevelPackPosition(int level) {
        for (var i = 0; i < allPacks.Length; i++) {
            for (var j = 0; j < allPacks[i].Length; j++) {
                if (allPacks[i][j] == level) {
                    return new[] {i, j};
                }
            }
        }

        return null;
    }

    private void UnlockNextLevel(int levelToUnlock) {
        var levelPosition = GetLevelPackPosition(levelToUnlock);

        if (levelPosition[1] == allPacks[levelPosition[0]].Length - 1) {
            levelsUnlocked[levelPosition[0] + 1][0] = true;
        }
        else {
            levelsUnlocked[levelPosition[0]][levelPosition[1] + 1] = true;
        }
    }

    private IEnumerator UnlockMovement(float seconds) {
        currentLevel.transform.GetChild(1).GetComponent<Player>().IsMovementLocked = true;
        yield return new WaitForSecondsRealtime(seconds);
        if (currentLevel != null) {
            currentLevel.transform.GetChild(1).GetComponent<Player>().IsMovementLocked = false;
        }
    }

    #endregion
}