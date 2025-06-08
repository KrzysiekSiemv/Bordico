using System;
using System.Collections.Generic;

namespace Server.Database;

public partial class ReadStatus
{
    public int? id_message { get; set; }

    public int? id_user { get; set; }

    public bool is_read { get; set; }

    public virtual Message? id_messageNavigation { get; set; }

    public virtual User? id_userNavigation { get; set; }
}
