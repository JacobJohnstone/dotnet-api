using DotnetApi.Models.Users;
using Microsoft.EntityFrameworkCore;
using DotnetApi.Models.Financials;

/// <summary>
/// The Budget app DB Context
/// </summary>
public class BudgetContext : DbContext
{
	public BudgetContext(DbContextOptions<BudgetContext> options) : base(options) { }

	public DbSet<Transaction> Transactions { get; set; } = null!;

	public DbSet<User> Users { get; set; } = null!;
}
