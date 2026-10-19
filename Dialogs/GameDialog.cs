using System.IO;
using Microsoft.Win32;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Slashers.WPF.Models;

namespace Slashers.WPF.Dialogs;

public sealed class GameDialog : Window
{
    private readonly TextBox _title = new();
    private readonly TextBox _platform = new();
    private readonly TextBox _genre = new();
    private readonly TextBox _buyPrice = new();
    private readonly TextBox _sellPrice = new();
    private readonly TextBox _stock = new();
    private readonly Image _coverPreview = new();
    private string _coverPath = "";
    private bool _removeCover;
    private readonly GameItem? _original;
    public GameItem? ResultGame { get; private set; }

    public GameDialog(GameItem? game)
    {
        _original = game;
        _coverPath = game?.CoverPath ?? "";
        Title = game is null ? "Tambah Game — Slashers" : "Edit Game — Slashers";
        Width = 700; Height = 730; MinWidth = 620; MinHeight = 650;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
        Background = (Brush)Application.Current.FindResource("AppBackgroundBrush");
        Foreground = (Brush)Application.Current.FindResource("TextPrimaryBrush");
        BuildContent();
        if (game is not null)
        {
            _title.Text = game.Title; _platform.Text = game.Platform; _genre.Text = game.Genre;
            _buyPrice.Text = game.BuyPrice.ToString("0.##", CultureInfo.CurrentCulture);
            _sellPrice.Text = game.SellPrice.ToString("0.##", CultureInfo.CurrentCulture);
            _stock.Text = game.Stock.ToString(CultureInfo.CurrentCulture);
            SetPreview(_coverPath);
        }
    }

    private void BuildContent()
    {
        var root = new Grid { Margin = new Thickness(22) };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        var heading = new StackPanel { Margin = new Thickness(0, 0, 0, 14) };
        heading.Children.Add(new TextBlock { Text = _original is null ? "ADD TO GAME CATALOG" : "EDIT GAME DETAILS", FontFamily = new FontFamily("Segoe UI"), FontSize = 21, Foreground = (Brush)Application.Current.FindResource("AccentAmberBrush") });
        heading.Children.Add(new TextBlock { Text = "Informasi game, platform, harga, stok, dan cover", Foreground = (Brush)Application.Current.FindResource("MutedBrush"), Margin = new Thickness(0, 5, 0, 0) });
        Grid.SetRow(heading, 0); root.Children.Add(heading);

        var body = new Grid(); body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) }); body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        var imageColumn = new StackPanel { Margin = new Thickness(0, 4, 18, 0) };
        var imageFrame = new Border { Height = 250, Background = new SolidColorBrush(Color.FromRgb(23, 26, 34)), BorderBrush = (Brush)Application.Current.FindResource("AccentAmberBrush"), BorderThickness = new Thickness(1), Padding = new Thickness(6), Child = _coverPreview };
        _coverPreview.Stretch = Stretch.Uniform;
        imageColumn.Children.Add(imageFrame);
        var browse = MakeButton("Pilih gambar…", true); browse.Margin = new Thickness(0, 12, 0, 7); browse.Click += BrowseCover_Click;
        var remove = MakeButton("Hapus gambar", false); remove.Click += (_, _) => { _coverPath = ""; _removeCover = true; _coverPreview.Source = null; };
        imageColumn.Children.Add(browse); imageColumn.Children.Add(remove);
        imageColumn.Children.Add(new TextBlock { Text = "Format: PNG, JPG, JPEG, BMP, GIF atau TIFF. Gambar disalin ke folder data aplikasi setelah disimpan.", TextWrapping = TextWrapping.Wrap, Foreground = (Brush)Application.Current.FindResource("MutedBrush"), FontSize = 10, Margin = new Thickness(0, 11, 0, 0) });
        Grid.SetColumn(imageColumn, 0); body.Children.Add(imageColumn);

        var fields = new StackPanel();
        AddField(fields, "Judul Game", _title);
        AddField(fields, "Platform", _platform);
        AddField(fields, "Genre", _genre);
        var prices = new Grid(); prices.ColumnDefinitions.Add(new ColumnDefinition()); prices.ColumnDefinitions.Add(new ColumnDefinition());
        var buyStack = new StackPanel { Margin = new Thickness(0, 0, 8, 0) }; AddField(buyStack, "Harga Beli (Rp)", _buyPrice);
        var sellStack = new StackPanel(); AddField(sellStack, "Harga Jual (Rp)", _sellPrice);
        Grid.SetColumn(buyStack, 0); Grid.SetColumn(sellStack, 1); prices.Children.Add(buyStack); prices.Children.Add(sellStack);
        fields.Children.Add(prices);
        AddField(fields, "Stok Awal", _stock);
        Grid.SetColumn(fields, 1); body.Children.Add(fields);
        Grid.SetRow(body, 1); root.Children.Add(body);

        var footer = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 18, 0, 0) };
        var save = MakeButton("Simpan Game", true); save.Click += Save_Click;
        var cancel = MakeButton("Batal", false); cancel.Margin = new Thickness(0); cancel.Click += (_, _) => DialogResult = false;
        footer.Children.Add(save); footer.Children.Add(cancel); Grid.SetRow(footer, 2); root.Children.Add(footer);
        Content = root;
    }

    private static void AddField(Panel panel, string label, TextBox input)
    {
        panel.Children.Add(new Label { Content = label });
        input.Padding = new Thickness(9, 7, 9, 7); input.Margin = new Thickness(0, 0, 0, 6); input.MinWidth = 100;
        panel.Children.Add(input);
    }

    private static Button MakeButton(string text, bool primary) => new()
    {
        Content = text, Padding = new Thickness(13, 9, 13, 9), Margin = new Thickness(0, 0, 8, 0),
        Background = new SolidColorBrush(primary ? Color.FromRgb(21, 95, 88) : Color.FromRgb(40, 49, 64)),
        Foreground = (Brush)Application.Current.FindResource("TextPrimaryBrush"),
        BorderBrush = (Brush)Application.Current.FindResource("AccentAmberBrush"),
        BorderThickness = new Thickness(1), Cursor = System.Windows.Input.Cursors.Hand
    };

    private void BrowseCover_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Title = "Pilih gambar sampul game", Filter = "Gambar|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff|Semua file|*.*" };
        if (dialog.ShowDialog(this) == true)
        {
            _coverPath = dialog.FileName; _removeCover = false; SetPreview(_coverPath);
        }
    }

    private void SetPreview(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) { _coverPreview.Source = null; return; }
            var bitmap = new BitmapImage(); bitmap.BeginInit(); bitmap.CacheOption = BitmapCacheOption.OnLoad; bitmap.UriSource = new Uri(path); bitmap.EndInit(); bitmap.Freeze();
            _coverPreview.Source = bitmap;
        }
        catch { _coverPreview.Source = null; }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_title.Text) || string.IsNullOrWhiteSpace(_platform.Text))
        { MessageBox.Show(this, "Judul game dan platform wajib diisi.", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        if (!decimal.TryParse(_buyPrice.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var buy) || buy < 0 ||
            !decimal.TryParse(_sellPrice.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var sell) || sell < 0)
        { MessageBox.Show(this, "Harga beli dan jual harus berupa angka nol atau lebih.", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        if (!int.TryParse(_stock.Text, out var stock) || stock < 0)
        { MessageBox.Show(this, "Stok harus berupa bilangan bulat nol atau lebih.", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        ResultGame = new GameItem(_original?.Id ?? 0, _title.Text.Trim(), _platform.Text.Trim(), _genre.Text.Trim(), buy, sell, stock,
            _removeCover ? "" : _coverPath);
        DialogResult = true;
    }
}
