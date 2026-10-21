# Slashers — Game Store Management

**Slashers** adalah aplikasi desktop Rich UI untuk mengelola katalog game, inventaris, transaksi pembelian dan penjualan, riwayat transaksi, stok kritis, mitra, serta laporan. Identitas visual menggunakan logo pixel-block berbentuk huruf **S** yang tersusun dari blok modular.

> Proyek ETS PBKK ini mencakup PRD, implementasi aplikasi desktop, dan video presentasi/demo. Nama Slashers adalah nama produk dan tidak membatasi katalog hanya pada game bergenre slasher.

## 1. Ringkasan Produk

**Slashers** adalah Aplikasi membantu pencatatan katalog game dan pergerakan stok melalui antarmuka Windows desktop. Pengguna dapat mengelola informasi game, mencatat pembelian dan penjualan, menelusuri transaksi, memantau stok kritis, melihat laporan, dan mengekspor data ke CSV.

Antarmuka menggunakan pendekatan Rich UI: sidebar navigasi, kartu statistik, grafik penjualan, katalog berbasis cover, panel detail, formulir, tabel transaksi, dan halaman laporan. Gaya visual memakai graphite `#171A22`, slate `#202631`, teal `#20BFA9`, amber `#FFC857`, coral `#FF647C`, dan off-white `#F0F2F5`.

## 2. Fitur yang Diimplementasikan

- **Dashboard:** jumlah game, total unit stok, penjualan hari ini, jumlah game dengan stok kritis, penjualan per hari selama tujuh hari, dan transaksi terbaru.
- **Katalog:** tambah, lihat, edit, hapus, pencarian judul/platform/genre, cover, harga beli, harga jual, genre, platform, stok, dan ekspor CSV.
- **Cover game:** memilih gambar berformat PNG, JPG/JPEG, BMP, GIF, atau TIFF. Gambar yang dipilih disalin ke direktori data lokal Slashers agar tidak bergantung pada lokasi asli file.
- **Pembelian stok:** mencatat game, kuantitas, harga unit, serta pemasok. Jika transaksi berhasil, stok bertambah dan catatan transaksi disimpan.
- **Penjualan:** mencatat game, kuantitas, harga unit, pelanggan, diskon, dan metode pembayaran. Penjualan melebihi stok ditolak.
- **Riwayat transaksi:** menampilkan transaksi pembelian/penjualan, mendukung filter jenis dan ekspor CSV.
- **Stok kritis:** menampilkan game dengan stok lima unit atau kurang.
- **Mitra toko:** merangkum pihak pelanggan/pemasok yang tercatat pada transaksi.
- **Laporan:** ringkasan total penjualan/pembelian dan game terlaris, dengan ekspor CSV untuk data yang didukung.

## 3. Teknologi

| Komponen | Pilihan |
|---|---|
| Bahasa | C# |
| UI | WPF dan XAML |
| Target framework | .NET 8 untuk Windows (`net8.0-windows`) |
| Database | Microsoft SQL Server LocalDB |
| Provider SQL | `Microsoft.Data.SqlClient` |
| IDE | Visual Studio untuk Windows |

**Slashers tidak menggunakan XAMPP/MySQL.** Untuk konfigurasi default, aplikasi menggunakan SQL Server Express LocalDB.

## 4. Struktur Repository

```text
Slashers.WPF/  (root repository setelah isi ZIP diekstrak dan diunggah)
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── Models/
│   └── Models.cs
├── Data/
│   └── Database.cs
├── Dialogs/
│   └── GameDialog.cs
├── Assets/
│   ├── Slashers_AppIcon.png
│   ├── Slashers_Logo_Primary.png
│   ├── Slashers_Logo_Monochrome.png
│   ├── Slashers_Logo_LightBackground.png
│   └── Slashers.ico
├── docs/
│   ├── PRD_Slashers_UTS_PBKK.pdf
│   ├── PRD_Slashers_UTS_PBKK.docx
│   └── IMPLEMENTATION_QA.md
├── Slashers.WPF.csproj
├── Slashers.WPF.sln
├── GITHUB_UPLOAD_GUIDE.md
└── README.md
