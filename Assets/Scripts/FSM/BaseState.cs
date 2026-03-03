using UnityEngine;

public class BaseState<TState, TView> : IState where TView : BaseView
{
    protected TView _view;

    public BaseState(TView view)
    {
        _view = view;
    }

    public virtual void Init()
    {
        Debug.Log($"[{typeof(TState).Name}] Init");
    }

    public virtual void Enter()
    {
        Debug.Log($"[{typeof(TState).Name}] Enter");
        _view.OnPrevClicked += OnPrevClicked;
        _view.OnHomeClicked += OnHomeClicked;
        _view.OnNextClicked += OnNextClicked;
        _view.Show();
    }

    public virtual void Exit()
    {
        Debug.Log($"[{typeof(TState).Name}] Exit");
        _view.OnPrevClicked -= OnPrevClicked;
        _view.OnHomeClicked -= OnHomeClicked;
        _view.OnNextClicked -= OnNextClicked;
        _view.Hide();
    }

    public virtual void Update()
    {
        //
    }

    /// <summary>
    /// 이전 버튼 클릭 시 호출 (서브클래스에서 override)
    /// </summary>
    protected virtual void OnPrevClicked() { }

    /// <summary>
    /// 홈 버튼 클릭 시 호출 (서브클래스에서 override)
    /// </summary>
    protected virtual void OnHomeClicked() { }

    /// <summary>
    /// 다음 버튼 클릭 시 호출 (서브클래스에서 override)
    /// </summary>
    protected virtual void OnNextClicked() { }
}
