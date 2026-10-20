using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();

while (true)
{
    Console.WriteLine("\n--- МЕНЮ ЗАДАНИЯ 5 ---");
    Console.WriteLine("1. Показать активных клиентов (5.1)");
    Console.WriteLine("2. Поиск по фамилии (5.2)");
    Console.WriteLine("3. Поиск по ID (5.3)");
    Console.WriteLine("4. Статистика (количество и наличие) (5.4)");
    Console.WriteLine("5. Постраничная загрузка (5.5)");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":

            Console.WriteLine("\nАктивные клиенты:");
            var activeClients = await db.Clients
                .AsNoTracking()
                .Where(c => c.Status == "active")
                .OrderBy(c => c.LastName)
                .ToListAsync();
            foreach (var c in activeClients)
                Console.WriteLine($"{c.ClientId}: {c.FirstName} {c.LastName}");
            break;

        case "2":

            Console.Write("Фрагмент фамилии: ");
            string term = Console.ReadLine()?.Trim() ?? string.Empty;
            if (term.Length == 0)
            {
                Console.WriteLine("Строка поиска не задана.");
            }
            else
            {
                var found = await db.Clients
                    .AsNoTracking()
                    .Where(c => c.LastName.Contains(term))
                    .OrderBy(c => c.LastName)
                    .ToListAsync();

                if (found.Count == 0) Console.WriteLine("Ничего не найдено.");
                foreach (var c in found)
                    Console.WriteLine($"{c.ClientId}: {c.FirstName} {c.LastName}");
            }
            break;

        case "3":

            Console.Write("ID клиента: ");
            if (int.TryParse(Console.ReadLine(), out int id) && id > 0)
            {
                var client = await db.Clients.FindAsync(id);
                if (client is null)
                    Console.WriteLine("Клиент не найден.");
                else
                    Console.WriteLine($"{client.ClientId}: {client.FirstName} {client.LastName}");
            }
            else
            {
                Console.WriteLine("ID должен быть положительным целым числом.");
            }
            break;

        case "4":

            int total = await db.Clients.CountAsync();
            bool hasInactive = await db.Clients.AnyAsync(c => c.Status == "inactive");
            int activeCount = await db.Clients.CountAsync(c => c.Status == "active");

            Console.WriteLine($"\nВсего клиентов: {total}");
            Console.WriteLine($"Активных клиентов: {activeCount}");
            Console.WriteLine(hasInactive
                ? "Есть неактивные клиенты."
                : "Неактивные клиенты отсутствуют.");
            break;

        case "5":

            int page = 1; 
            int pageSize = 2; 

            var pageOfClients = await db.Clients
                .AsNoTracking()
                .OrderBy(c => c.ClientId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"\nСтраница {page} (размер {pageSize}):");
            foreach (var c in pageOfClients)
                Console.WriteLine($"{c.ClientId}: {c.FirstName} {c.LastName}");
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Неверный выбор.");
            break;
    }
}