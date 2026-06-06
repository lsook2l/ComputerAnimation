using UnityEngine;

/// <summary>
/// 딸기 보석 아이템을 획득하는 스크립트입니다.
///
/// 담당 기능:
/// 1. 플레이어가 딸기에 닿으면 획득 처리
/// 2. 획득 사운드 재생
/// 3. 획득 이펙트 생성
/// 4. StrawberryManager에 수집 개수 증가 요청
/// 5. 딸기 오브젝트 삭제
///
/// 사용 위치:
/// - 각 GemStrawberry 오브젝트에 붙입니다.
/// - 딸기 오브젝트에는 Collider2D가 있어야 하며 Is Trigger를 체크합니다.
/// </summary>
public class GemStrawberry : MonoBehaviour
{
    [Header("획득 효과")]
    [Tooltip("딸기를 먹었을 때 생성할 파티클 이펙트 프리팹")]
    public GameObject collectEffect;

    // 한 딸기가 여러 번 수집 처리되는 것을 막습니다.
    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            isCollected = true;

            // 딸기 획득 효과음 재생
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCollectSound();
            }

            // 딸기 획득 이펙트 생성
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            // 전체 딸기 수집 개수를 관리하는 매니저에 수집 사실을 전달합니다.
            if (StrawberryManager.Instance != null)
            {
                StrawberryManager.Instance.CollectStrawberry();
            }

            // 획득한 딸기는 화면에서 제거합니다.
            Destroy(gameObject);
        }
    }
}
