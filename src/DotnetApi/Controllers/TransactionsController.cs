using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetApi.Models.Financials;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly BudgetContext _context;
    public TransactionsController(BudgetContext context)
    {
        _context = context;
    }

    // GET: api/Transaction
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
    {
        return await _context.Transactions.ToListAsync();
    }

    // GET: api/Transaction/{guid}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Transaction>> GetTransaction(Guid id)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction == null)
        {
            return NotFound();
        }

        return transaction;
    }

    // PUT: api/Transaction/{guid}
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutTransaction(Guid? id, Transaction transaction)
    {
        if (id != transaction.Id)
        {
            return BadRequest();
        }

        _context.Entry(transaction).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TransactionExists(id))
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

    // POST: api/Transaction
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
    }

    // DELETE: api/Transaction/{guid}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTransaction(Guid? id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TransactionExists(Guid? id)
    {
        return _context.Transactions.Any(e => e.Id == id);
    }
}
