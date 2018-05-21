using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Emonitorage.shared;
using Newtonsoft.Json.Linq;
using System.Linq;
using Firebase.Iid;
using WindowsAzure.Messaging;

namespace Emonitorage.Services
{
    class ApiLogin
    {
        private static string url = "http://10.192.3.201/hub/index";
        public async Task<int> LoginAsync(string userName, string password)
        {
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("UID",userName),
                new KeyValuePair<string, string>("PWD",password),
                new KeyValuePair<string, string>("URL","login"),
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new FormUrlEncodedContent(keyValues);
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            /*Debug.WriteLine(content+"SHES");*/
            return Convert.ToInt32(response.StatusCode);
        }
        public async Task<int> FirstLoginAsync(string userName, string password)
        {
            User u = new User();
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("UID",userName),
                new KeyValuePair<string, string>("PWD",password),
                new KeyValuePair<string, string>("URL","firstlogin"),
            };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri: url)
            {
                Content = new FormUrlEncodedContent(keyValues)
            };
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (Convert.ToInt32(response.StatusCode) == 400)
            {
                return 400;
            }
             
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            
             u = JsonConvert.DeserializeObject<User>(content, settings);
            UserItemManager.CreateDB();
            UserItemManager.AddUser(u);
            //var services = response.Headers.GetValues('Services');
            /*
            IEnumerable<string> values;
            if (response.Headers.TryGetValues("Services", out values))
            {
                string session = values.First();
                Console.WriteLine(session);
            }
            */
            return Convert.ToInt32(response.StatusCode);
        }
    }
}
