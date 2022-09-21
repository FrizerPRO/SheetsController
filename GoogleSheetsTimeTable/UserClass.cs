using System.Security.Cryptography;

namespace SheetsController;

public class User
{
    public string Nickname { get; set; }

    public List<Reservation> Reservations { get; set; }

    public User(string nickname, List<Reservation> reservations)
    {
        this.Nickname = nickname;
        this.Reservations = reservations;
    }

    public User(string nickname)
    {
        Nickname = nickname;
        this.Reservations = new List<Reservation>();
        var templeUser =  UserControl.DeserializeUser(this);
        Reservations = templeUser.Result.Reservations;
    }

    public User()
    {
        Nickname = "";
        this.Reservations = new List<Reservation>();
    }
}