# PagingTemplate

키오스크 환경에서 사용하는 **페이징 UI 템플릿**입니다.
Start → Content → Result 순서로 화면을 전환하며, 무입력 시 자동으로 홈(Start)으로 복귀합니다.

- **플랫폼**: Windows (키오스크), Android 대응 가능
- **엔진**: Unity (단일 씬 구조)

---

## 아키텍처

**MVP + FSM + DataRepository** 패턴을 조합하여 사용합니다.

| 레이어 | 역할 |
|--------|------|
| **View** | UI 표시, 버튼 이벤트 발행 |
| **State (Presenter)** | View 이벤트 수신, 데이터 바인딩, 화면 전환 결정 |
| **StateMachine** | 상태 등록 및 전환 관리 (Init/Enter/Exit/Dispose 라이프사이클) |
| **DataRepository** | CSV 데이터 로드 → PageData 변환 → View 타입별 제공 |
| **NavigationManager** | 전체 오케스트레이터, StateMachine 소유, IdleManager 연동 |

---

## 디렉토리 구조

```
Assets/Scripts/
├── FSM/
│   ├── IState.cs             # 상태 인터페이스 (Init/Enter/Update/Exit/Dispose)
│   ├── BaseState.cs          # 제네릭 상태 베이스 (View 바인딩, GoTo 제공)
│   ├── StateMachine.cs       # 상태 전환 관리 (MonoBehaviour)
│   └── States/
│       ├── StartState.cs
│       ├── ContentState.cs
│       └── ResultState.cs
├── Manager/
│   ├── NavigationManager.cs  # 오케스트레이터 (MonoSingleton)
│   └── IdleManager.cs        # 무입력 타임아웃 감지 (MonoSingleton)
├── Model/
│   ├── PageData.cs           # View별 CSV 데이터 컨테이너
│   └── DataRepository.cs     # CSV 로드 및 PageData 매핑
├── View/
│   ├── BaseView.cs           # 뷰 베이스 (Show/Hide, 버튼 이벤트)
│   ├── StartView.cs
│   ├── ContentView.cs
│   └── ResultView.cs
└── Util/
    ├── MonoSingleton.cs      # 제네릭 MonoBehaviour 싱글톤 베이스
    └── CSVParser.cs          # StreamingAssets CSV 파싱 유틸 (동기)
```

---

## 주요 클래스

| 클래스 | 설명 |
|--------|------|
| `MonoSingleton<T>` | DontDestroyOnLoad 적용, 앱 종료 안전 처리, OnSingletonAwake/Destroy 훅 제공 |
| `NavigationManager` | StateMachine 생성 및 State 등록, IdleTimeout 시 StartState 복귀 |
| `StateMachine` | 상태 등록(AddState), 전환(ChangeState), Init 최초 1회 보장 |
| `BaseState<TState,TView>` | View 이벤트 구독, BindView() 훅, GoTo<T>() 편의 메서드 |
| `BaseView` | Root 패널 Show/Hide, Prev/Home/Next 버튼 이벤트 발행 |
| `DataRepository` | CSV 파일 로드 후 View 타입 → PageData 매핑 |
| `PageData` | key-value 데이터 컨테이너 (Get, GetFlag, Has 제공) |
| `CSVParser` | StreamingAssets에서 key-value CSV 파일 동기 파싱 |
| `IdleManager` | 60초 무입력 감지, OnIdleTimeout 이벤트 발행 |

---

## 화면 전환 흐름

```
[Start] --Next--> [Content] --Next--> [Result]
   ^                  |                  |
   |               Prev/Home          Prev/Home
   +------------------+------------------+
              (무입력 60초 타임아웃도 Start로 복귀)
```

- `OnNextClicked` / `OnPrevClicked` / `OnHomeClicked` 이벤트로 구동
- State 내부에서 `GoTo<T>()` 호출 → `NavigationManager.Instance.GoTo<T>()` → `StateMachine.ChangeState<T>()`

---

## 데이터 흐름

```
StreamingAssets/DataConfig.json   ← 각 View가 참조할 CSV 파일 목록 정의
         ↓
DataRepository: JSON 파싱 → CSV 파일 로드 → Dictionary 병합
         ↓
PageData: View별 key-value 데이터 컨테이너
         ↓
BaseState.BindView(): Presenter가 PageData → View UI에 세팅
```

---

## 개선 예정 사항

- [ ] `DataRepository`: 현재 CSV 파일명 하드코딩 → `DataConfig.json` 기반으로 교체
  - View당 여러 CSV 파일 지원, 키 충돌 시 후순위 파일이 덮어씀
  - JSON 파싱: `Newtonsoft.Json` (Json.NET) 사용 예정
- [ ] `CSVParser`: 동기 파일 읽기 → 비동기(`UnityWebRequest`) 전환 검토

---

## 변경 이력

| 날짜 | 변경 내용 |
|------|-----------|
| 2026-03-06 | `NavigationManager` → `MonoSingleton<NavigationManager>` 적용 |
| 2026-03-06 | `BaseState` 생성자에서 `Action<Type> goTo` 콜백 제거, `NavigationManager.Instance.GoTo<T>()` 직접 호출로 변경 |
| 2026-03-09 | README.md 분리, 글로벌 CLAUDE.md 및 write-readme Skill 추가 |
