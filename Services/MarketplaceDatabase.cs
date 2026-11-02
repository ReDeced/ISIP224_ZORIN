using Microsoft.Data.Sqlite;
using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

// База данных маркетплейса (SQLite), нормализованная до 3НФ:
// - категория товара вынесена в таблицу Category;
// - корзина хранится в CartItem со ссылками на User и Product;
// - заказ — в «Order», его состав — в OrderItem;
// - сумма заказа не дублируется, а вычисляется по позициям OrderItem.
public class MarketplaceDatabase
{
    public const string DatabaseFileName = "MarketplaceDatabase.db";

    private readonly SqliteConnection connection = new($"Data Source={DatabaseFileName}");

    public SqliteConnection Connection => connection;

    // Создаёт таблицы, если их ещё нет, и наполняет справочники.
    public void Initialize()
    {
        connection.Open();

        Execute("""
            CREATE TABLE IF NOT EXISTS User (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nickname TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Category (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS Product (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CategoryId INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Price REAL NOT NULL CHECK (Price > 0),
                Stock INTEGER NOT NULL CHECK (Stock >= 0),
                Description TEXT NOT NULL,
                FOREIGN KEY (CategoryId) REFERENCES Category(Id)
            );

            CREATE TABLE IF NOT EXISTS PickupPoint (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Address TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS CartItem (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL CHECK (Quantity > 0),
                FOREIGN KEY (UserId) REFERENCES User(Id),
                FOREIGN KEY (ProductId) REFERENCES Product(Id)
            );

            -- Order — зарезервированное слово SQL, поэтому имя таблицы в кавычках.
            CREATE TABLE IF NOT EXISTS "Order" (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                PickupPointId INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES User(Id),
                FOREIGN KEY (PickupPointId) REFERENCES PickupPoint(Id)
            );

            CREATE TABLE IF NOT EXISTS OrderItem (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL CHECK (Quantity > 0),
                UnitPrice REAL NOT NULL CHECK (UnitPrice > 0),
                FOREIGN KEY (OrderId) REFERENCES "Order"(Id),
                FOREIGN KEY (ProductId) REFERENCES Product(Id)
            );
            """);

        Seed();
    }

    // Выполняет команду без результата, возвращает число затронутых строк.
    public int Execute(string sql, params SqliteParameter[] parameters)
    {
        using SqliteCommand command = CreateCommand(sql, parameters);
        return command.ExecuteNonQuery();
    }

    // Вставляет строку и возвращает её идентификатор.
    public long Insert(string sql, params SqliteParameter[] parameters)
    {
        using SqliteCommand command = CreateCommand(sql + "; SELECT last_insert_rowid();", parameters);
        return (long)command.ExecuteScalar()!;
    }

    // Читает набор строк и преобразует каждую в объект через map.
    public List<T> Query<T>(string sql, Func<SqliteDataReader, T> map, params SqliteParameter[] parameters)
    {
        using SqliteCommand command = CreateCommand(sql, parameters);
        using SqliteDataReader reader = command.ExecuteReader();

        List<T> rows = [];

        while (reader.Read())
            rows.Add(map(reader));

        return rows;
    }

    // Читает одно значение (первый столбец первой строки) или null.
    public object? Scalar(string sql, params SqliteParameter[] parameters)
    {
        using SqliteCommand command = CreateCommand(sql, parameters);
        return command.ExecuteScalar();
    }

    private SqliteCommand CreateCommand(string sql, SqliteParameter[] parameters)
    {
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        return command;
    }

    // Сокращение для создания параметра запроса.
    public static SqliteParameter P(string name, object? value)
        => new(name, value ?? DBNull.Value);

    // Наполняет базу демонстрационными данными при первом запуске.
    private void Seed()
    {
        if ((long)Scalar("SELECT COUNT(*) FROM Product;") > 0)
            return;

        long electronics = Insert("INSERT INTO Category (Name) VALUES ($name);", P("$name", "Электроника"));
        long books = Insert("INSERT INTO Category (Name) VALUES ($name);", P("$name", "Книги"));
        long clothes = Insert("INSERT INTO Category (Name) VALUES ($name);", P("$name", "Одежда"));

        InsertProduct(electronics, "Смартфон XPhone", 18990m, 12, "Экран 6.1 дюйма, 128 ГБ памяти");
        InsertProduct(electronics, "Беспроводные наушники", 2490m, 30, "Bluetooth 5.3, до 20 часов работы");
        InsertProduct(electronics, "Ноутбук DevBook", 45990m, 5, "16 ГБ ОЗУ, SSD 512 ГБ");
        InsertProduct(books, "Программируем на C#", 990m, 20, "Учебник по языку C# с практикой");
        InsertProduct(books, "Чистый код", 1490m, 8, "Классическая книга о качестве кода");
        InsertProduct(clothes, "Футболка «Hello, World»", 590m, 50, "Хлопок, размеры S–XXL");
        InsertProduct(clothes, "Худи с капюшоном", 1890m, 25, "Тёплое худи унисекс");

        Insert("INSERT INTO PickupPoint (Address) VALUES ($address);", P("$address", "ул. Университетская, 1"));
        Insert("INSERT INTO PickupPoint (Address) VALUES ($address);", P("$address", "пр. Студенческий, 22"));
        Insert("INSERT INTO PickupPoint (Address) VALUES ($address);", P("$address", "ул. Программистов, 7"));
    }

    private void InsertProduct(long categoryId, string title, decimal price, int stock, string description)
        => Insert("""
            INSERT INTO Product (CategoryId, Title, Price, Stock, Description)
            VALUES ($categoryId, $title, $price, $stock, $description);
            """,
            P("$categoryId", categoryId),
            P("$title", title),
            P("$price", (double)price),
            P("$stock", stock),
            P("$description", description));
}
