using UnityEngine;

public class CameraShake : MonoBehaviour 
{
    [SerializeField] private float shakingDurationInSeconds = .1f;

    private Transform cameraTransform;
    private Vector3 positionIncrement = new Vector3(.05f, 0f, 0f);   // make this variable weapon dependent
    private Vector3 cameraInitialPosition, cameraMaxPosition;
    private bool isCameraShaking = false;
    float timePassedShaking = 0f;

    const float FIFTY_PERCENT = 0.5f;    

    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
        cameraTransform = GetComponent<Transform>();
        PlayerShoot.OnBulletFired += BulletFiredCameraShake;
        ExplosionEmitterForBullet.OnBulletExplosion += ExplosionCameraShake;
    }

    private void OnDestroy()
    {
        PlayerShoot.OnBulletFired -= BulletFiredCameraShake;
        ExplosionEmitterForBullet.OnBulletExplosion -= ExplosionCameraShake;
    }

    private void FixedUpdate()
    {
        if (!HasShakingDurationBeenReached() && isCameraShaking)
        {
            timePassedShaking += Time.deltaTime;
            ShakeCamera();
        }
        else
        {
            ResetTimePassedShaking();
        }
    }

    public void ShakeCamera()
    {
        Vector3 newCameraPosition = CalculateNewCameraPosition();
        cameraTransform.transform.position = newCameraPosition;
    }

    private void ExplosionCameraShake()
    {
        PrepareCameraShake();
    }

    private void BulletFiredCameraShake(int useless) // this int is the remaing bullets on the gun, useless here, we are reusing an event
    {
        PrepareCameraShake();
    }
       
    private Vector3 CalculateNewCameraPosition()
    {
        if (ShakeProgressHasPercentage() >= FIFTY_PERCENT)
            return Vector3.Lerp(cameraMaxPosition, cameraInitialPosition, ShakeProgressHasPercentage());
        else
            return Vector3.Lerp(cameraInitialPosition, cameraMaxPosition, ShakeProgressHasPercentage());
    }
    
    private void PrepareCameraShake()
    {
        isCameraShaking = true;
        cameraInitialPosition = transform.position;
        cameraMaxPosition = transform.position + positionIncrement;
    }

    private void ResetTimePassedShaking()
    {
        timePassedShaking = 0;
        isCameraShaking = false;
    }

    private float ShakeProgressHasPercentage()
    {
        return timePassedShaking / shakingDurationInSeconds;
    }

    private bool HasShakingDurationBeenReached()
    {
        return timePassedShaking >= shakingDurationInSeconds;
    }
}
