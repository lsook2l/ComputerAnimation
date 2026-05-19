using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public enum Choice { None, Scissors, Rock, Paper }

    const int WinScore = 5;

    [Header("UI Buttons")]
    public Button buttonScissors;
    public Button buttonRock;
    public Button buttonPaper;
    public Button buttonRestart;
    public Button buttonReset;
    public Button buttonExit;

    // 플레이어가 선택한 값 저장
    private Choice playerChoice = Choice.None;

    [Header("Display Images")]
    public Image imagePlayer;
    public Image imageComputer;

    [Header("Sprites")]
    public Sprite spriteScissors;
    public Sprite spriteRock;
    public Sprite spritePaper;


    // 스프라이트 순환용 변수
    private bool isAnimating = true;
    private float animationInterval = 0.1f;
    private float animationTimer = 0f;
    private int currentSpriteIndex = 0;
    private Sprite[] sprites;

    [Header("Result Text")]
    public TextMeshProUGUI resultText;

    [Header("Score Board")]
    public TextMeshProUGUI textScorePlayer;
    public TextMeshProUGUI textScoreComputer;

    private int scorePlayer = 0;
    private int scoreComputer = 0;
    private bool gameEnded = false;

    [Header("UI Panel")]
    public GameObject panelResult;
    public TextMeshProUGUI textResultPanel;

    void Start()
    {
        // 스프라이트 배열 초기화
        sprites = new Sprite[] { spriteScissors, spriteRock, spritePaper };

        // 버튼 클릭 이벤트 연결
        buttonScissors.onClick.AddListener(() => OnPlayerChoice(Choice.Scissors));
        buttonRock.onClick.AddListener(() => OnPlayerChoice(Choice.Rock));
        buttonPaper.onClick.AddListener(() => OnPlayerChoice(Choice.Paper));
        buttonRestart.onClick.AddListener(OnRestart);
        buttonReset.onClick.AddListener(OnReset);
        buttonExit.onClick.AddListener(OnExit);

        if (panelResult != null)
            panelResult.SetActive(false);

        // 결과 텍스트 초기화
        resultText.text = "가위 바위 보 중 하나를 선택하세요!";
    }

    void Update()
    {
        if (isAnimating)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationInterval)
            {
                animationTimer = 0f;
                currentSpriteIndex = (currentSpriteIndex + 1) % 3;

                // 두 이미지가 서로 다른 스프라이트를 보여주도록
                // imagePlayer.sprite = sprites[currentSpriteIndex];
                imageComputer.sprite = sprites[(currentSpriteIndex + 1) % 3];
            }
        }
    }

    void OnRestart()
    {
        // 게임 종료 상태만 해제하고 점수는 유지 (게임 이어가기)
        gameEnded = false;

        if (panelResult != null)
            panelResult.SetActive(false);

        isAnimating = true;
        animationTimer = 0f;
        currentSpriteIndex = 0;
        imagePlayer.sprite = spriteRock;
        imageComputer.sprite = spriteRock;
        resultText.text = "가위 바위 보 중 하나를 선택하세요!";

        SetRpsButtonsInteractable(true);
    }

    void OnReset()
    {
        // 전체 초기화 (점수 포함)
        gameEnded = false;
        scorePlayer = 0;
        scoreComputer = 0;
        textScorePlayer.text = "0";
        textScoreComputer.text = "0";

        if (panelResult != null)
            panelResult.SetActive(false);

        isAnimating = true;
        animationTimer = 0f;
        currentSpriteIndex = 0;
        imagePlayer.sprite = spriteRock;
        imageComputer.sprite = spriteRock;
        resultText.text = "가위 바위 보 중 하나를 선택하세요!";

        SetRpsButtonsInteractable(true);
    }

    void OnExit()
    {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void SetRpsButtonsInteractable(bool value)
    {
        buttonScissors.interactable = value;
        buttonRock.interactable = value;
        buttonPaper.interactable = value;
    }

    void EndGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        if (panelResult != null)
            panelResult.SetActive(true);

        if (textResultPanel != null)
        {
            if (scorePlayer >= WinScore)
                textResultPanel.text = "게임 종료!\n\n플레이어 승리! (5점 달성)";
            else
                textResultPanel.text = "게임 종료!\n\n컴퓨터 승리! (5점 달성)";
        }

        SetRpsButtonsInteractable(false);
    }

    // 플레이어가 버튼을 눌렀을 때 호출
    void OnPlayerChoice(Choice choice)
    {
        if (gameEnded)
            return;

        // 애니메이션 중지
        isAnimating = false;

        playerChoice = choice;
        Debug.Log("플레이어 선택: " + choice.ToString());

        // 컴퓨터 선택 및 승부 판정
        Choice computerChoice = GetComputerChoice();
        Debug.Log("컴퓨터 선택: " + computerChoice.ToString());

        // 이미지에 선택 결과 표시
        imagePlayer.sprite = GetSpriteFromChoice(playerChoice);
        imageComputer.sprite = GetSpriteFromChoice(computerChoice);

        // 결과 판정 및 텍스트 표시
        string result = DetermineWinner(playerChoice, computerChoice);
        textScorePlayer.text = scorePlayer.ToString();
        textScoreComputer.text = scoreComputer.ToString();
        resultText.text = result;
        Debug.Log("결과: " + result);

        if (scorePlayer >= WinScore || scoreComputer >= WinScore)
            EndGame();
    }

    // Choice에 해당하는 스프라이트 반환
    Sprite GetSpriteFromChoice(Choice choice)
    {
        switch (choice)
        {
            case Choice.Scissors: return spriteScissors;
            case Choice.Rock: return spriteRock;
            case Choice.Paper: return spritePaper;
            default: return spriteRock;
        }
    }

    // 컴퓨터가 랜덤으로 가위/바위/보 중 하나 선택
    Choice GetComputerChoice()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0: return Choice.Scissors;
            case 1: return Choice.Rock;
            case 2: return Choice.Paper;
            default: return Choice.Rock;
        }
    }

    // 승부 판정
    string DetermineWinner(Choice player, Choice computer)
    {
        if (player == computer)
            return "무승부!";

        // 플레이어 승리 조건: 가위->보, 바위->가위, 보->바위
        bool playerWins = (player == Choice.Scissors && computer == Choice.Paper) ||
                          (player == Choice.Rock && computer == Choice.Scissors) ||
                          (player == Choice.Paper && computer == Choice.Rock);

        if (playerWins)
        {
            scorePlayer++;
        }
        else
        {
            scoreComputer++;
        }
        return playerWins ? "플레이어 승리!" : "컴퓨터 승리!";
    }
}
