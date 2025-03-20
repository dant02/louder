//-----------------------------------------------------------------------
//  <author>https://github.com/dant02</author>
//  <date>2025-03-15</date>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;
using LibVLCSharp.Shared;

namespace app
{
    public sealed class WebApp : IDisposable
    {
        private readonly WebApplication app;
        private readonly LibVLC libvlc;
        private readonly string? mediaPath;
        private readonly MediaPlayer player;
        private bool autoplay = true;
        private Media? currentMedia;
        private int currentMediaIndex;
        private Timer? timer;

        public WebApp(string[] args)
        {
            if (args.Length > 0)
                mediaPath = args[0];

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Core.Initialize(@"C:\Program Files\VideoLAN\VLC");
            }

            libvlc = new LibVLC("--verbose=2");
            player = new MediaPlayer(libvlc)
            {
                Volume = 20,
            };

            player.EndReached += Player_EndReached;

            var builder = WebApplication.CreateBuilder(args);
            app = builder.Build();

            app.MapGet("/", () => Results.File(@"index.html", contentType: "text/html"));
            app.MapGet("/status", GetStatus);
            app.MapPost("/play", Play);
            app.MapPost("/stop", Stop);
            app.MapPost("/reset", Reset);
            app.MapPost("/volume/{volume}", SetVolume);
            app.MapPost("/autoplay/{isChecked}", AutoPlay);
        }

        public void Dispose()
        {
            player.Dispose();
            libvlc.Dispose();
        }

        public void Run() => app.Run();

        private void AutoPlay(bool isChecked)
        {
            lock (this)
                autoplay = isChecked;
        }

        private void EndOfMedia()
        {
            lock (this)
            {
                currentMedia?.Dispose();
                currentMedia = null;
                currentMediaIndex++;
            }
        }

        private Status GetStatus()
        {
            lock (this)
            {
                return new Status()
                {
                    IsPlaying = currentMedia != null,
                    FileName = currentMedia?.Mrl ?? string.Empty,
                    CurrentIndex = currentMediaIndex
                };
            }
        }

        private void Play()
        {
            lock (this)
            {
                if (mediaPath == null)
                    return;

                var enumerator = Directory.EnumerateFiles(mediaPath);
                var cnt = enumerator.Count();
                if (cnt == 0)
                    return;

                if (cnt <= currentMediaIndex)
                    currentMediaIndex = 0;

                var files = enumerator.ToList();
                files.Sort();
                currentMedia = new Media(libvlc, new Uri(files[currentMediaIndex]));
                player.Play(currentMedia);
            }
        }

        private void Player_EndReached(object? sender, EventArgs e)
        {
            lock (this)
            {
                EndOfMedia();

                if (autoplay)
                {
                    timer?.Dispose();
                    timer = new Timer((state) => Play(), null, 1, Timeout.Infinite);
                }
            }
        }

        private void Reset()
        {
            lock (this)
            {
                Stop();
                currentMediaIndex = 0;
            }
        }

        private void SetVolume(int volume)
        {
            lock (this)
                player.Volume = volume;
        }

        private void Stop()
        {
            lock (this)
            {
                player.Stop();
                EndOfMedia();
            }
        }
    }
}