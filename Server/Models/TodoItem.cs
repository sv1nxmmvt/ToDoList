using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models;

public class TodoItem
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public string UserId { get; set; } = string.Empty;

    public virtual User User { get; set; } = null!;
}