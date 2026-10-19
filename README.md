# Yharnam Exchange
### Sistem Informasi Manajemen Penjualan dan Pembelian Game Berbasis Desktop

Yharnam Exchange adalah aplikasi desktop untuk membantu pengelolaan katalog game, inventaris, pembelian stok, penjualan, dan laporan transaksi. Aplikasi ini mengusung antarmuka **Rich UI** dengan tema Gothic Victorian yang terinspirasi oleh suasana *Bloodborne*, memadukan warna hitam arang, merah darah, dan emas antik.

Proyek ini dikembangkan sebagai proyek ETS/UTS mata kuliah Pemrograman Berbasis Kerangka Kerja (PBKK).

---

## Daftar Isi

- [Latar Belakang](#latar-belakang)
- [Tujuan](#tujuan)
- [Fitur Aplikasi](#fitur-aplikasi)
- [Teknologi](#teknologi)
- [Desain Antarmuka](#desain-antarmuka)
- [Struktur Proyek](#struktur-proyek)
- [Persyaratan Sistem](#persyaratan-sistem)
- [Cara Instalasi dan Menjalankan Aplikasi](#cara-instalasi-dan-menjalankan-aplikasi)
- [Rancangan Pengujian](#rancangan-pengujian)
- [Dokumentasi](#dokumentasi)
- [Anggota Kelompok](#anggota-kelompok)
- [Pengembangan Selanjutnya](#pengembangan-selanjutnya)

## Latar Belakang

Pengelolaan toko game membutuhkan pencatatan katalog, harga, stok, pembelian barang, dan penjualan secara terstruktur. Apabila pencatatan dilakukan secara manual, risiko kesalahan data, ketidaksesuaian stok, serta kesulitan dalam menyusun laporan dapat meningkat.

Yharnam Exchange dikembangkan sebagai solusi desktop untuk mengintegrasikan pengelolaan inventaris dan transaksi ke dalam satu aplikasi. Antarmuka dirancang agar informasi mudah ditemukan, navigasi konsisten, dan proses operasional lebih terorganisasi.

## Tujuan

1. Menyediakan aplikasi desktop untuk mengelola katalog dan inventaris game.
2. Mempermudah pencatatan transaksi pembelian dan penjualan.
3. Membantu pemantauan stok serta identifikasi game yang memerlukan restock.
4. Menyajikan ringkasan aktivitas melalui dashboard dan laporan.
5. Menerapkan antarmuka Rich UI dengan desain yang konsisten dan mudah digunakan.
6. Menggunakan basis data relasional untuk mendukung penyimpanan data aplikasi.

## Fitur Aplikasi

Fitur yang dikembangkan atau direncanakan dalam proyek ini meliputi:

| Fitur | Deskripsi |
|---|---|
| Dashboard | Ringkasan katalog, stok, aktivitas transaksi, dan informasi inventaris. |
| Katalog Game | Menampilkan informasi game, platform, genre, harga, dan jumlah stok. |
| Manajemen Game | Menambah, melihat, mengubah, dan menghapus data game. |
| Gambar Cover | Memilih gambar cover untuk setiap game. |
| Pencarian | Membantu menemukan game berdasarkan informasi yang tersedia. |
| Pembelian Stok | Mencatat pembelian game dan mendukung penambahan stok. |
| Penjualan | Mencatat penjualan game dan mendukung pengurangan stok. |
| Riwayat Transaksi | Menampilkan catatan transaksi pembelian dan penjualan. |
| Stok Kritis | Membantu mengidentifikasi game dengan jumlah stok rendah. |
| Mitra Toko | Mendukung pencatatan informasi mitra yang terkait dengan operasional toko. |
| Laporan | Menyediakan ringkasan transaksi dan informasi penjualan. |
| Ekspor CSV | Mendukung ekspor data laporan untuk pengolahan lebih lanjut. |

Status setiap fitur perlu dikonfirmasi melalui pengujian aplikasi sebelum proyek dinyatakan final.

## Teknologi

- **Bahasa pemrograman:** C#
- **Framework antarmuka:** Windows Presentation Foundation (WPF)
- **Platform:** .NET 8
- **Bahasa markup antarmuka:** XAML
- **Basis data:** Microsoft SQL Server LocalDB
- **Lingkungan pengembangan:** Visual Studio
- **Sistem operasi target:** Windows

## Desain Antarmuka

Yharnam Exchange menggunakan pendekatan Rich UI dengan karakter visual Gothic Victorian.

Prinsip desain yang digunakan:

- **Charcoal Black:** latar belakang utama yang gelap.
- **Blood Red:** warna aksen untuk tindakan utama dan penekanan informasi.
- **Antique Gold:** warna highlight, judul, dan detail dekoratif.
- **Bone White:** warna teks untuk menjaga keterbacaan.
- **Tipografi serif:** memberi identitas klasik dan gothic.
- **Kartu informasi dan navigasi konsisten:** membantu pengguna berpindah antarfitur.
- **Katalog visual:** menampilkan cover game sebagai bagian dari informasi inventaris.

Desain tetap mengutamakan keterbacaan, konsistensi, dan kemudahan penggunaan, bukan hanya aspek dekoratif.

## Struktur Proyek

Struktur utama proyek mengikuti susunan berikut. Nama atau lokasi beberapa berkas dapat berbeda sesuai versi source code yang digunakan.

```text
YharnamExchangeWPF/
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── Data/
│   └── Database.cs
├── Dialogs/
│   └── GameDialog.cs
├── Assets/
├── YharnamExchange.WPF.csproj
└── README.md
```

Keterangan:

- `App.xaml`: resource dan gaya antarmuka global.
- `MainWindow.xaml`: struktur visual jendela utama.
- `MainWindow.xaml.cs`: logika interaksi antarmuka.
- `Data/Database.cs`: akses dan pengelolaan basis data.
- `Dialogs/`: formulir atau dialog untuk operasi tertentu.
- `Assets/`: aset visual aplikasi, jika digunakan pada versi proyek tersebut.
- `.csproj`: konfigurasi proyek .NET dan dependensi.

## Persyaratan Sistem

Sebelum menjalankan aplikasi, siapkan:

1. Komputer dengan sistem operasi Windows.
2. Visual Studio dengan workload **.NET desktop development**.
3. .NET 8 SDK yang sesuai dengan konfigurasi proyek.
4. Microsoft SQL Server Express LocalDB.
5. Source code proyek Yharnam Exchange.

## Cara Instalasi dan Menjalankan Aplikasi

### 1. Instalasi lingkungan

Pasang Visual Studio dan pilih workload `.NET desktop development`. Pastikan SQL Server Express LocalDB tersedia melalui Visual Studio Installer atau installer resmi Microsoft.

### 2. Buka proyek

Ekstrak source code, kemudian buka berkas `YharnamExchange.WPF.sln` melalui Visual Studio.

### 3. Pulihkan dependensi

Tunggu proses pemulihan paket selesai. Jika diperlukan, gunakan menu:

`Build → Restore NuGet Packages`

### 4. Periksa konfigurasi database

Periksa `Data/Database.cs` untuk memastikan string koneksi sesuai dengan lingkungan lokal. Konfigurasi LocalDB yang umum digunakan adalah:

```text
(localdb)\MSSQLLocalDB
```

Nama instance dan database harus mengikuti konfigurasi source code yang digunakan.

### 5. Build aplikasi

Pilih:

`Build → Rebuild Solution`

Pastikan tidak ada error build sebelum menjalankan aplikasi.

### 6. Jalankan aplikasi

Tekan `F5` untuk menjalankan aplikasi melalui Visual Studio.

Apabila inisialisasi database otomatis tersedia pada versi proyek yang digunakan, aplikasi akan membuat atau menyiapkan struktur data sesuai implementasinya. Jika tidak, database dan tabel perlu disiapkan sesuai kode serta konfigurasi proyek.

## Rancangan Pengujian

Pengujian dilakukan untuk memastikan fungsi aplikasi sesuai dengan kebutuhan yang telah ditetapkan.

| No. | Skenario | Hasil yang diharapkan |
|---|---|---|
| 1 | Membuka aplikasi | Jendela utama tampil tanpa error. |
| 2 | Membuka katalog game | Data katalog ditampilkan dengan benar. |
| 3 | Menambahkan game | Data baru tersimpan dan tampil pada katalog. |
| 4 | Mengubah data game | Perubahan tersimpan dengan benar. |
| 5 | Menghapus game | Data dihapus sesuai konfirmasi dan aturan aplikasi. |
| 6 | Memilih gambar cover | Gambar yang dipilih tampil pada game terkait. |
| 7 | Melakukan pembelian stok | Transaksi tercatat dan stok bertambah sesuai aturan. |
| 8 | Melakukan penjualan | Transaksi tercatat dan stok berkurang sesuai aturan. |
| 9 | Menjual melebihi stok | Transaksi ditolak atau ditangani sesuai validasi yang ditetapkan. |
| 10 | Membuka riwayat transaksi | Catatan transaksi ditampilkan sesuai data tersimpan. |
| 11 | Membuka laporan | Ringkasan laporan sesuai dengan data transaksi. |
| 12 | Mengekspor CSV | Berkas ekspor terbentuk dan isinya dapat dibaca. |

Tabel tersebut merupakan rencana pengujian. Status berhasil atau gagal harus diisi berdasarkan pengujian aktual.

## Dokumentasi

Dokumentasi visual dapat dilengkapi dengan screenshot dari aplikasi yang benar-benar dijalankan, antara lain:

- Dashboard utama.
- Katalog game.
- Form tambah dan edit game.
- Form transaksi pembelian dan penjualan.
- Riwayat transaksi.
- Halaman laporan.

Video demonstrasi dapat menampilkan alur penggunaan aplikasi, mulai dari membuka katalog, mengelola data, mencatat transaksi, hingga memeriksa perubahan stok dan laporan.

## Anggota Kelompok

| No. | Nama | NIM | Tanggung Jawab |
|---|---|---|---|
| 1 | Rafael Mulia Wiratama Simarmata | [Isi NIM] | [Isi tanggung jawab aktual] |
| 2 | Muhammad Rivaldi Sholihin | [Isi NIM] | [Isi tanggung jawab aktual] |
| 3 | Ananda Faris GR | [Isi NIM] | [Isi tanggung jawab aktual] |

Sesuaikan daftar anggota, NIM, dan pembagian tanggung jawab berdasarkan data kelompok yang telah disepakati.

## Pengembangan Selanjutnya

Pengembangan berikutnya dapat mencakup:

- Validasi input dan penanganan error yang lebih lengkap.
- Penyempurnaan laporan dan visualisasi data.
- Pengujian integritas transaksi serta stok.
- Pengelolaan pengguna dan autentikasi jika dibutuhkan.
- Penyempurnaan dokumentasi instalasi dan hasil pengujian.
- Penyesuaian akhir berdasarkan PRD dan ketentuan resmi dosen.

## Status Proyek

Yharnam Exchange merupakan proyek aplikasi desktop yang dikembangkan secara bertahap. Kesiapan akhir ditentukan setelah source code berhasil dibangun, aplikasi berjalan di lingkungan target, skenario pengujian dilaksanakan, dan persyaratan UTS yang berlaku dikonfirmasi.

---

**Yharnam Exchange — A Hunter's Ledger for Every Game.**
