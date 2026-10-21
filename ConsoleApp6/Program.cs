using FitnessEf;
using Microsoft.EntityFrameworkCore;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();

int clientId = 1;
var clientExists = await db.Clients.AnyAsync(c => c.ClientId == clientId);

if (!clientExists)
{
    Console.WriteLine("Клиента с таким ID нет.");
}
else
{
    db.Visits.Add(new Visit
    {
        ClientId = clientId,
        VisitDate = DateOnly.FromDateTime(DateTime.Today),
        Note = "Первичное Задание"
    });

    await db.SaveChangesAsync();
    Console.WriteLine("Посещение успешно добавлено!");
}

Console.WriteLine("\nСписок клиентов и их посещений:");
var clients = await db.Clients
    .AsNoTracking()
    .Include(c => c.Visits)
    .OrderBy(c => c.ClientId)
    .ToListAsync();

foreach (var client in clients)
{
    Console.WriteLine($"{client.FirstName} {client.LastName}");
    if (client.Visits.Count == 0)
    {
        Console.WriteLine("  (нет посещений)");
    }
    else
    {
        foreach (var visit in client.Visits)
        {
            Console.WriteLine($"  {visit.VisitDate}: {visit.Note}");
        }
    }
}