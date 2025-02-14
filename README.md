# ApiMarvel

# Entity Framework Core - Migraciones

Este documento describe los comandos esenciales para administrar migraciones en **Entity Framework Core** dentro de una aplicación ASP.NET.

## 📌 Requisitos previos
Asegúrate de tener instalado el paquete de herramientas de Entity Framework Core en tu proyecto. Si no lo tienes, instálalo con el siguiente comando:

```bash
dotnet tool install --global dotnet-ef
```

Si ya lo tienes, asegúrate de que está actualizado:

```bash
dotnet tool update --global dotnet-ef
```

---

## 🚀 Comandos para manejar migraciones que te permitiran craer la base de datos

### 1️⃣ Agregar una nueva migración
Genera una nueva migración con los cambios realizados en el modelo de datos:

```bash
dotnet ef migrations add NombreDeLaMigracion
```

*Ejemplo:*
```bash
dotnet ef migrations add InitialCreate
```

### 2️⃣ Aplicar migraciones a la base de datos
Ejecuta las migraciones y actualiza la base de datos al último esquema definido en el código:

```bash
dotnet ef database update
```

### 3️⃣ Revertir la última migración
Si cometiste un error al generar la última migración, puedes eliminarla antes de aplicarla:

```bash
dotnet ef migrations remove
```

### 4️⃣ Actualizar la base de datos a una migración específica
Si necesitas aplicar una migración en particular, usa:

```bash
dotnet ef database update NombreDeLaMigracion
```

*Ejemplo:*
```bash
dotnet ef database update InitialCreate
```

### 5️⃣ Listar todas las migraciones existentes
Muestra un listado de todas las migraciones aplicadas o creadas:

```bash
dotnet ef migrations list
```

### 6️⃣ Revertir la base de datos a un estado anterior
Si necesitas deshacer cambios en la base de datos y regresar a un estado anterior, usa:

```bash
dotnet ef database update NombreDeLaMigracionAnterior
```

### 7️⃣ Eliminar la base de datos (⚠️ Uso con precaución)
Si necesitas eliminar completamente la base de datos, puedes hacerlo con:

```bash
dotnet ef database drop
```

⚠️ **Este comando elimina todos los datos y estructuras de la base de datos. Úsalo con precaución.**

---

## 🛠️ Solución de problemas
Si encuentras problemas con las migraciones, intenta:

- Verificar que `dotnet-ef` esté instalado correctamente.
- Asegurarte de que la conexión a la base de datos es válida.
- Revisar si hay conflictos entre migraciones anteriores.
- Usar `dotnet ef migrations remove` para eliminar una migración errónea y volver a intentarlo.

Si necesitas más información, consulta la documentación oficial de Microsoft: [Entity Framework Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

---

## 


### 📌 Autor
**Tu Nombre**  Deudibet Emilio Coquies Galindo
**Proyecto:** ApiMarvel  
**Fecha:** 📅 2025-02-13
