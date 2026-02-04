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

    float randomScale;

    public void Initialize()
    {
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);

        randomScale = Random.Range(0.4f, 1.1f);
        transform.localScale *= randomScale;

        // randomize spin direction too
        float sign = (Random.value < 0.5f) ? -1f : 1f;
        spinSpeed = Random.Range(minSpinSpeed, maxSpinSpeed) * sign;
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        transform.position += Vector3.down * fallSpeed * dt * randomScale;
        transform.Rotate(0f, 0f, spinSpeed * dt);
    }
}
