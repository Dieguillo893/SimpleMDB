using Microsoft.VisualBasic;

namespace SimpleMDB;

public class MockActorRepository : ActorRepository
{
    private List<Actor> actors;
    private int idCount;

    public MockActorRepository()
    {
        actors = [];
        idCount = 0;

        var firstNames = new[] { "Ana", "Abel", "Beatriz", "Bob", "Caroline", "Chris", "Daritzel", "Daniel", "Elizabeth", "Emilio", "Faith", "Farruko" };
        var lastNames = new[] { "Martinez", "Gomez", "Lopez", "Fernandez", "Perez", "Ramirez", "Torres", "Vargas", "Morales", "Castillo", "Herrera", "Navarro" };
        var bios = new[]
        {
            "An experienced performer known for versatility.",
            "Starred in multiple independent films.",
            "Award-winning talent with a background in theater.",
            "Emerging actor with a strong screen presence.",
            "Known for deep character work and charisma.",
            "A veteran of stage and television.",
            "Critically acclaimed for dramatic roles.",
            "Popular among fans for comedic timing.",
            "Frequently cast in action-packed roles.",
            "Praised for emotional authenticity on screen."
        };

        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];
            string bio = bios[random.Next(bios.Length)];
            float rating = (float)(random.NextDouble() * 10);

            actors.Add(new Actor(idCount++, firstName, lastName, bio, rating));
        }

        foreach (var actor in actors.GetRange(0, 5))
        {
            Console.WriteLine($"{actor.FirstName} {actor.LastName}: {actor.Bio} (Rating: {actor.Rating:F1})");
        }

    }

    public async Task<PagedResult<Actor>> ReadAll(int page, int size)
    {
        int totalCount = actors.Count;
        int start = Math.Clamp((page - 1) * size, 0, totalCount);
        int length = Math.Clamp(size, 0, totalCount - start);
        List<Actor> values = actors.Slice(start, length);
        var pagedResult = new PagedResult<Actor>(values, totalCount);

        return await Task.FromResult(pagedResult);
    }
    public async Task<Actor?> Create(Actor actor)
    {
        actor.Id = idCount++;
        actors.Add(actor);

        return await Task.FromResult(actor);
    }
    public async Task<Actor?> Read(int id)
    {
        Actor? actor = actors.FirstOrDefault((u) => u.Id == id);

        return await Task.FromResult(actor);
    }
    public async Task<Actor?> Update(int id, Actor newActor)
    {
        Actor? actor = actors.FirstOrDefault((u) => u.Id == id);

        if (actor != null)
        {
            actor.FirstName = newActor.FirstName;
            actor.LastName = newActor.LastName;
            actor.Bio = newActor.Bio;
            actor.Rating = newActor.Rating;
        }

        return await Task.FromResult(actor);
    }
    public async Task<Actor?> Delete(int id)
    {
        Actor? actor = actors.FirstOrDefault((u) => u.Id == id);

        if (actor != null)
        {
            actors.Remove(actor);
        }
        return await Task.FromResult(actor);
    }
}