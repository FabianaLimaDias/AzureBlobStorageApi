using System.Reflection.Metadata.Ecma335;

namespace AzureBlobStorageApi
{
    //Classe simplificada para retorno de API ( não tem logica para ser realizada)
    public class BlobDto
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string  Uri { get; set; }
    }
}
