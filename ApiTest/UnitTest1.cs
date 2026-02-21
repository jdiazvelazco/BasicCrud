using apiexamen;
using System.Net.Http.Json;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;



namespace ApiTest
{
    public class xUtClsExamen
    {
        private string _connectionString = "Server=DELLJDV\\SQLEXPRESS01;Database=bdiExamen;Trusted_Connection=True;TrustServerCertificate=True;";
        private string _apiUrl = "https://localhost:7137/wsApiExamen";
        private bool _useSPs = true;

        [Fact]
        public async Task AgregarExamenSatisfactorio()
        {
            int idExamen = -666;
            var cls = new ClsExamen(
                useSPs: _useSPs,
                connectionString: _connectionString,
                apiUrl: _apiUrl
            );
            try
            {
                var result = await cls.AgregarExamen(idExamen, "Tet01", "test02", CancellationToken.None);

                Assert.True(result.Procesado, $"Error: {result.Mensaje}");
                Assert.Equal("Registro insertado satisfactoriamente", result.Mensaje);
            }
            finally
            {
                await cls.EliminarExamen(idExamen, CancellationToken.None);
            }
        }

        [Fact]
        public async Task AgregarExamenErrorSinNombreSinDescripcion()
        {
            var cls = new ClsExamen(
                useSPs: _useSPs,
                connectionString: _connectionString,
                apiUrl: _apiUrl
            );

            var resultado = await cls.AgregarExamen(-667, "", "", CancellationToken.None);

            Assert.False(resultado.Procesado);
            Assert.Equal("El nombre y la descripción no pueden estar vacíos", resultado.Mensaje);
        }


        [Fact]
        public async Task ConsutlarExamen()
        {
            var cls = new ClsExamen(
                useSPs: _useSPs,
                connectionString: "Server=DELLJDV\\SQLEXPRESS01;Database=bdiExamen;Trusted_Connection=True;TrustServerCertificate=True;",
                apiUrl: "https://localhost:7137/wsApiExamen/"
            );
            var result = await cls.ConsultarExamen(0, "", "", CancellationToken.None);
            Assert.NotNull(result);
        }


        [Fact]
        public async Task ActualizarExamenSatisfactorio()
        {
            int idExamen = -666;
            var cls = new ClsExamen(
                useSPs: _useSPs,
                connectionString: _connectionString,
                apiUrl: _apiUrl
            );
            try
            {
                await cls.AgregarExamen(idExamen, "Tet01", "test02", CancellationToken.None);
                var result = await cls.ActualizarExamen(idExamen, "666", "666 666 666", CancellationToken.None);                
                Assert.True(result);
            }
            finally
            {
                await cls.EliminarExamen(idExamen, CancellationToken.None);
            }        
        }



        [Fact]
        public async Task ActualizarExamenErrorSinNombreSinDescripcion()
        {
            var cls = new ClsExamen(
                useSPs: _useSPs,
                connectionString: _connectionString,
                apiUrl: _apiUrl
            );

            var result = await cls.ActualizarExamen(-667, "", "", CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task EliminarExamenSatisfactorio()
        {
            int idExamen = -666;
            var cls = new ClsExamen(
                useSPs: _useSPs,
                connectionString: _connectionString,
                apiUrl: _apiUrl
            );
            try
            {
                await cls.AgregarExamen(idExamen, "BORRAR1", "BORRAR 1", CancellationToken.None);
            }
            finally
            {
                var result = await cls.EliminarExamen(idExamen, CancellationToken.None);
                Assert.True(result);
            }

        }

        [Fact]
        public async Task EliminarExamenNoExiste()
        {
            var cls = new ClsExamen(
                useSPs: _useSPs,
                connectionString: _connectionString,
                apiUrl: _apiUrl
            );

            var result = await cls.EliminarExamen(-667, CancellationToken.None);

            Assert.False(result);
        }


    }
}