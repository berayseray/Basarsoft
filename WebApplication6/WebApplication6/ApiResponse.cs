// Bu s�n�f, API'den d�nen t�m cevaplar� standart bir yap�da tutar.
public class ApiResponse<T>
{
    public T Data { get; set; }      // D�nen as�l veri (bir Point, liste vb.)
    public string Message { get; set; } // ��lemle ilgili mesaj
}