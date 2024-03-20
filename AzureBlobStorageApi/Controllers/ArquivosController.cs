using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorageApi.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Components.Route("[controller]")]
    public class ArquivosController:ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public ArquivosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("BlobConnectionString");
            _containerName = configuration.GetValue<string>("BlobContainerName");


        }
        //Metodo de Upload
        [HttpPost ("Upload")]
        public IActionResult UploadArquivo(IFormFile arquivo)
        {
            //Blob deve ser instalado no Nuget
            //Blob = Binary Large Object ( grande objeto binário, qualquer coisa video audio..)
            BlobContainerClient container = new(_connectionString, _containerName);
            BlobClient blob  = container.GetBlobClient(arquivo.FileName);


            using var data = arquivo.OpenReadStream();
            blob.Upload(data, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders {ContentType = arquivo.ContentType }

            });

            return Ok (blob.Uri.ToString ());
        }
        //Metodo de download

        [HttpGet("Dowload/{nome}")]
        public IActionResult DownloadArquivo (string nome)
        {
            //conexao
            BlobContainerClient container = new(_connectionString, _containerName);
            BlobClient blob = container.GetBlobClient(nome);

            //validacao
            if (!blob.Exists())//se o arquivo não existe
                return BadRequest();

            var retorno = blob.DownloadContent(); //realizar dowload
            return File(retorno.Value.Content.ToArray(), retorno.Value.Details.ContentType, blob.Name);

            //retornar os binarios do arquivo , tipo de conteudo o que o arquivo é , nome do arquiv

        }

        //Metodo para apagar arquivo

        [HttpDelete("Apagar/{nome}")]
        public IActionResult DeletarArquivo(string nome)
        {
            BlobContainerClient container = new(_connectionString, _containerName);//inicia conteiner;
            BlobClient blob = container.GetBlobClient(nome); //verifica se ele existe ou nao

            blob.DeleteIfExists();//usamos esse metodo entao nao precisamos verificar se ele existe
            return NoContent(); // sucesso e nao retorna nada

        }

        //Listar todos os blobs

        [HttpGet("Listar")]
        public IActionResult Listar()
        {

            List<BlobDto> blobsDto = new List<BlobDto>();
            BlobContainerClient container = new(_connectionString, _containerName);//inicia conteiner;

            foreach (var blob in container.GetBlobs())
            {
                blobsDto.Add(new BlobDto
                {
                    Nome = blob.Name,
                    Tipo = blob.Properties.ContentType,
                    Uri = container.Uri.AbsoluteUri + "/" + blob.Name
                });

            }
            return Ok(blobsDto);
        }

    }
}
