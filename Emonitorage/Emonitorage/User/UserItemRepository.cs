using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Emonitorage.shared
{
    class UserItemRepository
    {
        UserDataBase db = null;
        protected static string dbLocation;
        protected static UserItemRepository me;

        static UserItemRepository()
        {
            me = new UserItemRepository();
        }
        
        protected UserItemRepository()
        {
            // set the db location
            dbLocation = DatabaseFilePath;

            // instantiate the database	
            db = new UserDataBase(dbLocation);
        }

        public static string DatabaseFilePath
        {
            get
            {
                var sqliteFilename = "LoginDatabase.db3";
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
        public static void CreateDB()
        {
            me.db.CreateDB();
        }

        public static bool IsDB()
        {
            return me.db.IsDB();
        }
        public static int ChangeStatus(string username, int nStatus)
        {
            return me.db.ChangeStatus(username, nStatus);
        }
            public static User GetUser(string id)
        {
            return me.db.GetUser(id);
        }

        public static int AddUser(User item)
        {
            return me.db.AddUser(item);
        }

    }
}
