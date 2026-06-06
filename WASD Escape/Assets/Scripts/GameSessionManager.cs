using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 게임 전체 흐름을 관리하는 매니저 스크립트입니다.
///
/// 담당 기능:
/// 1. ESC 메뉴 열기 / 닫기
/// 2. 계속하기 버튼 클릭 시 3초 카운트다운 후 게임 재개
/// 3. 재시작 버튼 클릭 시 키 배정을 초기화하고 현재 씬 재시작
/// 4. 종료 버튼 클릭 시 게임 종료
/// 5. 장애물 충돌 시 기존 키 배정을 유지한 채 현재 씬 재시작
///
/// 사용 위치:
/// - Hierarchy에 빈 오브젝트를 만들고 이름을 GameSessionManager로 설정한 뒤 이 스크립트를 붙입니다.
/// - Inspector에서 EscMenuPanel, KeyAssignPanel, ClearPanel, CountdownText를 연결합니다.
/// </summary>
public class GameSessionManager : MonoBehaviour
{
    // 다른 스크립트에서 GameSessionManager.Instance로 접근하기 위한 싱글톤 참조입니다.
    public static GameSessionManager Instance;

    [Header("ESC 메뉴 패널")]
    [Tooltip("ESC를 눌렀을 때 보여줄 일시정지 메뉴 패널")]
    public GameObject escMenuPanel;

    [Header("키 부여 패널")]
    [Tooltip("게임 시작 시 플레이어 이름 입력 및 키 부여를 담당하는 패널")]
    public GameObject keyAssignPanel;

    [Header("클리어 패널")]
    [Tooltip("클리어 포털에 닿았을 때 보여줄 클리어 결과 패널")]
    public GameObject clearPanel;

    [Header("계속하기 카운트다운")]
    [Tooltip("계속하기 버튼을 눌렀을 때 3, 2, 1, START!를 표시할 TMP 텍스트")]
    public TMP_Text countdownText;

    // 카운트다운 코루틴이 중복 실행되는 것을 막기 위한 변수입니다.
    private Coroutine countdownCoroutine;

    // 카운트다운 중 ESC 입력을 막기 위한 상태값입니다.
    private bool isCountingDown = false;

    private void Awake()
    {
        // 현재 씬에서 이 매니저를 전역 접근 가능하게 등록합니다.
        Instance = this;

        // 씬이 다시 로드되었거나 이전에 일시정지 상태였더라도 게임 시간이 정상 흐르도록 초기화합니다.
        Time.timeScale = 1f;

        // 게임 시작 시 ESC 메뉴는 보이지 않도록 합니다.
        if (escMenuPanel != null)
        {
            escMenuPanel.SetActive(false);
        }

        // 게임 시작 시 클리어 패널도 보이지 않도록 합니다.
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        // 카운트다운 텍스트는 계속하기 버튼을 눌렀을 때만 보이도록 숨겨둡니다.
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // ESC 키를 누르면 일시정지 메뉴를 열거나 닫습니다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 카운트다운 중에는 메뉴 입력을 무시합니다.
            if (isCountingDown)
            {
                return;
            }

            // 키 부여 화면이 떠 있을 때는 게임이 시작 전이므로 ESC 메뉴를 열지 않습니다.
            if (keyAssignPanel != null && keyAssignPanel.activeSelf)
            {
                return;
            }

            // 클리어 화면이 떠 있을 때도 ESC 메뉴와 겹치지 않도록 막습니다.
            if (clearPanel != null && clearPanel.activeSelf)
            {
                return;
            }

            ToggleEscMenu();
        }
    }

    /// <summary>
    /// 장애물 또는 WallTilemap에 닿았을 때 호출합니다.
    /// 기존 키 배정은 유지하고, 현재 씬만 다시 시작합니다.
    /// </summary>
    public void ReStartWithSameKeys()
    {
        Time.timeScale = 1f;

        // 다음 씬 로드 후에도 기존 랜덤 키 배정을 유지하도록 설정합니다.
        DragonPlayerMove.KeepCurrentKeysOnNextLoad();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// ESC 메뉴의 재시작 버튼 또는 클리어 패널의 다시하기 버튼에서 호출합니다.
    /// 키 배정을 초기화하고 현재 씬을 다시 시작합니다.
    /// 씬이 다시 시작되면 키 부여 패널이 다시 나타납니다.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;

        // 기존 키 저장값을 지워서 다음 시작 때 새로 키를 부여받게 합니다.
        DragonPlayerMove.NewRandomKeysOnNextLoad();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// ESC 메뉴를 열거나 닫습니다.
    /// 메뉴가 열려 있으면 게임 시간을 멈추고, 닫히면 다시 진행합니다.
    /// </summary>
    public void ToggleEscMenu()
    {
        if (escMenuPanel == null)
        {
            return;
        }

        bool willOpen = !escMenuPanel.activeSelf;

        escMenuPanel.SetActive(willOpen);

        // ESC 메뉴가 다른 UI 뒤에 가려지지 않도록 Canvas 내에서 가장 앞으로 보냅니다.
        if (willOpen)
        {
            escMenuPanel.transform.SetAsLastSibling();
        }

        Time.timeScale = willOpen ? 0f : 1f;
    }

    /// <summary>
    /// ESC 메뉴의 계속하기 버튼에서 호출합니다.
    /// 메뉴를 닫고 3초 카운트다운을 보여준 뒤 게임을 재개합니다.
    /// </summary>
    public void ContinueGame()
    {
        if (isCountingDown)
        {
            return;
        }

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        countdownCoroutine = StartCoroutine(ContinueCountdown());
    }

    /// <summary>
    /// 3, 2, 1, START!를 표시한 뒤 게임 시간을 다시 흐르게 합니다.
    /// Time.timeScale이 0인 상태에서도 시간이 흘러야 하므로 WaitForSecondsRealtime을 사용합니다.
    /// </summary>
    private IEnumerator ContinueCountdown()
    {
        isCountingDown = true;

        // 카운트다운을 보여주기 전 ESC 메뉴를 닫습니다.
        if (escMenuPanel != null)
        {
            escMenuPanel.SetActive(false);
        }

        // 카운트다운 중에는 게임을 계속 멈춘 상태로 유지합니다.
        Time.timeScale = 0f;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.transform.SetAsLastSibling();

            countdownText.text = "3";
            yield return new WaitForSecondsRealtime(1f);

            countdownText.text = "2";
            yield return new WaitForSecondsRealtime(1f);

            countdownText.text = "1";
            yield return new WaitForSecondsRealtime(1f);

            countdownText.text = "START!";
            yield return new WaitForSecondsRealtime(0.4f);

            countdownText.gameObject.SetActive(false);
        }
        else
        {
            // 텍스트가 연결되지 않았더라도 기능이 멈추지 않도록 3초 대기 후 진행합니다.
            yield return new WaitForSecondsRealtime(3f);
        }

        Time.timeScale = 1f;
        isCountingDown = false;
    }

    /// <summary>
    /// 게임 종료 버튼에서 호출합니다.
    /// Unity Editor에서는 Play 모드를 종료하고, 빌드된 실행 파일에서는 프로그램을 종료합니다.
    /// </summary>
    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
