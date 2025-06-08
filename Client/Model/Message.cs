namespace Bordico.Client.Model;

public class Message {
    public int Id { get; set; }
    public string Author { get; set; } = "";
    public string Content { get; set; } = "";
    public string SentAt { get; set; } = DateTime.Now.ToString("dd.MM.yyyy, HH:mm:ss");
    public bool IsMine { get; set; }
}