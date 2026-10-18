# GameVault — Desktop Game Store Management

GameVault adalah starter project aplikasi desktop C# untuk tugas PBKK UTS. Aplikasi ini membantu toko game mengelola katalog, stok, transaksi pembelian/penjualan, dan ringkasan laporan. UI memakai tema dark navy dengan aksen ungu/mint, katalog berbentuk kartu visual, serta halaman data yang bisa difilter dan diekspor. Layout dashboard, laporan, header halaman, dan toolbar katalog menyesuaikan lebar jendela agar ruang layar dimanfaatkan lebih baik pada ukuran monitor dan skala Windows yang berbeda.

## Teknologi
- C# / .NET 8
- Windows Forms
- SQLite melalui `Microsoft.Data.Sqlite`

## Cara menjalankan
1. Install Visual Studio 2022 yang mendukung .NET 8 (atau Visual Studio versi lebih baru) dan workload **.NET desktop development**.
2. Ekstrak ZIP project.
3. Buka `GameVault.sln` melalui **Open a project or solution**.
4. Tunggu pemulihan paket NuGet selesai.
5. Jalankan dengan tombol **F5**.

**Jika Visual Studio menampilkan “debug executable ... does not exist”**, itu biasanya karena build sebelumnya gagal sehingga file `.exe` belum dibuat. Buka **View → Error List**, perbaiki semua baris berikon merah, lalu pilih **Build → Rebuild Solution** sebelum menekan F5 lagi. Gunakan source code dari ZIP terbaru ini.

Database dibuat otomatis di `%LOCALAPPDATA%\GameVault\GameVault.db`. Saat database kosong, aplikasi mengisi beberapa game contoh agar dashboard dapat dicoba. Data transaksi dan katalog tersimpan lokal pada komputer yang menjalankan aplikasi.

## Fitur

### Dashboard
- Jumlah judul game dan total unit stok.
- Nilai penjualan hari ini dan jumlah stok yang perlu diperiksa.
- Grafik penjualan tujuh hari terakhir.
- Daftar stok menipis dan transaksi terbaru.
- Tombol aksi cepat untuk penjualan dan restock.

### Katalog game
- Tambah, edit, dan hapus data game.
- Kartu katalog visual dengan warna cover ilustratif berdasarkan platform.
- Informasi platform, genre, harga beli, harga jual, estimasi margin per unit, dan stok.
- Pencarian judul/genre/platform.
- Filter platform, kondisi stok, dan urutan harga/stok.
- Ekspor katalog yang sedang difilter ke CSV.

### Pembelian dan penjualan
- Transaksi pembelian menambah stok, sedangkan penjualan mengurangi stok.
- Validasi jumlah dan stok tersedia.
- Diskon dan pilihan metode pembayaran pada penjualan.
- Pengisian nama pelanggan/supplier dengan saran dari riwayat transaksi sebelumnya.
- Harga total dihitung otomatis.

### Riwayat transaksi
- Filter jenis transaksi.
- Pencarian judul game, pelanggan, atau supplier.
- Filter berdasarkan tanggal tertentu.
- Ringkasan nilai transaksi berdasarkan hasil filter.
- Ekspor hasil yang ditampilkan ke CSV.

### Stok kritis, mitra, dan laporan
- Monitor produk dengan stok lima unit atau kurang.
- Tombol restock dari daftar stok kritis.
- Ringkasan pelanggan/supplier yang berasal dari transaksi tersimpan.
- Laporan game terlaris dan inventaris yang perlu diperhatikan.
- Ekspor data dan laporan ke CSV.

## Catatan perhitungan
- Margin per unit pada katalog adalah `harga jual - harga beli`.
- Estimasi margin kotor pada laporan memakai **harga beli pada katalog saat ini** untuk menghitung biaya unit. Jika harga beli berubah, estimasi transaksi lama dapat ikut berubah. Ini bukan laba akuntansi final dan belum mencakup biaya operasional.
- Diskon penjualan tercatat di transaksi dan total transaksi sudah mencerminkan diskon.
- File CSV memakai UTF-8 agar teks Indonesia lebih mudah dibaca di aplikasi spreadsheet.

## Uji alur demo yang disarankan
1. Buka Katalog Game dan cari salah satu judul.
2. Tambah game contoh baru; tunjukkan margin per unit pada form.
3. Catat pembelian stok, lalu cek jumlah stok bertambah.
4. Catat penjualan dengan diskon dan metode pembayaran, lalu cek stok berkurang.
5. Buka Riwayat Transaksi, cari transaksi, dan coba filter tanggal.
6. Buka Stok Kritis dan Laporan.
7. Ekspor katalog atau riwayat transaksi ke CSV.

## Catatan pengembangan UTS
Starter project ini dapat dikembangkan lebih lanjut berdasarkan PRD dan ketentuan dosen, seperti validasi tambahan, autentikasi pengguna, foto cover resmi, atau desain laporan yang lebih lengkap. Pastikan framework/database yang dipakai sudah sesuai instruksi dosen sebelum menjadikan versi ini sebagai versi final.
