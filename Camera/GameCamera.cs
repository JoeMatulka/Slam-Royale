using System.Collections;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    private readonly Vector3 DEF_GAME_VIEW_POS = new Vector3(0, 15, 200);
    private readonly Quaternion DEF_GAME_CAM_ROT = Quaternion.Euler(0, 180, 0);
    private readonly int DEF_GAME_FOV = 10;

    private readonly Vector3 MAIN_MENU_VIEW_POS = new Vector3(0, 225, 65);
    private readonly Quaternion MAIN_MENU_CAM_ROT = Quaternion.Euler(75, 180, 0);
    private readonly int MAIN_MENU_FOV = 25;

    private const float CAM_DAMPEN = .15f;
    private const float MAX_CAM_RAYCAST = 100;
    private Vector3 CAM_VELOCITY = Vector3.zero;
    private Camera m_camera;

    public bool m_IsCameraOffset = false;
    public Vector3 m_ManualCameraOffset = Vector3.zero;

    public float m_ZoomSpeed;
    public bool Zooming;
    private float m_ZoomTime;
    private float m_Zoom;

    public Transform Target;
    private int m_targetLayer;

    public delegate void CameraFollowDelegate(float deltaMovement);
    public CameraFollowDelegate onCameraTranslate;

    public delegate void CameraFollowMoveDelegate(float posX, float posY);
    public CameraFollowMoveDelegate onCameraMove;

    private float oldPositionX;
    private float oldPositionY;

    private bool m_shaking = false;

    private GameManager gameManager;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        m_camera = GetComponent<Camera>();
        m_camera.fieldOfView = MAIN_MENU_FOV;
        m_targetLayer = 1 << LayerMask.NameToLayer("Default");
    }

    void LateUpdate()
    {
        if (Target != null && !Zooming)
        {
            Vector3 position = GetCameraPointPosition();
            if (!m_shaking)
            {
                MoveCamera(position);
            }
            else {
                ShakeCamera();
            }
        }
    }

    public void Shake()
    {
        m_shaking = true;
        Invoke("StopShake", .2f);
    }

    private void StopShake()
    {
        m_shaking = false;
    }

    public void SetMainMenuCamera()
    {
        transform.position = MAIN_MENU_VIEW_POS;
        transform.rotation = MAIN_MENU_CAM_ROT;
        m_camera.fieldOfView = MAIN_MENU_FOV;
    }

    private void ShakeCamera()
    {
        if (!gameManager.isPaused)
        {
            float bounce = Mathf.Sin(Time.time * 100);
            transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    transform.localPosition.y + bounce,
                    transform.localPosition.z
                );
        }
    }

    private void MoveCamera(Vector3 position)
    {
        Vector3 point = m_camera.WorldToViewportPoint(position);
        Vector3 delta = position - m_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, point.z));
        Vector3 destination = transform.position + delta;
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref CAM_VELOCITY, CAM_DAMPEN);
    }

    private Vector3 GetCameraPointPosition()
    {
        RaycastHit hit;
        Vector3 pos = Target.position;
        if (m_IsCameraOffset)
        {
            pos.x += m_ManualCameraOffset.x;
            pos.y += m_ManualCameraOffset.y;
            pos.z += m_ManualCameraOffset.z;
        }
        if (Physics.Raycast(Target.position, -Target.up, out hit, MAX_CAM_RAYCAST, m_targetLayer, QueryTriggerInteraction.Ignore))
        {
            pos = hit.point;
        }
        return pos;
    }

    public IEnumerator ZoomToGameStart(float duration)
    {
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            m_camera.transform.rotation = MAIN_MENU_CAM_ROT;
            m_camera.transform.position = MAIN_MENU_VIEW_POS;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;
                // progress will equal 0 at startTime, 1 at endTime.
                m_camera.transform.rotation = Quaternion.Slerp(MAIN_MENU_CAM_ROT, DEF_GAME_CAM_ROT, progress);
                m_camera.transform.position = Vector3.Lerp(MAIN_MENU_VIEW_POS, DEF_GAME_VIEW_POS, progress);
                m_camera.fieldOfView = Mathf.Lerp(MAIN_MENU_FOV, DEF_GAME_FOV, progress);
                yield return null;
            }
        }
        m_camera.transform.rotation = DEF_GAME_CAM_ROT;
        m_camera.transform.position = DEF_GAME_VIEW_POS;
        m_camera.fieldOfView = DEF_GAME_FOV;
    }

    public IEnumerator ZoomToCinematicActors(float duration, Actor attacker, Actor reciever)
    {
        Zooming = true;
        Vector3 zoomTarget = Vector3.zero;
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;
                // progress will equal 0 at startTime, 1 at endTime.
                zoomTarget = Vector3.Lerp(attacker.transform.position, reciever.transform.position, .5f);
                Vector3 newPos = Vector3.Lerp(m_camera.transform.position, zoomTarget, progress);
                m_camera.transform.position = new Vector3(
                        newPos.x,
                        newPos.y,
                        newPos.z <= 75f ? 75f: newPos.z
                    );
                yield return null;
            }
        }
        zoomTarget = Vector3.Lerp(attacker.transform.position, reciever.transform.position, .5f);
        m_camera.transform.position = new Vector3(
                        zoomTarget.x,
                        zoomTarget.y,
                        zoomTarget.z <= 75f ? 75f : zoomTarget.z
                    );
    }

    public void ResetCameraToGameView() {
        m_camera.transform.rotation = DEF_GAME_CAM_ROT;
        m_camera.transform.position = DEF_GAME_VIEW_POS;
        m_camera.fieldOfView = DEF_GAME_FOV;
        Zooming = false;
    }
}

