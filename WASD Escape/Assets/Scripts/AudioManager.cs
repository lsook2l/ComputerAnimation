using UnityEngine;

/// <summary>
/// 게임 전체 사운드를 관리하는 스크립트입니다.
///
/// 담당 기능:
/// 1. 배경음악 재생
/// 2. 딸기 획득 효과음 재생
/// 3. 장애물 충돌 효과음 재생
/// 4. 게임 클리어 효과음 재생
///
/// 사용 위치:
/// - Hierarchy에 AudioManager 오브젝트를 만들고 이 스크립트를 붙입니다.
/// - Inspector에서 BGM, Collect, Hit, Clear 사운드 파일을 연결합니다.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // 다른 스크립트에서 AudioManager.Instance로 효과음을 재생할 수 있게 합니다.
    public static AudioManager Instance;

    [Header("배경음악")]
    [Tooltip("게임 진행 중 반복 재생할 배경음악")]
    public AudioClip bgmClip;

    [Header("효과음")]
    [Tooltip("딸기 보석을 획득했을 때 재생할 효과음")]
    public AudioClip collectSound;

    [Tooltip("장애물 또는 벽에 부딪혔을 때 재생할 효과음")]
    public AudioClip hitSound;

    [Tooltip("클리어 포털에 닿아 게임을 클리어했을 때 재생할 효과음")]
    public AudioClip clearSound;

    [Header("AudioSource")]
    [Tooltip("배경음악 전용 AudioSource")]
    public AudioSource bgmSource;

    [Tooltip("효과음 전용 AudioSource")]
    public AudioSource sfxSource;

    private void Awake()
    {
        // 씬이 재시작될 때 AudioManager가 중복 생성되는 것을 방지합니다.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 씬이 다시 로드되어도 배경음악이 끊기지 않도록 유지합니다.
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
    }

    private void Start()
    {
        PlayBGM();
    }

    /// <summary>
    /// AudioSource가 Inspector에 연결되지 않았을 경우 자동으로 생성합니다.
    /// 배경음악과 효과음을 분리하여 볼륨을 따로 조절할 수 있게 합니다.
    /// </summary>
    private void SetupAudioSources()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 0.4f;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = 0.8f;
    }

    /// <summary>
    /// 배경음악을 반복 재생합니다.
    /// 이미 재생 중이면 중복 재생하지 않습니다.
    /// </summary>
    public void PlayBGM()
    {
        if (bgmClip == null || bgmSource == null)
        {
            return;
        }

        if (bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }

    /// <summary>
    /// 딸기 보석 획득 효과음을 재생합니다.
    /// </summary>
    public void PlayCollectSound()
    {
        PlaySFX(collectSound);
    }

    /// <summary>
    /// 장애물 충돌 효과음을 재생합니다.
    /// </summary>
    public void PlayHitSound()
    {
        PlaySFX(hitSound);
    }

    /// <summary>
    /// 클리어 효과음을 재생합니다.
    /// </summary>
    public void PlayClearSound()
    {
        PlaySFX(clearSound);
    }

    /// <summary>
    /// 공통 효과음 재생 함수입니다.
    /// PlayOneShot을 사용해 여러 효과음이 겹쳐도 자연스럽게 재생되도록 합니다.
    /// </summary>
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }
}
