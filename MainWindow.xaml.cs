using System.IO;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Slashers.WPF.Data;
using Slashers.WPF.Dialogs;
using Slashers.WPF.Models;

namespace Slashers.WPF;

public partial class MainWindow : Window
{
    private bool _isLoaded;
    private Button? _activeNav;

    public MainWindow()
    {
        InitializeComponent();
        TodayText.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new CultureInfo("id-ID")).ToUpperInvariant();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;
        RefreshAll();
        SetPage("Dashboard", FindButtonForTag("Dashboard"));
    }

    private Button? FindButtonForTag(string tag) => FindVisualChildren<Button>(this).FirstOrDefault(b => string.Equals(b.Tag?.ToString(), tag, StringComparison.Ordinal));

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T target) yield return target;
            foreach (var descendant in FindVisualChildren<T>(child)) yield return descendant;
        }
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tag) SetPage(tag, button);
    }

    private void SetPage(string page, Button? navButton = null)
    {
        DashboardPage.Visibility = page == "Dashboard" ? Visibility.Visible : Visibility.Collapsed;
        CatalogPage.Visibility = page == "Catalog" ? Visibility.Visible : Visibility.Collapsed;
        TransactionsPage.Visibility = page == "Transactions" ? Visibility.Visible : Visibility.Collapsed;
        HistoryPage.Visibility = page == "History" ? Visibility.Visible : Visibility.Collapsed;
        StockPage.Visibility = page == "Stock" ? Visibility.Visible : Visibility.Collapsed;
        PartnersPage.Visibility = page == "Partners" ? Visibility.Visible : Visibility.Collapsed;
        ReportsPage.Visibility = page == "Reports" ? Visibility.Visible : Visibility.Collapsed;

        (PageTitleText.Text, PageSubtitleText.Text) = page switch
        {
            "Dashboard" => ("Dashboard", "Your game store, organized."),
            "Catalog" => ("Katalog Game", "Kelola koleksi, platform, harga, stok, dan cover setiap game."),
            "Transactions" => ("Pembelian / Penjualan", "Catat pergerakan barang dengan pembaruan stok otomatis."),
            "History" => ("Riwayat Transaksi", "Catatan pembelian dan penjualan yang telah tersimpan."),
            "Stock" => ("Stok Kritis", "Judul dengan lima unit atau kurang perlu diperiksa."),
            "Partners" => ("Mitra Toko", "Pelanggan dan pemasok yang tercatat dalam transaksi."),
            "Reports" => ("Laporan", "Ringkasan transaksi dan game yang paling banyak terjual."),
            _ => ("Slashers", "Game Store Management")
        };
        if (_activeNav is not null)
        {
            _activeNav.Background = new SolidColorBrush(Color.FromRgb(32, 38, 49));
            _activeNav.BorderBrush = new SolidColorBrush(Color.FromRgb(57, 66, 81));
        }
        if (navButton is not null)
        {
            navButton.Background = new SolidColorBrush(Color.FromRgb(21, 95, 88));
            navButton.BorderBrush = new SolidColorBrush(Color.FromRgb(32, 191, 169));
            _activeNav = navButton;
        }
        if (_isLoaded) RefreshPageData(page);
    }

    private void RefreshAll()
    {
        RefreshDashboard(); RefreshCatalog(); RefreshHistory(); RefreshStock(); RefreshPartners(); RefreshReports(); RefreshTransactionGames();
    }

    private void RefreshPageData(string page)
    {
        switch (page)
        {
            case "Dashboard": RefreshDashboard(); break;
            case "Catalog": RefreshCatalog(); break;
            case "Transactions": RefreshTransactionGames(); break;
            case "History": RefreshHistory(); break;
            case "Stock": RefreshStock(); break;
            case "Partners": RefreshPartners(); break;
            case "Reports": RefreshReports(); break;
        }
    }

    private void RefreshDashboard()
    {
        var summary = Database.GetDashboardSummary();
        StatGames.Text = summary.Games.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
        StatStock.Text = summary.Stock.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
        StatSales.Text = FormatCurrency(summary.SalesToday);
        StatTransactions.Text = $"{summary.TransactionsToday} transaksi hari ini";
        StatLowStock.Text = summary.LowStockGames.ToString(CultureInfo.GetCultureInfo("id-ID"));
        LowStockGrid.ItemsSource = Database.GetLowStockGames();
        RecentTransactionsGrid.ItemsSource = Database.GetTransactions().Take(7).ToList();
        var sales = Database.GetDailySales();
        var max = Math.Max(1m, sales.Max(x => x.Total));
        SalesBars.ItemsSource = sales.Select(x => new SalesBar(
            x.Date.ToString("dd/MM"),
            Math.Max(4d, (double)(x.Total / max * 96m)),
            ShortCurrency(x.Total))).ToList();
        StatusText.Text = $"SLASHERS UPDATED • {DateTime.Now:HH:mm:ss} • SQL SERVER LOCALDB";
    }

    private void RefreshCatalog()
    {
        CatalogGrid.ItemsSource = Database.GetGames(CatalogSearchTextBox?.Text ?? "");
        StatusText.Text = "SLASHERS CATALOG • PILIH GAME UNTUK MELIHAT COVER";
    }

    private void RefreshTransactionGames()
    {
        var selectedId = (TransactionGameCombo.SelectedItem as GameItem)?.Id;
        var games = Database.GetGames();
        TransactionGameCombo.ItemsSource = games;
        TransactionGameCombo.SelectedItem = selectedId is null ? games.FirstOrDefault() : games.FirstOrDefault(g => g.Id == selectedId) ?? games.FirstOrDefault();
        UpdateTransactionPrice();
    }

    private void RefreshHistory()
    {
        var type = (HistoryFilterCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "SEMUA";
        HistoryGrid.ItemsSource = Database.GetTransactions(type);
    }

    private void RefreshStock() => StockGrid.ItemsSource = Database.GetLowStockGames();

    private void RefreshPartners() => PartnersGrid.ItemsSource = Database.GetPartnerSummary();

    private void RefreshReports()
    {
        var summary = Database.GetDashboardSummary();
        ReportSales.Text = FormatCurrency(summary.TotalSales);
        ReportPurchases.Text = FormatCurrency(summary.TotalPurchases);
        TopSellingGrid.ItemsSource = Database.GetTopSellingGames()
            .Select(x => new { x.Title, x.Units, x.Total }).ToList();
    }

    private void CatalogSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isLoaded) RefreshCatalog();
    }

    private void CatalogGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CatalogGrid.SelectedItem is not GameItem game)
        {
            CatalogSelectedTitle.Text = "Pilih game dari katalog";
            CatalogSelectedDetails.Text = "Sampul game akan ditampilkan di sini.";
            SetCoverPreview("");
            return;
        }
        CatalogSelectedTitle.Text = game.Title;
        CatalogSelectedDetails.Text = $"{game.Platform} • {game.Genre}\nHarga jual: {FormatCurrency(game.SellPrice)}\nHarga beli: {FormatCurrency(game.BuyPrice)}\nStok tersedia: {game.Stock} unit";
        SetCoverPreview(game.CoverPath);
    }

    private void SetCoverPreview(string path)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                var image = new BitmapImage(); image.BeginInit(); image.CacheOption = BitmapCacheOption.OnLoad; image.UriSource = new Uri(path); image.EndInit(); image.Freeze();
                CatalogCoverPreview.Source = image;
            }
            else
                CatalogCoverPreview.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Slashers_AppIcon.png"));
        }
        catch { CatalogCoverPreview.Source = null; }
    }

    private void AddGame_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new GameDialog(null);
        if (dialog.ShowDialog() != true || dialog.ResultGame is null) return;
        try
        {
            var item = dialog.ResultGame with { CoverPath = PersistCover(dialog.ResultGame.CoverPath) };
            Database.AddGame(item); RefreshAll(); StatusText.Text = $"GAME ADDED • {item.Title}";
        }
        catch (Exception ex) { ShowError("Game tidak berhasil ditambahkan.", ex); }
    }

    private void EditGame_Click(object sender, RoutedEventArgs e)
    {
        if (CatalogGrid.SelectedItem is not GameItem game) { ShowInfo("Pilih game yang ingin diedit dari katalog."); return; }
        var dialog = new GameDialog(game);
        if (dialog.ShowDialog() != true || dialog.ResultGame is null) return;
        try
        {
            var item = dialog.ResultGame with { CoverPath = PersistCover(dialog.ResultGame.CoverPath) };
            Database.UpdateGame(item); RefreshAll(); SelectGame(item.Id); StatusText.Text = $"GAME DETAILS UPDATED • {item.Title}";
        }
        catch (Exception ex) { ShowError("Perubahan game gagal disimpan.", ex); }
    }

    private string PersistCover(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return "";
        var destinationRoot = Path.GetFullPath(Database.CoverDirectory) + Path.DirectorySeparatorChar;
        var fullPath = Path.GetFullPath(path);
        if (fullPath.StartsWith(destinationRoot, StringComparison.OrdinalIgnoreCase)) return fullPath;
        Directory.CreateDirectory(Database.CoverDirectory);
        var extension = Path.GetExtension(fullPath);
        var target = Path.Combine(Database.CoverDirectory, $"cover_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{extension}");
        File.Copy(fullPath, target, false);
        return target;
    }

    private void SelectGame(int id)
    {
        foreach (var item in CatalogGrid.Items)
            if (item is GameItem game && game.Id == id) { CatalogGrid.SelectedItem = item; CatalogGrid.ScrollIntoView(item); break; }
    }

    private void DeleteGame_Click(object sender, RoutedEventArgs e)
    {
        if (CatalogGrid.SelectedItem is not GameItem game) { ShowInfo("Pilih game yang ingin dihapus dari katalog."); return; }
        if (MessageBox.Show(this, $"Hapus '{game.Title}' dari katalog? Game yang sudah memiliki riwayat transaksi tidak bisa dihapus.", "Konfirmasi Hapus", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        try { Database.DeleteGame(game.Id); RefreshAll(); StatusText.Text = $"GAME REMOVED • {game.Title}"; }
        catch (Exception ex) { ShowError("Game tidak dapat dihapus.", ex); }
    }

    private void TransactionTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isLoaded) return;
        bool sale = SelectedTransactionType() == "PENJUALAN";
        TransactionDiscountText.IsEnabled = sale;
        TransactionPaymentCombo.IsEnabled = sale;
        UpdateTransactionPrice(); CalculateTransactionTotal();
    }

    private void TransactionGameCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isLoaded) { UpdateTransactionPrice(force: true); CalculateTransactionTotal(); }
    }

    private void TransactionFields_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isLoaded) CalculateTransactionTotal();
    }

    private string SelectedTransactionType() => (TransactionTypeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "PENJUALAN";

    private void UpdateTransactionPrice(bool force = false)
    {
        if (TransactionUnitPriceText is null || TransactionGameCombo is null) return;
        if (TransactionGameCombo.SelectedItem is not GameItem game) { TransactionUnitPriceText.Text = "0"; return; }
        bool isBuy = SelectedTransactionType() == "PEMBELIAN";
        decimal current = decimal.TryParse(TransactionUnitPriceText.Text, out var n) ? n : -1;
        // Replace the value only if the field is empty or if it equals a previous catalog price.
        if (force || current < 0 || current == 0 || current == game.BuyPrice || current == game.SellPrice)
            TransactionUnitPriceText.Text = (isBuy ? game.BuyPrice : game.SellPrice).ToString("0.##", CultureInfo.CurrentCulture);
        CalculateTransactionTotal();
    }

    private void CalculateTransactionTotal()
    {
        int quantity = int.TryParse(TransactionQuantityText.Text, out var q) && q > 0 ? q : 0;
        decimal price = decimal.TryParse(TransactionUnitPriceText.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var p) && p >= 0 ? p : 0;
        decimal discount = decimal.TryParse(TransactionDiscountText.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var d) ? Math.Clamp(d, 0, 100) : 0;
        if (SelectedTransactionType() == "PEMBELIAN") discount = 0;
        TransactionTotalText.Text = FormatCurrency(decimal.Round(price * quantity * (1 - discount / 100m), 0, MidpointRounding.AwayFromZero));
    }

    private void SaveTransaction_Click(object sender, RoutedEventArgs e)
    {
        if (TransactionGameCombo.SelectedItem is not GameItem game) { ShowInfo("Belum ada game dalam katalog. Tambahkan game terlebih dahulu."); return; }
        if (!int.TryParse(TransactionQuantityText.Text, out var quantity) || quantity <= 0) { ShowInfo("Jumlah unit harus bilangan bulat lebih besar dari nol."); return; }
        if (!decimal.TryParse(TransactionUnitPriceText.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var price) || price < 0) { ShowInfo("Harga per unit harus berupa angka nol atau lebih."); return; }
        if (!decimal.TryParse(TransactionDiscountText.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var discount)) discount = 0;
        if (discount < 0 || discount > 100) { ShowInfo("Diskon harus berada di antara 0–100%."); return; }
        var partner = TransactionPartnerText.Text.Trim();
        if (string.IsNullOrWhiteSpace(partner)) { ShowInfo("Nama pelanggan atau supplier wajib diisi."); return; }
        var type = SelectedTransactionType();
        var payment = (TransactionPaymentCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tunai";
        try
        {
            Database.RecordTransaction(type, game.Id, quantity, price, partner, payment, discount);
            ShowInfo($"Transaksi {type.ToLowerInvariant()} berhasil dicatat. Stok telah diperbarui.", "Transaksi Berhasil");
            TransactionQuantityText.Text = "1"; TransactionPartnerText.Clear(); TransactionDiscountText.Text = "0";
            RefreshAll(); SetPage("Transactions", FindButtonForTag("Transactions"));
        }
        catch (Exception ex) { ShowError("Transaksi tidak berhasil disimpan.", ex); }
    }

    private void HistoryFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isLoaded) RefreshHistory();
    }

    private void OpenStock_Click(object sender, RoutedEventArgs e) => SetPage("Stock", FindButtonForTag("Stock"));

    private void ExportCatalog_Click(object sender, RoutedEventArgs e)
    {
        var rows = Database.GetGames(CatalogSearchTextBox.Text);
        ExportCsv("katalog_game_slashers.csv", new[] { "ID", "Judul Game", "Platform", "Genre", "Harga Beli", "Harga Jual", "Stok", "CoverPath" },
            rows.Select(g => new object?[] { g.Id, g.Title, g.Platform, g.Genre, g.BuyPrice, g.SellPrice, g.Stock, g.CoverPath }));
    }

    private void ExportPartners_Click(object sender, RoutedEventArgs e)
    {
        var rows = Database.GetPartnerSummary();
        ExportCsv("mitra_toko_slashers.csv", new[] { "Nama Pihak", "Kategori", "Jumlah Transaksi", "Total Nilai" },
            rows.Select(p => new object?[] { p.Partner, p.Category, p.TransactionCount, p.TotalValue }));
    }

    private void ExportHistory_Click(object sender, RoutedEventArgs e)
    {
        var rows = Database.GetTransactions((HistoryFilterCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "SEMUA");
        ExportCsv("riwayat_transaksi_slashers.csv", new[] { "ID", "Jenis", "Nama Game", "Jumlah", "Harga Unit", "Pihak", "Total", "Tanggal", "Pembayaran", "Diskon Persen" },
            rows.Select(t => new object?[] { t.Id, t.Type, t.GameTitle, t.Quantity, t.UnitPrice, t.Partner, t.Total, t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), t.PaymentMethod, t.DiscountRate }));
    }

    private void ExportReport_Click(object sender, RoutedEventArgs e)
    {
        var rows = Database.GetTopSellingGames();
        ExportCsv("game_terlaris_slashers.csv", new[] { "Nama Game", "Unit Terjual", "Total Penjualan" },
            rows.Select(t => new object?[] { t.Title, t.Units, t.Total }));
    }

    private void ExportCsv(string defaultName, IEnumerable<string> headers, IEnumerable<object?[]> rows)
    {
        var dialog = new SaveFileDialog { Title = "Ekspor data ke CSV", FileName = defaultName, Filter = "CSV (*.csv)|*.csv|Semua file (*.*)|*.*", DefaultExt = ".csv" };
        if (dialog.ShowDialog(this) != true) return;
        try
        {
            using var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true));
            writer.WriteLine(string.Join(",", headers.Select(CsvEscape)));
            foreach (var row in rows) writer.WriteLine(string.Join(",", row.Select(x => CsvEscape(Convert.ToString(x, CultureInfo.InvariantCulture) ?? ""))));
            ShowInfo("File CSV berhasil diekspor ke:\n" + dialog.FileName, "Ekspor Berhasil");
        }
        catch (Exception ex) { ShowError("Ekspor CSV gagal.", ex); }
    }

    private static string CsvEscape(string value) => "\"" + value.Replace("\"", "\"\"") + "\"";
    private static string FormatCurrency(decimal amount) => "Rp " + amount.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
    private static string ShortCurrency(decimal amount) => amount >= 1_000_000m ? (amount / 1_000_000m).ToString("0.#", CultureInfo.GetCultureInfo("id-ID")) + " jt" : amount >= 1_000m ? (amount / 1_000m).ToString("0.#", CultureInfo.GetCultureInfo("id-ID")) + " rb" : amount.ToString("0", CultureInfo.GetCultureInfo("id-ID"));

    private void ShowInfo(string text, string title = "Slashers") => MessageBox.Show(this, text, title, MessageBoxButton.OK, MessageBoxImage.Information);
    private void ShowError(string text, Exception ex) => MessageBox.Show(this, text + "\n\nDetail: " + ex.Message, "Slashers — Error", MessageBoxButton.OK, MessageBoxImage.Error);

    private sealed record SalesBar(string Label, double BarHeight, string ShortTotal);
}
