using UnityEngine;

public class Scr_ForSaleSticker : MonoBehaviour
{
    private Scr_TankBounds registeredTank;

    private void OnEnable()
    {
        RegisterToTank();
    }

    private void Start()
    {
        // in case enable order is weird, try again on Start
        if (registeredTank == null)
            RegisterToTank();
    }

    private void OnDisable()
    {
        UnregisterFromTank();
    }

    private void OnDestroy()
    {
        UnregisterFromTank();
    }

    private void RegisterToTank()
    {
        // Find all tanks and pick the one whose bounds contains this sign
        Scr_TankBounds[] tanks = FindObjectsOfType<Scr_TankBounds>();
        foreach (var tank in tanks)
        {
            if (tank == null) continue;

            if (tank.GetBounds().Contains(transform.position))
            {
                registeredTank = tank;
                registeredTank.RegisterForSaleSticker(this);
                return;
            }
        }

        // If we got here, it isn't inside any tank
        registeredTank = null;
    }

    private void UnregisterFromTank()
    {
        if (registeredTank != null)
        {
            registeredTank.UnregisterForSaleSticker(this);
            registeredTank = null;
        }
    }
}
