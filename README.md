# Slashers — Game Store Management

**Slashers** adalah aplikasi desktop berbasis Rich UI untuk mengelola katalog game, inventaris, pembelian stok, penjualan, riwayat transaksi, dan laporan. Aplikasi menggunakan identitas visual pixel-block dengan warna graphite, teal, amber, dan coral.

Proyek ini dikembangkan untuk memenuhi tugas UTS mata kuliah **Pemrograman Berbasis Kerangka Kerja (PBKK)**, yang mencakup Product Requirements Document (PRD), implementasi aplikasi desktop, serta video presentasi/demo.

## Fitur Utama

- **Dashboard:** ringkasan jumlah game, total stok, penjualan, grafik, dan aktivitas terbaru.
- **Katalog Game:** tambah, lihat, ubah, hapus, dan cari data game.
- **Gambar Cover:** memilih gambar cover untuk setiap game.
- **Pembelian Stok:** mencatat pembelian dan menambah stok.
- **Penjualan:** mencatat transaksi, menghitung total, dan memperbarui stok.
- **Riwayat Transaksi:** melihat dan memfilter catatan pembelian serta penjualan.
- **Stok Kritis:** memantau game dengan stok lima unit atau kurang.
- **Mitra Toko:** melihat informasi pelanggan dan pemasok dari transaksi.
- **Laporan:** melihat ringkasan transaksi dan game terlaris.
- **Ekspor CSV:** menyimpan data yang didukung ke file CSV.

## Teknologi

| Komponen | Teknologi |
|---|---|
| Bahasa pemrograman | C# |
| Framework UI | WPF dan XAML |
| Platform | .NET 8 |
| Database | Microsoft SQL Server LocalDB |
| Akses database | Microsoft.Data.SqlClient |
| IDE | Visual Studio untuk Windows |

**Catatan:** aplikasi menggunakan SQL Server LocalDB, bukan MySQL/MariaDB dari XAMPP.

## Struktur Proyek

```text
Slashers.WPF/
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
└── README.md
```

## Persyaratan Sistem

Sebelum menjalankan aplikasi, pastikan komputer sudah memiliki:

1. Windows.
2. Visual Studio dengan workload **.NET desktop development**.
3. .NET 8 SDK.
4. Microsoft SQL Server Express LocalDB.

## Cara Menjalankan Aplikasi

### 1. Unduh source code

Unduh repository melalui tombol **Code → Download ZIP**, kemudian ekstrak file tersebut ke folder lokal.

### 2. Buka proyek

Buka file `Slashers.WPF.sln` menggunakan Visual Studio.

### 3. Pastikan database tersedia

Pastikan SQL Server Express LocalDB sudah terpasang. Instance default yang digunakan adalah:

```text
(localdb)\MSSQLLocalDB
```

Pada konfigurasi default, aplikasi akan mencoba membuat database `Slashers`, tabel `Games` dan `Transactions`, serta data game contoh saat inisialisasi pertama.

### 4. Build aplikasi

Di Visual Studio, pilih:

`Build → Rebuild Solution`

Tunggu hingga proses build selesai. Jika tidak terdapat error build, lanjutkan ke langkah berikutnya.

### 5. Jalankan aplikasi

Tekan **F5** untuk menjalankan Slashers.

Jika koneksi database gagal, periksa instalasi LocalDB dan konfigurasi connection string pada `Data/Database.cs`.

## Alur Demo

Urutan yang disarankan untuk mencoba aplikasi:

1. Buka dashboard dan amati ringkasan inventaris.
2. Buka katalog, cari game, lalu lihat detail dan cover.
3. Tambahkan atau edit game dan simpan perubahan.
4. Catat pembelian stok dan periksa penambahan stok.
5. Catat penjualan dan periksa pengurangan stok.
6. Coba menjual melebihi stok yang tersedia untuk menguji validasi.
7. Buka riwayat transaksi, stok kritis, dan laporan.
8. Ekspor data ke CSV jika diperlukan.

## Dokumentasi

- **PRD:** `docs/PRD_Slashers_UTS_PBKK.pdf`
- **PRD yang dapat diedit:** `docs/PRD_Slashers_UTS_PBKK.docx`
- **Checklist pengujian:** `docs/IMPLEMENTATION_QA.md`

Video presentasi/demo dapat ditambahkan ke repository setelah selesai direkam dan diunggah.

## Anggota Kelompok

| No. | Nama | NIM |
|---|---|---|
| 1 | Rafael Mulia Wiratama Simarmata | 5025241196 |
| 2 | Muhammad Rivaldi Sholihin | 5025241188 |
| 3 | Ananda Faris GR | Isi NIM |

## Status Proyek

Slashers dikembangkan sebagai proyek UTS PBKK. Pengujian fungsional, dokumentasi, dan demonstrasi dilakukan untuk memastikan implementasi sesuai dengan kebutuhan pada PRD.

Fitur yang dinyatakan selesai harus berdasarkan hasil pengujian aplikasi, bukan hanya keberadaan kode atau tampilan antarmuka.

---

**Slashers — Your collection. Fully in control.**
