namespace ISIP224_ZORIN.Models;

// Таблица PickupPoint: пункт выдачи заказов (ПВЗ).
public class PickupPoint
{
    public PickupPoint(int id, string address)
    {
        Id = id;
        Address = address;
    }

    public int Id { get; }

    public string Address { get; }

    public override string ToString()
        => $"№{Id} — {Address}";
}
