using System.Collections;
using UnityEngine;

public class Scr_FishIntroMovement : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public GameObject bubbleParticles;

    [Header("required")]
    public Scr_FishAnimation fishAnimation;

    [Header("movement")]
    public float baseSpeed = 2f;

    private float baseSpeedFactored;
    private float currentSpeed;
    private Vector2 target;
    private bool forceForward = false;

    [Header("minigame bounds")]
    [Tooltip("set this from your minigame controller (collider bounds is easiest)")]
    public Bounds movementBounds;

    private void Awake()
    {
        gameManager = Scr_GameManager.GMinstance;

        baseSpeedFactored = baseSpeed;
    }

    private void Start()
    {

        // match your existing behavior: wait until the current spawn/drop animation state finished

        fishAnimation.frontAnimator.speed = 2;
        // ADD TO Scr_FishAnimation...
        // public Animator sideAnimator;        to the top
        // sideAnimator = sideContainer.GetComponent<Animator>();       in awake

        SetTarget(transform.position);
        IdleOrMoveNormal();
    }

    private void Update()
    {
        // identical guard: don't move during spawn animation
        if (fishAnimation != null &&
            fishAnimation.frontAnimator != null &&
            fishAnimation.IsAnimationPlaying(fishAnimation.frontAnimator, "Fish Spawn"))
        {
            return;
        }

        currentSpeed = baseSpeedFactored;

        transform.position = Vector2.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);

        // if reached target while moving, choose idle or a new target
        if (target == new Vector2(transform.position.x, transform.position.y) &&
            fishAnimation != null &&
            fishAnimation.GetState() == Scr_FishAnimation.FishState.Move)
        {

            IdleOrMoveNormal();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SpawnParticles(bubbleParticles, transform.position, Quaternion.identity, transform);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            forceForward = true;
            IdleOrMoveNormal();
        }
    }

    public void IdleOrMoveNormal()
    {
        CancelInvoke(nameof(IdleOrMoveNormal));

        // identical random speed factor
        float speedFactor = Random.Range(0.5f, 1.5f);
        baseSpeedFactored = baseSpeed * speedFactor;

        fishAnimation.sideAnimator.speed = baseSpeedFactored;

        if (Random.Range(0, 2) == 0 || forceForward)
        {
            // chose to idle for exactly 2 seconds (no fast forward)
            SetTarget(transform.position);
            Invoke(nameof(IdleOrMoveNormal), 2f);

            Debug.Log("PICKED IDLE");
        }
        else
        {
            Debug.Log("PICKED MOVE");

            SetTarget(RandomPointInBounds());
        }
    }


    // ----- bounds setup helpers -----

    public void SetBounds(Bounds b)
    {
        movementBounds = b;
    }

    public void SetBoundsFromCollider(Collider2D col)
    {
        if (col != null) movementBounds = col.bounds;
    }

    public void SetBoundsFromMinMax(float minX, float maxX, float minY, float maxY)
    {
        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
        Vector3 size = new Vector3(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY), 0f);
        movementBounds = new Bounds(center, size);
    }

    private Vector2 RandomPointInBounds()
    {
        float x = Random.Range(movementBounds.min.x, movementBounds.max.x);
        float y = Random.Range(movementBounds.min.y, movementBounds.max.y);
        return new Vector2(x, y);
    }

    // ----- target + animation -----

    public void SetTarget(float x, float y)
    {
        target = new Vector2(x, y);
        ApplyTargetToAnimation();
    }

    public void SetTarget(Vector2 pos)
    {
        target = pos;
        ApplyTargetToAnimation();
    }

    private void ApplyTargetToAnimation()
    {
        if (fishAnimation == null) return;

        if (target == new Vector2(transform.position.x, transform.position.y))
            fishAnimation.SetState(Scr_FishAnimation.FishState.Idle);
        else
        {
            Debug.Log(fishAnimation.GetState());

            Debug.Log("APPLIED MOVE STATE");
            fishAnimation.SetState(Scr_FishAnimation.FishState.Move);

            Debug.Log(fishAnimation.GetState());


        }

        fishAnimation.FaceDirection(target);
    }



    public void SpawnParticles(GameObject _particles, Vector3 _position, Quaternion _rotation, Transform _parent)
    {
        GameObject newParticlesObject;
        if (_parent)
        {
            newParticlesObject = Instantiate(_particles, _position, _rotation, _parent);
        }
        else
        {
            newParticlesObject = Instantiate(_particles, _position, _rotation);
        }

        ParticleSystem newParticleSystem = newParticlesObject.GetComponent<ParticleSystem>();
        newParticleSystem.Play();


        if (!newParticleSystem.main.loop)
        {
            float totalLifetime = newParticleSystem.main.duration + newParticleSystem.main.startLifetime.constantMax;
            Destroy(newParticlesObject, totalLifetime);
        }
    }
}
