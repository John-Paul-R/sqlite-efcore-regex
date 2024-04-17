using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

public class MyDbContext : DbContext
{
    public DbSet<Person> People { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=mydb.db");
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

public class Program
{
    static async Task Main(string[] args)
    {
        await ResetDatabaseAndInsertSeedData();

        await QueryData();
    }

    static async Task ResetDatabaseAndInsertSeedData()
    {
        using (var db = new MyDbContext())
        {
            // Drop and recreate the database
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            // Insert seed data
            db.People.Add(new Person { Name = "Alice", Age = 30 });
            db.People.Add(new Person { Name = "Bob", Age = 25 });
            db.People.Add(new Person { Name = "Charlie", Age = 35 });
            await db.SaveChangesAsync();
        }
    }

    static async Task QueryData()
    {
        using (var db = new MyDbContext())
        {
            var people = await db.People
                .Where(p => Regex.IsMatch(p.Name, "^B.+"))
                .ToListAsync();
            foreach (var person in people)
            {
                Console.WriteLine($"{person.Name} is {person.Age} years old.");
            }
        }
    }
}


