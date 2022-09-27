namespace SheetsController
{
    public static class DataBase
    {
        private static readonly string PathBegin = Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location) +"/DataBase/";

        public static List<PlayZone> Zones
        {
            get
            {
                List<PlayZone> result = new();
                var lines = File.ReadAllLines(PathBegin + "Zones.txt");
                Console.WriteLine(lines[0]);
                foreach (var line in lines)
                {
                    var name = line.Split(":")[0];
                    Console.WriteLine(name);
                    var capacityStr = line.Split(":")[1];
                    int capacity;
                    int.TryParse(capacityStr, out capacity);
                    result.Add(new PlayZone(name, capacity));
                }

                return result;
            }
        }

        public static string SpreadSheetId =>
            "1PQTy_fIQSOaw2RQoDwkY2F_TCRdPxbQ3E64jclb-Av0"; //File.ReadAllText(PathBegin + "SpreadSheetId.txt");
    }
}