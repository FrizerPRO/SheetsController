namespace SheetsController;

public class User
{
    public User(string nickname, List<Reservation> reservations)
    {
        Nickname = nickname;
        Reservations = reservations;
        UserControl.SerializeUser(this).Wait();
    }

    public User(string nickname)
    {
        Nickname = nickname;
        Reservations = new List<Reservation>();
        var templeUser = UserControl.DeserializeUser(this);
        Reservations = templeUser.Result.Reservations;
        UserControl.SerializeUser(this).Wait();
    }

    public User()
    {
        Nickname = "";
        Reservations = new List<Reservation>();
    }

    public string Nickname { get; set; }

    public List<Reservation> Reservations { get; set; }
}