using Microsoft.AspNetCore.Identity;

namespace TodoApp.Models;

public class User : IdentityUser
{
    public virtual ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}