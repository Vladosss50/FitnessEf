using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();

while (true)
{
    Console.WriteLine("\n--- МЕНЮ ЗАДАНИЯ 7 ");
    Console.WriteLine("1. Изменить телефон клиента ");
    Console.WriteLine("2. Удалить клиента ");
    Console.WriteLine("3. Выполнить несколько действий сразу ");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Введите ID клиента для изменения телефона: ");
            if (int.TryParse(Console.ReadLine(), out int id1))
            {
                var client1 = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == id1);
                if (client1 is null)
                {
                    Console.WriteLine("Клиент не найден.");
                    break;
                }

                client1.Phone = "79990001122";
                await db.SaveChangesAsync();
                Console.WriteLine("Телефон изменён.");
            }
            break;

        case "2":
            Console.Write("Введите ID клиента для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int id2))
            {
                var client2 = await db.Clients.FindAsync(id2);
                if (client2 is null)
                {
                    Console.WriteLine("Клиент не найден.");
                    break;
                }

                db.Clients.Remove(client2);
                await db.SaveChangesAsync();
                Console.WriteLine("Клиент удалён.");
            }
            break;

        case "3":
            db.Clients.Add(new Client
            {
                FirstName = "Анна",
                LastName = "Смирнова",
                RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
                Status = "active"
            });

            var existing = await db.Clients.FindAsync(1);
            if (existing is not null)
                existing.Status = "inactive";

            await db.SaveChangesAsync();
            Console.WriteLine("Добавлена Анна, а клиенту с ID 1 установлен статус inactive.");
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Неверный выбор.");
            break;
    }
}