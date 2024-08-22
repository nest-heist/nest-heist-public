using System;
using UnityEngine;

public abstract class BaseCondition<T> : MonoBehaviour, ICondition
{
    protected int _current;
    [SerializeField] public int Current { get => _current; set => _current = value; }
    [SerializeField] public int Max { get; set; }
    //해당 시스템에서 타입 정하기
    public abstract ConditionType Type { get; }

    public event Action<int, int> OnModifyEvent;
    public event Action OnFilledEvent;
    public event Action OnLackedEvent;

    //미리 스텟 핸들러를 받는다.
    protected T _statHandler;

    protected virtual void Awake()
    {
        //BaseCondition을 상속받은 객체는 MonsterHealthSystem : BaseCondition<MonsterStatHandler> 이런식으로 상속하면 될 거 같다.
    }

    protected virtual void Start()
    {
    }

    public virtual void Init()
    {
        _statHandler = GetComponent<T>();
        /*_current = Max;
        Modify(0);*/
    }

    public virtual bool Modify(int amount)
    {
        int prev = _current;
        int calCurrent = _current + amount;
        if (calCurrent > Max)
        {
            Fill();
            return false;
        }

        if (calCurrent < 0)
        {
            Lack();
            //return false;
        }

        _current = calCurrent;

        if (prev != _current)
        {
            OnModifyEvent?.Invoke(_current, Max);
            return true;
        }

        return false;
    }
    protected virtual void Fill()
    {
        OnFilledEvent?.Invoke();
    }
    protected virtual void Lack()
    {
        OnLackedEvent?.Invoke();
    }

    public void SetCurrentHealth(int health)
    {
        int prev = _current;
        _current = Mathf.Clamp(health, 0, Max);
        OnModifyEvent?.Invoke(_current, Max);
    }

}