namespace ISIP224_ZORIN.Models;

public class CartItem
{
    public Book Book { get; }

    public int Quantity { get; set; }

    public decimal Total => Book.Price * Quantity;

    public CartItem(Book book, int quantity)
    {
        Book = book;
        Quantity = quantity;
    }
}