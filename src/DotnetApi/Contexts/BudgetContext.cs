using System.ComponentModel.DataAnnotations.Schema;
using DotnetApi.Models.Users;
using Microsoft.EntityFrameworkCore;
using DotnetApi.Models.Financials;

namespace DotnetApi.Contexts;

/// <summary>
///		The Budget app DB Context
/// </summary>
public class BudgetContext : DbContext
{
	public BudgetContext(DbContextOptions<BudgetContext> options) : base(options) { }

	/// <summary>
	///		Contains all transactions, including both income and expenses
	/// </summary>
	public DbSet<Transaction> Transactions { get; set; } = null!;
	/// <summary>
	///		Contains all users
	/// </summary>
	public DbSet<User> Users { get; set; } = null!;
	/// <summary>
	///		Container refresh tokens, uses foreign key userId
	/// </summary>
	public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
}
