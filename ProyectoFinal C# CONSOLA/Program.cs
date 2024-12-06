using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

class Program
{
    // Ajusta tu cadena de conexión según tu entorno
    static string connectionString = "Server=localhost\\SQLEXPRESS;Database=ProyectoRestaurante;Trusted_Connection=True;";

    static void Main(string[] args)
    {
        if (IniciarSesion())
        {
            MostrarMenuPrincipal();
        }
        else
        {
            Console.WriteLine("Presione una tecla para salir...");
            Console.ReadKey();
        }
    }

    static bool IniciarSesion()
    {
        Console.WriteLine("=== Inicio de Sesión ===");
        Console.Write("Usuario: ");
        string usuario = Console.ReadLine();

        Console.Write("Contraseña: ");
        string contrasena = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
        {
            Console.WriteLine("Por favor ingrese ambos campos.");
            return false;
        }

        if (AutenticarUsuario(usuario, contrasena, out int empleadoId, out string actividad, out string puesto))
        {
            Console.WriteLine("¡Felicidades! Lograste entrar.");
            Console.WriteLine("ID Empleado: " + empleadoId);
            Console.WriteLine("Actividad: " + actividad);
            Console.WriteLine("Puesto: " + puesto);
            return true;
        }
        else
        {
            Console.WriteLine("No se ha podido acceder. Credenciales inválidas.");
            return false;
        }
    }

    static bool AutenticarUsuario(string usuario, string contrasena, out int empleadoId, out string actividad, out string puesto)
    {
        empleadoId = 0;
        actividad = string.Empty;
        puesto = string.Empty;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            string query = "SELECT id, actividad, puesto FROM empleados WHERE usuario = @usuario AND contrasena = @contrasena";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@contrasena", EncriptarContrasena(contrasena));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        empleadoId = reader.GetInt32(0);
                        actividad = reader.GetString(1);
                        puesto = reader.GetString(2);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Método para encriptar la contraseña utilizando SHA256
    static string EncriptarContrasena(string contrasena)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Menú principal que se muestra después del login
    static void MostrarMenuPrincipal()
    {
        int opcion = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("=== Menú Principal ===");
            Console.WriteLine("1. CRUD Clientes");
            Console.WriteLine("2. CRUD Productos");
            Console.WriteLine("3. Salir");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Opción inválida. Intente de nuevo.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    MenuCrudClientes(); // Aquí se llama al menú del CRUD Clientes
                    break;
                case 2:
                    MenuCrudProductos();
                    break;
                case 3:
                    Console.WriteLine("Saliendo del programa...");
                    break;
                default:
                    Console.WriteLine("Opción inválida. Intente de nuevo.");
                    break;
            }

        } while (opcion != 3);

        Console.WriteLine("Presione una tecla para cerrar...");
        Console.ReadKey();
    }

    // Aquí integras el menú CRUD de Clientes con el código que proporcionaste
    static void MenuCrudClientes()
    {
        int opcion = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("=== CRUD de Clientes ===");
            Console.WriteLine("1. Insertar Cliente");
            Console.WriteLine("2. Mostrar Clientes");
            Console.WriteLine("3. Actualizar Cliente");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");
            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Opción no válida");
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
                continue;
            }

            switch (opcion)
            {
                case 1:
                    InsertarCliente();
                    break;
                case 2:
                    MostrarClientes();
                    break;
                case 3:
                    ActualizarCliente();
                    break;
                case 4:
                    Console.WriteLine("Volviendo al Menú Principal...");
                    break;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }

            if (opcion != 4)
            {
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }

        } while (opcion != 4);
    }

    // Menú CRUD Productos (Ejemplo vacío, por ahora)
    static void InsertarCliente()
    {
        Console.Clear();
        Console.WriteLine("=== Insertar Cliente ===");
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Dirección: ");
        string direccion = Console.ReadLine();
        Console.Write("Actividad: ");
        string actividad = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Clientes (nombre, email, telefono, direccion, actividad) VALUES (@nombre, @email, @telefono, @direccion, @actividad)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@nombre", nombre);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@telefono", telefono);
            command.Parameters.AddWithValue("@direccion", direccion);
            command.Parameters.AddWithValue("@actividad", actividad);
            command.ExecuteNonQuery();
            Console.WriteLine("Cliente insertado exitosamente.");
        }
    }

    static void MostrarClientes()
    {
        Console.Clear();
        Console.WriteLine("=== Mostrar Clientes ===");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT id, nombre, email, telefono, direccion, actividad FROM Clientes";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine("ID | NOMBRE | EMAIL | TELEFONO | DIRECCION | ACTIVIDAD");
            Console.WriteLine("----------------------------------------------------");
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                string nombre = reader.GetString(reader.GetOrdinal("nombre"));
                string email = reader.GetString(reader.GetOrdinal("email"));
                string telefono = reader.GetString(reader.GetOrdinal("telefono"));
                string direccion = reader.GetString(reader.GetOrdinal("direccion"));
                string actividad = reader.GetString(reader.GetOrdinal("actividad"));

                Console.WriteLine($"{id} | {nombre} | {email} | {telefono} | {direccion} | {actividad}");
            }
            reader.Close();
        }
    }

    static void ActualizarCliente()
    {
        Console.Clear();
        Console.WriteLine("=== Actualizar Cliente ===");
        Console.Write("Ingrese el ID del cliente a actualizar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        Console.Write("Nuevo Nombre: ");
        string nuevoNombre = Console.ReadLine();
        Console.Write("Nuevo Teléfono: ");
        string nuevoTelefono = Console.ReadLine();
        Console.Write("Nuevo Correo: ");
        string nuevoCorreo = Console.ReadLine();
        Console.Write("Nueva Dirección: ");
        string nuevaDireccion = Console.ReadLine();

        string query = "UPDATE Clientes SET nombre=@nombre, telefono=@telefono, email=@email, direccion=@direccion WHERE id=@id";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@nombre", nuevoNombre);
            command.Parameters.AddWithValue("@telefono", nuevoTelefono);
            command.Parameters.AddWithValue("@email", nuevoCorreo);
            command.Parameters.AddWithValue("@direccion", nuevaDireccion);
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
                Console.WriteLine("Cliente actualizado correctamente.");
            else
                Console.WriteLine("No se encontró un cliente con ese ID.");
        }
    }

    // EliminarCliente no está en el menú actual, pero puedes agregarlo si lo deseas.
    static void MenuCrudProductos()
    {
        int opcion = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("=== CRUD de Productos ===");
            Console.WriteLine("1. Insertar Producto");
            Console.WriteLine("2. Mostrar Productos");
            Console.WriteLine("3. Actualizar Producto");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Opción no válida");
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
                continue;
            }

            switch (opcion)
            {
                case 1:
                    InsertarProducto();
                    break;
                case 2:
                    MostrarProductos();
                    break;
                case 3:
                    ActualizarProducto();
                    break;
                case 4:
                    Console.WriteLine("Volviendo al Menú Principal...");
                    break;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }

            if (opcion != 4)
            {
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }

        } while (opcion != 4);
    }

    static void InsertarProducto()
    {
        Console.Clear();
        Console.WriteLine("=== Insertar Producto ===");
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();

        Console.Write("Precio: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal precio))
        {
            Console.WriteLine("Precio inválido.");
            return;
        }

        Console.Write("Categoría: ");
        string categoria = Console.ReadLine();

        Console.Write("Stock: ");
        if (!int.TryParse(Console.ReadLine(), out int stock))
        {
            Console.WriteLine("Stock inválido.");
            return;
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Productos (nombre, precio, categoria, stock) VALUES (@nombre, @precio, @categoria, @stock)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@nombre", nombre);
            command.Parameters.AddWithValue("@precio", precio);
            command.Parameters.AddWithValue("@categoria", categoria);
            command.Parameters.AddWithValue("@stock", stock);

            command.ExecuteNonQuery();
            Console.WriteLine("Producto insertado exitosamente.");
        }
    }

    static void MostrarProductos()
    {
        Console.Clear();
        Console.WriteLine("=== Mostrar Productos ===");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT id, nombre, precio, categoria, stock FROM Productos";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine("ID | NOMBRE | PRECIO | CATEGORIA | STOCK");
            Console.WriteLine("----------------------------------------");
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                string nombre = reader.GetString(reader.GetOrdinal("nombre"));
                decimal precio = reader.GetDecimal(reader.GetOrdinal("precio"));
                string categoria = reader.GetString(reader.GetOrdinal("categoria"));
                int stock = reader.GetInt32(reader.GetOrdinal("stock"));

                Console.WriteLine($"{id} | {nombre} | {precio} | {categoria} | {stock}");
            }
            reader.Close();
        }
    }

    static void ActualizarProducto()
    {
        Console.Clear();
        Console.WriteLine("=== Actualizar Producto ===");
        Console.Write("Ingrese el ID del producto a actualizar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        Console.Write("Nuevo Nombre: ");
        string nuevoNombre = Console.ReadLine();

        Console.Write("Nuevo Precio: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal nuevoPrecio))
        {
            Console.WriteLine("Precio inválido.");
            return;
        }

        Console.Write("Nueva Categoría: ");
        string nuevaCategoria = Console.ReadLine();

        Console.Write("Nuevo Stock: ");
        if (!int.TryParse(Console.ReadLine(), out int nuevoStock))
        {
            Console.WriteLine("Stock inválido.");
            return;
        }

        string query = "UPDATE Productos SET nombre=@nombre, precio=@precio, categoria=@categoria, stock=@stock WHERE id=@id";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@nombre", nuevoNombre);
            command.Parameters.AddWithValue("@precio", nuevoPrecio);
            command.Parameters.AddWithValue("@categoria", nuevaCategoria);
            command.Parameters.AddWithValue("@stock", nuevoStock);
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
                Console.WriteLine("Producto actualizado correctamente.");
            else
                Console.WriteLine("No se encontró un producto con ese ID.");
        }
    }
}
