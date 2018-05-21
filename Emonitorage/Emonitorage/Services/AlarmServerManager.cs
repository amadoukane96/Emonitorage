using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Emonitorage.shared
{
    class AlarmServerManager
    {
        private static string url = "http://10.192.3.201/hub/index";
        public static async Task<IList<AlarmItem>> getAlarms(int profil)
        {
            AlarmItemManager.DeleteAlarms();
            var alarms = new List<AlarmItem>();
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("URL","refresh"),
                new KeyValuePair<string, string>("Profil",Convert.ToString(profil)),
            };
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new FormUrlEncodedContent(keyValues);
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            
            //Console.WriteLine(content);
            if (Convert.ToInt32(response.StatusCode) == 200)
            {
                var settings = new JsonSerializerSettings
                {  
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                alarms = JsonConvert.DeserializeObject<List<AlarmItem>>(content, settings);
                alarms.ForEach(delegate (AlarmItem a)
                {
                    AlarmItemManager.AddAlarm(a);
                });
                
            }
            return alarms;
        }

        public static async Task<bool> changeStatusAsync(int id, int idIntervenant)
        {
            // requete pour changer le statut de l'alarme dont l'id est passe en parametre
            // retourne vrai si le serveur a change la statut de l'alarme
            var keyValues = new List<KeyValuePair<string,string>>
            {
                new KeyValuePair<string, string>("URL","changestatus"),
                new KeyValuePair<string, string>("ID_Alarme",id.ToString()),
                new KeyValuePair<string, string>("ID_Intervenant",idIntervenant.ToString()),
            };
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new FormUrlEncodedContent(keyValues);
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            if (Convert.ToInt32(response.StatusCode) == 200)
            {
                return true;
            }
            return false;
        }
    }
}
