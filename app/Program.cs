//-----------------------------------------------------------------------
//  <author>https://github.com/dant02</author>
//  <date>2025-03-08</date>
//-----------------------------------------------------------------------

namespace app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var app = new WebApp(args);
            app.Run();
        }
    }
}