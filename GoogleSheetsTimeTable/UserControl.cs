namespace SheetsController;
using System.Text.Json;
[Serializable]

public static class UserControl
{
    private static readonly string Folder = "../../../../ConsoleApp1/SerializedUsers/";
    public static void SerializeUser(User user)
    {
        var fileName = user.Nickname + ".json";
        var totalPath = Folder + fileName;
        var json = JsonSerializer.Serialize(user);
        File.WriteAllText(totalPath,json);
    }
    public static User DeserializeUser(User user)
    {
        var fileName = user.Nickname + ".json";
        var totalPath = Folder + fileName;
        if (!File.Exists(totalPath))
        {
            SerializeUser(user);
            return user;
        }
        var json = File.ReadAllText(totalPath);
        var result = JsonSerializer.Deserialize<User>(json);
        return result!;
    }
    public static void RenewReservations()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Folder);
        var files = directoryInfo.GetFiles("*.json");
        foreach (var file in files)
        {
            RenewForSingleUser(new User(Path.GetFileNameWithoutExtension(file.Name)));
        }
    }
    
    private static void RenewForSingleUser(User user)
    { 
        user = DeserializeUser(user);
        var resulReservations = new List<Reservation>();
        foreach (var reservation in user.Reservations)
        {
            if (reservation.StartTime + reservation.Duration >= DateTime.Now - DateTime.Now.Date)
            {
                resulReservations.Add(reservation);
            }
        }
        user.Reservations = resulReservations;
        SerializeUser(user);
    }
    
    public static bool AddReservation(User user,List<GameTable> tables,TimeSpan startTime, TimeSpan duration,string additionalInfo,int tableNumber)
    {
        user = DeserializeUser(user);
        if (tables.Count == 0)
            return false;
        var res =  tables[0].Zone.SheetsController.
            TrySetFreeTime(tables, startTime, user.Nickname, duration, tables[0].Zone, additionalInfo,tableNumber);
        if (res == null)
            return false;
        user.Reservations.Add(res);
        UserControl.SerializeUser(user);
        return true;
    }
    public static bool AddReservation(User user,Reservation reservation)
    {
        user = DeserializeUser(user);
        var resToAddInfo = user.Reservations.Find(reservation1 => reservation1.InProcess);
        if (resToAddInfo != null)
        {
            user.Reservations.Remove(resToAddInfo);
        }
        user.Reservations.Add(reservation);
        UserControl.SerializeUser(user);
        return true;
    }
    public static void RemoveReservation(User user,Reservation reservation)
    {      
        user = DeserializeUser(user);
        reservation.Table.Zone.CancelReservation(reservation);
        UserControl.SerializeUser(user);
    }
    public static void DeleteJsonUser(User user)
    {
        var fileName = user.Nickname + ".json";
        var totalPath = Folder + fileName;
        if(File.Exists(totalPath))
            File.Delete(totalPath);
    }

    public static void DeleteAllUsersJson()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Folder);
        var files = directoryInfo.GetFiles("*.json");
        foreach (var file in files)
        {
            DeleteJsonUser(new User(Path.GetFileNameWithoutExtension(file.Name)));
        }
    }
}