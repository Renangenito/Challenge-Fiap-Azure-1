using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace app_sqlazure
{

    public class ClienteRepository : IClienteRepository
    {

        IConfiguration _configuration;
        public ClienteRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string GetConnection()
        {
            var conn = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            return conn;
        }
        int IClienteRepository.Add(Cliente cliente)
        {
            var connection = this.GetConnection();
            using (var cn = new SqlConnection(connection))
            {
                byte[] imagemBytes;

                using (var binaryReader = new BinaryReader(cliente.Imagem.OpenReadStream()))
                {
                    imagemBytes = binaryReader.ReadBytes((int)cliente.Imagem.Length);
                }

                var novo = cn.Execute("INSERT INTO TBClientes (Nome, Email, Imagem) VALUES (@Nome, @Email, @Imagem)",
                    new { cliente.Nome, cliente.Email, Imagem = imagemBytes }
                );

                return novo;
            }
        }

        IEnumerable<Cliente> IClienteRepository.GetAll()
        {
            var connection = this.GetConnection();
            using (var cn = new SqlConnection(connection))
            {
                var clientes = cn.Query<Cliente>("SELECT * FROM TBClientes");

                return clientes;
            }
        }
    }
}