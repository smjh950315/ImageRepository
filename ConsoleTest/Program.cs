using ImgRepo.Service.Helpers;

namespace ConsoleTest
{
    internal class Program
    {
        class ClassA
        {
            public string? Str;
            public string? Str2 { get; set; }
            public string? Str3;
        }
        static void Main(string[] args)
        {

            var img = File.ReadAllBytes(@"D:\srcShot.png");

            var fmtName = ImageHelper.GetFormat(img);

            var bytes = ImageHelper.Resize(img, 256, 256);

            File.WriteAllBytes(@"D:\dstShot.png", bytes);

            Console.WriteLine("Hello, World!");
        }
    }
}
