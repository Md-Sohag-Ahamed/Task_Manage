using Microsoft.EntityFrameworkCore;
using Task_Manage.Models;

namespace Task_Manage.Data
{
  public class Task_Context:DbContext
  {
    public Task_Context(DbContextOptions<Task_Context> options) : base(options)
    {

    }
    public DbSet<tblTask> tblTasks { get; set; }
    public DbSet<tblUser> tblUsers {  get; set; }
  }
}
