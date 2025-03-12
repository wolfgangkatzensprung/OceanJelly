using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager _instance;
    public static PlayerInputManager Instance { get { return _instance; } }

    public PlayerInput playerInput;
    Camera mainCam;

    internal InputAction touchPressAction;      // tap
    //internal InputAction touchPressPassThroughAction;   // tap als passthrough
    internal InputAction touchPositionAction;
    internal InputAction touchPositionPassThroughAction;

    //internal InputAction zoomAction;      // pinch zoom
    //internal delegate void OnZoomStartDelegate();
    //internal OnZoomStartDelegate onZoomStart;
    //internal delegate void OnZoomEndDelegate();
    //internal OnZoomStartDelegate onZoomEnd;

    internal delegate void OnPlayerMoveDelegate(Vector2 touchPos);
    internal OnPlayerMoveDelegate onTap;
    internal delegate void OnTouchPressStartDelegate(Vector2 touchPos);
    internal OnPlayerMoveDelegate onTouchPressStart;

    public delegate void StartTouchDelegate(Vector2 position, float time);
    public StartTouchDelegate onStartTouch;
    public delegate void EndTouchDelegate(Vector2 position, float time);
    public EndTouchDelegate onEndTouch;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (playerInput == null)
            Debug.Log("PlayerInput is null");

        mainCam = Camera.main;

        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
        touchPositionPassThroughAction = playerInput.actions["TouchPosition_PassThrough"];
        //zoomAction = playerInput.actions["Zoom"];
    }

    private void OnEnable()
    {
        touchPressAction.Enable();
        touchPressAction.performed += TapMoveAction;
        touchPressAction.started += TouchPressStart;
        touchPressAction.canceled += TouchPressEnd;

        //zoomAction.Enable();
        //zoomAction.performed += ZoomAction;
        //zoomAction.started += StartZoom;
        //zoomAction.canceled += EndZoom;

    }
    private void OnDisable()
    {
        touchPressAction.Disable();
        touchPressAction.performed -= TapMoveAction;
        touchPressAction.started -= TouchPressStart;
        touchPressAction.canceled -= TouchPressEnd;

        //zoomAction.Disable();
        //zoomAction.performed -= ZoomAction;
        //zoomAction.started -= StartZoom;
        //zoomAction.canceled -= EndZoom;
    }

    private void TouchPressStart(InputAction.CallbackContext context) // when touch press action is started
    {
        if (GameController.Instance.IsGamePaused())
        {
            //Debug.Log($"TouchPressAction mit TouchPosition 0)");
            onTouchPressStart?.Invoke(Vector2.zero);
        }
        //Debug.Log($"TouchPressAction mit TouchPosition: {touchPositionAction.ReadValue<Vector2>()}");

        onStartTouch?.Invoke(touchPositionPassThroughAction.ReadValue<Vector2>(), (float)context.startTime);
        onTouchPressStart?.Invoke(touchPressAction.ReadValue<Vector2>());
    }

    private void TouchPressEnd(InputAction.CallbackContext context)
    {
        //Debug.Log("TouchPressEnd");
        onEndTouch?.Invoke(touchPositionPassThroughAction.ReadValue<Vector2>(), (float)context.time);
    }

    private void TapMoveAction(InputAction.CallbackContext context)
    {
        //Debug.Log($"Tap MoveAction mit TouchPosition: {touchPositionAction.ReadValue<Vector2>()}");
        onTap?.Invoke(touchPositionAction.ReadValue<Vector2>());
    }

    /// <summary>
    /// World Position of Touch (Passthrough)
    /// </summary>
    /// <returns></returns>
    public Vector2 TouchPositionWorld()
    {
        return mainCam.ScreenToWorldPoint(touchPositionPassThroughAction.ReadValue<Vector2>());
    }

    //private void ZoomAction(InputAction.CallbackContext context)
    //{
    //    float zoomAmount = context.ReadValue<Vector2>().y;
    //    onZoomStart?.Invoke(zoomAmount);
    //}

}
