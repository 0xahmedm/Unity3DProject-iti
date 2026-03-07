using UnityEngine;
using TMPro;

public enum ZoneType { Base, Customer }

public class AreaCheck : MonoBehaviour
{
    [Header("Zone Settings")]
    public ZoneType zoneType;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int requiredCount = 1; 

    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TextMeshProUGUI zoneLabel;

    [Header("Customer Settings (if Customer zone)")]
    [SerializeField] private Vector2 boxesRequiredRange;
    public int orderID; 
    public int boxesRequired = 1;

    private int currentCount = 0;
    private CarInventory playerInventory;

    void Start()
    {
        boxesRequired = (int)Random.Range(boxesRequiredRange.x, boxesRequiredRange.y + 1);
        if (zoneType == ZoneType.Customer) meshRenderer.enabled = false;  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            currentCount++;
            UpdateMaterial();
            PlayerInput playerInput = other.GetComponentInParent<PlayerInput>();
            playerInventory = playerInput.GetComponentInChildren<CarInventory>();
            Debug.Log("Player entered zone" + playerInventory.name);
            if (playerInventory == null) return;

            if (currentCount >= requiredCount)
                HandleZoneAction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            currentCount--;
            playerInventory = null;
            UpdateMaterial();
        }
    }

    private void HandleZoneAction()
    {
        switch (zoneType)
        {
            case ZoneType.Base:
                HandleBase();
                break;
            case ZoneType.Customer:
                HandleDelivery();
                break;
        }
    }

    private void HandleBase()
    {
        
        playerInventory.Refill();
        
        DeliveryManager.Instance.AssignNextOrder();
        Debug.Log("Order picked up! Go deliver.");
    }

    private void HandleDelivery()
    {
        if (!DeliveryManager.Instance.HasActiveOrder(orderID)) return;
        if (playerInventory.BoxCount < boxesRequired)
        {
            Debug.Log("Not enough boxes!");
            return;
        }

        playerInventory.ConsumeBoxes(boxesRequired);
        DeliveryManager.Instance.CompleteOrder(orderID);
        UpdateMaterial(); 
    }

    private void UpdateMaterial()
    {
        bool valid = zoneType == ZoneType.Customer
            ? DeliveryManager.Instance != null && DeliveryManager.Instance.HasActiveOrder(orderID)
            : currentCount >= requiredCount;

        meshRenderer.material = valid ? validMaterial : invalidMaterial;
        if (zoneLabel) zoneLabel.text = valid ? (zoneType == ZoneType.Base ? "PICK UP" : "DELIVER HERE") : "";
    }
    public void SetMeshRenderer(bool isActive)
    {
        meshRenderer.enabled = isActive;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = zoneType == ZoneType.Base ? Color.blue : Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}