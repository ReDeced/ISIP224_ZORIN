using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

public class ShoppingCart
{
    private readonly List<CartItem> _items = new();

    public int Count => _items.Count;

    public bool IsEmpty => _items.Count == 0;

    public int TotalQuantity => _items.Sum(item => item.Quantity);

    public decimal TotalPrice => _items.Sum(item => item.Total);

    public CartItem Add(Book book, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Количество должно быть больше нуля.");

        CartItem? existing = _items.FirstOrDefault(item => item.Book.Id == book.Id);

        if (existing is not null)
        {
            existing.Quantity += quantity;
            return existing;
        }

        CartItem item = new(book, quantity);
        _items.Add(item);
        return item;
    }

    public bool Remove(int bookId)
        => _items.RemoveAll(item => item.Book.Id == bookId) > 0;

    public bool Decrease(int bookId)
    {
        CartItem? item = _items.FirstOrDefault(i => i.Book.Id == bookId);

        if (item is null)
            return false;

        if (item.Quantity > 1)
        {
            item.Quantity--;
            return true;
        }

        return Remove(bookId);
    }

    public void Clear() => _items.Clear();

    public List<CartItem> GetItems()
        => _items.OrderBy(item => item.Book.Title, StringComparer.OrdinalIgnoreCase).ToList();
}