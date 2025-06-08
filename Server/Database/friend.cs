using System;
using System.Collections.Generic;

namespace Server.Database;

public partial class Friend
{
    public int id_first_user { get; set; }

    public int id_second_user { get; set; }

    public bool accepted { get; set; } = false;

    public string? first_nickname { get; set; }

    public string? second_nickname { get; set; }

    public DateTime created_at { get; set; }

    public virtual User id_first_userNavigation { get; set; } = null!;

    public virtual User id_second_userNavigation { get; set; } = null!;
}
