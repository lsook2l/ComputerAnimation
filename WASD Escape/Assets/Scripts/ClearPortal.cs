using UnityEngine;

/// <summary>
/// 플레이어가 클리어 포털에 닿았을 때 클리어 패널을 표시하는 스크립트입니다.
///
/// 사용 위치:
/// - ClearPortal 오브젝트에 붙입니다.
/// - ClearPortal에는 Collider2D가 있어야 하며 Is Trigger를 체크합니다.
/// - clearPanel에는 Canvas_UI 안의 ClearPanel을 연결합니다.
/// </summary>
public class ClearPortal : MonoBehaviour
{
    [Header("클리어 화면 패널")]
    [Tooltip("플레이어가 클리어 포털에 닿았을 때 표시할 UI 패널")]
    public GameObject clearPanel;

    // 같은 포털에서 클리어 처리가 여러 번 실행되는 것을 막습니다.
    private bool isCleared = false;

    private void Start()
    {
        // 게임 시작 시 클리어 패널은 보이지 않게 합니다.
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCleared)
        {
            return;
        }

        // Player 태그를 가진 오브젝트가 포털에 닿았을 때만 클리어 처리합니다.
        if (other.CompareTag("Player"))
        {
            isCleared = true;

            // 클리어 효과음 재생
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClearSound();
            }

            // 클리어 패널 표시
            if (clearPanel != null)
            {
                clearPanel.SetActive(true);
                clearPanel.transform.SetAsLastSibling();
            }

            // 클리어 화면에서 게임이 더 이상 진행되지 않도록 일시정지합니다.
            Time.timeScale = 0f;
        }
    }
}
