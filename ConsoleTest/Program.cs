using ImageHelperSharp;

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

            var img1 = File.ReadAllBytes(@"D:\test0.png");

            var img2 = File.ReadAllBytes(@"D:\test1.png");


            var diff = OpenCVService.GetImageDifferential(img1, img2);

            var cmp = OpenCVService.GetPatterMatchImage(img1, img2);

            File.WriteAllBytes(@"D:\diff.png", cmp);

            Console.WriteLine("Hello, World!");
        }
    }
}
