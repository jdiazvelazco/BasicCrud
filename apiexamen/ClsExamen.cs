using System.Data;
using System.Net.Http.Json;
using Microsoft.Data.SqlClient;
using Modelos;

namespace apiexamen
{
    public class ClsExamen
    {
        private bool _useSPs;
        private string _connectionString;
        private string _apiUrl;

        public ClsExamen(bool useSPs, string connectionString, string apiUrl)
        {
            _useSPs = useSPs;
            _connectionString = connectionString;
            _apiUrl = apiUrl;
        }

        public async Task<(bool Procesado, string Mensaje)> AgregarExamen(int id, string nombre, string descripcion, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(descripcion))
            {
                return (false, "El nombre y la descripción no pueden estar vacíos");
            }
            if (_useSPs)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = new SqlCommand("spAgregar", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Id", id);
                                command.Parameters.AddWithValue("@Nombre", nombre);
                                command.Parameters.AddWithValue("@Descripcion", descripcion);
                                int codigoRetorno = 1;
                                string descripcionRetorno = "No se recibió respuesta del SP";
                                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                                {
                                    if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                    {
                                        codigoRetorno = reader.GetInt32(reader.GetOrdinal("CodigoRetorno"));
                                        descripcionRetorno = reader.GetString(reader.GetOrdinal("DescripconRetorno"));
                                    }
                                }
                                if (codigoRetorno == 0)
                                    transaction.Commit();
                                return (codigoRetorno == 0, descripcionRetorno);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return (false, ex.Message + " InnerException:" + ex.InnerException?.Message);
                        }
                    }
                }
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync($"{_apiUrl}/AgregarExamen?id={id}&nombre={nombre}&descripcion={descripcion}", null, cancellationToken).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                        return (true, "");
                    else
                        return (false, await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
            }
        }


        public async Task<bool> ActualizarExamen(int id, string nombre, string descripcion, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(descripcion))
            {
                return (false);
            }
            if (_useSPs)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = new SqlCommand("spActualizar", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Id", id);
                                command.Parameters.AddWithValue("@Nombre", nombre);
                                command.Parameters.AddWithValue("@Descripcion", descripcion);
                                int codigoRetorno = 1;
                                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                                {
                                    if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                    {
                                        codigoRetorno = reader.GetInt32(reader.GetOrdinal("CodigoRetorno"));
                                    }
                                }
                                if (codigoRetorno == 0)
                                    transaction.Commit();
                                return (codigoRetorno == 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return (false);
                        }
                    }
                }
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PutAsync($"{_apiUrl}/ActualizarExamen?id={id}&nombre={nombre}&descripcion={descripcion}", null, cancellationToken).ConfigureAwait(false);
                    return (response.IsSuccessStatusCode);
                }
            }
        }

        public async Task<bool> EliminarExamen(int id, CancellationToken cancellationToken)
        {
            if (_useSPs)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = new SqlCommand("spEliminar", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Id", id);
                                int codigoRetorno = 1;
                                string descripcionRetorno = "";
                                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                                {
                                    if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                    {
                                        codigoRetorno = reader.GetInt32(reader.GetOrdinal("CodigoRetorno"));
                                        descripcionRetorno = reader.GetString(reader.GetOrdinal("DescripconRetorno"));
                                    }
                                }
                                if (codigoRetorno == 0)
                                    transaction.Commit();
                                return (codigoRetorno == 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return (false);
                        }
                    }
                }
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Delete, $"{_apiUrl}/EliminarExamen?id={id}");
                    var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    return (response.IsSuccessStatusCode);
                }
            }
        }

        public async Task<IEnumerable<Examen>> ConsultarExamen(int? id, string nombre, string descripcion, CancellationToken cancellationToken)
        {
            var examenes = new List<Examen>();
            if (_useSPs)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    try
                    {
                        using (var command = new SqlCommand("spConsultar", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Id", id);
                            command.Parameters.AddWithValue("@Nombre", nombre);
                            command.Parameters.AddWithValue("@Descripcion", descripcion);

                            using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                            {
                                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                {
                                    examenes.Add(new Examen
                                    {
                                        IdExamen = reader.GetInt32(reader.GetOrdinal("IdExamen")),
                                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                        Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"))
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiUrl}/ConsultarExamen?id={id}&nombre={nombre}&descripcion={descripcion}", cancellationToken).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<IEnumerable<Examen>>(cancellationToken).ConfigureAwait(false);
                        return result ?? Enumerable.Empty<Examen>();
                    }
                }
            }
            return examenes;
        }
    }
}
