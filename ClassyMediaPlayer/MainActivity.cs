using Android.App;
using Android.OS;
using Android.Content;

using Android.Widget;
using Android.Media;
using Java.IO;

using System.Collections.Generic;
using Android.Views;

namespace ClassyMediaPlayer
{
    [Activity(Label = "@string/app_name", Theme = "@android:style/Theme.Holo.NoActionBar.TranslucentDecor",
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, MainLauncher = true)]
    public class MainActivity : Activity
    {
        private MediaPlayer mediaPlayer;
        private ArrayAdapter adapter;         
        public static IList<string> list { get; private set; }

        int pos;
        bool videoCompleted = false;

        VideoView videoView;
        TextView textView;
        Button btnOpen, btnPlay, btnStop, btnRepeat, btnNext, btnPrevious;
        Android.Net.Uri uri;        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            btnOpen = FindViewById<Button>(Resource.Id.open);
            btnPlay = FindViewById<Button>(Resource.Id.play);
            btnStop = FindViewById<Button>(Resource.Id.stop);
            btnRepeat = FindViewById<Button>(Resource.Id.repeat);
            btnPrevious = FindViewById<Button>(Resource.Id.previous);
            btnNext = FindViewById<Button>(Resource.Id.next);           
            
            videoView = FindViewById<VideoView>(Resource.Id.videoView);
            videoView.SetMediaController(new MediaController(this));
            textView = FindViewById<TextView>(Resource.Id.textView);
            list = new List<string>();
            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1);
            pos = 0;
            
            btnOpen.Click += delegate {

                adapter.Clear();
                textView.Text = "";
                Intent mediaIntent = new Intent(Intent.ActionGetContent);
                mediaIntent.SetType("*/*");
                string[] mimetypes = { "audio/*", "video/*" };
                mediaIntent.PutExtra(Intent.ExtraMimeTypes, mimetypes);
                mediaIntent.PutExtra(Intent.ExtraAllowMultiple, true);
                StartActivityForResult(Intent.CreateChooser(mediaIntent, "Choose a file"), 0);

                btnStop.Enabled = false;
                btnNext.Enabled = false;
                btnRepeat.Enabled = false;
                btnPrevious.Enabled = false;
                btnPlay.Enabled = false;
                				
            };

            btnPlay.Click += delegate {
                btnPlay.Enabled = !btnPlay.Enabled;

                pos = 0;

                if (adapter.Count != 0)
                {                    
                    videoView.SetMediaController(new MediaController(this));                    
                    PlayFile();
                    btnRepeat.Enabled = true;
                    btnStop.Enabled = true;
                    DisplayTrackInfo();
                }                                     

                if (pos < (adapter.Count - 1))
                {
                    btnNext.Enabled = true;
                    btnRepeat.Enabled = true;
                }
            };

            btnStop.Click += delegate {
                Stop();
            };

            btnNext.Click += delegate {
                PlayNextTrack(pos);
            };

            btnPrevious.Click += delegate {
                Previous(pos);
            };

            btnRepeat.Click += delegate {
                Repeat(pos);
            };

            videoView.Completion += delegate {

                videoCompleted = true;
                btnPrevious.Enabled = true;
                PlayNextTrack(pos);
            };
        }

        public void PlayFile()
        {
            videoView.SetVideoURI(Android.Net.Uri.Parse(adapter.GetItem(pos).ToString()));
            videoView.RequestFocus();
            videoView.Start();
        }

        public void DisplayTrackInfo()
        {
            textView.Text = "";            
            string s = SelectedFilePath.GetActualPathFromFile(this, Android.Net.Uri.Parse(adapter.GetItem(pos).ToString()));
            File file = new File(s);
            textView.Text = file.Name;               
        }

        public void Stop()
        {
            videoView.StopPlayback();
            videoView.ClearFocus();
            textView.Text = "";

            btnStop.Enabled = false;
            btnNext.Enabled = false;
            btnRepeat.Enabled = false;
            btnPrevious.Enabled = false;
            btnPlay.Enabled= true;
        }

        public void PlayNextTrack(int trackIndex)
        {
            pos = trackIndex;

            if (pos < (adapter.Count - 1))
            {
                pos = pos + 1;

                PlayFile();
                DisplayTrackInfo();
                btnPrevious.Enabled = true;

                if (pos == (adapter.Count) -1)
                {
                    btnNext.Enabled = false;
                }
            }
            else
            {
                Stop();
            }
        }

        public void Repeat(int trackIndex)
        {
            pos = trackIndex;

            PlayFile();            
        }

        public void Previous(int trackIndex)
        {
            pos = trackIndex;

            if (pos != 0)
            {
                pos = pos - 1;

                PlayFile();
                DisplayTrackInfo();

                if (pos < (adapter.Count) - 1)
                {
                    btnNext.Enabled = true;
                }

                if (pos == 0)
                {
                    btnPrevious.Enabled = false;
                }
            }            
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                if (null != data)
                { 
                    if (null != data.ClipData)
                    { 
                        for (int i = 0; i < data.ClipData.ItemCount; i++)
                        {
                            uri = data.ClipData.GetItemAt(i).Uri;                            
                            adapter.Add(uri);
                        }
                    }
                    else
                    {
                        uri = data.Data;
                        adapter.Add(uri);
                    }                    
                }

                if (adapter.Count != 0)
                {
                    btnPlay.Enabled = true;
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }             

        protected void OnBackPressed()
        {
            base.OnBackPressed();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }       
    }
}

