//-----------------------------------------------------------------------
//  <author>https://github.com/dant02</author>
//  <date>2025-03-08</date>
//-----------------------------------------------------------------------

namespace app
{
    using Vlc.DotNet.Core;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            DirectoryInfo info = new(@"C:\Program Files\VideoLAN\VLC");

            var file = new FileInfo(@"d:\music\my.flac");

            var player = new VlcMediaPlayer(info);
            player.EncounteredError += Player_EncounteredError;

            player.Play(file);

            app.Run();
        }

        private static void Player_EncounteredError(object? sender, VlcMediaPlayerEncounteredErrorEventArgs e)
        {
        }
    }
}