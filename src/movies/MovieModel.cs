namespace SimpleMDB;

public class Movie
{
    private float rating;

    public int Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string Description { get; set; }
    public float Rating { get; set; }

    public Movie(int id = 0, string title = "", int year = 2025, string description = "", float rating = 0)
    {
        Id = id;
        Title = title;
        Year = year;
        Description = description;
        Rating = rating;
    }

    public Movie(string title = "", int year = 2025, string description = "", float rating = 0)
    {
        Title = title;
        Year = year;
        Description = description;
        this.rating = rating;
    }

    public override string ToString()
    {
        return $"Id={Id}, Title={Title}, Year={Year}, Description={Description}, Rating={Rating}";
    }
}