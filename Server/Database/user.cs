using System;
using System.Collections.Generic;

namespace Server.Database;

public partial class User
{
    public int id_user { get; set; }

    public string username { get; set; } = null!;

    public string email_address { get; set; } = null!;

    public string? nickname { get; set; }

    public string? description { get; set; }

    public string password { get; set; } = null!;

    public bool allow_messages { get; set; } = true;

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<ConversationGroup> conversation_groups { get; set; } = new List<ConversationGroup>();

    public virtual ICollection<Conversation> conversationid_first_userNavigations { get; set; } = new List<Conversation>();

    public virtual ICollection<Conversation> conversationid_second_userNavigations { get; set; } = new List<Conversation>();

    public virtual ICollection<Message> messages { get; set; } = new List<Message>();
}
