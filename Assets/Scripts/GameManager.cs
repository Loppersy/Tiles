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
    public GameObject panelSettings;
    public GameObject levelsGridUI;
    public GameObject levelButton;
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
    private int currentBackground;
    private int lastBackground;

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
        preselectedBackground = true;
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
        LoadSavedProgress();

        Instance = this;
        //Preload last played level and deactivate it.
        nextLevel = Instantiate(levels[lastLevelPlayedNumber]);
        nextLevel.SetActive(false);


        //Debug (unlock all levels):
        //////////////////////////
        for (int j = 0; j < levelsUnlocked.Length; j++)
        {
        
            for (int i = 0; i < levelsUnlocked[j].Length; i++)
            {
        
                levelsUnlocked[j][i] = true;
            }
        }
        ////////////////////////////

        SwitchState(GameState.Menu);
    }

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
        if (Input.anyKeyDown) {
            SwitchState(GameState.Menu);
        }
    }

    private void UpdatePlay() {
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
        foreach (SpriteRenderer spriteRenderer in currentLevel.GetComponentsInChildren<SpriteRenderer>()) {
            spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
        }
        panelPlay.SetActive(true);
        IsStuck = false;
        isCurrentLevelCompleted = false;
        StartCoroutine(UnlockMovement(0.5f));
    }

    private void BeginLoadLevel() {
        if (currentLevel != null) {
            foreach (Transform child in currentLevel.transform) {
                if (child.GetComponent<Player>() != null) {
                        child.GetComponent<Player>().SetIsCosmetic(true);
                        child.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            if (transitionLevel != null) {
                Destroy(transitionLevel);
            }
            transitionLevel = Instantiate(currentLevel);
            Destroy(currentLevel);
            foreach (SpriteRenderer spriteRenderer in transitionLevel.GetComponentsInChildren<SpriteRenderer>()) {
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                Debug.Log(spriteRenderer.name + " " + spriteRenderer.maskInteraction);
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
        nextLevel = Instantiate(levels[CurrentLevelNumber + 1]);
        nextLevel.SetActive(false);

        foreach (SpriteRenderer spriteRenderer in currentLevel.GetComponentsInChildren<SpriteRenderer>()) {
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        
        StartCoroutine(Transition());

        loadNewLevel = false;

        currentLevelNumberUIText.text = (GetLevelPackPosition(CurrentLevelNumber)[0] + 1) + "-" +
                                        (GetLevelPackPosition(CurrentLevelNumber)[1] + 1);
        IsStuck = false;
        lastLevelPlayedNumber = CurrentLevelNumber;
        if (!preselectedBackground) {
            while (lastBackground == currentBackground) {
                currentBackground = Random.Range(0, backgroundTextures.Length);
            }
        }

        background.GetComponent<SpriteRenderer>().sprite = backgroundTextures[currentBackground];
        preselectedBackground = false;
        retrying = false;


        SaveSystem.SaveData(levelsUnlocked, lastLevelPlayedNumber);
        
    }

    private IEnumerator Transition() {
        transition.SetTrigger("StartLevelTransition");
        yield return new WaitForSeconds(1f);
        if (transitionLevel != null) { 
            Destroy(transitionLevel);
        }
        unloadlevel = false;
        SwitchState(GameState.Play);
        
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
        }
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
                panelPlay.SetActive(false);

                
                if (unfinishedLevel) {
                    unfinishedLevel = false;
                    Destroy(nextLevel);
                    nextLevel = Instantiate(levels[CurrentLevelNumber]);
                    nextLevel.SetActive(false);
                }
                if(unloadlevel) {
                    Destroy(currentLevel);
                }
                break;
            case GameState.LevelCompleted:
                // panelLevelCompleted.SetActive(false);
                CurrentLevelNumber++;
                break;
            case GameState.LoadLevel:
                break;
            case GameState.GameOver:
                panelPlay.SetActive(false);
                panelGameOver.SetActive(false);
                break;
            case GameState.Settings:
                panelSettings.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
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