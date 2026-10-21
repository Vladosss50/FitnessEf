using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

while (true)
{
    Console.WriteLine("\n--- УПРАВЛЕНИЕ КЛИЕНТАМИ ФИТНЕС-КЛУБА ---");
    Console.WriteLine("1. Список клиентов");
    Console.WriteLine("2. Найти клиента по ID");
    Console.WriteLine("3. Поиск по фрагменту фамилии");
    Console.WriteLine("4. Добавить клиента");
    Console.WriteLine("5. Изменить телефон или статус");
    Console.WriteLine("6. Удалить клиента");
    Console.WriteLine("7. Статистика");
    Console.WriteLine("0. Выход");
    Console.Write("Выбор: ");

    string choice = Console.ReadLine() ?? string.Empty;
    if (choice == "0") break;

    try
    {
        switch (choice)
        {
            case "1": await ListClientsAsync(); break;
            case "2": await FindClientAsync(); break;
            case "3": await SearchByLastNameAsync(); break;
            case "4": await AddClientAsync(); break;
            case "5": await UpdateClientAsync(); break;
            case "6": await DeleteClientAsync(); break;
            case "7": await ShowStatisticsAsync(); break;
            default: Console.WriteLine("Неизвестный пункт меню."); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static async Task ListClientsAsync()
{
    await using var db = new FitnessContext();
    var clients = await db.Clients
        .AsNoTracking()
        .OrderBy(c => c.LastName)
        .ThenBy(c => c.FirstName)
        .ToListAsync();

    Console.WriteLine("\nСписок клиентов:");
    if (clients.Count == 0) Console.WriteLine("База пуста.");
    foreach (var c in clients)
    {
        Console.WriteLine($"{c.ClientId}: {c.LastName} {c.FirstName} | Тел: {c.Phone ?? "нет"} | Статус: {c.Status}");
    }
}

static async Task FindClientAsync()
{
    Console.Write("Введите ID клиента: ");
    if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
    {
        Console.WriteLine("Неверный ID. Введите положительное число.");
        return;
    }

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);

    if (client == null)
        Console.WriteLine("Клиент с таким ID не найден.");
    else
        Console.WriteLine($"Найден: {client.LastName} {client.FirstName} | Тел: {client.Phone ?? "нет"} | Статус: {client.Status}");
}

static async Task SearchByLastNameAsync()
{
    Console.Write("Введите фрагмент фамилии: ");
    string term = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrEmpty(term))
    {
        Console.WriteLine("Строка поиска пуста.");
        return;
    }

    await using var db = new FitnessContext();
    var found = await db.Clients
        .AsNoTracking()
        .Where(c => c.LastName.ToLower().Contains(term.ToLower()))
        .OrderBy(c => c.LastName)
        .ToListAsync();

    if (found.Count == 0) Console.WriteLine("Ничего не найдено.");
    else
    {
        foreach (var c in found)
            Console.WriteLine($"{c.ClientId}: {c.LastName} {c.FirstName}");
    }
}

static async Task AddClientAsync()
{
    Console.Write("Имя: ");
    string firstName = Console.ReadLine()?.Trim() ?? string.Empty;

    Console.Write("Фамилия: ");
    string lastName = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
    {
        Console.WriteLine("Имя и фамилия обязательны для заполнения.");
        return;
    }

    Console.Write("Телефон (необязательно): ");
    string phone = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(phone)) phone = null;

    var newClient = new Client
    {
        FirstName = firstName,
        LastName = lastName,
        Phone = phone,
        RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
        Status = "active"
    };

    await using var db = new FitnessContext();
    db.Clients.Add(newClient);
    await db.SaveChangesAsync();
    Console.WriteLine($"Клиент добавлен. Присвоен ID: {newClient.ClientId}");
}

static async Task UpdateClientAsync()
{
    Console.Write("Введите ID клиента для изменения: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);

    if (client == null)
    {
        Console.WriteLine("Клиент не найден.");
        return;
    }

    Console.Write($"Новый телефон (текущий: {client.Phone ?? "нет"}, оставьте пустым чтобы не менять): ");
    string phone = Console.ReadLine()?.Trim();
    if (!string.IsNullOrWhiteSpace(phone)) client.Phone = phone;

    Console.Write($"Новый статус (текущий: {client.Status}, оставьте пустым чтобы не менять): ");
    string status = Console.ReadLine()?.Trim();
    if (!string.IsNullOrWhiteSpace(status)) client.Status = status;

    await db.SaveChangesAsync();
    Console.WriteLine("Данные клиента обновлены.");
}

static async Task DeleteClientAsync()
{
    Console.Write("Введите ID клиента для удаления: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);

    if (client == null)
    {
        Console.WriteLine("Клиент не найден.");
        return;
    }

    try
    {
        db.Clients.Remove(client);
        await db.SaveChangesAsync();
        Console.WriteLine("Клиент успешно удален.");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine("Ошибка удаления: У этого клиента есть связанные посещения (visits).");
        Console.WriteLine("Сначала удалите его посещения.");
    }
}

static async Task ShowStatisticsAsync()
{
    await using var db = new FitnessContext();
    int total = await db.Clients.CountAsync();
    int active = await db.Clients.CountAsync(c => c.Status == "active");

    Console.WriteLine($"\nВсего клиентов: {total}");
    Console.WriteLine($"Активных клиентов: {active}");
}