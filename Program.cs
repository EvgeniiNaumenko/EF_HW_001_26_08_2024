using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace En;

//Опишите таблицу: «Поезда» (минимум 6 столбцов). 
//Создайте соответствующий класс.
//Выполните: добавление, получение, редактирование и удаление данных из таблицы. Каждая операция, в отдельном методе.
//Для соединения с базой данных, используйте файл конфигурации .json.
class Program
{
    static void Main()
    {
        //основная часть дз
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");
            var optinsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optinsBuilder.UseSqlServer(connectionString).Options;

            using (ApplicationContext db = new ApplicationContext(options))
            {
                db.Database.EnsureCreated();
                // CRUD Операции
                AddTrain(db, "123A", 5, 120, new DateTime(2024, 8, 27, 14, 30, 0), new DateTime(2024, 8, 28, 6, 0, 0));

                List<Trains> trains = GetTrains(db);
                foreach (var train in trains)
                {
                    Console.WriteLine($"ID: {train.Id}, Train Number: {train.trainNumber}, Railway Carriage Number: {train.railwayСarriageNum}, " +
                                      $"Seats Number: {train.seatsNumber}, Departure Time: {train.departureTime}, Arrival Time: {train.arrivelTime}");
                }

                UpdateTrain(db, 1, "124B", 6, 100, new DateTime(2024, 8, 27, 15, 10, 0), new DateTime(2024, 8, 28, 12, 15, 0));

                DeleteTrain(db, 1);
            }
        }

        //===========================================================================================================
        //ДОП ДЗ
        //Описать класс “Меню Блюд”. Используя 2 отдельных using, добавить данные в таблицу, 
        //как один объект, так и коллекцию.Считать из таблицы информацию в коллекцию, 
        //проверить перед этим доступность базы данных.Получить все блюда, в названии которых содержится слово “Суп”.
        //Получить блюдо по Id. Получить самое последнее блюдо из таблицы.
        {

            var builder2 = new ConfigurationBuilder();
            builder2.SetBasePath(Directory.GetCurrentDirectory());
            builder2.AddJsonFile("appsettings2.json");
            var config2 = builder2.Build();
            string connectionString2 = config2.GetConnectionString("DefaultConnection");
            var optionsBuilder2 = new DbContextOptionsBuilder<ApplicationContext2>();
            var options2 = optionsBuilder2.UseSqlServer(connectionString2).Options;


            using (var db2 = new ApplicationContext2(options2))
            {
                db2.Database.EnsureCreated();
                CheckDatabaseAvailability(db2);


                AddDish(db2, new Dish
                {
                    Name = "Суп",
                    Description = "Вкусный и питательный суп из свежих овощей.",
                    Price = 150.00m
                });

                AddDishes(db2, new List<Dish>
                {
                    new Dish { Name = "Борщ", Description = "Классический борщ с говядиной.", Price = 200.00m },
                    new Dish { Name = "Овощной суп", Description = "Суп с разнообразными овощами.", Price = 120.00m }
                });


                var allDishes = GetDishes(db2);
                var soups = GetDishesByName(db2, "Суп");
                var dishById = GetDishById(db2, 1);
                var latestDish = GetLatestDish(db2);


                Console.WriteLine("Все блюда:");
                foreach (var dish in allDishes)
                {
                    Console.WriteLine($"{dish.Id}: {dish.Name} - {dish.Description} - {dish.Price:C}");
                }

                Console.WriteLine("\nБлюда, содержащие 'Суп':");
                foreach (var soup in soups)
                {
                    Console.WriteLine($"{soup.Id}: {soup.Name} - {soup.Description} - {soup.Price:C}");
                }

                Console.WriteLine($"\nБлюдо с ID 1:");
                if (dishById != null)
                {
                    Console.WriteLine($"{dishById.Id}: {dishById.Name} - {dishById.Description} - {dishById.Price:C}");
                }
                else
                {
                    Console.WriteLine("Блюдо с таким ID не найдено.");
                }

                Console.WriteLine("\nСамое последнее блюдо:");
                if (latestDish != null)
                {
                    Console.WriteLine($"{latestDish.Id}: {latestDish.Name} - {latestDish.Description} - {latestDish.Price:C}");
                }
                else
                {
                    Console.WriteLine("Нет блюд в базе данных.");
                }
            }
        }
    }
    //====================================================================
    //ОСНОВНОЕ ДЗ
    static void AddTrain(ApplicationContext db, string trainNumber, int railwayCarriageNum, int seatsNumber, DateTime departureTime, DateTime arrivelTime)
    {
        Trains newTrain = new Trains
        {
            trainNumber = trainNumber,
            railwayСarriageNum = railwayCarriageNum,
            seatsNumber = seatsNumber,
            departureTime = departureTime,
            arrivelTime = arrivelTime
        };
        db.trains.Add(newTrain);
        db.SaveChanges();
    }

    static List<Trains> GetTrains(ApplicationContext db)
    {
        var trains = db.trains.ToList();
        Console.WriteLine("Список поездов:");
        return trains;
    }

    static void UpdateTrain(ApplicationContext db, int id, string newTrainNumber, int newRailwayCarriageNum, int newSeatsNumber, DateTime newDepartureTime, DateTime newArrivelTime)
    {
        var train = db.trains.FirstOrDefault(t => t.Id == id);
        if (train != null)
        {
            train.trainNumber = newTrainNumber;
            train.railwayСarriageNum = newRailwayCarriageNum;
            train.seatsNumber = newSeatsNumber;
            train.departureTime = newDepartureTime;
            train.arrivelTime = newArrivelTime;
            db.SaveChanges();
        }
        else
        {
            Console.WriteLine($"Поезд с ID {id} не найден.");
        }
    }

    static void DeleteTrain(ApplicationContext db, int id)
    {
        var train = db.trains.FirstOrDefault(t => t.Id == id);
        if (train != null)
        {
            db.trains.Remove(train);
            db.SaveChanges();
        }
        else
        {
            Console.WriteLine($"Поезд с ID {id} не найден.");
        }
    }
    //==============================================================================
    //ДОП ДЗ
    static void CheckDatabaseAvailability(ApplicationContext2 db)
    {
        try
        {
            db.Database.OpenConnection();
            db.Database.CloseConnection();
            Console.WriteLine("База данных доступна.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка подключения к базе данных: {ex.Message}");
        }
    }

    static void AddDish(ApplicationContext2 db, Dish dish)
    {
        db.Dishes.Add(dish);
        db.SaveChanges();
        Console.WriteLine($"Блюдо '{dish.Name}' добавлено.");
    }

    static void AddDishes(ApplicationContext2 db, List<Dish> dishes)
    {
        db.Dishes.AddRange(dishes);
        db.SaveChanges();
        Console.WriteLine($"Коллекция блюд добавлена.");
    }

    static List<Dish> GetDishes(ApplicationContext2 db)
    {
        return db.Dishes.ToList();
    }

    static List<Dish> GetDishesByName(ApplicationContext2 db, string keyword)
    {
        return db.Dishes
            .Where(d => d.Name.Contains(keyword))
            .ToList();
    }

    static Dish GetDishById(ApplicationContext2 db, int id)
    {
        return db.Dishes.FirstOrDefault(d => d.Id == id);
    }

    static Dish GetLatestDish(ApplicationContext2 db)
    {
        return db.Dishes
            .OrderByDescending(d => d.Id)
            .FirstOrDefault();
    }
}

public class Trains
{
    public int Id { get; set; }
    public string trainNumber { get; set; }
    public int railwayСarriageNum { get; set; }
    public int seatsNumber { get; set; }
    public DateTime departureTime { get; set; }
    public DateTime arrivelTime {  get; set; }
}

class ApplicationContext : DbContext
{

    public ApplicationContext(DbContextOptions options) : base(options) { }
    public DbSet<Trains> trains { get; set; } =null!;

}
// ДОП
public class Dish
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}

public class ApplicationContext2 : DbContext
{
    public ApplicationContext2(DbContextOptions<ApplicationContext2> options2) : base(options2) { }
    public DbSet<Dish> Dishes { get; set; }
}