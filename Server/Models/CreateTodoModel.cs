namespace Server.Models;

public class CreateTodoModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
}