using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using TMPro;
using BehaviorDesigner.Runtime;

[TaskCategory("Boss001")]
public class PatternAlertText : Action
{
    [SerializeField] public SharedGameObject AlertText;
    private TextMeshProUGUI _alertText;
    [Header("경고 내용")]
    public string AlertTextStr;

    public float AlertTime;
    public float TotalTime;
    public float StartTime;


    public override void OnStart()
    {
        _alertText = AlertText.Value.GetComponent<TextMeshProUGUI>();
        _alertText.text = AlertTextStr;
        _alertText.gameObject.SetActive(true);
        StartTime = Time.time;
        TotalTime = Time.time + AlertTime;
    }

    public override TaskStatus OnUpdate()
    {
        if(TotalTime <= Time.time)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        _alertText.gameObject.SetActive(false);
    }
}
