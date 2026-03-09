# 새 View 추가 가이드

MVP + FSM + DataRepository 패턴 기반의 PagingTemplate에 새 화면을 추가하는 방법을 설명합니다.

---

## 전체 흐름

```
XxxView (화면 표시)
  ↕  이벤트
XxxState (로직 처리)  ←  PageData (CSV 데이터)
  ↕  GoTo<T>()
NavigationManager (화면 전환)
```

- **View**: UI 요소 보유, 버튼 이벤트 발행
- **State**: View에 데이터 세팅(Presenter 역할), 버튼 이벤트 구독, 화면 전환 결정
- **PageData**: CSV에서 읽어온 key-value 데이터 묶음

---

## 체크리스트

새 화면 `Xxx`를 추가할 때 수정/생성해야 하는 항목입니다.

- [ ] `Assets/Scripts/View/XxxView.cs` 생성
- [ ] `Assets/Scripts/FSM/States/XxxState.cs` 생성
- [ ] `Assets/Scripts/Manager/NavigationManager.cs` 수정
- [ ] `Assets/StreamingAssets/DataConfig.json` 수정
- [ ] `Assets/StreamingAssets/XxxData.csv` 생성
- [ ] Unity 에디터: GameObject 생성 및 SerializeField 연결

---

## Step 1. XxxView 생성

`Assets/Scripts/View/XxxView.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using PagingTemplate.View;

namespace PagingTemplate.View
{

public class XxxView : BaseView
{
    // View가 보유한 UI 컴포넌트를 SerializeField로 선언
    [SerializeField] private Text _txtTitle;
    [SerializeField] private Image _imgBackground;

    // State에서 호출할 데이터 세팅 메서드 정의
    public void SetTitle(string title) => _txtTitle.text = title;
}

} // namespace PagingTemplate.View
```

**규칙:**
- `BaseView`를 상속한다.
- UI 컴포넌트는 `[SerializeField]`로 선언한다.
- State가 데이터를 세팅할 수 있도록 public 메서드를 제공한다.
- `BaseView`에서 이미 제공하는 기능은 중복 구현하지 않는다.

> **BaseView가 기본 제공하는 것**
> - `_rootPanel`: 화면 활성화/비활성화 대상 패널
> - `Show()` / `Hide()` / `Toggle()`
> - `OnPrevClicked` / `OnHomeClicked` / `OnNextClicked` 이벤트
> - `_btnPrev` / `_btnHome` / `_btnNext` 버튼 바인딩

---

## Step 2. XxxState 생성

`Assets/Scripts/FSM/States/XxxState.cs`

```csharp
using PagingTemplate.View;
using PagingTemplate.Model;
using PagingTemplate.FSM;
using PagingTemplate.FSM.States;

namespace PagingTemplate.FSM.States
{

public class XxxState : BaseState<XxxState, XxxView>
{
    public XxxState(XxxView view, PageData data) : base(view, data)
    {
    }

    public override void Init()
    {
        base.Init();
        // 최초 1회만 실행: 리소스 로딩, 추가 이벤트 구독 등
    }

    public override void Enter()
    {
        base.Enter();   // 내부적으로 BindView() → View.Show() 순서로 호출됨
        // 매번 진입 시 실행: UI 리셋, 값 초기화 등
    }

    public override void Exit()
    {
        base.Exit();    // 내부적으로 View.Hide() 호출
    }

    protected override void BindView()
    {
        // Enter() 직전에 호출됨. CSV 데이터를 View에 세팅하는 곳
        _view.SetTitle(_data.Get("Title"));
    }

    // 버튼 이벤트 핸들러: 필요한 것만 override
    protected override void OnPrevClicked() => GoTo<PrevState>();
    protected override void OnHomeClicked() => GoTo<StartState>();
    protected override void OnNextClicked() => GoTo<NextState>();
}

} // namespace PagingTemplate.FSM.States
```

---

## Step 3. NavigationManager에 등록

`Assets/Scripts/Manager/NavigationManager.cs`

### 3-1. SerializeField 추가

```csharp
[Header("Views")]
[SerializeField] private StartView   _startView;
[SerializeField] private ContentView _contentView;
[SerializeField] private ResultView  _resultView;
[SerializeField] private XxxView     _xxxView;      // 추가
```

### 3-2. ActivateAllViews()에 추가

```csharp
private void ActivateAllViews()
{
    _startView.gameObject.SetActive(true);
    _contentView.gameObject.SetActive(true);
    _resultView.gameObject.SetActive(true);
    _xxxView.gameObject.SetActive(true);            // 추가
}
```

### 3-3. RegisterState()에 추가

```csharp
private void RegisterState()
{
    var repo = new DataRepository();

    StateMachine.AddState(new StartState  (_startView,   repo.GetData<StartView>()));
    StateMachine.AddState(new ContentState(_contentView, repo.GetData<ContentView>()));
    StateMachine.AddState(new ResultState (_resultView,  repo.GetData<ResultView>()));
    StateMachine.AddState(new XxxState    (_xxxView,     repo.GetData<XxxView>()));  // 추가
}
```

---

## Step 4. DataConfig.json에 항목 추가

`Assets/StreamingAssets/DataConfig.json`

```json
{
  "views": [
    { "viewType": "PagingTemplate.View.StartView",   "csvFiles": ["StartData.csv"] },
    { "viewType": "PagingTemplate.View.ContentView", "csvFiles": ["ContentData.csv"] },
    { "viewType": "PagingTemplate.View.ResultView",  "csvFiles": ["ResultData.csv"] },
    { "viewType": "PagingTemplate.View.XxxView",     "csvFiles": ["XxxData.csv"] }
  ]
}
```

- `viewType`은 반드시 **네임스페이스 포함 전체 이름**으로 작성한다.
- `csvFiles`는 배열이므로 View 하나에 여러 CSV를 연결할 수 있다. 키 충돌 시 후순위 파일이 덮어쓴다.

---

## Step 5. CSV 파일 생성

`Assets/StreamingAssets/XxxData.csv`

```
Key,Value
Title,화면 제목
Description,설명 텍스트
```

- 1행은 반드시 `Key,Value` 헤더여야 한다.
- State의 `BindView()`에서 `_data.Get("Key")` 형태로 값을 읽는다.

### PageData API

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Get("Key")` | `string` | 문자열 값 조회. 없으면 `""` 반환 |
| `Get("Key", "기본값")` | `string` | 없을 때 기본값 반환 |
| `GetFlag("Key")` | `bool` | `"true"` 또는 `"1"` → true |
| `Has("Key")` | `bool` | 키 존재 여부 확인 |

---

## Step 6. Unity 에디터 설정

1. Hierarchy에 새 GameObject를 생성하고 이름을 `XxxView`로 지정한다.
2. `XxxView.cs`를 해당 GameObject에 컴포넌트로 추가한다.
3. Inspector에서 `BaseView`의 SerializeField를 연결한다.
   - `Root Panel`: 화면 전체를 감싸는 패널 오브젝트
   - `Btn Prev` / `Btn Home` / `Btn Next`: 해당 버튼 (없으면 비워둠)
4. NavigationManager의 Inspector에서 `_xxxView` 필드에 해당 GameObject를 연결한다.

> `_showOnAwake`는 반드시 **false**로 유지한다.
> NavigationManager가 Awake 시 `ActivateAllViews()`로 강제 활성화하고, BaseView가 즉시 `Hide()`를 호출하여 숨긴다.

---

## State 라이프사이클 정리

```
등록 시 (RegisterState)
  └─ new XxxState(view, data)   ← 생성자: 필드 초기화

최초 진입 시
  └─ Init()      ← 단 1회. 이벤트 구독, 리소스 초기화

매번 진입 시
  └─ Enter()     ← BindView() → View.Show() 순서로 진행
       └─ BindView()  ← CSV 데이터를 View UI에 세팅

다른 화면으로 이동 시
  └─ Exit()      ← View.Hide()

매 프레임
  └─ Update()    ← 애니메이션, 타이머 등 프레임 처리

앱 종료 시
  └─ Dispose()   ← 이벤트 구독 해제
```

### 무엇을 어디에 넣어야 하는가

| 작업 | 위치 | 이유 |
|------|------|------|
| 이벤트 구독 (버튼 외) | `Init()` | 구독 중복 방지 (1회만 실행) |
| 이벤트 구독 해제 (버튼 외) | `Dispose()` | 앱 종료 시 StateMachine이 자동 호출 |
| View에 데이터 세팅 | `BindView()` | Enter 직전 자동 호출됨 |
| UI 리셋 / 값 초기화 | `Enter()` | 화면 진입 시마다 실행 |
| 애니메이션, 카운트다운 등 | `Update()` | 매 프레임 처리 |
| View 숨기기 외 정리 작업 | `Exit()` | 다른 화면으로 나갈 때 |

> 버튼 3개(`OnPrevClicked`, `OnHomeClicked`, `OnNextClicked`)의 이벤트 구독/해제는 `BaseState`가 자동으로 처리한다.

---

## 완성된 구조 예시

`XxxView`가 추가된 이후의 전체 흐름:

```
Start → [다음] → Content → [다음] → Xxx → [다음] → Result
                             ↑ [이전]          ↑ [이전]
         [홈] 누르면 어디서든 Start로 복귀
```

각 State의 버튼 핸들러:

```csharp
// XxxState
protected override void OnPrevClicked() => GoTo<ContentState>();
protected override void OnHomeClicked() => GoTo<StartState>();
protected override void OnNextClicked() => GoTo<ResultState>();
```
