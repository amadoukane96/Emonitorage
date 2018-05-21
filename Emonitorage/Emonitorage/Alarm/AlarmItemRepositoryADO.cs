using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Emonitorage.shared
{
    public class AlarmItemRepositoryADO
    {
        AlarmDatabase db = null;
        protected static string dbLocation;
        protected static AlarmItemRepositoryADO me;

        static AlarmItemRepositoryADO()
        {
            me = new AlarmItemRepositoryADO();
        }

        protected AlarmItemRepositoryADO()
        {
            // set the db location
            dbLocation = DatabaseFilePath;

            // instantiate the database	
            db = new AlarmDatabase(dbLocation);
        }

        public static string DatabaseFilePath
        {
            get
            {
                var sqliteFilename = "AlarmDatabase.db3";
#if NETFX_CORE
				var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
#else

#if SILVERLIGHT
				// Windows Phone expects a local path, not absolute
				var path = sqliteFilename;
#else

#if __ANDROID__
                // Just use whatever directory SpecialFolder.Personal returns
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
#endif
                var path = Path.Combine(libraryPath, sqliteFilename);
#endif

#endif
                return path;
            }
        }

        public static AlarmItem GetAlarm(int id)
        {
            return me.db.GetItem(id);
        }

        public static IEnumerable<AlarmItem> GetAlarms()
        {
            return me.db.GetItems();
        }

        public static int ChangeStatus(int id, int nStatus)
        {
            return me.db.ChangeStatus(id, nStatus);
        }

        public static int AddAlarm(AlarmItem item)
        {
            return me.db.AddItem(item);
        }

        public static int DeleteAlarm(int id)
        {
            return me.db.DeleteItem(id);
        }
        public static int DeleteAlarms()
        {
            return me.db.DeleteItems();
        }

        public static int DeleteTable()
        {
            return me.db.DeleteTable();
        }
    }
}
