using System.Collections;
using UnityEngine;

public class Scr_FishMinigameMovement : MonoBehaviour
{
    [Header("required")]
    public Scr_FishAnimation fishAnimation;

    [Header("movement")]
    public float baseSpeed = 2f;
    public float fastSpeed = 2f;

    private float baseSpeedFactored;
    private float currentSpeed;
    private Vector2 target;

    [Header("minigame bounds")]
    [Tooltip("set this from your minigame controller (collider bounds is easiest)")]
    public Bounds movementBounds;

    private bool justSpawned = true;
    public bool hooked = false;

    private void Awake()
    {
        baseSpeedFactored = baseSpeed;
    }

    private void Start()
    {

        // match your existing behavior: wait until the current spawn/drop animation state finished

        fishAnimation.frontAnimator.speed = 2;
        // ADD TO Scr_FishAnimation...
        // public Animator sideAnimator;        to the top
        // sideAnimator = sideContainer.GetComponent<Animator>();       in awake

        //SetTarget(transform.position);

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

            if (!hooked)
                IdleOrMoveNormal();
            else
                IdleOrMoveFast();
        }
    }

    public void IdleOrMoveNormal()
    {
        CancelInvoke(nameof(IdleOrMoveNormal));

        // identical random speed factor
        float speedFactor = Random.Range(0.5f, 1.5f);
        baseSpeedFactored = baseSpeed * speedFactor;

        fishAnimation.sideAnimator.speed = baseSpeedFactored;

        //first time spawned

        if (justSpawned)
        {
            //choose to move
            SetTarget(RandomPointInBounds());
            return;


            // chose to idle for exactly 2 seconds (no fast forward)
            /*
            SetTarget(transform.position);
            Invoke(nameof(IdleOrMoveNormal), 2f);
            justSpawned = false;

            Debug.Log("JUST IDLED OR MOVED");

            return;*/
        }

        //Debug.Log("JUST IDLED OR MOVED");

        if (Random.Range(0, 4) == 0)
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

    public void IdleOrMoveFast()
    {
        CancelInvoke(nameof(IdleOrMoveFast));

        float speedFactor = Random.Range(0.5f, 1.5f) * fastSpeed;
        baseSpeedFactored = baseSpeed * speedFactor;

        fishAnimation.sideAnimator.speed = baseSpeedFactored;

        if (Random.Range(0, 8) == 0)
        {
            // chose to idle for exactly 2 seconds (no fast forward)
            SetTarget(transform.position);
            Invoke(nameof(IdleOrMoveFast), 1f);
        }
        else
        {
            SetTarget(RandomPointInBounds());
        }
    }

    public void GoFast()
    {
        CancelInvoke(nameof(IdleOrMoveNormal));
        CancelInvoke(nameof(IdleOrMoveFast));

        IdleOrMoveFast();

        hooked = true;

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

}
