using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

while (true)
{
    Console.WriteLine("\n--- МЕНЮ ЗАДАНИЯ 11 (Обработка ошибок и диагностика) ---");
    Console.WriteLine("1. Базовая обработка ошибок ");
    Console.WriteLine("2. Асинхронные запросы ");
    Console.WriteLine("3. Сравнение отслеживаемого и нетрекаемого ");
    Console.WriteLine("4. Диагностика SQL ");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            try
            {
                await using var db = new FitnessContext();
                var clients = await db.Clients.AsNoTracking().ToListAsync();
                Console.WriteLine($"Загружено клиентов: {clients.Count}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Ошибка конфигурации: {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка сохранения данных.");
                Console.WriteLine(ex.GetBaseException().Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла непредвиденная ошибка.");
                Console.WriteLine(ex.Message);
            }
            break;

        case "2":
            await using (var db = new FitnessContext())
            {
                var count = await db.Clients.CountAsync();
                var first = await db.Clients
                    .AsNoTracking()
                    .OrderBy(c => c.ClientId)
                    .FirstOrDefaultAsync();

                Console.WriteLine($"Всего клиентов (CountAsync): {count}");
                if (first != null)
                    Console.WriteLine($"Первый клиент (FirstOrDefaultAsync): {first.FirstName} {first.LastName}");
            }
            break;

        case "3":
            await using (var db = new FitnessContext())
            {
                var list = await db.Clients
                    .AsNoTracking()
                    .ToListAsync();
                Console.WriteLine($"Прочитано без отслеживания (AsNoTracking): {list.Count}");

                int id = 1;
                var client = await db.Clients
                    .FirstOrDefaultAsync(c => c.ClientId == id);
                if (client is not null)
                {
                    client.Status = "inactive";
                    await db.SaveChangesAsync();
                    Console.WriteLine($"Статус клиента {client.FirstName} изменен на inactive (через отслеживание).");
                }
                else
                {
                    Console.WriteLine("Клиент с ID=1 не найден.");
                }
            }
            break;

        case "4":
            await using (var db = new FitnessContext())
            {
                var query = db.Clients
                    .Where(c => c.Status == "active")
                    .OrderBy(c => c.LastName);

                Console.WriteLine("\nСгенерированный SQL-запрос:");
                Console.WriteLine(query.ToQueryString());
            }
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Неверный выбор.");
            break;
    }
}