# Slashers — Checklist Verifikasi Implementasi

Dokumen ini digunakan setelah source code dibuka dan dijalankan di Windows. Tandai hasil hanya sesudah tes dilakukan.

| ID | Langkah | Hasil yang diharapkan | Status | Catatan |
|---|---|---|---|---|
| QA-01 | Build → Rebuild Solution | Tidak ada error build | Belum diuji | |
| QA-02 | Jalankan aplikasi dengan LocalDB aktif | Dashboard terbuka | Belum diuji | |
| QA-03 | Tambah game dengan data valid | Game muncul di katalog | Belum diuji | |
| QA-04 | Tambah game dengan judul kosong | Validasi menolak input | Belum diuji | |
| QA-05 | Pilih cover dan simpan | Cover tetap tampil setelah katalog dimuat ulang | Belum diuji | |
| QA-06 | Edit judul/harga game | Perubahan tersimpan | Belum diuji | |
| QA-07 | Hapus game tanpa transaksi | Game dihapus setelah konfirmasi | Belum diuji | |
| QA-08 | Hapus game yang punya transaksi | Penghapusan ditolak untuk menjaga riwayat | Belum diuji | |
| QA-09 | Beli 2 unit | Stok bertambah 2; pembelian dicatat | Belum diuji | |
| QA-10 | Jual 1 unit dari stok cukup | Stok berkurang 1; penjualan dicatat | Belum diuji | |
| QA-11 | Jual lebih banyak dari stok | Transaksi ditolak; stok tidak berubah | Belum diuji | |
| QA-12 | Masukkan diskon di atas 100% | Validasi menolak transaksi | Belum diuji | |
| QA-13 | Filter riwayat per jenis | Data sesuai filter | Belum diuji | |
| QA-14 | Buka stok kritis | Game dengan stok ≤ 5 ditampilkan | Belum diuji | |
| QA-15 | Buka laporan dan game terlaris | Angka sama dengan data transaksi | Belum diuji | |
| QA-16 | Ekspor katalog/riwayat/laporan | CSV tersimpan dan dapat dibuka | Belum diuji | |
| QA-17 | Jalankan tanpa LocalDB | Pesan error informatif tampil | Belum diuji | |
| QA-18 | Ubah ukuran jendela | Konten tidak saling menutupi pada ukuran minimum | Belum diuji | |

## Bug log

| Tanggal | ID tes | Gejala | Pesan error | Perbaikan | Status |
|---|---|---|---|---|---|
| | | | | | |
