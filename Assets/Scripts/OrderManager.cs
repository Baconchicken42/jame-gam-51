using System.Linq;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public float timeLimitSeconds = 180;

    [SerializeField]
    public Order[] orders;

    public Employee[] employees;
    private float timer = 0f;
    private int ordersCompleted = 0;

    private Player player;


    private void Start()
    {
        employees = FindObjectsByType<Employee>(FindObjectsSortMode.None);
        for (int i = 0; i < employees.Length; i++)
        {
            employees[i].onOrderCompleted.AddListener(incrementOrdersCompleted);
        }

        player = FindAnyObjectByType<Player>();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < employees.Length; i++)
        {
            employees[i].onOrderCompleted.RemoveListener(incrementOrdersCompleted);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        player.uiManager.updateTime(Mathf.RoundToInt(timeLimitSeconds - timer));

        if (timer >= timeLimitSeconds)
        {
            endGame();
        }

        //not gonna bother trying to avoid looping every frame, this game will not be very performance intensive and I don't want to require orders to be sorted and invite human error
        for (int j = 0; j < orders.Length; j++)
        {
            if (!orders[j].wasDispensed && timer >= orders[j].activationTimeSeconds)
            {
                for (int i = 0; i < employees.Length; i++)
                {
                    if (employees[i].id == orders[j].employeeID)
                    {
                        //not really sure if this will pass by reference or not so making a duplicate just to be certain (employee uses wasDispensed differently)
                        employees[i].addOrder(new Order(orders[j].employeeID, orders[j].color, orders[j].type, orders[j].timeRequired, orders[j].activationTimeSeconds));
                        orders[j].wasDispensed = true;
                        Debug.Log($"Order was dispensed to Employee {employees[i].id}");
                    }
                }
            }
        }
    }

    private void incrementOrdersCompleted()
    {
        ordersCompleted++;
    }

    private void endGame()
    {
        player.moveAction.action.Disable();
        player.interactAction.action.Disable();
        player.moveSelectionAction.action.Disable();

        player.uiManager.endLevelUI(ordersCompleted);
    }

}

[System.Serializable]
public class Order
{
    public int employeeID;
    public documentColor color;
    public documentType type;
    public float timeRequired;
    public float activationTimeSeconds;

    [HideInInspector]
    public bool wasDispensed;

    public Order(int myId, documentColor myColor, documentType myType, float myTimeRequired, float myActivationTime)
    {
        employeeID = myId;
        color = myColor;
        type = myType;
        timeRequired = myTimeRequired;
        activationTimeSeconds = myActivationTime;
    }
}
