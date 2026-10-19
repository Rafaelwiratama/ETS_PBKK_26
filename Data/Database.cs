using System.IO;
using Microsoft.Data.SqlClient;
using Slashers.WPF.Models;

namespace Slashers.WPF.Data;

public static class Database
{
    public const int LowStockThreshold = 5;
    private const string DatabaseName = "Slashers";
    private static readonly string Folder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Slashers");
    public static string CoverDirectory => Path.Combine(Folder, "Covers");

    // LocalDB is a lightweight SQL Server instance commonly installed with Visual Studio.
    // To use a full SQL Server instance, set SLASHERS_SQL_CONNECTION as an environment variable.
    private static string ConnectionString => Environment.GetEnvironmentVariable("SLASHERS_SQL_CONNECTION")
        ?? $"Server=(localdb)\\MSSQLLocalDB;Database={DatabaseName};Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

    private static SqlConnection Open()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public static void Initialize()
    {
        Directory.CreateDirectory(Folder);
        Directory.CreateDirectory(CoverDirectory);
        EnsureDatabaseExists();
        using var con = Open();
        using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID(N'dbo.Games', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Games (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(160) NOT NULL,
        Platform NVARCHAR(80) NOT NULL,
        Genre NVARCHAR(80) NOT NULL,
        BuyPrice DECIMAL(18,2) NOT NULL CONSTRAINT CK_Games_BuyPrice CHECK (BuyPrice >= 0),
        SellPrice DECIMAL(18,2) NOT NULL CONSTRAINT CK_Games_SellPrice CHECK (SellPrice >= 0),
        Stock INT NOT NULL CONSTRAINT DF_Games_Stock DEFAULT 0 CONSTRAINT CK_Games_Stock CHECK (Stock >= 0),
        CoverPath NVARCHAR(600) NOT NULL CONSTRAINT DF_Games_CoverPath DEFAULT N''
    );
END;
IF OBJECT_ID(N'dbo.Transactions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Transactions (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        Type NVARCHAR(20) NOT NULL CONSTRAINT CK_Transactions_Type CHECK (Type IN (N'PENJUALAN', N'PEMBELIAN')),
        GameId INT NOT NULL,
        GameTitle NVARCHAR(160) NOT NULL,
        Quantity INT NOT NULL CONSTRAINT CK_Transactions_Quantity CHECK (Quantity > 0),
        UnitPrice DECIMAL(18,2) NOT NULL CONSTRAINT CK_Transactions_UnitPrice CHECK (UnitPrice >= 0),
        Partner NVARCHAR(160) NOT NULL,
        Total DECIMAL(18,2) NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Transactions_CreatedAt DEFAULT SYSDATETIME(),
        PaymentMethod NVARCHAR(40) NOT NULL CONSTRAINT DF_Transactions_Payment DEFAULT N'Tunai',
        DiscountRate DECIMAL(5,2) NOT NULL CONSTRAINT DF_Transactions_Discount DEFAULT 0 CONSTRAINT CK_Transactions_Discount CHECK (DiscountRate >= 0 AND DiscountRate <= 100),
        CONSTRAINT FK_Transactions_Games FOREIGN KEY (GameId) REFERENCES dbo.Games(Id)
    );
END;
IF COL_LENGTH(N'dbo.Games', N'CoverPath') IS NULL
    ALTER TABLE dbo.Games ADD CoverPath NVARCHAR(600) NOT NULL CONSTRAINT DF_Games_CoverPath_Migration DEFAULT N'';
IF COL_LENGTH(N'dbo.Transactions', N'PaymentMethod') IS NULL
    ALTER TABLE dbo.Transactions ADD PaymentMethod NVARCHAR(40) NOT NULL CONSTRAINT DF_Transactions_Payment_Migration DEFAULT N'Tunai';
IF COL_LENGTH(N'dbo.Transactions', N'DiscountRate') IS NULL
    ALTER TABLE dbo.Transactions ADD DiscountRate DECIMAL(5,2) NOT NULL CONSTRAINT DF_Transactions_Discount_Migration DEFAULT 0;";
            cmd.ExecuteNonQuery();
        }

        using var count = con.CreateCommand();
        count.CommandText = "SELECT COUNT(*) FROM dbo.Games;";
        if (Convert.ToInt32(count.ExecuteScalar()) == 0)
        {
            // Starter catalog for demonstration; prices and stock are sample data, not real sales records.
            AddGame(new GameItem(0, "Bloodborne", "PlayStation 4", "Soulslike", 250000, 349000, 4));
            AddGame(new GameItem(0, "Dark Souls III", "PlayStation 4", "Soulslike", 280000, 399000, 6));
            AddGame(new GameItem(0, "Elden Ring", "PC", "Action RPG", 500000, 649000, 8));
            AddGame(new GameItem(0, "Sekiro: Shadows Die Twice", "PlayStation 4", "Action", 320000, 449000, 5));
            AddGame(new GameItem(0, "Lies of P", "PlayStation 5", "Soulslike", 350000, 499000, 3));
            AddGame(new GameItem(0, "The Witcher 3: Wild Hunt", "PC", "RPG", 180000, 279000, 7));
            AddGame(new GameItem(0, "Resident Evil 4", "PlayStation 5", "Horror", 400000, 529000, 3));
            AddGame(new GameItem(0, "Hollow Knight", "PC", "Metroidvania", 90000, 149000, 12));
            AddGame(new GameItem(0, "Dark Souls Remastered", "Nintendo Switch", "Soulslike", 240000, 329000, 2));
        }
    }

    private static void EnsureDatabaseExists()
    {
        var builder = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = "master" };
        using var con = new SqlConnection(builder.ConnectionString);
        con.Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = $"IF DB_ID(N'{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}];";
        cmd.ExecuteNonQuery();
    }

    public static List<GameItem> GetGames(string search = "")
    {
        var items = new List<GameItem>();
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"SELECT Id, Title, Platform, Genre, BuyPrice, SellPrice, Stock, CoverPath
FROM dbo.Games
WHERE @Search = N'' OR Title LIKE @LikeSearch OR Platform LIKE @LikeSearch OR Genre LIKE @LikeSearch
ORDER BY Title;";
        cmd.Parameters.AddWithValue("@Search", search.Trim());
        cmd.Parameters.AddWithValue("@LikeSearch", $"%{search.Trim()}%");
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) items.Add(ReadGame(reader));
        return items;
    }

    public static void AddGame(GameItem game)
    {
        ValidateGame(game);
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"INSERT INTO dbo.Games (Title, Platform, Genre, BuyPrice, SellPrice, Stock, CoverPath)
VALUES (@Title, @Platform, @Genre, @BuyPrice, @SellPrice, @Stock, @CoverPath);";
        AddGameParameters(cmd, game);
        cmd.ExecuteNonQuery();
    }

    public static void UpdateGame(GameItem game)
    {
        ValidateGame(game);
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"UPDATE dbo.Games SET Title=@Title, Platform=@Platform, Genre=@Genre,
BuyPrice=@BuyPrice, SellPrice=@SellPrice, Stock=@Stock, CoverPath=@CoverPath WHERE Id=@Id;";
        AddGameParameters(cmd, game);
        cmd.Parameters.AddWithValue("@Id", game.Id);
        cmd.ExecuteNonQuery();
    }

    private static void ValidateGame(GameItem game)
    {
        if (string.IsNullOrWhiteSpace(game.Title)) throw new ArgumentException("Nama game wajib diisi.");
        if (string.IsNullOrWhiteSpace(game.Platform)) throw new ArgumentException("Platform wajib diisi.");
        if (game.BuyPrice < 0 || game.SellPrice < 0) throw new ArgumentException("Harga tidak boleh negatif.");
        if (game.Stock < 0) throw new ArgumentException("Stok tidak boleh negatif.");
    }

    private static void AddGameParameters(SqlCommand cmd, GameItem game)
    {
        cmd.Parameters.AddWithValue("@Title", game.Title.Trim());
        cmd.Parameters.AddWithValue("@Platform", game.Platform.Trim());
        cmd.Parameters.AddWithValue("@Genre", (game.Genre ?? "").Trim());
        cmd.Parameters.AddWithValue("@BuyPrice", game.BuyPrice);
        cmd.Parameters.AddWithValue("@SellPrice", game.SellPrice);
        cmd.Parameters.AddWithValue("@Stock", game.Stock);
        cmd.Parameters.AddWithValue("@CoverPath", game.CoverPath ?? "");
    }

    public static void DeleteGame(int id)
    {
        using var con = Open();
        using var check = con.CreateCommand();
        check.CommandText = "SELECT COUNT(*) FROM dbo.Transactions WHERE GameId=@Id;";
        check.Parameters.AddWithValue("@Id", id);
        if (Convert.ToInt32(check.ExecuteScalar()) > 0)
            throw new InvalidOperationException("Game sudah memiliki riwayat transaksi. Game tersebut tidak bisa dihapus agar riwayat tetap utuh.");
        using var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM dbo.Games WHERE Id=@Id;";
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    public static List<TransactionItem> GetTransactions(string type = "SEMUA")
    {
        var list = new List<TransactionItem>();
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"SELECT Id, Type, GameTitle, Quantity, UnitPrice, Partner, Total, CreatedAt, PaymentMethod, DiscountRate
FROM dbo.Transactions WHERE @Type=N'SEMUA' OR Type=@Type ORDER BY Id DESC;";
        cmd.Parameters.AddWithValue("@Type", type);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(new TransactionItem(reader.GetInt64(0), reader.GetString(1), reader.GetString(2),
                reader.GetInt32(3), reader.GetDecimal(4), reader.GetString(5), reader.GetDecimal(6),
                reader.GetDateTime(7), reader.GetString(8), reader.GetDecimal(9)));
        return list;
    }

    public static void RecordTransaction(string type, int gameId, int quantity, decimal unitPrice,
        string partner, string paymentMethod = "Tunai", decimal discountRate = 0)
    {
        if (type is not ("PENJUALAN" or "PEMBELIAN")) throw new ArgumentException("Jenis transaksi tidak valid.");
        if (quantity <= 0) throw new ArgumentException("Jumlah harus lebih dari nol.");
        if (unitPrice < 0) throw new ArgumentException("Harga tidak boleh negatif.");
        if (string.IsNullOrWhiteSpace(partner)) throw new ArgumentException("Nama pelanggan atau supplier wajib diisi.");
        if (discountRate < 0 || discountRate > 100) throw new ArgumentException("Diskon harus berada di antara 0–100%.");
        if (type == "PEMBELIAN") { paymentMethod = "—"; discountRate = 0; }

        using var con = Open();
        using var tx = con.BeginTransaction(System.Data.IsolationLevel.Serializable);
        try
        {
            string title; int currentStock;
            using (var get = con.CreateCommand())
            {
                get.Transaction = tx;
                get.CommandText = "SELECT Title, Stock FROM dbo.Games WITH (UPDLOCK, ROWLOCK) WHERE Id=@Id;";
                get.Parameters.AddWithValue("@Id", gameId);
                using var reader = get.ExecuteReader();
                if (!reader.Read()) throw new InvalidOperationException("Game tidak ditemukan.");
                title = reader.GetString(0); currentStock = reader.GetInt32(1);
            }
            if (type == "PENJUALAN" && quantity > currentStock)
                throw new InvalidOperationException($"Stok tidak cukup. Stok tersedia: {currentStock} unit.");
            int newStock = type == "PENJUALAN" ? currentStock - quantity : currentStock + quantity;
            decimal total = decimal.Round(unitPrice * quantity * (1 - discountRate / 100m), 0, MidpointRounding.AwayFromZero);

            using (var update = con.CreateCommand())
            {
                update.Transaction = tx;
                update.CommandText = "UPDATE dbo.Games SET Stock=@Stock WHERE Id=@Id;";
                update.Parameters.AddWithValue("@Stock", newStock);
                update.Parameters.AddWithValue("@Id", gameId);
                update.ExecuteNonQuery();
            }
            using (var insert = con.CreateCommand())
            {
                insert.Transaction = tx;
                insert.CommandText = @"INSERT INTO dbo.Transactions
(Type, GameId, GameTitle, Quantity, UnitPrice, Partner, Total, CreatedAt, PaymentMethod, DiscountRate)
VALUES (@Type, @GameId, @Title, @Quantity, @UnitPrice, @Partner, @Total, SYSDATETIME(), @PaymentMethod, @DiscountRate);";
                insert.Parameters.AddWithValue("@Type", type);
                insert.Parameters.AddWithValue("@GameId", gameId);
                insert.Parameters.AddWithValue("@Title", title);
                insert.Parameters.AddWithValue("@Quantity", quantity);
                insert.Parameters.AddWithValue("@UnitPrice", unitPrice);
                insert.Parameters.AddWithValue("@Partner", partner.Trim());
                insert.Parameters.AddWithValue("@Total", total);
                insert.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                insert.Parameters.AddWithValue("@DiscountRate", discountRate);
                insert.ExecuteNonQuery();
            }
            tx.Commit();
        }
        catch { try { tx.Rollback(); } catch { /* keep original database error */ } throw; }
    }

    public static DashboardSummary GetDashboardSummary()
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"SELECT
(SELECT COUNT(*) FROM dbo.Games),
(SELECT COALESCE(SUM(Stock),0) FROM dbo.Games),
(SELECT COALESCE(SUM(Total),0) FROM dbo.Transactions WHERE Type=N'PENJUALAN' AND CAST(CreatedAt AS date)=CAST(GETDATE() AS date)),
(SELECT COUNT(*) FROM dbo.Transactions WHERE CAST(CreatedAt AS date)=CAST(GETDATE() AS date)),
(SELECT COUNT(*) FROM dbo.Games WHERE Stock<=@Threshold),
(SELECT COALESCE(SUM(Total),0) FROM dbo.Transactions WHERE Type=N'PENJUALAN'),
(SELECT COALESCE(SUM(Total),0) FROM dbo.Transactions WHERE Type=N'PEMBELIAN');";
        cmd.Parameters.AddWithValue("@Threshold", LowStockThreshold);
        using var reader = cmd.ExecuteReader(); reader.Read();
        return new DashboardSummary(reader.GetInt32(0), reader.GetInt32(1), reader.GetDecimal(2),
            reader.GetInt32(3), reader.GetInt32(4), reader.GetDecimal(5), reader.GetDecimal(6));
    }

    public static List<GameItem> GetLowStockGames() => GetGames().Where(g => g.Stock <= LowStockThreshold).OrderBy(g => g.Stock).ThenBy(g => g.Title).ToList();

    public static List<(string Title, int Units, decimal Total)> GetTopSellingGames()
    {
        var list = new List<(string, int, decimal)>();
        using var con = Open(); using var cmd = con.CreateCommand();
        cmd.CommandText = @"SELECT TOP 10 GameTitle, SUM(Quantity) AS Units, SUM(Total) AS Total
FROM dbo.Transactions WHERE Type=N'PENJUALAN' GROUP BY GameTitle ORDER BY SUM(Quantity) DESC, GameTitle;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) list.Add((reader.GetString(0), Convert.ToInt32(reader.GetValue(1)), reader.GetDecimal(2)));
        return list;
    }

    public static List<PartnerSummary> GetPartnerSummary()
    {
        var list = new List<PartnerSummary>();
        using var con = Open(); using var cmd = con.CreateCommand();
        cmd.CommandText = @"SELECT Partner,
CASE Type WHEN N'PENJUALAN' THEN N'Pelanggan' ELSE N'Supplier' END AS Category,
COUNT(*) AS TransactionCount, SUM(Total) AS TotalValue
FROM dbo.Transactions GROUP BY Partner, Type ORDER BY Partner, Category;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) list.Add(new PartnerSummary(reader.GetString(0), reader.GetString(1), Convert.ToInt32(reader.GetInt32(2)), reader.GetDecimal(3)));
        return list;
    }

    public static List<(DateTime Date, decimal Total)> GetDailySales()
    {
        var dict = new Dictionary<DateTime, decimal>();
        using (var con = Open()) using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = @"SELECT CAST(CreatedAt AS date), SUM(Total) FROM dbo.Transactions
WHERE Type=N'PENJUALAN' AND CreatedAt>=@Start GROUP BY CAST(CreatedAt AS date);";
            cmd.Parameters.AddWithValue("@Start", DateTime.Today.AddDays(-6));
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) dict[reader.GetDateTime(0).Date] = reader.GetDecimal(1);
        }
        return Enumerable.Range(0, 7).Select(n => DateTime.Today.AddDays(-6 + n))
            .Select(d => (d, dict.GetValueOrDefault(d.Date, 0m))).ToList();
    }

    private static GameItem ReadGame(SqlDataReader reader) => new(
        reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3),
        reader.GetDecimal(4), reader.GetDecimal(5), reader.GetInt32(6), reader.GetString(7));
}
