using System;
using System.Collections.Generic;

namespace ConsoleApp6.ExistingData.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateOnly RegistrationDate { get; set; }

    public string Status { get; set; } = null!;
}
