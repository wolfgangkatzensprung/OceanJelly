using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DirectorScript : MonoBehaviour
{
    public static DirectorScript Instance;

    [Header("References")]
    [Tooltip("Parent of all the background images")]
    public Transform backgroundImage;
    private CinemachineVirtualCamera activeVcam;
    CinemachineBrain brain;
    Transform mainCamTrans;

    public GameObject vcamObject;
    public GameObject cameraCollider;

    CinemachineVirtualCamera vcam;

    public CustomDeadzone customDeadzone;

    CinemachineBasicMultiChannelPerlin camPerlinNoise;

    [Header("Zoom")]
    float startZoom;

    [Header("ScreenShake")]
    bool shaking;
    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeIntensity;
    float shakeTimer;
    float perlinNoise;
    float perlinAdd;

    // Borders
    CinemachineComposer cc;

    // External Cam
    public bool externalCamActive = false;

    CinemachineConfiner confiner;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        mainCamTrans = Camera.main.transform;

        vcam = vcamObject.GetComponent<CinemachineVirtualCamera>();

        camPerlinNoise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        vcam.MoveToTopOfPrioritySubqueue();
        activeVcam = vcam;

        brain = Camera.main.transform.GetComponent<CinemachineBrain>();
        confiner = vcam.GetComponent<CinemachineConfiner>();

        startZoom = vcam.m_Lens.OrthographicSize;

        //vcamRechts.m_Lens.OrthographicSize = 64 * Screen.height / Screen.width;
        //vcamLinks.m_Lens.OrthographicSize = 64 * Screen.height / Screen.width;

        AbilityManager.Instance.onAbilityIncrease += UpgradeVision;
        PlayerManager.Instance.onRespawn += ResetVision;
    }

    private void Update()
    {
        if (shaking)
        {
            shakeTimer += shakeSpeed * Time.deltaTime;
        }
    }

    //void LateUpdate()
    //{
    //    if (shaking)
    //    {
    //        if (PlayerPrefs.GetInt("Screenshake") == 1)
    //        {
    //            ApplyScreenShake();
    //        }
    //    }
    //    if (!externalCamActive)
    //    {
    //        if (Input.GetAxis("Horizontal") == 0)
    //        {
    //            SwapToMidVcam();
    //        }

    //        else if (Input.GetAxisRaw("Horizontal") < 0)
    //        {
    //            if (!leftActive)
    //            {
    //                SwapToLeftVcam();
    //            }
    //        }

    //        else if (Input.GetAxisRaw("Horizontal") > 0)
    //        {
    //            if (!rightActive)
    //            {
    //                SwapToRightVcam();
    //            }
    //        }
    //    }
    //}

    public void UpgradeVision(AbilityManager.Abilities ability)
    {
        //if (ability.Equals(AbilityManager.Abilities.Vision))
        //{
        //    vcam.m_Lens.OrthographicSize = startZoom + AbilityManager.Instance.abilityLevels[ability] * 5f;
        //    backgroundImage.localScale = backgroundImage.localScale + Vector3.one * .1f * AbilityManager.Instance.abilityLevels[ability];
        //}
    }
    public void ResetVision()
    {
        vcam.m_Lens.OrthographicSize = startZoom;
    }
    public CinemachineVirtualCamera GetActiveVcam()
    {
        return activeVcam;
    }


    #region Toggle Brain
    internal void DisableBrain()
    {
        brain.enabled = false;
    }
    internal void EnableBrain()
    {
        brain.enabled = true;
    }
    #endregion

    #region Confiner
    internal void DisableConfiner()
    {
        confiner.enabled = false;
    }
    internal void EnableConfiner()
    {
        confiner.enabled = true;
    }
    internal void UpdateConfiner(Tilemap tilemap)
    {
        if (!confiner.enabled)
            confiner.enabled = true;
        SetCameraColliderForTilemap(tilemap);
    }
    internal void UpdateConfiner(EdgeCollider2D ec) // Delayed Update because need to wait for EdgeCollider Update from Splines
    {
        if (!confiner.enabled)
            confiner.enabled = true;
        StartCoroutine(UpdateCameraColliderCoroutine(ec));
    }
    private IEnumerator UpdateCameraColliderCoroutine(EdgeCollider2D ec)
    {
        // Wait for the end of the frame
        yield return new WaitForEndOfFrame();

        // Update the camera collider using the modified edge collider
        SetCameraColliderFromEdgeCollider(ec);
    }

    #endregion

    public void SetCameraColliderForTilemap(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;

        Vector2[] corners = new Vector2[4];
        corners[0] = tilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, 0));
        corners[1] = tilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMin, 0));
        corners[2] = tilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMax, 0));
        corners[3] = tilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMax, 0));

        cameraCollider.GetComponent<PolygonCollider2D>().SetPath(0, corners);
        cameraCollider.transform.position = tilemap.transform.position;
    }

    public void SetCameraColliderForSpline(UnityEngine.U2D.Spline spline, Vector3 centerPos)
    {
        Vector2[] points = new Vector2[spline.GetPointCount()];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = spline.GetPosition(i);
        }

        cameraCollider.GetComponent<PolygonCollider2D>().SetPath(0, points);
        cameraCollider.transform.position = centerPos;
    }

    //public void SetCameraColliderForCurvedSpline(UnityEngine.U2D.Spline spline, Vector3 centerPos)
    //{
    //    int numPointsPerSegment = 122;

    //    List<Vector2> points = new List<Vector2>();

    //    int numSegments = spline.GetPointCount() - 1;

    //    for (int i = 0; i < numSegments; i++)
    //    {
    //        Vector3 p0 = spline.GetPosition(i);
    //        Vector3 p1 = spline.GetPosition(i + 1);
    //        Vector3 t0 = spline.GetRightTangent(i).normalized;
    //        Vector3 t1 = spline.GetRightTangent(i + 1).normalized;

    //        for (int j = 0; j < numPointsPerSegment; j++)
    //        {
    //            float t = (float)j / (numPointsPerSegment - 1);
    //            Vector3 position = EvaluateCubicCurve(p0, p1, t0, t1, t);
    //            points.Add(position);
    //        }
    //    }

    //    PolygonCollider2D collider = cameraCollider.GetComponent<PolygonCollider2D>();
    //    collider.SetPath(0, points.ToArray());
    //    collider.transform.position = centerPos;
    //}

    //private Vector3 EvaluateCubicCurve(Vector3 p0, Vector3 p1, Vector3 t0, Vector3 t1, float t)
    //{
    //    float tSquared = t * t;
    //    float tCubed = tSquared * t;

    //    float blend0 = 2f * tCubed - 3f * tSquared + 1f;
    //    float blend1 = -2f * tCubed + 3f * tSquared;
    //    float blend2 = tCubed - 2f * tSquared + t;
    //    float blend3 = tCubed - tSquared;

    //    Vector3 position = blend0 * p0 + blend1 * p1 + blend2 * t0 + blend3 * t1;

    //    return position;
    //}

    //public void SetCameraColliderForCurvedSpline(UnityEngine.U2D.Spline spline, Vector3 centerPos)
    //{
    //    int numPointsPerSegment = 50;

    //    List<Vector2> points = new List<Vector2>();

    //    int numSegments = spline.GetPointCount() - 1;

    //    for (int i = 0; i < numSegments; i++)
    //    {
    //        Vector3 p0 = spline.GetPosition(i);
    //        Vector3 p1 = spline.GetPosition(i + 1);

    //        Vector3 t0 = spline.GetRightTangent(i).normalized;
    //        Vector3 t1 = spline.GetLeftTangent(i + 1).normalized;

    //        for (int j = 0; j < numPointsPerSegment; j++)
    //        {
    //            float t = (float)j / (numPointsPerSegment - 1);
    //            Vector3 position = CatmullRom(p0, p1, t0, t1, t);
    //            points.Add(position);
    //        }
    //    }

    //    PolygonCollider2D collider = cameraCollider.GetComponent<PolygonCollider2D>();
    //    collider.SetPath(0, points.ToArray());
    //    collider.transform.position = centerPos;
    //}

    public void SetCameraColliderFromEdgeCollider(EdgeCollider2D edgeCollider)
    {
        float offset = 12f; // Adjust the offset value as needed

        Vector2[] points = edgeCollider.points;
        Vector2[] expandedPoints = new Vector2[points.Length];

        Vector2 center = edgeCollider.bounds.center;
        Vector2 colliderPosition = edgeCollider.transform.position;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 direction = (points[i] - center).normalized;
            expandedPoints[i] = points[i] + direction * offset;
        }

        PolygonCollider2D polygonCollider = cameraCollider.GetComponent<PolygonCollider2D>();
        polygonCollider.SetPath(0, expandedPoints);
        polygonCollider.transform.position = colliderPosition;
    }





    //internal void InitializeCameraPositions(Vector2 targetLocation)
    //{
    //    // only works if vcams brain is off
    //    mainCamTrans.position = targetLocation;
    //    foreach (CinemachineVirtualCamera vc in vcamList)
    //    {
    //        vc.transform.position = targetLocation;
    //        Debug.Log($"vcam position: {vc.transform.position}");
    //    }
    //}

    #region Screenshake

    public void Screenshake()
    {
        shaking = true;
        shakeTimer = 0;
    }

    #endregion
}
