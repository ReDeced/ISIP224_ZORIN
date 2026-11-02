namespace ISIP224_ZORIN.Models;

// Таблица Order: заказ пользователя. Состав заказа хранится в OrderItem,
// итоговая сумма не дублируется, а вычисляется по позициям (3НФ).
public class Order
{
    public Order(int id, int userId, int pickupPointId, DateTime createdAt,
        string pickupPointAddress = "")
    {
        Id = id;
        UserId = userId;
        PickupPointId = pickupPointId;
        CreatedAt = createdAt;
        PickupPointAddress = pickupPointAddress;
    }

    public int Id { get; }

    // Внешний ключ на User.Id.
    public int UserId { get; }

    // Внешний ключ на PickupPoint.Id — ПВЗ, где заберут заказ.
    public int PickupPointId { get; }

    public DateTime CreatedAt { get; }

    // Заполняется из PickupPoint при чтении, для вывода на консоль.
    public string PickupPointAddress { get; }
}
