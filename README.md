# PagingTemplate

> 키오스크 환경을 위한 페이징 UI 템플릿

`[📷 이미지 배치 예정 - 앱 메인 화면 스크린샷]`

| 항목 | 내용 |
|------|------|
| 플랫폼 | Windows (키오스크) |
| 엔진 | Unity 6 / C# |
| 구조 | 단일 씬, MVP + FSM + DataRepository |
| 관련 링크 | [GitHub](https://github.com/kih0/side_unity_PagingTemplate) |

**문서**: [포트폴리오 명세서](PORTFOLIO_SPEC.md) · [화면 추가 가이드](GUIDE_AddView.md)

---

## 화면 구성

| Start | Content | Result |
|:-----:|:-------:|:------:|
| `[📷 이미지 배치 예정]` | `[📷 이미지 배치 예정]` | `[📷 이미지 배치 예정]` |

```
[Start] ──Next──▶ [Content] ──Next──▶ [Result]
   ▲                  │                   │
   │              Prev/Home           Prev/Home
   └──────────────────┴───────────────────┘
              (무입력 60초 → Start 자동 복귀)
```

| 화면 | 설명 |
|------|------|
| Start | 초기 진입 화면, Next 버튼으로 Content 이동 |
| Content | 메인 콘텐츠 화면, Prev/Home/Next 내비게이션 |
| Result | 결과 화면, Prev로 Content · Home으로 Start 이동 |

- 모든 화면에서 Home 클릭 또는 60초 무입력 시 Start로 자동 복귀
- 버튼 이벤트(`OnPrevClicked`/`OnHomeClicked`/`OnNextClicked`)로 화면 전환 구동

---

## 핵심 기능

| 기능 | 설명 |
|------|------|
| FSM 기반 화면 전환 | State별 Init/Enter/Exit/Dispose 라이프사이클 관리 |
| CSV 데이터 바인딩 | DataConfig.json으로 View별 CSV 파일 동적 로딩, 복수 CSV 병합 지원 |
| 무입력 자동 복귀 | 마우스·키보드·터치 60초 무입력 감지 시 Start 화면 복귀 |
| 싱글톤 매니저 | MonoSingleton 기반 NavigationManager, IdleManager |
| 템플릿 확장 | BaseView/BaseState 상속으로 새 화면 추가 가능 |

---

## 아키텍처

**MVP + FSM + DataRepository** 패턴을 조합하여 View-State-Data를 분리합니다.

| 레이어 | 역할 | 주요 클래스 |
|--------|------|-------------|
| View | UI 표시, 버튼 이벤트 발행 | `BaseView`, `StartView`, `ContentView`, `ResultView` |
| State (Presenter) | View 이벤트 수신, 데이터 바인딩, 화면 전환 결정 | `BaseState<TState,TView>`, `StartState`, `ContentState`, `ResultState` |
| FSM | 상태 등록·전환 관리, Init 최초 1회 보장 | `IState`, `StateMachine` |
| Model | DataConfig.json 기반 CSV 로드 → PageData 변환 | `DataRepository`, `PageData` |
| Manager | 오케스트레이터, FSM 소유, 무입력 감지 | `NavigationManager`, `IdleManager` |
| Util | 싱글톤 베이스, CSV 파싱 | `MonoSingleton<T>`, `CSVParser` |

> 모든 네임스페이스 루트: `PagingTemplate` (예: `PagingTemplate.FSM.States`)

---

## 디렉토리 구조

```
Assets/Scripts/
├── FSM/
│   ├── IState.cs                # 상태 인터페이스
│   ├── BaseState.cs             # 제네릭 상태 베이스
│   ├── StateMachine.cs          # 상태 전환 관리
│   └── States/                  # 화면별 State 구현 (3개)
├── Manager/
│   ├── NavigationManager.cs     # 오케스트레이터 (MonoSingleton)
│   └── IdleManager.cs           # 무입력 타임아웃 감지 (MonoSingleton)
├── Model/
│   ├── PageData.cs              # View별 key-value 데이터 컨테이너
│   └── DataRepository.cs        # DataConfig.json 기반 CSV 로드
├── View/                        # BaseView + 화면별 View (3개)
└── Util/
    ├── MonoSingleton.cs         # 제네릭 싱글톤 베이스
    └── CSVParser.cs             # StreamingAssets CSV 파싱

Assets/StreamingAssets/
├── DataConfig.json              # View-CSV 매핑 설정
└── *.csv                        # 화면별 데이터 파일
```

---

## 데이터 흐름

```
DataConfig.json          ← View별 CSV 파일 목록 정의
     ↓
DataRepository           ← JSON 파싱 → CSV 로드 → Dictionary 병합
     ↓
PageData                 ← View별 key-value 컨테이너
     ↓
BaseState.BindView()     ← Presenter가 PageData → View UI에 세팅
```

**DataConfig.json 예시:**
```json
{
  "items": [
    { "viewType": "StartView",   "fileNames": ["StartData.csv"] },
    { "viewType": "ContentView", "fileNames": ["ContentData.csv"] },
    { "viewType": "ResultView",  "fileNames": ["ResultData.csv"] }
  ]
}
```

---

## 변경 이력

| 날짜 | 내용 |
|------|------|
| 2026-03-09 | DataRepository: CSV 파일명 하드코딩 → DataConfig.json 기반 동적 로딩으로 전환 |
| 2026-03-09 | 전체 클래스에 `PagingTemplate.*` 네임스페이스 적용 |
| 2026-03-06 | `NavigationManager` → `MonoSingleton<NavigationManager>` 적용 |
| 2026-03-06 | `BaseState` 생성자에서 `Action<Type> goTo` 콜백 제거, `NavigationManager.Instance.GoTo<T>()` 직접 호출로 변경 |
