using UnityEngine;

public class SwipeHandler : Singleton<SwipeHandler>
{
    PlayerInputManager pim;
    public TrailRenderer trail;
    public ParticleSystem swipeParticles;

    [SerializeField]
    float minDistance = .2f;
    [SerializeField]
    float maxDistance = 10f;
    [SerializeField]
    float swipeSpeed = 12f;
    Vector2 startPos;   // screen pos of trail start
    Vector2 endPos; // screen pos of trail end
    float startTime;
    float endTime;
    [SerializeField]
    float maxSwipeTime = 1.2f;
    float swipeTimer = 0f;
    float maxTapDuration = .2f; // max tap duration = swipe buildup time
    bool swiping;

    internal delegate void OnSwipedDelegate(Vector2 direction);
    internal OnSwipedDelegate onSwiped;

    private new void Awake()
    {
        base.Awake();
        pim = PlayerInputManager.Instance;
        trail.emitting = false;
        swipeParticles.enableEmission = false;
    }
    private void OnEnable()
    {
        pim.onStartTouch += SwipeStart;
        pim.onEndTouch += SwipeEnd;
        pim.onTap += ClearSwipe;
    }
    private void OnDisable()
    {
        pim.onStartTouch -= SwipeStart;
        pim.onEndTouch -= SwipeEnd;
        pim.onTap -= ClearSwipe;
    }
    private void Update()
    {
        if (swiping)
        {
            swipeTimer += Time.unscaledTime;
            if (swipeTimer > maxTapDuration)
            {
                if (!trail.emitting)
                {
                    trail.emitting = true;
                    swipeParticles.enableEmission = true;
                }
                trail.transform.position = pim.TouchPositionWorld();
            }
            else if (swipeTimer > maxSwipeTime)
            {
                SwipeEnd(trail.transform.position, maxSwipeTime);
            }
        }
    }
    private void SwipeStart(Vector2 position, float time)
    {
        Debug.Log("Swipe Start");
        trail.transform.position = pim.TouchPositionWorld();
        swipeTimer = 0f;
        startTime = time;
        startPos = pim.touchPositionPassThroughAction.ReadValue<Vector2>();
        swiping = true;
        trail.Clear();
    }
    private void SwipeEnd(Vector2 position, float time)
    {
        if (!swiping)
            return;

        Debug.Log("Swipe End");
        endTime = time;
        endPos = pim.touchPositionPassThroughAction.ReadValue<Vector2>();
        ClearSwipe(position);
        DetectSwipe();
    }

    private void ClearSwipe(Vector2 nix)
    {
        Debug.Log("Clear Swipe");
        swipeTimer = 0f;
        swiping = false;
        trail.emitting = false;
        trail.Clear();
        swipeParticles.enableEmission = false;
    }

    void DetectSwipe()
    {
        if (Vector2.Distance(startPos, endPos) >= minDistance)
        {
            onSwiped?.Invoke((endPos - startPos));
        }
    }
}
