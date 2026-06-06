using System.Collections;
using UnityEngine;

/// <summary>
/// 장애물 또는 위험 타일에 플레이어가 닿았을 때 게임을 리셋하는 스크립트입니다.
///
/// 담당 기능:
/// 1. Player 태그를 가진 오브젝트와 충돌했는지 확인
/// 2. 충돌 사운드 재생
/// 3. 플레이어 이동 및 애니메이션 즉시 정지
/// 4. 충돌 이펙트 생성
/// 5. 잠시 후 기존 키 배정을 유지한 채 현재 씬 재시작
///
/// 사용 위치:
/// - Obstacle_FireClump 프리팹 또는 WallTilemap에 붙입니다.
/// - 같은 오브젝트에 Collider2D가 있어야 하며, Trigger 방식이면 Is Trigger를 체크합니다.
/// </summary>
public class GameOverTrigger : MonoBehaviour
{
    [Header("충돌 이펙트")]
    [Tooltip("플레이어가 장애물에 닿았을 때 생성할 이펙트 프리팹")]
    public GameObject hitEffect;

    [Header("리셋 대기 시간")]
    [Tooltip("충돌 이펙트를 보여준 뒤 씬을 리셋하기까지 기다릴 시간")]
    public float resetDelay = 0.4f;

    // 충돌 처리가 여러 번 중복 실행되는 것을 방지합니다.
    private bool isHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHit)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            isHit = true;

            // 장애물 충돌 효과음 재생
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHitSound();
            }

            // 플레이어가 리셋 대기 시간 동안 계속 움직이지 않도록 즉시 멈춥니다.
            DragonPlayerMove playerMove = other.GetComponent<DragonPlayerMove>();

            if (playerMove != null)
            {
                playerMove.StopMovementImmediately();
            }

            // 플레이어 위치에 충돌 이펙트를 생성합니다.
            if (hitEffect != null)
            {
                Instantiate(hitEffect, other.transform.position, Quaternion.identity);
            }

            StartCoroutine(ResetAfterDelay());
        }
    }

    /// <summary>
    /// 이펙트가 보일 시간을 조금 준 뒤, GameSessionManager를 통해 씬을 다시 시작합니다.
    /// WaitForSecondsRealtime을 사용해 Time.timeScale 상태와 관계없이 작동하게 합니다.
    /// </summary>
    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSecondsRealtime(resetDelay);

        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.ReStartWithSameKeys();
        }
    }
}
