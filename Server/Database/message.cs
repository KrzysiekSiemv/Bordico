using System;
using System.Collections.Generic;

namespace Server.Database;

public partial class Message
{
    public int id_message { get; set; }

    public int? id_conversation { get; set; }

    public int? id_group { get; set; }

    public int? id_sent_by { get; set; }

    public string? content { get; set; }

    public DateTime sent_at { get; set; }

    public bool edited { get; set; }

    public bool delivered { get; set; }

    public bool is_read { get; set; }

    public virtual Conversation? id_conversationNavigation { get; set; }

    public virtual ConversationGroup? id_groupNavigation { get; set; }

    public virtual User? id_sent_byNavigation { get; set; }
}
