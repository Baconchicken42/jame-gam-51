using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public float timeLimit;

    [SerializeField]
    public Order[] orders;

    private Employee[] employees;


    private void Start()
    {
        employees = FindObjectsByType<Employee>(FindObjectsSortMode.None);
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
}
