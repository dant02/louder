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
        private static LibVLC libvlc = new LibVLC();
        private static MediaPlayer player = new MediaPlayer(libvlc);

        public static void Main(string[] args)
        {
            string bin = @"C:\Program Files\VideoLAN\VLC";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                bin = "/usr/bin/vlc";

            Core.Initialize(bin);

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => Results.File(@"index.html", contentType: "text/html"));

            app.MapGet("/play", () =>
            {
                var media = new Media(libvlc, new Uri(@"d:\music\my.flac"));
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