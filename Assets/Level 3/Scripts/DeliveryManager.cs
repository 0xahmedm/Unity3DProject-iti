using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;

    public static event System.Action OnWin;

    [Header("Orders")]
    [SerializeField] private List<AreaCheck> customerZones;
    [SerializeField] private int maxActiveOrders = 3;

    [Header("Timer")]
    [SerializeField] private float orderTimeLimit = 60f;
    [SerializeField] private TextMeshProUGUI timerUI;
    [SerializeField] private TextMeshProUGUI scoreUI;

    [Header("Marker")]
    [SerializeField] private FaceTarget marker; 
    [SerializeField] private Transform baseTransform;






    private Dictionary<int, float> activeOrders = new Dictionary<int, float>();
    private int score = 0;

    private void Awake()
    {
        Instance = this;
        marker.target = baseTransform;
    }

    private void Update()
    {
        List<int> expired = new List<int>();

        foreach (var key in new List<int>(activeOrders.Keys))
        {
            activeOrders[key] -= Time.deltaTime;
            if (activeOrders[key] <= 0)
                expired.Add(key);
        }

        foreach (int id in expired)
            FailOrder(id);

        UpdateUI();
        if (Input.GetKeyDown(KeyCode.X)) UpdateScore(100); ;

    }

    public void AssignNextOrder()
    {
        if (activeOrders.Count >= maxActiveOrders) return;

        
        List<AreaCheck> available = customerZones.FindAll(z => !activeOrders.ContainsKey(z.orderID));
        if (available.Count == 0) return;

        AreaCheck target = available[Random.Range(0, available.Count)];
        activeOrders[target.orderID] = orderTimeLimit;
        
        target.SetMeshRenderer(true);

        marker.target = target.transform;

        Debug.Log($"New order assigned to house {target.orderID}! {orderTimeLimit}s to deliver.");
    }

    public bool HasActiveOrder(int orderID)
    {
        return activeOrders.ContainsKey(orderID);
    }

    public void CompleteOrder(int orderID)
    {
        if (!activeOrders.ContainsKey(orderID)) return;

        float timeLeft = activeOrders[orderID];
        int bonus = Mathf.RoundToInt(timeLeft);
        UpdateScore(bonus + 100);

        activeOrders.Remove(orderID);
        customerZones.Find(z => z.orderID == orderID).SetMeshRenderer(false);

        marker.target = baseTransform;
        Debug.Log($"Order {orderID} delivered! +{100 + bonus} pts");
    }

    private void UpdateScore(int amount)
    {
        score += amount;
        if (score <= 1000)
        {
            OnWin?.Invoke();
            Debug.Log("You win!");
        }
        UpdateUI();
    }

    private void FailOrder(int orderID)
    {
        activeOrders.Remove(orderID);
        UpdateScore(-50);
        marker.target = baseTransform;
        Debug.Log($"Order {orderID} expired! -50 pts");
    }

    private void UpdateUI()
    {
        if (scoreUI) scoreUI.text = $"Score: {score}";

        if (timerUI)
        {
            if (activeOrders.Count > 0)
            {
                float min = float.MaxValue;
                foreach (var t in activeOrders.Values) min = Mathf.Min(min, t);
                timerUI.text = $"Next deadline: {min:F1}s";
            }
            else
            {
                timerUI.text = "Go to base for order!";
            }
        }
    }
}