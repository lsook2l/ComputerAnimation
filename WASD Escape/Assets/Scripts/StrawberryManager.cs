using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// 딸기 보석 수집 상태와 클리어 포털 활성화를 관리하는 스크립트입니다.
///
/// 담당 기능:
/// 1. 전체 딸기 개수 관리
/// 2. 딸기를 먹을 때마다 수집 개수 증가
/// 3. 딸기 수집 개수를 UI에 표시
/// 4. 모든 딸기를 수집하면 클리어 포털 활성화
/// 5. 클리어 포털 사용 가능 안내 문구 표시
///
/// 사용 위치:
/// - Hierarchy에 StrawberryManager 오브젝트를 만들고 이 스크립트를 붙입니다.
/// - Inspector에서 ClearPortal, StrawberryCountText, PortalOpenMessageText를 연결합니다.
/// </summary>
public class StrawberryManager : MonoBehaviour
{
    public static StrawberryManager Instance;

    [Header("딸기 설정")]
    [Tooltip("스테이지에 배치된 전체 딸기 개수")]
    public int totalStrawberryCount = 4;

    [Header("클리어 포털")]
    [Tooltip("딸기 4개를 모두 먹으면 활성화될 클리어 포털")]
    public GameObject clearPortal;

    [Header("딸기 개수 UI")]
    [Tooltip("딸기 수집 개수를 보여줄 TMP 텍스트")]
    public TMP_Text strawberryCountText;

    [Header("포털 오픈 메시지 UI")]
    [Tooltip("딸기를 모두 먹었을 때 보여줄 안내 문구 오브젝트")]
    public GameObject portalOpenMessageText;

    [Tooltip("포털 오픈 메시지를 화면에 보여줄 시간")]
    public float messageShowTime = 2f;

    private int collectedCount = 0;
    private bool isPortalOpened = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        collectedCount = 0;
        isPortalOpened = false;

        // 게임 시작 시 클리어 포털은 닫힌 상태로 시작합니다.
        if (clearPortal != null)
        {
            clearPortal.SetActive(false);
        }

        // 포털 오픈 안내 문구도 시작 시 숨겨둡니다.
        if (portalOpenMessageText != null)
        {
            portalOpenMessageText.SetActive(false);
        }

        UpdateStrawberryUI();
    }

    /// <summary>
    /// 딸기를 먹었을 때 GemStrawberry에서 호출합니다.
    /// 수집 개수를 증가시키고, 모두 먹었는지 확인합니다.
    /// </summary>
    public void CollectStrawberry()
    {
        if (isPortalOpened)
        {
            return;
        }

        collectedCount++;

        UpdateStrawberryUI();

        if (collectedCount >= totalStrawberryCount)
        {
            OpenClearPortal();
        }
    }

    /// <summary>
    /// 딸기 수집 개수 UI를 갱신합니다.
    /// 예: 0 / 4, 1 / 4, 4 / 4
    /// </summary>
    private void UpdateStrawberryUI()
    {
        if (strawberryCountText != null)
        {
            strawberryCountText.text = collectedCount + " / " + totalStrawberryCount;
        }
    }

    /// <summary>
    /// 클리어 포털을 활성화하고, 포털 사용 가능 안내 문구를 표시합니다.
    /// </summary>
    private void OpenClearPortal()
    {
        isPortalOpened = true;

        if (clearPortal != null)
        {
            clearPortal.SetActive(true);
        }

        if (portalOpenMessageText != null)
        {
            portalOpenMessageText.SetActive(true);
            portalOpenMessageText.transform.SetAsLastSibling();

            StartCoroutine(HidePortalOpenMessage());
        }
    }

    /// <summary>
    /// 일정 시간이 지난 뒤 포털 오픈 안내 문구를 숨깁니다.
    /// Time.timeScale이 0이어도 동작할 수 있게 WaitForSecondsRealtime을 사용합니다.
    /// </summary>
    private IEnumerator HidePortalOpenMessage()
    {
        yield return new WaitForSecondsRealtime(messageShowTime);

        if (portalOpenMessageText != null)
        {
            portalOpenMessageText.SetActive(false);
        }
    }
}
