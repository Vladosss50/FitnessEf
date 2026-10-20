using FitnessEf;

Environment.SetEnvironmentVariable("FITNESS_EF_CONNECTION", "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=123");

await using var db = new FitnessContext();
var canConnect = await db.Database.CanConnectAsync();
Console.WriteLine(canConnect
    ? "Соединение с PostgreSQL установлено."
    : "PostgreSQL недоступен.");