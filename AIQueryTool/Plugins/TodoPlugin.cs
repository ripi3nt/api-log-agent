using System.ComponentModel;
using System.Security.Claims;
using AIToolbox.Models;
using AIToolbox.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AIToolbox.Plugins;

public class TodoPlugin
{

   private readonly TodoService _todoService;
   public TodoPlugin(TodoService todoService)
   {
      _todoService = todoService;
   }
   
   [KernelFunction("removeTodoItem")]
   [Description("Removes todo item")]
   public void removeTodoItem(int id)
   {
      _todoService.DeleteTodoItem(id);
   }

   [KernelFunction("getAllTodos")]
   [Description("Gets all todo items")]
   public async Task<IEnumerable<TodoItem>> getAllTodos()
   {
       var todos = await _todoService.GetAllTodos();
       List<TodoItem>  todoItems = todos.ToList(); 
       return todos;
   }

   [KernelFunction("createTodo")]
   [Description("Creates a new todo for the current user. Returns true if the task was successfully added")]
   public async Task<bool> addTodoItem(string task, bool isComplete)
   {
      if (await _todoService.AddTodo(isComplete, task))
      {
         return true;
      }

      return false;
   }

   [KernelFunction("updateTodo")]
   [Description("Marks todo as complete")]
   public async Task<bool> markTodoComplete(int id)
   {
      TodoItem todoItem = _todoService.GetTodoItem(id);
      return await _todoService.UpdateTodoItem(todoItem.Id, isComplete: true, todoItem.Name);

   }
}