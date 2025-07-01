// Bu sınıf, API'den dönen tüm cevapları standart bir yapıda tutar.
public class ApiResponse<T>
{
    public T Data { get; set; }      // Dönen asıl veri (bir Point, liste vb.)
    public string Message { get; set; } // İşlemle ilgili mesaj
}