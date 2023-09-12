using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    public bool isTestingBuild;

    [SerializeField] private MatchSettingsTestManager _matchSettingsTestManager;
    [SerializeField] private GameObject _testSettingsPanel;

    [Header("Diffuculty configs")]
    [SerializeField] private List<TimeAttackConfig> _timeAttackDifficultyConfigs;
    [SerializeField] private List<MatchPointConfig> _matchPointDifficultyConfigs;

    [Header("General UI")]
    [SerializeField] private GridView _gridView;
    [SerializeField] private MatchTimer _matchTimer;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private TMP_Text _scoreText;

    [Header("Time Attack")]
    [SerializeField] private TMP_Text _timeAttackHeader;
    [SerializeField] private TMP_Text _livesText;
    [SerializeField] private int _lives = 3;

    [Header("Match Point")]
    [SerializeField] private TMP_Text _matchPointHeader;

    GridPresenter _gridPresenter;
    ObjectiveService _objectiveService;
    Objective _currObjective;
    bool isPlaying = false;
    int _testHeight = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _objectiveService = new ObjectiveService();
        _gridPresenter = new GridPresenter(_gridView);
    }

    private async void Start()
    {
        _testSettingsPanel.SetActive(true);
        _countdownText.gameObject.SetActive(false);

        if (isTestingBuild)
            return;

        // Load list with the objectives (connected to pointed node)
        List<Objective> connectedObjectivesList = await _objectiveService.LoadLocalObjectivesList();
        int selectedMapNodeId = PlayerPrefs.GetInt("SelectedNodeId");

        // Get selected objective from list
        int objIndex = 0;
        foreach (var obj in connectedObjectivesList)
        {
            if (obj.MapNodeId == selectedMapNodeId)
            {
                _currObjective = obj;
                break;
            }
            objIndex++;
        }

        // Load or create new grid
        if (_currObjective.Grid == null)
        {
            _currObjective.Grid = _gridPresenter.CreateAndInitializeGrid(8);

            connectedObjectivesList[objIndex].Grid = _currObjective.Grid;

            if (await _objectiveService.SaveLocalObjectivesListData(connectedObjectivesList))
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

    public void InitializeGameTest()
    {
        _testSettingsPanel.SetActive(false);

        _currObjective = new Objective();
        int x = _matchSettingsTestManager.GetGameMode();
        if (x == 1)
            _currObjective.GameMode = GameMode.TimeAttack;
        else
            _currObjective.GameMode = GameMode.MatchPoint;

        _testHeight = _matchSettingsTestManager.GetHeight();

        // AT PRODUCTION, GET DATA FROM CONFIG FILES
        // Setup Match
        switch (_currObjective.GameMode)
        {
            case GameMode.TimeAttack:
                int findMatchTime = _matchSettingsTestManager.GetFindMatchTime();
                int numberOfMatchesPerTime = _matchSettingsTestManager.GetNumberOfMatchesPerTime();
                SetupTimeAttackMode(findMatchTime, numberOfMatchesPerTime);
                break;
            case GameMode.MatchPoint:
                int pointsGoal = _matchSettingsTestManager.GetPointsGoal();
                int pointsPerMatch = _matchSettingsTestManager.GetPointsPerMatchGoal();
                int totalTime = _matchSettingsTestManager.GetTotalTime();
                SetupMatchPointMode(pointsGoal, pointsPerMatch, totalTime);
                break;
        }
    }

    private void SetupTimeAttackMode(int findMatchTime, int numberOfMatchesPerTime)
    {
        _scoreText.text = "Score: 0";
        _livesText.text = "Lives: " + _lives;

        _matchTimer.OnTimerEnd.AddListener(DecreaseLives);
        _matchTimer.InitializeTimeAttackTimer(findMatchTime);

        StartCoroutine(StartCountDown(3));
    }

    private void SetupMatchPointMode(int pointsGoal, int pointsPerMatch, int numberOfMatches)
    {

    }

    private void Update()
    {
        if (!isPlaying)
            return;
    }

    private IEnumerator StartCountDown(int value)
    {
        _countdownText.gameObject.SetActive(true);

        do
        {
            _countdownText.text = value.ToString();
            yield return new WaitForSeconds(1);
            value--;
        }
        while (value > 0);

        _countdownText.gameObject.SetActive(false);

        StartGame();
    }

    private void StartGame()
    {
        _currObjective.Grid = _gridPresenter.CreateAndInitializeGrid(_testHeight);
        _gridView.InjectGridPresenter(_gridPresenter);

        _matchTimer.StartAndRepeatTimer();
    }


    private void DecreaseLives()
    {
        _lives--;

        //Update UI
        _livesText.text = "Lives: " + _lives;

        if (_lives <= 0)
        {
            _matchTimer.StopTimer();
            Debug.Log($"<color=red>GAME OVER!</color>");
        }

    }

    public void FinishGame()
    {
        PlayerPrefs.SetInt("CurrentPointedNodeId", PlayerPrefs.GetInt("SelectedNodeId"));

        GameManager.Instance.UpdateGameState(GameState.FinishPlaying);
    }
}
