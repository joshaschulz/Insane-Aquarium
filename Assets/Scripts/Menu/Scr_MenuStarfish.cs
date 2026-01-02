using UnityEngine;

public class Scr_MenuStarfish : MonoBehaviour
{
    [Header("Fall")]
    public float minFallSpeed = 1.5f;
    public float maxFallSpeed = 3f;

    [Header("Spin (degrees/sec)")]
    public float minSpinSpeed = 60f;
    public float maxSpinSpeed = 220f;

    private float fallSpeed;
    private float spinSpeed;

    public void Initialize()
    {
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);

        // randomize spin direction too
        float sign = (Random.value < 0.5f) ? -1f : 1f;
        spinSpeed = Random.Range(minSpinSpeed, maxSpinSpeed) * sign;
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        transform.position += Vector3.down * fallSpeed * dt;
        transform.Rotate(0f, 0f, spinSpeed * dt);
    }
}
