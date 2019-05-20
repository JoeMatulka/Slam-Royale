using UnityEngine;

public class Crowd : MonoBehaviour
{
    private Camera cam;
    private GameManager gameManager;
    private float bounceSpeed;

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        bounceSpeed = Random.Range(5, 10);
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void Update()
    {
        if (!gameManager.isPaused && !gameManager.GameSlowMo)
        {
            Billboard();
            Bounce();
        }
    }

    private void Billboard()
    {
        Vector3 targetPostition = new Vector3(cam.transform.position.x,
                                       transform.position.y,
                                       cam.transform.position.z);
        transform.LookAt(targetPostition);
    }

    private void Bounce()
    {
        float bounce = Mathf.Sin(Time.time * bounceSpeed) * .0004f;
        transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z + bounce
            );
    }

    private void FlashPicture() {

    }
}
