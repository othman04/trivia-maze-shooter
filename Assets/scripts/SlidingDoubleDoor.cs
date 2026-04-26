using UnityEngine;

/// <summary>
/// Simple sliding double door controller:
/// - Left door slides left
/// - Right door slides right
/// - Player presses E while inside trigger to toggle open/close
/// </summary>
public class SlidingDoubleDoor : MonoBehaviour
{
    [Header("Door Panels")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Sliding Settings")]
    [SerializeField] private float slideDistance = 2f;
    [SerializeField] private float slideSpeed = 3f;

    [Header("Player Detection")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool playerNearby;
    private bool isOpen;

    private void Awake()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("SlidingDoubleDoor: Assign both leftDoor and rightDoor in the Inspector.", this);
            enabled = false;
            return;
        }

        // Store initial local positions as closed positions.
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;

        // Left door moves left (-X), right door moves right (+X).
        leftOpenPos = leftClosedPos + Vector3.left * slideDistance;
        rightOpenPos = rightClosedPos + Vector3.right * slideDistance;
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(interactKey))
        {
            isOpen = !isOpen;
        }

        // Smoothly move each panel toward its current target.
        Vector3 leftTarget = isOpen ? leftOpenPos : leftClosedPos;
        Vector3 rightTarget = isOpen ? rightOpenPos : rightClosedPos;

        leftDoor.localPosition = Vector3.MoveTowards(
            leftDoor.localPosition,
            leftTarget,
            slideSpeed * Time.deltaTime
        );

        rightDoor.localPosition = Vector3.MoveTowards(
            rightDoor.localPosition,
            rightTarget,
            slideSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerNearby = false;
        }
    }
}
