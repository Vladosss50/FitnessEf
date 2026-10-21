using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();

while (true)
{
    Console.WriteLine("\n--- МЕНЮ ЗАДАНИЯ 6 (Добавление данных) ---");
    Console.WriteLine("1. Добавить тестового клиента (Дмитрий Соколов) ");
    Console.WriteLine("2. Добавить клиента с проверкой ввода ");
    Console.WriteLine("3. Добавить клиента с обработкой ошибок ");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            var client1 = new Client
            {
                FirstName = "Дмитрий",
                LastName = "Соколов",
                BirthDate = new DateOnly(2000, 5, 15),
                Phone = "79995554433",
                Email = "sokolov@example.com",
                RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
                Status = "active"
            };

            db.Clients.Add(client1);
            int saved = await db.SaveChangesAsync();

            Console.WriteLine($"Сохранено изменений: {saved}");
            Console.WriteLine($"ID нового клиента: {client1.ClientId}");
            break;

        case "2":
            Console.Write("Имя: ");
            string firstName = Console.ReadLine()?.Trim() ?? string.Empty;

            Console.Write("Фамилия: ");
            string lastName = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                Console.WriteLine("Имя и фамилия обязательны.");
                break;
            }

            var client2 = new Client
            {
                FirstName = firstName,
                LastName = lastName,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
                Status = "active"
            };

            db.Clients.Add(client2);
            await db.SaveChangesAsync();
            Console.WriteLine($"Клиент {firstName} {lastName} успешно добавлен!");
            break;

        case "3":
            Console.WriteLine("Введите имя (попробуйте ввести больше 100 символов для проверки ошибки):");
            string badName = Console.ReadLine() ?? "Тест";

            var client3 = new Client
            {
                FirstName = badName,
                LastName = "Тестов",
                RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
                Status = "active"
            };

            try
            {
                db.Clients.Add(client3);
                await db.SaveChangesAsync();
                Console.WriteLine($"Клиент добавлен. ID: {client3.ClientId}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Не удалось сохранить клиента. Проверьте данные и ограничения БД.");
                Console.WriteLine(ex.GetBaseException().Message);
            }
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Неверный выбор.");
            break;
    }
}