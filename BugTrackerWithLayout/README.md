#  Hata Takip ve Yönetim Uygulaması (BugTrackerWithLayout)

Bu proje, yazılım geliştirme sürecinde karşılaşılan hataların kaydedilmesi, yönetilmesi ve çözüm süreçlerinin takip edilmesi amacıyla geliştirilmiş bir ASP.NET MVC tabanlı web uygulamasıdır.

## Özellikler

-  Hata ekleme, güncelleme ve silme
-  Hatalara çözüm ekleme
-  Kategori sistemi ile filtreleme
-  Giriş yapan kullanıcıya göre kişisel hata listesi
-  Kullanıcı girişi ve yetkilendirme (Admin/Kullanıcı)
-  Hatalara dosya/fotoğraf ekleme özelliği
-  Sayfalama (Pagination)
-  JSON API ile hata listesini çekme (GetBugs)

##  Kullanılan Teknolojiler

- ASP.NET MVC (.NET Framework)
- Entity Framework (Code First)
- SQL Server
- HTML / CSS / JavaScript
- Bootstrap
- LINQ

##  Veritabanı Yapısı

| Tablo Adı     | Açıklama                        |
|---------------|----------------------------------|
| `Bugs`        | Hataların tutulduğu ana tablo    |
| `Users`       | Giriş yapan kullanıcılar         |
| `Categories`  | Hata kategorileri                |
| `Solutions`   | Eklenen çözüm açıklamaları       |

Bu projeyi kendi bilgisayarınıza almak için:
git clone https://github.com/aysenurkucuk45/BugTrackerWithLayout.git
