using SGE.Infraestructura;

Console.WriteLine("Iniciando el sistema y verificando base de datos...");

// Instanciamos el contexto. Al ejecutarse el constructor, se va a crear el archivo SQLite solo.
using (var context = new SgeContext())
{
    Console.WriteLine("¡Base de datos verificada/creada con éxito!");
}