namespace SimpleMDB;

public class ActorHtmlTemplates
{
  public static string ViewAllActorsGet(List<Actor> actors, int actorCount, int page, int size)
  {
    int pageCount = (int)Math.Ceiling((double)actorCount / size);

    string rows = "";

    foreach (var actor in actors)
    {
      rows += @$"
                <tr>
                  <td>{actor.Id}</td>
                  <td>{actor.FirstName}</td> 
                  <td>{actor.LastName}</td> 
                  <td>{actor.Bio}</td> 
                  <td>{actor.Rating}</td> 
                  <td><a href=""/actors/view?aid={actor.Id}"">View</a></td>
                  <td><a href=""/actors/edit?aid={actor.Id}"">Edit</a></td>
                  <td>
                    <form action=""/actors/remove?aid={actor.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure that you want to delete this actor?.')"">
                      <input type=""submit"" value=""Remove"">
                    </form>
                  </td>
                </tr>
                ";
    }

    string html = $@"
            <div class=""add"">
                <a href=""/actors/add"">Add New Actor</a>
            </div>
            <table class=""viewall"">
             <thead>
              <th>Id</th>
              <th>First Name</th>
              <th>Last Name</th>
              <th>Bio</th>
              <th>Rating</th>
              <th>View</th>
              <th>Edit</th>
              <th>Remove</th>
             </thead>
             <tbody>
             {rows}
             </tbody>
            </table>
            <div class=""pagination"">
              <a href=""?page=1&size={size}"">First</a>
              <a href=""?page={page - 1}&size={size}"">Prev</a>
              <span>{page} / {pageCount}</span>
              <a href=""?page={page + 1}&size={size}"">Next</a>
              <a href=""?page={pageCount}&size={size}"">Last</a>
            </div>
            ";
    return html;
  }

  public static string AddActorGet(string firstname, string lasname, string bio, string rating)
  {
    string html = $@"
            <form class=""addform"" action=""/actors/add"" method=""POST"">
              <label for=""firstname"">FirstName</label>
              <input id=""firstname"" name=""firstname"" type=""text"" placeholder=""First Name"" value=""{firstname}"">
              <label for=""lastname"">LastName</label>
              <input id=""lastname"" name=""lastname"" type=""text"" placeholder=""Last Name"" value=""{lasname}"">
              <label for=""bio"">Bio</label>
              <input id=""bio"" name=""bio"" type=""lastnbioame"" placeholder=""Bio"" value=""{bio}"">
              <label for=""rating"">Rating</label>
              <input id=""rating"" name=""rating"" type=""number"" min=""0"" max=""10"" step=""0.1"" value=""{rating}"">
             <input type=""submit"" value=""Add"">
            </form>
            ";

    return html;
  }

  public static string ViewActorGet(Actor actor)
  {
    string html = $@"
            <table class=""view"">
             <thead>
              <th>Id</th>
              <th>First Name</th>
              <th>Last Name</th>
              <th>Bio</th>
              <th>Rating</th>
             </thead>
             <tbody>
             <tr>
                  <td>{actor.Id}</td>
                  <td>{actor.FirstName}</td> 
                  <td>{actor.LastName}</td> 
                  <td>{actor.Bio}</td> 
                  <td>{actor.Rating}</td> 
                </tr>
             </tbody>
            </table>
            <div>
            ";

    return html;
  }

  public static string EditActorGet(int aid, Actor actor)
  {

    string html = $@"
            <form class=""editform"" action=""/actors/edit?aid={aid}"" method=""POST"">
              <label for=""firstname"">First Name</label>
              <input id=""firstname"" name=""firstname"" type=""text"" placeholder=""First Name"" value=""{actor.FirstName}"">
              <label for=""lastname"">Last Name</label>
              <input id=""lastname"" name=""lastname"" type=""text"" placeholder=""Last Name"" value=""{actor.LastName}"">
              <label for=""bio"">Bio</label>
              <input id=""bio"" name=""bio"" type=""text"" placeholder=""Bio"" value=""{actor.Bio}"">
              <label for=""rating"">Rating</label>
              <input id=""rating"" name=""rating"" type=""number"" min=""0"" max=""10"" step=""0.1"" value=""{actor.Rating}"">
             <input type=""submit"" value=""Edit"">
            </form>
            ";

    return html;
  }
}
