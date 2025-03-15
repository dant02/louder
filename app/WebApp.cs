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
        private Media? currentMedia;
        private int currentMediaIndex;

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
            app.MapGet("/play", Play);
            app.MapGet("/stop", Stop);
            app.MapPost("/volume/{volume}", SetVolume);
        }

        public void Dispose()
        {
            player.Dispose();
            libvlc.Dispose();
        }

        public void Run() => app.Run();

        private void EndOfMedia()
        {
            currentMedia?.Dispose();
            currentMedia = null;
            currentMediaIndex++;
        }

        private void Play()
        {
            if (mediaPath == null)
                return;

            var files = Directory.EnumerateFiles(mediaPath);
            var cnt = files.Count();
            if (cnt == 0)
                return;

            if (cnt <= currentMediaIndex)
                currentMediaIndex = 0;

            var file = files.ElementAt(currentMediaIndex);
            currentMedia = new Media(libvlc, new Uri(file));
            player.Play(currentMedia);
        }

        private void Player_EndReached(object? sender, EventArgs e)
        {
            EndOfMedia();
        }

        private void SetVolume(int volume) => player.Volume = volume;

        private void Stop()
        {
            player.Stop();
            EndOfMedia();
        }
    }
}