using UnityEngine;
using TMPro;

/// <summary>
/// 키 부여 화면 UI를 관리하는 스크립트입니다.
///
/// 담당 기능:
/// 1. 플레이어 이름 입력값 읽기
/// 2. 키 부여 버튼 클릭 시 DragonPlayerMove에 랜덤 키 배정 요청
/// 3. 배정 결과를 KeyResultText에 표시
/// 4. 키 부여 전에는 게임 시간을 멈추고, 키 부여 후 게임을 시작
///
/// 사용 위치:
/// - Canvas_UI 또는 KeyAssignPanel 관리 오브젝트에 붙입니다.
/// - Inspector에서 Dragon_Player, KeyAssignPanel, 이름 입력칸, 결과 텍스트를 연결합니다.
/// </summary>
public class KeyAssignUI : MonoBehaviour
{
    [Header("플레이어")]
    [Tooltip("키 배정을 적용할 DragonPlayerMove 스크립트")]
    public DragonPlayerMove player;

    [Header("키 부여 패널")]
    [Tooltip("플레이어 이름 입력과 키 부여 버튼이 들어있는 패널")]
    public GameObject keyAssignPanel;

    [Header("이름 입력칸")]
    [Tooltip("Player1 이름 입력 TMP Input Field")]
    public TMP_InputField player1NameInput;

    [Tooltip("Player2 이름 입력 TMP Input Field")]
    public TMP_InputField player2NameInput;

    [Header("키 배정 결과 텍스트")]
    [Tooltip("게임 화면에 Player1 / Player2 키 배정 결과를 표시할 TMP Text")]
    public TMP_Text keyResultText;

    private void Start()
    {
        // Inspector에 직접 연결하는 것이 가장 좋지만, 비어 있을 경우 자동으로 찾습니다.
        if (player == null)
        {
            player = FindFirstObjectByType<DragonPlayerMove>();
        }

        RefreshUI();
    }

    /// <summary>
    /// 현재 키 배정 상태에 따라 UI를 갱신합니다.
    /// 키가 배정되지 않았으면 키 부여 패널을 보여주고 게임을 멈춥니다.
    /// 키가 이미 배정되어 있으면 패널을 숨기고 게임을 진행합니다.
    /// </summary>
    public void RefreshUI()
    {
        if (player == null)
        {
            return;
        }

        if (player.HasAssignedKeys)
        {
            if (keyAssignPanel != null)
            {
                keyAssignPanel.SetActive(false);
            }

            if (keyResultText != null)
            {
                keyResultText.text = player.GetAssignedKeyResultText();
            }

            Time.timeScale = 1f;
        }
        else
        {
            if (keyAssignPanel != null)
            {
                keyAssignPanel.SetActive(true);
            }

            if (keyResultText != null)
            {
                keyResultText.text = "Player1: -\nPlayer2: -";
            }

            // 키를 부여받기 전까지 게임이 진행되지 않도록 멈춥니다.
            Time.timeScale = 0f;
        }
    }

    /// <summary>
    /// 키 부여받기 버튼에서 호출합니다.
    /// 입력한 이름을 읽고, 플레이어 이동 스크립트에 랜덤 키 배정을 요청합니다.
    /// </summary>
    public void AssignKeys()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<DragonPlayerMove>();
        }

        if (player == null)
        {
            return;
        }

        string player1Name = "Player1";
        string player2Name = "Player2";

        // 입력칸이 비어 있으면 기본 이름을 사용합니다.
        if (player1NameInput != null && !string.IsNullOrWhiteSpace(player1NameInput.text))
        {
            player1Name = player1NameInput.text;
        }

        if (player2NameInput != null && !string.IsNullOrWhiteSpace(player2NameInput.text))
        {
            player2Name = player2NameInput.text;
        }

        player.AssignNewRandomKeysFromButton(player1Name, player2Name);

        if (keyResultText != null)
        {
            keyResultText.text = player.GetAssignedKeyResultText();
        }

        if (keyAssignPanel != null)
        {
            keyAssignPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}
