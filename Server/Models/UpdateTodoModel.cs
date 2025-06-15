namespace Server.Models;

public class UpdateTodoModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
}