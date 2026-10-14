using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Semana_9
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    namespace Semana_9
    {
        internal class Program
        {
            static int max_emp = 50;
            static List<Employee> empleados = new List<Employee>();
            static string rutaArchivo = "Empleado_datos.txt";

            static public void Titulo()
            {
                Console.WriteLine("*************************");
                Console.WriteLine("SISTEMA DE EMPLEADOS");
                Console.WriteLine("*************************");
            }

            static public void Registro_empleado()
            {
                Console.WriteLine("**** REGISTRAR EMPLEADO ****");
                if (empleados.Count >= max_emp)
                {
                    Console.WriteLine("Alcanzó la capacidad máxima de datos.");
                    return;
                }

                Console.Write("Ingresar nombres y apellidos: ");
                string entrada = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(entrada))
                {
                    Console.WriteLine("Error: el nombre no puede estar vacío.");
                    return;
                }

                string nombreFormateado = string.Join(" ", entrada.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                                                .ToUpperInvariant();

                string[] partes = nombreFormateado.Split(' ');
                string primer = partes.FirstOrDefault() ?? "usuario";
                string ultimo = partes.Length > 1 ? partes.Last() : primer;

                string primerLimpio = ToAsciiIdentifier(primer);
                string ultimoLimpio = ToAsciiIdentifier(ultimo);
                string correoBase = $"{primerLimpio}.{ultimoLimpio}@upn.edu.pe".ToLowerInvariant();

                string correoFinal = correoBase;
                int sufijo = 1;
                while (empleados.Any(e => e.Email.Equals(correoFinal, StringComparison.OrdinalIgnoreCase)))
                {
                    correoFinal = $"{primerLimpio}.{ultimoLimpio}{sufijo}@upn.edu.pe".ToLowerInvariant();
                    sufijo++;
                }

                empleados.Add(new Employee { Name = nombreFormateado, Email = correoFinal });
                Console.WriteLine("Empleado registrado correctamente.");
                Console.WriteLine($"Nombre: {nombreFormateado}");
                Console.WriteLine($"Correo: {correoFinal}");
            }

            static public void buscar_empleado()
            {
                Console.WriteLine("********** Búsqueda de empleado **********");
                Console.WriteLine("******************************************");

                if (empleados.Count == 0)
                {
                    Console.WriteLine("No hay empleados registrados.");
                    return;
                }

                Console.Write("Ingresar nombre o apellido a buscar: ");
                string buscar = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(buscar))
                {
                    Console.WriteLine("Búsqueda vacía.");
                    return;
                }

                string term = buscar.ToUpperInvariant();
                var encontrados = empleados
                    .Where(e => e.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                if (!encontrados.Any())
                {
                    Console.WriteLine("No se encuentran coincidencias en empleados.");
                    return;
                }

                Console.WriteLine($"Se encontraron {encontrados.Count} coincidencia(s):");
                foreach (var e in encontrados)
                {
                    Console.WriteLine($"- {e.Name} | {e.Email}");
                }
            }

            static public void ordenar()
            {
                if (empleados.Count <= 1)
                {
                    Console.WriteLine("No hay suficientes registros para ordenar.");
                    return;
                }

                empleados.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
                Console.WriteLine("Registros ordenados alfabéticamente por nombre.");
            }

            static public void mostrar()
            {
                Console.WriteLine("******** LISTADO DE EMPLEADOS ********");
                Console.WriteLine("**************************************");

                if (empleados.Count == 0)
                {
                    Console.WriteLine("No hay empleados para mostrar.");
                    return;
                }

                int i = 1;
                foreach (var e in empleados)
                {
                    Console.WriteLine($"{i}. Nombres: {e.Name} | Correo: {e.Email}");
                    i++;
                }
            }

            static public void Guardar_datos_Archivo()
            {
                Console.WriteLine("***** GUARDAR DATOS EN ARCHIVO ******");
                Console.WriteLine("***********************************");
                if (empleados.Count == 0)
                {
                    Console.WriteLine("No hay registros para guardar");
                    return;
                }

                try
                {
                    using (StreamWriter escribir = new StreamWriter(rutaArchivo, false, Encoding.UTF8))
                    {
                        foreach (var e in empleados)
                        {
                            // Guardar como: Nombre,Correo
                            escribir.WriteLine($"{e.Name.Replace(',', ' ')},{e.Email}");
                        }
                    }
                    Console.WriteLine($"{empleados.Count} empleado(s) guardado(s) en '{rutaArchivo}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al guardar archivo: " + ex.Message);
                }
            }

            static public void Leer_datos_Archivo()
            {
                Console.WriteLine("***** LEER DATOS DE ARCHIVO ******");
                Console.WriteLine("*********************************");
                if (!File.Exists(rutaArchivo))
                {
                    Console.WriteLine($"No existe el archivo '{rutaArchivo}'");
                    return;
                }

                try
                {
                    int num_linea = 0;
                    using (StreamReader lector = new StreamReader(rutaArchivo, Encoding.UTF8))
                    {
                        string linea;
                        while ((linea = lector.ReadLine()) != null)
                        {
                            num_linea++;
                            Console.WriteLine($"{num_linea}. {linea}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al leer archivo: " + ex.Message);
                }
            }

            static public void Cargar_datos_Archivo()
            {
                Console.WriteLine("***** CARGAR DATOS DE ARCHIVO ******");
                Console.WriteLine("**********************************");
                if (!File.Exists(rutaArchivo))
                {
                    Console.WriteLine($"No existe el archivo de empleados '{rutaArchivo}'");
                    return;
                }

                try
                {
                    var lineas = File.ReadAllLines(rutaArchivo, Encoding.UTF8);
                    empleados.Clear();
                    foreach (var linea in lineas)
                    {
                        if (empleados.Count >= max_emp) break;
                        var partes = linea.Split(new[] { ',' }, 2);
                        if (partes.Length == 2)
                        {
                            string name = partes[0].Trim();
                            string email = partes[1].Trim();
                            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
                            {
                                empleados.Add(new Employee { Name = name, Email = email });
                            }
                        }
                    }
                    Console.WriteLine($"Se cargaron {empleados.Count} registro(s) en memoria (máx {max_emp}).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al cargar archivo: " + ex.Message);
                }
            }

            static void Main(string[] args)
            {
                int opc = 0;
                do
                {
                    Console.Clear();
                    Titulo();
                    Console.WriteLine("[1] Registrar empleado");
                    Console.WriteLine("[2] Buscar empleado");
                    Console.WriteLine("[3] Ordenar alfabéticamente");
                    Console.WriteLine("[4] Mostrar");
                    Console.WriteLine("[5] Guardar datos en archivo");
                    Console.WriteLine("[6] Leer contenido del archivo");
                    Console.WriteLine("[7] Cargar datos desde archivo");
                    Console.WriteLine("[8] Salir");
                    Console.Write("Seleccione una opción: ");

                    string entrada = Console.ReadLine();
                    if (!int.TryParse(entrada, out opc))
                    {
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        continue;
                    }

                    Console.Clear();
                    switch (opc)
                    {
                        case 1: Registro_empleado(); break;
                        case 2: buscar_empleado(); break;
                        case 3: ordenar(); break;
                        case 4: mostrar(); break;
                        case 5: Guardar_datos_Archivo(); break;
                        case 6: Leer_datos_Archivo(); break;
                        case 7: Cargar_datos_Archivo(); break;
                        case 8: Console.WriteLine("Saliendo..."); break;
                        default: Console.WriteLine("Opción no válida."); break;
                    }

                    if (opc != 8)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Presione una tecla para continuar...");
                        Console.ReadKey();
                    }

                } while (opc != 8);
            }

            static string ToAsciiIdentifier(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;

                string normalized = text.Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder();
                foreach (char c in normalized)
                {
                    var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (uc != UnicodeCategory.NonSpacingMark)
                        sb.Append(c);
                }
                string noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

                var cleaned = new StringBuilder();
                foreach (char c in noDiacritics)
                {
                    if (char.IsLetterOrDigit(c))
                        cleaned.Append(c);
                }

                return cleaned.ToString().ToLowerInvariant();
            }

            class Employee
            {
                public string Name { get; set; }
                public string Email { get; set; }
            }
        }
    }
}