namespace Bordico.Client.Model;

public class User
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
}