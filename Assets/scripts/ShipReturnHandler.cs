using UnityEngine;

public class ShipReturnHandler : MonoBehaviour
{
    void Start()
    {
        // ✅ Lock cursor back for the ship movement controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        if (ReturnPositionData.hasReturnPoint)
        {
            transform.position                = ReturnPositionData.savedPosition;
            ReturnPositionData.hasReturnPoint = false;
        }
    }
}