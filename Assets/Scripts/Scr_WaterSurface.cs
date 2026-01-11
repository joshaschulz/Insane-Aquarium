using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Scr_WaterSurface : MonoBehaviour
{
    [SerializeField] private Renderer underwaterRenderer;

    // Shader property names
    private static readonly int BaseY_ID = Shader.PropertyToID("_WaterBaseY");
    private static readonly int Width_ID = Shader.PropertyToID("_WaterWidth");

    private static readonly int WaveAmp1_ID = Shader.PropertyToID("_WaveAmp1");
    private static readonly int WaveFreq1_ID = Shader.PropertyToID("_WaveFreq1");
    private static readonly int WaveSpeed1_ID = Shader.PropertyToID("_WaveSpeed1");

    private static readonly int WaveAmp2_ID = Shader.PropertyToID("_WaveAmp2");
    private static readonly int WaveFreq2_ID = Shader.PropertyToID("_WaveFreq2");
    private static readonly int WaveSpeed2_ID = Shader.PropertyToID("_WaveSpeed2");

    private static readonly int Time_ID = Shader.PropertyToID("_WaterTime");



    public int points = 60;

    [Header("Primary Wave")]
    public float waveAmplitude = 6f;
    public float waveFrequency = 0.02f;
    public float waveSpeed = 1f;

    [Header("Secondary Wave (Irregularity)")]
    public float secondaryAmplitude = 2f;
    public float secondaryFrequency = 0.05f;
    public float secondarySpeed = 0.6f;

    [Header("Position")]
    public float yOffset = -10f;



    private Scr_GameManager gameManager;
    private EdgeCollider2D edgeCollider;
    private LineRenderer line;
    private RectTransform tankRect;
    private float width;
    private float timeOffset;

    private void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
    }
    void Awake()
    {

        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;

        edgeCollider = GetComponent<EdgeCollider2D>();
        if (edgeCollider == null)
        {
            edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        }

        tankRect = GetComponentInParent<RectTransform>();
        if (tankRect == null)
        {
            Debug.LogError("WaterSurface must be a child of a RectTransform.");
            enabled = false;
            return;
        }

        width = tankRect.rect.width;
        line.positionCount = points;

        float topY = tankRect.rect.height / 2f;
        transform.localPosition = new Vector3(0, topY + yOffset, 0);


        if (underwaterRenderer == null)
        {
            Debug.LogError("Underwater Renderer not assigned.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        timeOffset += Time.deltaTime;


        Material mat = underwaterRenderer.material;

        // Convert local water Y to world Y
        float worldWaterY = transform.position.y;

        mat.SetFloat(BaseY_ID, worldWaterY);
        mat.SetFloat(Width_ID, width);

        mat.SetFloat(WaveAmp1_ID, waveAmplitude);
        mat.SetFloat(WaveFreq1_ID, waveFrequency);
        mat.SetFloat(WaveSpeed1_ID, waveSpeed);

        mat.SetFloat(WaveAmp2_ID, secondaryAmplitude);
        mat.SetFloat(WaveFreq2_ID, secondaryFrequency);
        mat.SetFloat(WaveSpeed2_ID, secondarySpeed);

        mat.SetFloat(Time_ID, timeOffset);


        for (int i = 0; i < points; i++)
        {
            float t = i / (float)(points - 1);
            float x = (t - 0.5f) * width;

            float primary =
                Mathf.Sin(x * waveFrequency + timeOffset * waveSpeed)
                * waveAmplitude;

            float secondary =
                Mathf.Sin(x * secondaryFrequency - timeOffset * secondarySpeed)
                * secondaryAmplitude;

            float y = primary + secondary;

            line.SetPosition(i, new Vector3(x, y, 0));
        }

        // Also update EdgeCollider points
        Vector2[] edgePoints = new Vector2[points];
        for (int i = 0; i < points; i++)
        {
            Vector3 lrPos = line.GetPosition(i);
            edgePoints[i] = new Vector2(lrPos.x, lrPos.y);
        }
        edgeCollider.points = edgePoints;
    }

    public float GetWaterHeightAtX(float worldX)
    {
        // Convert world X to local X
        float localX = transform.InverseTransformPoint(new Vector3(worldX, 0, 0)).x;

        float primary = Mathf.Sin(localX * waveFrequency + timeOffset * waveSpeed) * waveAmplitude;
        float secondary = Mathf.Sin(localX * secondaryFrequency - timeOffset * secondarySpeed) * secondaryAmplitude;

        // Water height in world space
        float waterY = transform.position.y + primary + secondary;
        return waterY;
    }

}