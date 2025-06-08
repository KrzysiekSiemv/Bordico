using System;
using System.Collections.Generic;

namespace Server.Database;

public partial class ConversationGroup
{
    public int id_group { get; set; }

    public string? group_name { get; set; }

    public int? id_owner { get; set; }

    public bool requires_confirmation { get; set; }

    public virtual User? id_ownerNavigation { get; set; }

    public virtual ICollection<Message> messages { get; set; } = new List<Message>();
}
