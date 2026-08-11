using System;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

/// <summary>
/// The Todo app DB Context
/// </summary>
public class TodoContext : DbContext
{
	public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

	public DbSet<TodoItem> TodoItems { get; set; } = null;
}
