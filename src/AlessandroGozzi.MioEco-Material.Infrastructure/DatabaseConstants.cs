using System;
using System.Collections.Generic;
using System.Text;

namespace AlessandroGozzi.MioEco_Material.Infrastructure
{
    public static class DatabaseConstants
    {
        public const string DatabaseFilename = "MioEco_Material.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);
    }
}
