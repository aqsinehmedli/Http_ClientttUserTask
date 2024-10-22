using System;
using System.Collections.Generic;

namespace Http_Clienttt;

public partial class UserTask
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? EmailAdress { get; set; }
}
