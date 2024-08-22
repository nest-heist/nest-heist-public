using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
/// <summary>
/// 타겟 몬스터가 존재하는지 확인하고 다음 과정으로 넘어가는것을
/// 체크하는 스크립트
/// </summary>
[TaskCategory("Boss001")]
public class CheckPlayer : Conditional
{
    public SharedTransform Target;
    public SharedMON10001 Boss;
    public string PartnerTag;
    public string PlayerTag;
    //Inspector 창에서 넣어줘야함

    private Transform[] _possibleTargets;

    public override void OnStart()
    {
        var targets = GameObject.FindGameObjectsWithTag(PlayerTag);
        _possibleTargets = new Transform[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            _possibleTargets[i] = targets[i].transform;
        }
    }

    public override TaskStatus OnUpdate()
    {
        for (int i = 0; i < _possibleTargets.Length; i++)
        {
            if (IsInside(_possibleTargets[i]))
            {
                Target.Value = _possibleTargets[i].transform;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
        return TaskStatus.Failure;
    }

    private bool IsInside(Transform transform)
    {
        if (transform.position.x >= Boss.Value.TopLeft.x && transform.position.x <= Boss.Value.BottomRight.x && transform.position.y >= Boss.Value.BottomRight.y && transform.position.y <= Boss.Value.TopLeft.y)
        {
            return true;
        }
        return false;
    }
}

