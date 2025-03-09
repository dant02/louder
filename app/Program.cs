//-----------------------------------------------------------------------
//  <author>https://github.com/dant02</author>
//  <date>2025-03-08</date>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;
using LibVLCSharp.Shared;

namespace app
{
    public class Program
    {
        private static LibVLC? libvlc;
        private static MediaPlayer? player;

        public static void Main(string[] args)
        {
            var sound = @"/home/my.flac";
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Core.Initialize(@"C:\Program Files\VideoLAN\VLC");
                sound = @"d:\music\my.flac";
            }

            libvlc = new LibVLC("--verbose=2");
            player = new MediaPlayer(libvlc);

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => Results.File(@"index.html", contentType: "text/html"));

            app.MapGet("/play", () =>
            {
                var media = new Media(libvlc, new Uri(sound));
                player.Play(media);
            });

            app.MapGet("/stop", () =>
            {
                player.Stop();
            });

            app.Run();
        }
    }
}