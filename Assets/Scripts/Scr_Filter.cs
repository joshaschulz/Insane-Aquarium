using UnityEngine;

public class Scr_Filter : MonoBehaviour
{
    [Header("Cleaning")]
    [Tooltip("How many poop units to remove each cleaning event.")]
    public int poopRemovedPerClean = 1;

    [Tooltip("How many ticks between cleaning events. 1 = every tick. 0 = paused.")]
    public int cleanIntervalTicks = 1;

    private Scr_GameManager gameManager;
    private Scr_TimeHandler timeHandler;

    public GameObject filterBubblesPrefab;

    public Vector2 myTankPos;
    private int tickCounter = 0;

    private void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        //myTankPos = new Vector2(gameManager.GetTankPos(transform.position).position.x, gameManager.GetTankPos(transform.position).position.y);
    }

    private void OnEnable()
    {
        filterBubblesPrefab.SetActive(true);

        timeHandler = FindObjectOfType<Scr_TimeHandler>();
        if (timeHandler != null)
            timeHandler.tickEvent.AddListener(OnTickEvent);
    }

    private void OnDisable()
    {
        if (timeHandler != null)
            timeHandler.tickEvent.RemoveListener(OnTickEvent);
    }

    private void OnTickEvent()
    {
        if (cleanIntervalTicks <= 0) return;
        if (poopRemovedPerClean <= 0) return;
        if (gameManager == null) return;

        tickCounter++;
        if (tickCounter < cleanIntervalTicks) return;
        tickCounter = 0;

        // remove poop (negative adds cleaning)
        gameManager.UpdatePoopLevel(myTankPos, -poopRemovedPerClean);
    }
}
