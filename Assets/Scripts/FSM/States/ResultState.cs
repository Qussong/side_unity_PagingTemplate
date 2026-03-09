public class ResultState : BaseState<ResultState, ResultView>
{
    public ResultState(ResultView view, PageData data) : base(view, data)
    {
    }

    public override void Init()
    {
        base.Init();
        // 최초 1회: 리소스 로딩, 이벤트 바인딩 등
    }

    public override void Enter()
    {
        base.Enter();
        // 매번 진입 시: UI 리셋, 값 초기화 등
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected override void OnPrevClicked()
    {
        GoTo<ContentState>();
    }

    protected override void OnHomeClicked()
    {
        GoTo<StartState>();
    }
}
