using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Services;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN;

internal class Program
{
    private static readonly MarketplaceDatabase Database = new();
    private static readonly UserService Users = new(Database);
    private static readonly ProductService Products = new(Database);
    private static readonly CartService Cart = new(Database);
    private static readonly OrderService Orders = new(Database);

    // null — гость: товары смотреть можно, покупать только после входа.
    private static User? currentUser;

    static void Main(string[] args)
    {
        try
        {
            Database.Initialize();
            RunMarketplace();
        }
        catch (InputClosedException)
        {
            Console.WriteLine("\nВвод закрыт, программа завершает работу.");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"\nНепредвиденная ошибка: {exception.Message}");
        }

        Console.WriteLine("\nДо свидания!");
    }

    private static void RunMarketplace()
    {
        Console.WriteLine("==========================================================");
        Console.WriteLine("          МАРКЕТПЛЕЙС GMWOG || GG.MOW || WONGG");
        Console.WriteLine("==========================================================");
        Console.WriteLine("Товары может смотреть любой посетитель, покупают — после входа.");

        while (true)
        {
            if (currentUser is null)
            {
                Console.WriteLine("\n--- МЕНЮ ГОСТЯ ---");
                Console.WriteLine("1. Показать товары");
                Console.WriteLine("2. Регистрация");
                Console.WriteLine("3. Вход");
                Console.WriteLine("0. Выход");

                switch (Input.ReadText("Выберите команду: "))
                {
                    case "1":
                        PrintProducts(Products.GetAllProducts());
                        break;

                    case "2":
                        Register();
                        break;

                    case "3":
                        Login();
                        break;

                    case "0":
                        return;

                    default:
                        Input.ShowError("Такой команды нет. Введите номер из меню.");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"\n--- МЕНЮ ПОКУПАТЕЛЯ {currentUser.Nickname} ---");
                Console.WriteLine("1. Показать товары");
                Console.WriteLine("2. Добавить товар в корзину");
                Console.WriteLine("3. Показать корзину");
                Console.WriteLine("4. Убрать товар из корзины");
                Console.WriteLine("5. Очистить корзину");
                Console.WriteLine("6. Купить товар из корзины");
                Console.WriteLine("7. Купить всю корзину");
                Console.WriteLine("8. История заказов");
                Console.WriteLine("9. Выйти из аккаунта");
                Console.WriteLine("0. Выход");

                switch (Input.ReadText("Выберите команду: "))
                {
                    case "1":
                        PrintProducts(Products.GetAllProducts());
                        break;

                    case "2":
                        AddToCart();
                        break;

                    case "3":
                        ShowCart();
                        break;

                    case "4":
                        RemoveFromCart();
                        break;

                    case "5":
                        ClearCart();
                        break;

                    case "6":
                        BuySingle();
                        break;

                    case "7":
                        BuyWholeCart();
                        break;

                    case "8":
                        ShowOrderHistory();
                        break;

                    case "9":
                        LeaveAccount();
                        break;

                    case "0":
                        return;

                    default:
                        Input.ShowError("Такой команды нет. Введите номер из меню.");
                        break;
                }
            }
        }
    }

    private static void Register()
    {
        Console.WriteLine("\n--- РЕГИСТРАЦИЯ ---");

        string nickname = Input.ReadText("  Придумайте ник (без пробелов): ");
        string password = Input.ReadText($"  Придумайте пароль (не короче {UserService.MinPasswordLength} символов): ");
        string confirmation = Input.ReadText("  Повторите пароль: ");

        try
        {
            currentUser = Users.Register(nickname, password, confirmation);
            Console.WriteLine($"\n  Аккаунт создан. Вы вошли как {currentUser.Nickname}.");
        }
        catch (ArgumentException exception)
        {
            Input.ShowError($"Регистрация не выполнена: {exception.Message}");
        }
        catch (InvalidOperationException exception)
        {
            Input.ShowError(exception.Message);
        }
    }

    private static void Login()
    {
        Console.WriteLine("\n--- ВХОД В АККАУНТ ---");

        string nickname = Input.ReadText("  Ник: ");
        string password = Input.ReadText("  Пароль: ");

        User? user = Users.Login(nickname, password);

        if (user is null)
        {
            Input.ShowError("Неверный ник или пароль.");
            return;
        }

        currentUser = user;
        Console.WriteLine($"\n  С возвращением, {user.Nickname}!");
    }

    private static void LeaveAccount()
    {
        Console.WriteLine($"\n  Аккаунт {currentUser!.Nickname} покинут. Корзина сохранена.");
        currentUser = null;
    }

    private static void AddToCart()
    {
        Console.WriteLine("\n--- ДОБАВЛЕНИЕ В КОРЗИНУ ---");

        List<(Product Product, string CategoryName)> products = Products.GetAllProducts();

        if (products.Count == 0)
        {
            Input.ShowError("В каталоге пока нет товаров.");
            return;
        }

        PrintProducts(products);

        int productId = Input.ReadId("  Идентификатор товара: ");
        Product? product = Products.FindById(productId);

        if (product is null)
        {
            Input.ShowError($"Товар с идентификатором {productId} не найден.");
            return;
        }

        if (product.Stock == 0)
        {
            Input.ShowError($"Товара «{product.Title}» нет на складе.");
            return;
        }

        int quantity = Input.ReadInt($"  Сколько штук (1–{product.Stock}): ", 1, product.Stock);

        try
        {
            CartItem item = Cart.AddItem(currentUser!, product, quantity);
            Console.WriteLine($"\n  В корзине: {item.ProductTitle} — {item.Quantity} шт. на {Input.FormatMoney(item.Total)} руб.");
        }
        catch (InvalidOperationException exception)
        {
            Input.ShowError(exception.Message);
        }
    }

    private static void ShowCart()
    {
        Console.WriteLine("\n--- КОРЗИНА ---");
        PrintCart(Cart.GetItems(currentUser!));
    }

    private static void RemoveFromCart()
    {
        Console.WriteLine("\n--- УДАЛЕНИЕ ИЗ КОРЗИНЫ ---");

        List<CartItem> items = Cart.GetItems(currentUser!);

        if (items.Count == 0)
        {
            Input.ShowError("Корзина пуста.");
            return;
        }

        PrintCart(items);

        int position = Input.ReadInt($"  Номер позиции (1–{items.Count}): ", 1, items.Count);
        Cart.RemoveItem(currentUser!, items[position - 1].Id);
        Console.WriteLine($"\n  Позиция убрана. Осталось позиций: {items.Count - 1}.");
    }

    private static void ClearCart()
    {
        Console.WriteLine("\n--- ОЧИСТКА КОРЗИНЫ ---");

        List<CartItem> items = Cart.GetItems(currentUser!);

        if (items.Count == 0)
        {
            Input.ShowError("Корзина уже пуста.");
            return;
        }

        PrintCart(items);

        if (!Input.ReadYesNo("  Точно очистить корзину? (д/н): "))
        {
            Console.WriteLine("  Очистка отменена.");
            return;
        }

        Cart.Clear(currentUser!);
        Console.WriteLine("  Корзина очищена.");
    }

    private static void BuySingle()
    {
        Console.WriteLine("\n--- ПОКУПКА ТОВАРА ИЗ КОРЗИНЫ ---");

        List<CartItem> items = Cart.GetItems(currentUser!);

        if (items.Count == 0)
        {
            Input.ShowError("Корзина пуста.");
            return;
        }

        PrintCart(items);

        int position = Input.ReadInt($"  Номер позиции (1–{items.Count}): ", 1, items.Count);
        CartItem item = items[position - 1];

        Product? product = Products.FindById(item.ProductId);

        if (product is null)
        {
            Input.ShowError("Товар из корзины больше не продаётся.");
            return;
        }

        if (product.Stock < item.Quantity)
        {
            Input.ShowError($"На складе осталось {product.Stock} шт. вместо {item.Quantity}. Уберите лишнее из корзины.");
            return;
        }

        PickupPoint pickupPoint = ChoosePickupPoint();

        if (pickupPoint is null)
            return;

        Console.WriteLine($"\n  К покупке: {item.ProductTitle} — {item.Quantity} шт. на {Input.FormatMoney(item.Total)} руб.");
        Console.WriteLine($"  Получение: {pickupPoint.Address}");

        if (!Input.ReadYesNo("  Оформить заказ? (д/н): "))
        {
            Console.WriteLine("  Покупка отменена.");
            return;
        }

        try
        {
            Order order = Orders.CreateOrder(currentUser!, pickupPoint, [(product, item.Quantity)]);
            Cart.RemoveItem(currentUser!, item.Id);
            PrintOrder(order, Orders.GetOrderItems(order));
            Console.WriteLine($"  Остаток на складе: {product.Stock - item.Quantity} шт.");
        }
        catch (InvalidOperationException exception)
        {
            Input.ShowError(exception.Message);
        }
    }

    private static void BuyWholeCart()
    {
        Console.WriteLine("\n--- ПОКУПКА ВСЕЙ КОРЗИНЫ ---");

        List<CartItem> items = Cart.GetItems(currentUser!);

        if (items.Count == 0)
        {
            Input.ShowError("Корзина пуста.");
            return;
        }

        PrintCart(items);

        List<(Product Product, int Quantity)> lines = [];

        foreach (CartItem item in items)
        {
            Product? product = Products.FindById(item.ProductId);

            if (product is null)
            {
                Input.ShowError($"Товар «{item.ProductTitle}» больше не продаётся. Уберите его из корзины.");
                return;
            }

            if (product.Stock < item.Quantity)
            {
                Input.ShowError($"Товара «{product.Title}» на складе {product.Stock} шт., в корзине {item.Quantity} шт.");
                return;
            }

            lines.Add((product, item.Quantity));
        }

        PickupPoint? pickupPoint = ChoosePickupPoint();

        if (pickupPoint is null)
            return;

        decimal total = Cart.GetTotal(items);
        Console.WriteLine($"\n  Позиций в заказе: {lines.Count} на сумму {Input.FormatMoney(total)} руб.");
        Console.WriteLine($"  Получение: {pickupPoint.Address}");

        if (!Input.ReadYesNo("  Оформить заказ? (д/н): "))
        {
            Console.WriteLine("  Покупка отменена.");
            return;
        }

        try
        {
            Order order = Orders.CreateOrder(currentUser!, pickupPoint, lines);
            Cart.Clear(currentUser!);
            PrintOrder(order, Orders.GetOrderItems(order));
        }
        catch (InvalidOperationException exception)
        {
            Input.ShowError(exception.Message);
        }
    }

    private static void ShowOrderHistory()
    {
        Console.WriteLine("\n--- ИСТОРИЯ ЗАКАЗОВ ---");
        Console.WriteLine("1. Сначала новые");
        Console.WriteLine("2. Сначала старые");

        bool newestFirst = Input.ReadInt("Выберите сортировку: ", 1, 2) == 1;

        List<Order> orders = Orders.GetUserOrders(currentUser!, newestFirst);

        if (orders.Count == 0)
        {
            Console.WriteLine("  Заказов пока нет.");
            return;
        }

        Console.WriteLine($"\n  Заказов: {orders.Count}.");

        foreach (Order order in orders)
        {
            Console.WriteLine();
            PrintOrder(order, Orders.GetOrderItems(order));
        }
    }

    // Возвращает выбранный ПВЗ или null, если пользователь передумал.
    private static PickupPoint? ChoosePickupPoint()
    {
        Console.WriteLine("\n--- ВЫБОР ПУНКТА ВЫДАЧИ ---");

        List<PickupPoint> pickupPoints = Orders.GetPickupPoints();

        if (pickupPoints.Count == 0)
        {
            Input.ShowError("Доступных ПВЗ нет.");
            return null;
        }

        foreach (PickupPoint point in pickupPoints)
            Console.WriteLine($"  {point}");

        int number = Input.ReadInt($"  Номер ПВЗ (1–{pickupPoints.Count}): ", 1, pickupPoints.Count);
        return pickupPoints[number - 1];
    }

    private static void PrintProducts(List<(Product Product, string CategoryName)> products)
    {
        Console.WriteLine("\n--- КАТАЛОГ ТОВАРОВ ---");

        if (products.Count == 0)
        {
            Console.WriteLine("  Товаров пока нет.");
            return;
        }

        Console.WriteLine($"  {"ID",-5}{"Название",-28}{"Категория",-14}{"Цена, руб.",-12}{"Остаток",-9}Описание");

        foreach ((Product product, string categoryName) in products)
        {
            Console.WriteLine($"  {product.Id,-5}{Cut(product.Title, 27),-28}{Cut(categoryName, 13),-14}{Input.FormatMoney(product.Price),-12}{product.Stock,-9}{Cut(product.Description, 30)}");
        }

        Console.WriteLine($"\n  Всего товаров: {products.Count}.");
    }

    private static void PrintCart(List<CartItem> items)
    {
        if (items.Count == 0)
        {
            Console.WriteLine("  Корзина пуста.");
            return;
        }

        Console.WriteLine($"  {"№",-5}{"Название",-30}{"Цена, руб.",-12}{"Кол-во",-9}{"Сумма, руб.",-12}");

        int position = 1;

        foreach (CartItem item in items)
        {
            Console.WriteLine($"  {position,-5}{Cut(item.ProductTitle, 29),-30}{Input.FormatMoney(item.UnitPrice),-12}{item.Quantity,-9}{Input.FormatMoney(item.Total),-12}");
            position++;
        }

        Console.WriteLine($"  {"ИТОГО",-47}{Input.FormatMoney(Cart.GetTotal(items)),-12}");
    }

    private static void PrintOrder(Order order, List<OrderItem> items)
    {
        Console.WriteLine($"  Заказ №{order.Id} от {Input.FormatDate(order.CreatedAt)} | ПВЗ: {order.PickupPointAddress}");

        int position = 1;
        decimal total = 0m;

        foreach (OrderItem item in items)
        {
            Console.WriteLine($"    {position}. {item.ProductTitle} — {item.Quantity} шт. × {Input.FormatMoney(item.UnitPrice)} = {Input.FormatMoney(item.Total)} руб.");
            total += item.Total;
            position++;
        }

        Console.WriteLine($"    ИТОГО: {Input.FormatMoney(total)} руб.");
    }

    private static string Cut(string text, int maxLength)
        => text.Length <= maxLength ? text : text[..(maxLength - 1)] + "…";
}
