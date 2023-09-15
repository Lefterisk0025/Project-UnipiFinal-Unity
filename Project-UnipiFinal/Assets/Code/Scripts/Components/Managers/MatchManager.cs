using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    public bool isTestingBuild;

    [SerializeField] private MatchSettingsTestManager _matchSettingsTestManager;
    [SerializeField] private GameObject _testSettingsPanel;

    [Header("Diffuculty configs")]
    [SerializeField] private List<TimeAttackConfig> _timeAttackDifficultyConfigs;
    [SerializeField] private List<MatchPointConfig> _matchPointDifficultyConfigs;
    [SerializeField] private MissionMapView _missionMapView;

    [Header("General settings")]
    [SerializeField] private GridView _gridView;
    [SerializeField] private BarCountdownTimer _repeatBarTimer;
    [SerializeField] private TextCountdownTimer _startGameCountdownTimer;
    [SerializeField] private TextCountdownTimer _generalCountdownTimer;
    [SerializeField] private PlayerMatchPerformanceView _performanceView;
    [SerializeField] private MatchResultsView _matchResultsView;

    [Header("Time Attack")]
    [SerializeField] private TMP_Text _timeAttackHeader;

    [Header("Match Point")]
    [SerializeField] private TMP_Text _matchPointHeader;

    GridPresenter _gridPresenter;
    ObjectiveService _objectiveService;
    Objective _currObjective;
    List<Objective> _connectedObjectivesList;
    int _objIndex;
    int _height = 0;
    int _totalTime = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _objectiveService = new ObjectiveService();
        _gridPresenter = new GridPresenter(_gridView);
    }

    private void Start()
    {
        LoadingScreen.Instance.OnLoadFinish.AddListener(InitializeGame);
    }

    // Being called after load screen finishes
    private async void InitializeGame()
    {
        GameManager.Instance.EnableMainCamera();
        _repeatBarTimer.gameObject.SetActive(false);
        _testSettingsPanel.SetActive(false);

        // Load list with the objectives (connected to pointed node)
        _connectedObjectivesList = new List<Objective>(await _objectiveService.LoadLocalObjectivesList());
        int selectedMapNodeId = PlayerPrefs.GetInt("SelectedNodeId");

        // Get selected objective from list
        _objIndex = 0;
        foreach (var obj in _connectedObjectivesList)
        {
            if (obj.MapNodeId == selectedMapNodeId)
            {
                _currObjective = obj;
                break;
            }
            _objIndex++;
        }

        if (_currObjective.Grid == null)
            Debug.Log($"<color=red>There is no grid...</color>");
        else
            Debug.Log($"<color=yellow>Grid found!</color>");

        MatchConfig matchConfig = null;
        if (_currObjective.Difficulty == "Easy")
            matchConfig = GetMatchConfigBasedOnDifficulty(Difficulty.Easy);
        else if (_currObjective.Difficulty == "Medium")
            matchConfig = GetMatchConfigBasedOnDifficulty(Difficulty.Medium);
        else if (_currObjective.Difficulty == "Hard")
            matchConfig = GetMatchConfigBasedOnDifficulty(Difficulty.Hard);
        else if (_currObjective.Difficulty == "Very Hard")
            matchConfig = GetMatchConfigBasedOnDifficulty(Difficulty.VeryHard);

        _height = matchConfig.Height;
        _totalTime = matchConfig.TotalTime;

        TimeAttackConfig timeAttackConfig = null;
        MatchPointConfig matchPointConfig = null;
        if (matchConfig.GetType() == typeof(TimeAttackConfig))
            timeAttackConfig = (TimeAttackConfig)matchConfig;
        else if (matchConfig.GetType() == typeof(MatchPointConfig))
            matchPointConfig = (MatchPointConfig)matchConfig;

        switch (_currObjective.GameMode)
        {
            case GameMode.TimeAttack:
                int findMatchTime = timeAttackConfig.FindMatchTime;
                int numberOfMatchesPerTime = timeAttackConfig.NumberOfMatchesPerTime;
                SetupTimeAttackMode(findMatchTime, numberOfMatchesPerTime);
                break;
            case GameMode.MatchPoint:
                int pointsGoal = matchPointConfig.ScoreGoal;
                int pointsPerMatch = matchPointConfig.PointsPerMatch;
                SetupMatchPointMode(pointsGoal, pointsPerMatch);
                break;
        }
    }

    private void SetupTimeAttackMode(int findMatchTime, int numberOfMatchesPerTime)
    {
        // Setup stats text
        _performanceView.InitializeTimeAttackPerformanceStats(numberOfMatchesPerTime);

        _repeatBarTimer.gameObject.SetActive(true);

        // Setup events
        _startGameCountdownTimer.OnTimerEnd.AddListener(StartTimeAttackGame);
        _generalCountdownTimer.OnTimerEnd.AddListener(_performanceView.OpenResultScreen);
        _repeatBarTimer.OnTimerEnd.AddListener(_performanceView.DecreaseLives);
        _repeatBarTimer.OnTimerEnd.AddListener(_performanceView.ResetNumberOfMatchesFound);
        _performanceView.OnLivesDrain.AddListener(_repeatBarTimer.StopTimer);
        _performanceView.OnLivesDrain.AddListener(_performanceView.OpenResultScreen);
        _performanceView.OnAllMatchesFound.AddListener(_repeatBarTimer.StartAndRepeatBarTimer);
        _gridView.OnMatchFound.AddListener(_performanceView.IncreaseScore);
        _gridView.OnMatchFound.AddListener(_performanceView.IncreaseNumberOfMatchesFound);

        _repeatBarTimer.InitializeRepeatTimer(findMatchTime);

        StartCoroutine(_startGameCountdownTimer.StartCountDown(3));
    }

    private void SetupMatchPointMode(int pointsGoal, int pointsPerMatch)
    {
        // Setup stats text
        _performanceView.InitializeMatchPointPerformanceStats(pointsGoal, pointsPerMatch);

        // Setup events
        _startGameCountdownTimer.OnTimerEnd.AddListener(StartMatchPointGame);
        _generalCountdownTimer.OnTimerEnd.AddListener(_performanceView.OpenResultScreen);
        _performanceView.OnScoreGoalReached.AddListener(_performanceView.OpenResultScreen);
        _gridView.OnMatchFound.AddListener(_performanceView.IncreaseScore);
        _gridView.OnMatchFound.AddListener(_performanceView.IncreaseNumberOfMatchesFound);

        StartCoroutine(_startGameCountdownTimer.StartCountDown(3));
    }

    private void StartTimeAttackGame()
    {
        GenerateGrid();

        StartCoroutine(_generalCountdownTimer.StartCountDownInTimeFormat(_totalTime));
        _repeatBarTimer.StartAndRepeatBarTimer();

    }

    private void StartMatchPointGame()
    {
        GenerateGrid();

        StartCoroutine(_generalCountdownTimer.StartCountDownInTimeFormat(_totalTime));
    }

    private async void GenerateGrid()
    {
        // Load or create new grid
        if (_currObjective.Grid == null)
        {
            _currObjective.Grid = _gridPresenter.CreateAndInitializeGrid(8);

            _connectedObjectivesList[_objIndex].Grid = _currObjective.Grid;

            if (await _objectiveService.SaveLocalObjectivesListData(_connectedObjectivesList))
                Debug.Log("Save success!");
            else
                Debug.Log("Save failed...");
        }
        else
        {
            _gridPresenter.InitializeGrid(_currObjective.Grid);
        }

        _gridView.InjectGridPresenter(_gridPresenter);
    }

    private MatchConfig GetMatchConfigBasedOnDifficulty(Difficulty difficulty)
    {

        if (_currObjective.GameMode == GameMode.TimeAttack)
        {
            foreach (var config in _timeAttackDifficultyConfigs)
            {
                if (config.Difficulty == difficulty)
                {
                    return (TimeAttackConfig)config;
                }
            }
            return null;
        }
        else if (_currObjective.GameMode == GameMode.MatchPoint)
        {
            foreach (var config in _matchPointDifficultyConfigs)
            {
                if (config.Difficulty == difficulty)
                {
                    return (MatchPointConfig)config;
                }
            }
            return null;
        }

        return null;
    }

    public void FinishGame()
    {
        _gridView.ClearGrid();
        _connectedObjectivesList.Clear();

        _repeatBarTimer.StopTimer();

        _matchResultsView.ResetUI();

        // Clear all events
        _startGameCountdownTimer.OnTimerEnd.RemoveAllListeners();
        _generalCountdownTimer.OnTimerEnd.RemoveAllListeners();
        _repeatBarTimer.OnTimerEnd.RemoveAllListeners();
        _performanceView.OnLivesDrain.RemoveAllListeners();
        _performanceView.OnAllMatchesFound.RemoveAllListeners();
        _gridView.OnMatchFound.RemoveAllListeners();

        Time.timeScale = 1;

        if (PlayerPrefs.GetInt("TerminateMission") == 0)
        {
            PlayerPrefs.SetInt("PreviousPointedNodeId", PlayerPrefs.GetInt("CurrentPointedNodeId"));
            PlayerPrefs.SetInt("CurrentPointedNodeId", PlayerPrefs.GetInt("SelectedNodeId"));

            GameManager.Instance.UpdateGameState(GameState.FinishingMatch);
        }
        else
        {
            _missionMapView.InvokeAbandonMission();
        }
    }
}
