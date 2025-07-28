# Mekansal Veri API Projesi

Bu proje, .NET 8 ve PostgreSQL/PostGIS veritabanı kullanarak geliştirilmiş bir mekansal (geospatial) veri yönetim API'sidir. Proje, harita üzerinde geometrik şekillerin (Nokta, Poligon vb.) oluşturulmasını, listelenmesini ve yönetilmesini sağlar.

Projenin en dikkat çekici özelliği, **yeni bir poligon eklendiğinde, var olan ve bu yeni poligonla kesişen diğer poligonlardan, kesişim alanını otomatik olarak çıkarmasıdır.** Bu, NetTopologySuite kütüphanesinin "fark alma" (difference) operasyonu ile gerçekleştirilir.

## 🚀 Temel Özellikler

*   **CRUD Operasyonları:** Mekansal nesneler için temel Ekleme, Okuma, Güncelleme ve Silme işlemleri.
*   **WKT Desteği:** `POINT (X Y)`, `POLYGON ((...))` gibi standart WKT (Well-Known Text) formatında geometrileri kabul eder ve döndürür.
*   **Akıllı Kesişim Yönetimi:** Bir poligon eklediğinizde, kesiştiği diğer poligonlar otomatik olarak "kırpılır" veya tamamen silinir.
*   **Katmanlı Mimari:** Proje, Controller, Service, Unit of Work ve Repository katmanları kullanılarak temiz ve sürdürülebilir bir yapıda tasarlanmıştır.
*   **Standart API Yanıtları:** Tüm API cevapları, istemci tarafında kolayca işlenebilmesi için `ApiResponse<T>` adlı standart bir model ile sarmalanmıştır.

## 🛠️ Kullanılan Teknolojiler

### Backend
*   **.NET 8** (ASP.NET Core Web API)
*   **Entity Framework Core 7**
*   **PostgreSQL** (Veritabanı)
*   **PostGIS** (PostgreSQL için mekansal veri eklentisi)
*   **NetTopologySuite** (Mekansal veri tipleri ve operasyonları için)
*   **AutoMapper** (DTO ve Model dönüşümleri için)

### Frontend (`forntend` klasörü)
*   **React** (Veya kullandığınız başka bir kütüphane)
*   **Vite** (Geliştirme sunucusu)
*   **OpenLayers** (Harita için)

## 🏁 Başlarken (Kurulum ve Çalıştırma)

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyin.

### Gereksinimler
*   [Git](https.git-scm.com/)
*   [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
*   [Node.js ve npm](https://nodejs.org/en/)
*   [PostgreSQL](https://www.postgresql.org/download/)
*   **PostGIS Eklentisi:** PostgreSQL kurulumundan sonra PostGIS eklentisini kurmanız **zorunludur**. [Kurulum rehberi](https://postgis.net/install/).

### Kurulum Adımları

1.  **Projeyi Klonlayın:**
    ```bash
    git clone https://github.com/kullanici-adiniz/proje-adiniz.git
    cd proje-adiniz
    ```

2.  **Backend'i Ayarlayın:**
    *   `WebApplication6` klasöründeki `appsettings.Development.json` dosyasını açın.
    *   `ConnectionStrings` bölümündeki veritabanı bağlantı bilgilerinizi kendi PostgreSQL kullanıcı adı ve şifrenizle güncelleyin. Veritabanınızın adını da burada belirtebilirsiniz.
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Host=localhost;Database=MekansalDB;Username=postgres;Password=sifreniz"
        }
        ```
    *   Veritabanı tablolarını oluşturmak için **EF Core Migrations**'ı çalıştırın:
        ```bash
        cd WebApplication6
        dotnet ef database update
        ```

3.  **Frontend'i Ayarlayın:**
    *   `forntend` klasörüne gidin ve bağımlılıkları yükleyin.
        ```bash
        cd ../forntend
        npm install
        ```

4.  **Uygulamayı Çalıştırın:**
    *   **Backend'i çalıştırmak için** (`WebApplication6` klasöründeyken):
        ```bash
        dotnet run
        ```
        API `https://localhost:7001` (veya benzeri) bir adreste çalışmaya başlayacaktır.

    *   **Frontend'i çalıştırmak için** (yeni bir terminalde, `forntend` klasöründeyken):
        ```bash
        npm run dev
        ```
        Frontend uygulaması `http://localhost:5173` adresinde açılacaktır.

## 📡 API Uç Noktaları (Endpoints)

| Metot | URL                  | Açıklama                                   |
| :---- | :------------------- | :----------------------------------------- |
| `GET` | `/api/Features`      | Tüm mekansal nesneleri listeler.           |
| `GET` | `/api/Features/{id}` | Belirtilen ID'ye sahip nesneyi getirir.    |
| `POST`| `/api/Features`      | Yeni bir mekansal nesne ekler.             |
| `PUT` | `/api/Features/{id}` | Var olan bir nesneyi günceller.            |
| `DELETE`| `/api/Features/{id}` | Belirtilen ID'ye sahip nesneyi siler.      |

### Örnek `POST` İsteği
Yeni bir poligon eklemek için `POST /api/Features` adresine aşağıdaki gibi bir JSON gönderebilirsiniz:

```json
{
  "name": "Ankara Kalesi Bölgesi",
  "locationWkt": "POLYGON ((32.86 39.94, 32.87 39.94, 32.87 39.93, 32.86 39.93, 32.86 39.94))"
}
```
