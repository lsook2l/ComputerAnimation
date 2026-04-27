# Unity 가위바위보 게임

Unity로 제작한 간단한 **가위바위보 게임**입니다.  
플레이어가 가위, 바위, 보 중 하나를 선택하면 컴퓨터가 랜덤으로 선택하고, 승부 결과에 따라 점수가 증가합니다.  
먼저 **5점**을 달성한 쪽이 최종 승리합니다.

---

## 프로젝트 소개

이 프로젝트는 Unity UI 버튼과 이미지, TextMeshPro를 활용하여 제작한 가위바위보 게임입니다.

주요 흐름은 다음과 같습니다.

1. 플레이어가 가위, 바위, 보 버튼 중 하나를 선택
2. 컴퓨터가 랜덤으로 가위, 바위, 보 중 하나 선택
3. 플레이어와 컴퓨터의 선택 이미지를 화면에 표시
4. 승패 판정 후 점수 반영
5. 한쪽이 5점에 도달하면 게임 종료 패널 표시

---

## 주요 기능

### 가위바위보 선택

플레이어는 UI 버튼을 통해 다음 중 하나를 선택할 수 있습니다.

- 가위
- 바위
- 보

선택한 값에 따라 플레이어 이미지가 변경됩니다.

### 컴퓨터 랜덤 선택

컴퓨터는 `Random.Range(0, 3)`을 사용하여 가위, 바위, 보 중 하나를 랜덤으로 선택합니다.

### 승패 판정

다음 규칙에 따라 승패를 판정합니다.

| 플레이어 | 컴퓨터 | 결과 |
|---|---|---|
| 가위 | 보 | 플레이어 승리 |
| 바위 | 가위 | 플레이어 승리 |
| 보 | 바위 | 플레이어 승리 |
| 같은 선택 | 같은 선택 | 무승부 |
| 그 외 | - | 컴퓨터 승리 |

### 점수 시스템

- 플레이어가 이기면 플레이어 점수 +1
- 컴퓨터가 이기면 컴퓨터 점수 +1
- 무승부일 경우 점수 변화 없음
- 먼저 5점을 달성한 쪽이 최종 승리

### 결과 패널

플레이어 또는 컴퓨터가 5점에 도달하면 결과 패널이 활성화됩니다.

- 플레이어가 5점 달성: `플레이어 승리`
- 컴퓨터가 5점 달성: `컴퓨터 승리`

게임 종료 후에는 가위, 바위, 보 버튼이 비활성화됩니다.

### 재시작 기능

`Restart` 버튼을 누르면 현재 점수는 유지한 채 게임을 다시 이어서 진행할 수 있습니다.

### 초기화 기능

`Reset` 버튼을 누르면 전체 게임 상태가 초기화됩니다.

초기화되는 항목은 다음과 같습니다.

- 플레이어 점수
- 컴퓨터 점수
- 결과 텍스트
- 선택 이미지
- 게임 종료 상태

### 종료 기능

`Exit` 버튼을 누르면 게임이 종료됩니다.

Unity Editor에서 실행 중일 경우에는 플레이 모드가 종료되고, 빌드된 실행 파일에서는 애플리케이션이 종료됩니다.

---

## 사용 기술

- Unity
- C#
- Unity UI
- TextMeshPro

---

## 스크립트 구성

### GameManager.cs

게임 전체 흐름을 관리하는 핵심 스크립트입니다.

주요 역할은 다음과 같습니다.

- 버튼 클릭 이벤트 연결
- 플레이어 선택 처리
- 컴퓨터 랜덤 선택 처리
- 가위바위보 승패 판정
- 점수 업데이트
- 결과 텍스트 출력
- 게임 종료 처리
- 재시작 및 초기화 처리
- 게임 종료 처리

---

## 주요 변수 설명

| 변수명 | 설명 |
|---|---|
| `buttonScissors` | 가위 버튼 |
| `buttonRock` | 바위 버튼 |
| `buttonPaper` | 보 버튼 |
| `buttonRestart` | 게임 재시작 버튼 |
| `buttonReset` | 게임 전체 초기화 버튼 |
| `buttonExit` | 게임 종료 버튼 |
| `imagePlayer` | 플레이어 선택 이미지 |
| `imageComputer` | 컴퓨터 선택 이미지 |
| `spriteScissors` | 가위 스프라이트 |
| `spriteRock` | 바위 스프라이트 |
| `spritePaper` | 보 스프라이트 |
| `resultText` | 현재 라운드 결과 텍스트 |
| `textScorePlayer` | 플레이어 점수 텍스트 |
| `textScoreComputer` | 컴퓨터 점수 텍스트 |
| `panelResult` | 최종 결과 패널 |
| `textResultPanel` | 최종 결과 텍스트 |
| `scorePlayer` | 플레이어 점수 |
| `scoreComputer` | 컴퓨터 점수 |
| `gameEnded` | 게임 종료 여부 |

---

## 실행 방법

1. Unity에서 프로젝트를 엽니다.
2. `GameManager.cs` 스크립트를 빈 오브젝트 또는 게임 관리 오브젝트에 추가합니다.
3. Inspector에서 버튼, 이미지, 텍스트, 스프라이트를 연결합니다.
4. Play 버튼을 눌러 게임을 실행합니다.
5. 가위, 바위, 보 버튼 중 하나를 눌러 게임을 진행합니다.

---

## Inspector 연결 항목

`GameManager` 스크립트에는 다음 항목을 Unity Inspector에서 직접 연결해야 합니다.

### UI Buttons

- Button Scissors
- Button Rock
- Button Paper
- Button Restart
- Button Reset
- Button Exit

### Display Images

- Image Player
- Image Computer

### Sprites

- Sprite Scissors
- Sprite Rock
- Sprite Paper

### Result Text

- Result Text

### Score Board

- Text Score Player
- Text Score Computer

### UI Panel

- Panel Result
- Text Result Panel

---

## 게임 규칙

- 플레이어와 컴퓨터가 각각 가위, 바위, 보 중 하나를 선택합니다.
- 같은 선택을 하면 무승부입니다.
- 플레이어가 이기면 플레이어 점수가 1점 증가합니다.
- 컴퓨터가 이기면 컴퓨터 점수가 1점 증가합니다.
- 먼저 5점을 달성한 쪽이 최종 승리합니다.

---

## 폴더 업로드 시 주의사항

Unity 프로젝트를 GitHub에 올릴 때는 `Library`, `Temp`, `Obj`, `Build`, `Builds`, `Logs` 같은 자동 생성 폴더는 업로드하지 않는 것이 좋습니다.  
프로젝트 최상단에 `.gitignore` 파일을 만들어 Unity용 ignore 설정을 적용해야 합니다.

예시:

```gitignore
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/
```

---

## 향후 개선 가능 사항

- 효과음 추가
- 승리/패배 애니메이션 추가
- 라운드별 기록 저장
- 난이도 선택 기능 추가
- UI 디자인 개선
- 모바일 해상도 대응
- WebGL 빌드 지원

---

## 라이선스

개인 학습 및 포트폴리오 목적으로 자유롭게 사용할 수 있습니다.
