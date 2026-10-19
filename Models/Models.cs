namespace Slashers.WPF.Models;

public sealed record GameItem(int Id, string Title, string Platform, string Genre,
    decimal BuyPrice, decimal SellPrice, int Stock, string CoverPath = "")
{
    // The card gallery displays the app emblem until a game-specific cover is selected.
    public string CoverDisplayPath => string.IsNullOrWhiteSpace(CoverPath)
        ? "pack://application:,,,/Assets/Slashers_AppIcon.png"
        : CoverPath;
}

public sealed record TransactionItem(long Id, string Type, string GameTitle, int Quantity,
    decimal UnitPrice, string Partner, decimal Total, DateTime CreatedAt,
    string PaymentMethod = "Tunai", decimal DiscountRate = 0m);

public sealed record DashboardSummary(int Games, int Stock, decimal SalesToday,
    int TransactionsToday, int LowStockGames, decimal TotalSales, decimal TotalPurchases);

public sealed record PartnerSummary(string Partner, string Category, int TransactionCount, decimal TotalValue);
