using UnityEngine;
using UnityEngine.Rendering;

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
    private Collider2D myCollider;
    public GameObject backBody;
    public GameObject frontBody;

    public void Initialize()
    {
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);

        randomScale = Random.Range(0.4f, 1.1f);
        transform.localScale *= randomScale;

        transform.GetChild(0).GetComponent<SortingGroup>().sortingOrder = (int)(10 * randomScale);

        // randomize spin direction too
        float sign = (Random.value < 0.5f) ? -1f : 1f;
        spinSpeed = Random.Range(minSpinSpeed, maxSpinSpeed) * sign;
    }

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        {
            CheckMouseClick();
        }

        float dt = Time.unscaledDeltaTime;

        transform.position += Vector3.down * fallSpeed * dt * randomScale;
        transform.Rotate(0f, 0f, spinSpeed * dt);
    }

    void CheckMouseClick()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == myCollider)
            {
                backBody.SetActive(!backBody.activeSelf);
                frontBody.SetActive(!frontBody.activeSelf);

                Scr_GameManager gameManager = FindObjectOfType<Scr_GameManager>();

                if (!frontBody)
                    gameManager.PlaySoundEffect(gameManager.SFX_StarfishFlop, 0.4f, 1.5f, 2f);
                else
                    gameManager.PlaySoundEffect(gameManager.SFX_StarfishFlop, 0.4f, 0.5f, 1f);


            }
        }
    }
}
