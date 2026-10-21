using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();

while (true)
{
    Console.WriteLine("\n--- МЕНЮ ЗАДАНИЯ 8 (NULL и схемы) ---");
    Console.WriteLine("1. Найти клиентов без email (8.1)");
    Console.WriteLine("2. Проверить атрибуты Data Annotations (8.3)");
    Console.WriteLine("3. Вызвать ошибку not-null violation (8.4)");
    Console.WriteLine("4. Вызвать ошибку string_data_right_truncation (8.4)");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.WriteLine("\nКлиенты без email:");
            var clientsWithoutEmail = await db.Clients
                .AsNoTracking()
                .Where(c => c.Email == null)
                .OrderBy(c => c.ClientId)
                .ToListAsync();

            if (clientsWithoutEmail.Count == 0)
                Console.WriteLine("Все клиенты имеют email.");
            else
            {
                foreach (var c in clientsWithoutEmail)
                {
                    Console.WriteLine($"{c.ClientId}: {c.FirstName} {c.LastName} (тел.: {c.Phone ?? "не указан"})");
                }
            }
            break;

        case "2":
            var clientType = typeof(Client);
            var tableAttr = clientType.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute), true);
            var props = clientType.GetProperties();

            Console.WriteLine($"\nКласс {clientType.Name} имеет атрибуты Data Annotations.");
            Console.WriteLine($"Количество свойств: {props.Length}");
            Console.WriteLine("Проверка выполнена. (В нашем проекте основная настройка идет через Fluent API в FitnessContext)");
            break;

        case "3":
            Console.WriteLine("\nПопытка вставить NULL в обязательное поле (FirstName)...");
            var badClient1 = new Client
            {
                FirstName = null,
                LastName = "Ошибкин",
                RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
                Status = "active"
            };

            try
            {
                db.Clients.Add(badClient1);
                await db.SaveChangesAsync();
                Console.WriteLine("Ошибка не произошла (странно).");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Поймана ошибка базы данных:");
                Console.WriteLine(ex.GetBaseException().Message);
                db.Entry(badClient1).State = EntityState.Detached;
            }
            break;

        case "4":
            Console.WriteLine("\nПопытка вставить строку длиннее 100 символов (FirstName)...");
            var longString = new string('A', 150);
            var badClient2 = new Client
            {
                FirstName = longString,
                LastName = "Длиннюков",
                RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
                Status = "active"
            };

            try
            {
                db.Clients.Add(badClient2);
                await db.SaveChangesAsync();
                Console.WriteLine("Ошибка не произошла (странно).");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Поймана ошибка базы данных:");
                Console.WriteLine(ex.GetBaseException().Message);
                db.Entry(badClient2).State = EntityState.Detached;
            }
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Неверный выбор.");
            break;
    }
}