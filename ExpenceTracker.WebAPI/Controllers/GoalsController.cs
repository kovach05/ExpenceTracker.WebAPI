using ExpenceTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenceTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoalsController : ControllerBase
{
    private readonly ExpenseTrackerDbContext _context;

    public GoalsController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Goal>>> GetGoals()
    {
        return await _context.Goals.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Goal>> PostGoal(Goal goal)
    {
        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();
        return Ok(goal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutGoal(int id, Goal goal)
    {
        if (id != goal.Id) return BadRequest();
        _context.Entry(goal).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGoal(int id)
    {
        var goal = await _context.Goals.FindAsync(id);
        if (goal == null) return NotFound();

        _context.Goals.Remove(goal);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete("cancel/{goalId}/{accountId}")]
public async Task<IActionResult> CancelGoal(int goalId, int accountId)
{
    var goal = await _context.Goals.FindAsync(goalId);
    var account = await _context.Accounts.FindAsync(accountId);

    if (goal == null || account == null) return NotFound();

    // Повертаємо гроші на рахунок (врахуй валюту, якщо потрібно)
    // Якщо ціль в UAH, а рахунок в USD — треба конвертувати назад
    account.Balance += goal.Current; 

    _context.Goals.Remove(goal);
    await _context.SaveChangesAsync();

    return NoContent();
}   
}