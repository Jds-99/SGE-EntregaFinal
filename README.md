# Sistema de Gestión de Expedientes (SGE) - Fase 1

Este proyecto es una aplicación de consola en .NET desarrollada bajo los principios de **Arquitectura Limpia** (Clean Architecture).

Estructura de la Solución

La solución está dividida en 4 proyectos separados para garantizar el desacoplamiento y el respeto por las capas arquitectónicas:

* **SGE.Dominio**: Contiene las entidades ricas (`Expediente`, `Tramite`), enumerativos (`EstadoExpediente`, `EtiquetaTramite`) y Objetos de Valor (`Contenido`, `Caratula`), concentrando toda la lógica matemática y reglas esenciales del negocio.
* **SGE.Aplicacion**: Define los Casos de Uso (`AgregarTramiteUseCase`, etc.), los Data Transfer Objects (DTOs), las interfaces de los repositorios (`IExpedienteRepository`, `ITramiteRepository`) y los servicios de aplicación controladores de flujo.
* **SGE.Infraestructura**: Implementa la persistencia transitoria a través de archivos de texto plano (`.txt`) sirviendo como repositorios y los servicios provisorios del sistema (como la simulación de autorizaciones).
* **SGE.Consola**: Interfaz de usuario basada en línea de comandos que actúa como el punto de entrada de la aplicación (*Composition Root*), orquestando la inyección de dependencias.

---

## 🚀 Funcionalidades Clave Implementadas

1.  **Modelo de Dominio Rico**: Las entidades validan sus propios estados internos y mutan de forma autónoma. No existen setters públicos descontrolados.
2.  **Uso de Objetos de Valor (Value Objects)**: Tipos encapsulados para textos críticos como el `Contenido` de un trámite o la `Caratula` del expediente, asegurando que no viajen datos vacíos por el sistema.
3.  **Persistencia en Texto Plano**: Lectura y escritura de datos en archivos independientes delimitados por punto y coma (`;`).
4.  **Hidratación mediante Factory Methods**: Al leer del disco, los repositorios usan métodos estáticos dedicados (como `FactoryMethodTramite`) para reconstruir las entidades con sus IDs y fechas históricas originales, sin disparar efectos secundarios de construcciones nuevas.
5.  **Control de Permisos**: Inserción de un `IAutorizacionService` para validar que el usuario que intenta mutar el sistema posea el permiso requerido antes de ejecutar la acción.
6.  **Coordinación de Reglas de Estado**: Inclusión de un servicio coordinador que, ante cualquier alteración de un trámite, recalcula automáticamente cuál es el estado que le corresponde asumir al expediente contenedor según su historial.

---

## 🛠️ Cómo Ejecutar el Proyecto

### Requisitos previos
* Disponer del SDK de **.NET 8.0** instalado.

### Pasos para correr la aplicación:
1.  Abrir una terminal en la carpeta raíz de la solución (donde se encuentra el archivo `.sln`).
2.  Restaurar las dependencias del proyecto:
    ```bash
    dotnet restore
    ```
3.  Compilar la solución entera para verificar que no existan advertencias ni errores:
    ```bash
    dotnet build
    ```
4.  Ejecutar el proyecto de Consola:
    ```bash
    dotnet run --project SGE.Consola
    ```

---

## 📝 Formato de Persistencia Local (.txt)

Los archivos se generan dinámicamente en el directorio de ejecución de la infraestructura. Los formatos de línea estructurados son:

* **Expedientes:** `Id;Caratula;FechaCreacion;FechaModificacion;UsuarioId;Estado`
* **Trámites:** `Id;IdExpediente;UsuarioUltimoCambio;Contenido;FechaCreacion;FechaModificacion;Etiqueta`

---
