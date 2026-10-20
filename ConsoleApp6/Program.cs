using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

using var context = new FitnessContext();

try
{
    Console.WriteLine("Проверка подключения...");
    if (context.Database.CanConnect())
    {
        Console.WriteLine("Подключение успешно! Создаем таблицы...");

        context.Database.Migrate();

        Console.WriteLine("Таблицы успешно созданы!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}