using System.Reflection;
using System.Text.Json;

namespace SheetsController;

[Serializable]
public static class UserControl
{
    private static readonly string Folder = Directory.GetParent(Assembly.GetEntryAssembly().Location) +
                                            "/SerializedUsers/";

    public static async Task SerializeUser(User user)
    {
        var fileName = user.Nickname + ".json";
        var totalPath = Folder + fileName;
        var json = JsonSerializer.Serialize(user);
        await File.WriteAllTextAsync(totalPath, json);
    }

    public static async Task<User> DeserializeUser(User user)
    {
        var fileName = user.Nickname + ".json";
        var totalPath = Folder + fileName;
        if (!File.Exists(totalPath))
        {
            await SerializeUser(user);
            return user;
        }

        var json = File.ReadAllTextAsync(totalPath);
        var result = JsonSerializer.Deserialize<User>(json.Result);
        return result!;
    }

    public static async Task RenewReservations()
    {
        var directoryInfo = new DirectoryInfo(Folder);
        var files = directoryInfo.GetFiles("*.json");

        foreach (var file in files) await RenewForSingleUser(new User(Path.GetFileNameWithoutExtension(file.Name)));
    }

    private static async Task RenewForSingleUser(User user)
    {
        user = await DeserializeUser(user);
        var resulReservations = new List<Reservation>();
        foreach (var reservation in user.Reservations)
            if (reservation.StartTime + reservation.Duration >= DateTime.Now - DateTime.Now.Date)
                resulReservations.Add(reservation);
        user.Reservations = resulReservations;
        await SerializeUser(user);
    }

    public static async Task<bool> AddReservation(User user, List<GameTable> tables, TimeSpan startTime,
        TimeSpan duration, string additionalInfo, int tableNumber)
    {
        user = await DeserializeUser(user);
        if (tables.Count == 0)
            return false;
        var res = await SheetsController.TrySetFreeTime(tables, startTime, user.Nickname, duration, tables[0].Zone,
            additionalInfo, tableNumber);
        if (res == null)
            return false;
        user.Reservations.Add(res);
        await SerializeUser(user);
        return true;
    }

    public static async Task<bool> AddReservation(User user, Reservation reservation)
    {
        user = await DeserializeUser(user);
        var resToAddInfo = user.Reservations.Find(reservation1 => reservation1.InProcess);
        if (resToAddInfo != null) user.Reservations.Remove(resToAddInfo);
        user.Reservations.Add(reservation);
        await SerializeUser(user);
        return true;
    }

    public static async Task<User> RemoveReservation(User user, Reservation reservation)
    {
        user = await DeserializeUser(user);
        user.Reservations.Remove(reservation);
        await reservation.Table.Zone.CancelReservation(reservation);
        await SerializeUser(user);
        return user;
    }

    public static void DeleteJsonUser(User user)
    {
        var fileName = user.Nickname + ".json";
        var totalPath = Folder + fileName;
        if (File.Exists(totalPath))
            File.Delete(totalPath);
    }

    public static void DeleteAllUsersJson()
    {
        var directoryInfo = new DirectoryInfo(Folder);
        var files = directoryInfo.GetFiles("*.json");
        foreach (var file in files) DeleteJsonUser(new User(Path.GetFileNameWithoutExtension(file.Name)));
    }
}