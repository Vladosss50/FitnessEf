using Npgsql;

var connectString = "Host=localhost;Port=5432;Database=Fitnes;Username=postgres;Password=ВАШ_ПАРОЛЬ";

using var connection = new NpgsqlConnection(connectString);

connection.Open();
Console.WriteLine("Подключение к БД успешно!");