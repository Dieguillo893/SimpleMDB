namespace SimpleMDB;

public class Actor
{
    private float rating;

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public float Rating { get; set; }

    public Actor(int id = 0, string firstName = "", string lastName = "", string bio = "", float rating = 0)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        Rating = rating;
    }

    public Actor(string firstName, string lastName, string bio, float rating)
    {
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        this.rating = rating;
    }

    public override string ToString()
    {
        return $"Id={Id}, FirstName={FirstName}, LastName={LastName}, Bio={Bio}, Rating={Rating}";
    }
}