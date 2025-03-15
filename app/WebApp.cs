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
        private readonly MediaPlayer player;
        private readonly string sound;

        public WebApp(string[] args)
        {
            sound = @"/home/my.flac";
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Core.Initialize(@"C:\Program Files\VideoLAN\VLC");
                sound = @"d:\music\my.flac";
            }

            libvlc = new LibVLC("--verbose=2");
            player = new MediaPlayer(libvlc)
            {
                Volume = 20,
            };

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

        private void Play()
        {
            var media = new Media(libvlc, new Uri(sound));
            player.Play(media);
        }

        private void SetVolume(int volume) => player.Volume = volume;

        private void Stop() => player.Stop();
    }
}