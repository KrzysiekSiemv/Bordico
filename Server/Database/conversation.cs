using System;
using System.Collections.Generic;

namespace Server.Database;

public partial class Conversation
{
    public int id_conversation { get; set; }

    public int id_first_user { get; set; }

    public int id_second_user { get; set; }

    public virtual User id_first_userNavigation { get; set; } = null!;

    public virtual User id_second_userNavigation { get; set; } = null!;

    public virtual ICollection<Message> messages { get; set; } = new List<Message>();
}
