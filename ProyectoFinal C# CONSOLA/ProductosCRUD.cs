using System;
using System.Data.SqlClient;

namespace ProyectoFinal_C__CONSOLA
{
    internal class ProductosCRUD
    {
        static string connectionString = "Server=localhost\\SQLEXPRESS;Database=ProyectoRestaurante;Trusted_Connection=True;";

        // Método para insertar un producto
        public static void InsertarProducto()
        {
            Console.Write("Ingrese el nombre del producto: ");
            string nombre = Console.ReadLine();

            Console.Write("Ingrese el precio del producto: ");
            decimal precio = Convert.ToDecimal(Console.ReadLine());

            string query = "INSERT INTO Productos (Nombre, Precio) VALUES (@Nombre, @Precio)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Precio", precio);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Producto insertado correctamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al insertar producto: " + ex.Message);
                }
            }
        }

        // Método para mostrar productos
        public static void MostrarProductos()
        {
            string query = "SELECT * FROM Productos";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    Console.WriteLine("\nProductos en la base de datos:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["Id"]}, Nombre: {reader["Nombre"]}, Precio: {reader["Precio"]}");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al mostrar productos: " + ex.Message);
                }
            }
        }
    }
}
