using System;
using System.Collections.Generic;
using System.Text;

namespace Emonitorage.shared
{
    class UserItemManager
    {
        static UserItemManager(){
        }
        
        public static bool IsDB()
        {
            return UserItemRepository.IsDB();
        }

        public static void CreateDB()
        {
            UserItemRepository.CreateDB();
        }

        public static int AddUser(User item)
        {
            return UserItemRepository.AddUser(item);
        }
        public static User GetUser(string id)
        {
            return UserItemRepository.GetUser(id);
        }
        public static int ChangeStatus(string username, int nStatus)
        {
            return UserItemRepository.ChangeStatus(username, nStatus);
        }
    }
}
