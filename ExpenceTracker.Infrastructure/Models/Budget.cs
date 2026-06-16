namespace ExpenseTracker.Infrastructure.Models;

public class Budget
{
    public int Id { get; set; }
    public string UserId { get; set; } // Щоб прив'язати до користувача
    public string BudgetType { get; set; } // "Needs", "Wants", або "Savings"
    public decimal MaxAmount { get; set; } // Загальний ліміт (50%, 30% або 20% від доходу)
    public decimal CurrentSpent { get; set; } // Скільки вже витрачено з цього ліміту
}