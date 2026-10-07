using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace LogicPOS.Persistence.Seeding
{
    public class SeedCreator(LogicPOSDbContext database)
    {
        public void Create()
        {
            var dbSetProperties = database.GetType()
               .GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(p => p.PropertyType.IsGenericType &&
                           p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            foreach (var prop in dbSetProperties)
            {
                var entityType = prop.PropertyType.GetGenericArguments()[0];
                var dbSet = prop.GetValue(database);

                var entities = (dbSet as IEnumerable<object>)!.ToList();

                var json = JsonSerializer.Serialize(entities, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                });

                string filePath = Path.Combine("SeedCreator", $"{prop.Name.ToLower()}.json");

                File.WriteAllText(filePath, json);
            }
        }


    }
}
