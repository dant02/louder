//-----------------------------------------------------------------------
//  <author>https://github.com/dant02</author>
//  <date>2025-03-08</date>
//-----------------------------------------------------------------------

using Vlc.DotNet.Core;

namespace app
{
    public class Program
    {
        private static VlcMediaPlayer player;

        public static void Main(string[] args)
        {
            var info = new DirectoryInfo(@"C:\Program Files\VideoLAN\VLC");
            player = new VlcMediaPlayer(info);

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => Results.File(@"index.html", contentType: "text/html"));

            app.MapGet("/play", () =>
            {
                var file = new FileInfo(@"d:\music\my.flac");
                player.Play(file);
            });

            app.MapGet("/stop", () =>
            {
                player.Stop();
            });

            app.Run();
        }
    }
}