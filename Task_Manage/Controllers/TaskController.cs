using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authorization;
using Task_Manage.Data;
using Task_Manage.Models;

namespace Task_Manage.Controllers
{
  //[Authorize]
  //[Route("api/tasks")]
  [Route("api/[controller]")]
  [ApiController]
  public class TaskController : ControllerBase
  {
    private readonly Task_Context _context;

    public TaskController(Task_Context context)
    {
      _context = context;
    }

    // GET: api/tasks
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {

      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      // Get tasks for the authenticated user
      //var tasks = await _context.tblTasks.ToListAsync();
      var tasks = await _context.tblTasks.Where(t => t.UserId == userId).ToListAsync();
      return Ok(tasks);
    }

    // GET: api/tasks/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
      
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      // Get the task for the authenticated user
      var task = await _context.tblTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

      if (task == null)
      {
        return NotFound();
      }

      return Ok(task);
    }
    
    // POST: api/tasks
    [HttpPost]
    public async Task<IActionResult> CreateTask(tblTask task)
    {
      
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      // Assign the user ID to the task
      task.UserId = userId;

      // Add and save the new task
      _context.tblTasks.Add(task);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetTask", new { id = task.Id }, task);
    }
    
    // PUT: api/tasks/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, tblTask task)
    {
      // Retrieve the user ID from the claims
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      if (id != task.Id || userId != task.UserId)
      {
        return BadRequest();
      }

      // Update the task
      _context.Entry(task).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!TaskExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // DELETE: api/tasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
     
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var task = await _context.tblTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
      if (task == null)
      {
        return NotFound();
      }

      // Delete the task
      _context.tblTasks.Remove(task);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool TaskExists(int id)
    {
      return _context.tblTasks.Any(e => e.Id == id);
    }
  }
}
