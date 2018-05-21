using Android.App;
using Android.Widget;
using Android.OS;

using Android.Util;
using Emonitorage.shared;
using Emonitorage.Services;
using System;
using Firebase.Iid;
using WindowsAzure.Messaging;

namespace EAndroid
{
    [Activity(Label = "Connexion", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ApiLogin _apiLogin = new ApiLogin();
        private AlarmServerManager _a = new AlarmServerManager();

        EditText username;
        EditText pwd;
        Button connexionButton;
        public const string TAG = "MainActivity";


        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    if (key != null)
                    {
                        var value = Intent.Extras.GetString(key);
                        Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                    }
                }
            }
            if (UserItemManager.IsDB())
            {
                User u = UserItemManager.GetUser("a");
                if (u.IsLogged == 1)
                {
                    StartActivity(typeof(Accueil));
                }
            }
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            connexionButton = FindViewById<Button>(Resource.Id.connexionButton);
            username = FindViewById<EditText>(Resource.Id.idEditText);
            pwd = FindViewById<EditText>(Resource.Id.pswdEditText);

            // button clicks 
            connexionButton.Click += async (sender, e) => {

                if (UserItemManager.IsDB())
                {
                    var resp = await _apiLogin.LoginAsync(username.Text, pwd.Text);
                    if (resp == 200)
                    {
                        UserItemManager.ChangeStatus(username.Text, 1);
                        StartActivity(typeof(Accueil));
                    }
                }
                else
                {
             
                    var resp = await _apiLogin.FirstLoginAsync(username.Text, pwd.Text);
                   
                    if (resp == 200) {
                        /*var token = FirebaseInstanceId.Instance.Token;
                        NotificationHub hub = new NotificationHub(EAndroid.Resources.Constants.NotificationHubName,
                                                                  EAndroid.Resources.Constants.ListenConnectionString,
                                                                  Android.App.Application.Context);

                        hub.Register(token, "3");*/

                        StartActivity(typeof(Accueil));
                    }
                   
            

                }
            };
        }
    }
}

