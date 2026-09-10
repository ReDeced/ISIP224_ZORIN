namespace MyConsoleApp {
    class Program {
        static void Main(string[] args) {
            int count = 0;
            while (true) {
                Console.Write("Введите количество операций (2-40): ");
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out count) && count >= 2 && count <= 40)
                    break;
                Console.WriteLine("Ошибка! Введите число от 2 до 40.");
            }

            string[] names = new string[count];
            double[] prices = new double[count];

            for (int i = 0; i < count; i++) {
                Console.WriteLine($"Операция {i + 1}:");
                Console.Write("  Название (Название; Цена): ");
                string line = Console.ReadLine() ?? "";
                string[] parts = line.Split(';');
                string rawName = parts[0].Trim();
                if (rawName.StartsWith("(")) rawName = rawName.Substring(1);
                names[i] = rawName;

                string rawPrice = parts[1].Trim().TrimEnd(')').TrimEnd('(').Trim();
                double.TryParse(rawPrice, out prices[i]);
            }

            while (true) {
                Console.WriteLine("\n--- МЕНЮ ---");
                Console.WriteLine("1. Вывод данных");
                Console.WriteLine("2. Статистика");
                Console.WriteLine("3. Сортировка по цене (пузырьковая)");
                Console.WriteLine("4. Конвертация валюты");
                Console.WriteLine("5. Поиск по названию");
                Console.WriteLine("0. Выход");
                Console.Write("Выбор: ");
                string choice = Console.ReadLine() ?? "";

                if (choice == "0") break;

                switch (choice) {
                    case "1":
                        Console.WriteLine("\n--- ДАННЫЕ ---");
                        for (int i = 0; i < count; i++)
                            Console.WriteLine($"{i + 1}. {names[i]} - {prices[i]} руб.");
                        break;

                    case "2":
                        double sum = 0, max = prices[0], min = prices[0];
                        for (int i = 0; i < count; i++) {
                            sum += prices[i];
                            if (prices[i] > max) max = prices[i];
                            if (prices[i] < min) min = prices[i];
                        }
                        Console.WriteLine($"\n--- СТАТИСТИКА ---");
                        Console.WriteLine($"Сумма:    {sum} руб.");
                        Console.WriteLine($"Среднее:  {sum / count} руб.");
                        Console.WriteLine($"Максимум: {max} руб.");
                        Console.WriteLine($"Минимум:  {min} руб.");
                        break;

                    case "3":
                        for (int i = 0; i < count - 1; i++)
                            for (int j = 0; j < count - 1 - i; j++)
                                if (prices[j] > prices[j + 1]) {
                                    double tmpP = prices[j];
                                    prices[j] = prices[j + 1];
                                    prices[j + 1] = tmpP;
                                    string tmpN = names[j];
                                    names[j] = names[j + 1];
                                    names[j + 1] = tmpN;
                                }
                        Console.WriteLine("Сортировка выполнена.");
                        for (int i = 0; i < count; i++)
                            Console.WriteLine($"{names[i]} - {prices[i]} руб.");
                        break;

                    case "4":
                        Console.WriteLine("Выберите валюту:");
                        Console.WriteLine("1. USD (доллар США)");
                        Console.WriteLine("2. EUR (евро)");
                        Console.WriteLine("3. BYN (белорусский рубль)");
                        Console.WriteLine("4. Ввести свой курс");
                        Console.Write("Выбор: ");
                        string curChoice = Console.ReadLine() ?? "";
                        double rate = 0;
                        string curName = "";
                        switch (curChoice) {
                            case "1": rate = 0.011; curName = "USD"; break;
                            case "2": rate = 0.010; curName = "EUR"; break;
                            case "3": rate = 0.035; curName = "BYN"; break;
                            case "4":
                                Console.Write("Введите курс (1 рубль = ? единиц валюты): ");
                                double.TryParse(Console.ReadLine() ?? "", out rate);
                                Console.Write("Название валюты: ");
                                curName = Console.ReadLine() ?? "";
                                break;
                            default:
                                Console.WriteLine("Неверный выбор.");
                                continue;
                        }
                        Console.WriteLine($"\n--- КОНВЕРТАЦИЯ ({curName}) ---");
                        double totalSum = 0;
                        for (int i = 0; i < count; i++) {
                            double converted = prices[i] * rate;
                            totalSum += converted;
                            Console.WriteLine($"{names[i]} - {prices[i]} руб. = {converted:F2} {curName}");
                        }
                        Console.WriteLine($"Итого: {totalSum:F2} {curName}");
                        break;

                    case "5":
                        Console.Write("Введите название для поиска: ");
                        string search = (Console.ReadLine() ?? "").ToLower();
                        Console.WriteLine("\n--- РЕЗУЛЬТАТЫ ПОИСКА ---");
                        bool found = false;
                        for (int i = 0; i < count; i++)
                            if (names[i].ToLower().Contains(search)) {
                                Console.WriteLine($"{names[i]} - {prices[i]} руб.");
                                found = true;
                            }
                        if (!found) Console.WriteLine("Ничего не найдено.");
                        break;

                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }

            Console.WriteLine("До свидания!");
        }
    }
}
