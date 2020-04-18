﻿// ASP.NET Core supports creating RESTful services, also known as web APIs, using C#. To handle requests, a web API 
// uses controllers. Controllers in a web API are classes that derive from ControllerBase. 
// https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
  //    [Route("api/[TodoItems]")] // Throws System.InvalidOperationException
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
//        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
//            return await _context.TodoItems.ToListAsync();
            return await _context.TodoItems
              .Select(x => ItemToDTO(x))
              .ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
//        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

//            return todoItem;
            return ItemToDTO(todoItem);
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
//        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
//            if (id != todoItem.Id)
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }

//            _context.Entry(todoItem).State = EntityState.Modified;
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Name = todoItemDTO.Name;
            todoItem.IsComplete = todoItemDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
//            catch (DbUpdateConcurrencyException)
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
/*                if (!TodoItemExists(id))
                {
                    return NotFound();
                }

                else
                {
                    throw;
                }
*/
                return NotFound();
            }

            return NoContent();
        }

        // The proceding code is an HTTP POST method, as indicated by the [HttpPost] attribute. The method gets the 
        // value of the to-do item from the body of the HTTP request.
        // POST: api/TodoItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
//        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
              IsComplete = todoItemDTO.IsComplete,
              Name = todoItemDTO.Name
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = todoItem.Id },
                ItemToDTO(todoItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
//        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        public async Task<ActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

//            return todoItem;
            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
          new TodoItemDTO
          {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
          };

  }
}
