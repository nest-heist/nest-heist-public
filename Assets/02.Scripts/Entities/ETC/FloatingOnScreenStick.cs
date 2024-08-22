using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class FloatingOnScreenJoystick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float moveThreshold = 1f;
    public JoystickType joystickType = JoystickType.Fixed;

    private Vector2 fixedPosition = Vector2.zero;

    [SerializeField]
    private RectTransform background; // 조이스틱 배경

    [SerializeField]
    private RectTransform handle; // 조이스틱 핸들

    [InputControl(layout = "Vector2")] // Input System을 사용해 Control Path 지정
    [SerializeField]
    private string m_ControlPath; // Control Path를 설정할 수 있는 필드

    public Canvas Canvas; // Canvas 객체를 저장할 변수
    private Vector2 input = Vector2.zero;

    protected override void OnEnable()
    {
        base.OnEnable();
        InitializeJoystick();
    }

    private void InitializeJoystick()
    {
        if (background == null || handle == null)
        {
            Debug.LogError("Background or Handle is not assigned!");
            return;
        }

        if (Canvas == null)
        {
            Debug.LogError("Canvas is not assigned!");
            return;
        }

        fixedPosition = background.anchoredPosition;
        SetMode(joystickType);
    }

    public void SetMode(JoystickType joystickType)
    {
        this.joystickType = joystickType;
        if (joystickType == JoystickType.Fixed)
        {
            background.anchoredPosition = fixedPosition;
            background.gameObject.SetActive(true);
        }
        else
        {
            background.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (joystickType != JoystickType.Fixed)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.gameObject.SetActive(true);
        }
        OnDrag(eventData); // 바로 드래그 상태로 들어가도록
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(null, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * Canvas.scaleFactor);

        // 민감도를 높이기 위해 스케일링
        float sensitivityMultiplier = 1.5f; // 민감도 조정 값
        input *= sensitivityMultiplier;

        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius);
        handle.anchoredPosition = input * radius;
        SendValueToControl(input);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        if (joystickType != JoystickType.Fixed)
        {
            background.gameObject.SetActive(false);
        }
        SendValueToControl(input);
    }

    private void FormatInput()
    {
        if (joystickType == JoystickType.Dynamic && input.magnitude > moveThreshold)
        {
            Vector2 difference = input.normalized * (input.magnitude - moveThreshold);
            background.anchoredPosition += difference;
        }
    }

    private void HandleInput(float magnitude, Vector2 normalised, Vector2 radius)
    {
        if (magnitude > 1)
            input = normalised;
    }

    private Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background.parent as RectTransform,
            screenPosition,
            null, // ScreenSpaceOverlay 모드에서는 Camera가 필요하지 않음
            out var localPoint
        );
        return localPoint;
    }

    // controlPathInternal 구현으로 Inspector에서 Control Path 설정 가능
    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}