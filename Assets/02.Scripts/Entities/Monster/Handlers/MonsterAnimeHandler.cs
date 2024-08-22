using UnityEngine;

public class MonsterAnimeHandler
{
    private MonsterStateMachine _stateMachine;
    public MonsterAnimeHandler(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void StartAnimation(int animatorHash)
    {
        _stateMachine.Monster.Animator.SetBool(animatorHash, true);
    }

    public virtual void StopAnimation(int animatorHash)
    {
        _stateMachine.Monster.Animator.SetBool(animatorHash, false);
    }

    public virtual void SetTriggerAnimation(int animatorHash)
    {
        _stateMachine.Monster.Animator.SetTrigger(animatorHash);
    }

    public virtual void SetFloatAnimation(int animatorHash, float value)
    {
        _stateMachine.Monster.Animator.SetFloat(animatorHash, value);
    }
    public virtual float GetNormalizedTime(Animator animator, string tag)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }
}
