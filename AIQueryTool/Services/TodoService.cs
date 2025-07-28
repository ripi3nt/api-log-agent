using System.Security.Claims;
using AIToolbox.Models;
using Microsoft.EntityFrameworkCore;

namespace AIToolbox.Services;

public class TodoService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TodoService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetUserId()
    {
        string userid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        return userid;
    }
    public async Task<IEnumerable<TodoItem>> GetAllTodos()
    {
      List<TodoItem> todos = await _context.TodoItems.Where(p => p.userId == GetUserId()).ToListAsync();

      return todos;
    }

    public async Task<bool> AddTodo(bool isComplete, string task)
    {
        TodoItem todoItem = new TodoItem { userId = GetUserId(), IsComplete = isComplete, Name = task };
        await _context.TodoItems.AddAsync(todoItem);
     
        int changes = await _context.SaveChangesAsync();

        if (changes > 0)
        {
            return true;
        }
     
        return false;
    }

    public TodoItem GetTodoItem(int todoid)
    {
        var todoItem = _context.TodoItems.Find(todoid);

        if (todoItem == null)
        {
            return null;
        }

        return todoItem;
    }

    public bool DeleteTodoItem(int todoid)
    {
        var todoItem = _context.TodoItems.Find(todoid);
        if (todoItem == null)
        {
            return false;
        }

        _context.TodoItems.Remove(todoItem);

        if (_context.SaveChanges() > 0)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateTodoItem(int todoid, bool isComplete, string task)
    {
        TodoItem todoItem = await _context.TodoItems.FindAsync(todoid);

        if (todoItem == null || todoItem.userId != GetUserId())
        {
            return false;
        }
      
        todoItem.IsComplete = true;


        if (_context.SaveChanges() > 0)
        {
            return true;
        }

        return false;


    }
}