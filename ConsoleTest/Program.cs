using Cyh.Net.Data.Predicate;
using System.Linq;
using System.Linq.Expressions;

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

            List<ClassA> list = new List<ClassA>();
            list.Add(new ClassA()
            {
                Str = "0",
            });
            list.Add(new ClassA()
            {
                Str = null,
                Str2 = "str"
            });
            list.Add(new ClassA()
            {
                Str = "2",
            });

            var prdc = Predicate.NewLegacyPredicate<ClassA>(CompareType.Equal);
            var pdc1 = Predicate.GetExpression<ClassA>("Str2", CompareType.NotEqual, "str");
            var pdc2 = Predicate.GetExpression<ClassA, string>(x => x.Str??"", CompareType.Equal, "0");
            var hpdch = Predicate.NewPredicateHolder<ClassA>(x => true);
            hpdch.And(x => x.Str2 == null);
            hpdch.And(x => x.Str == "0");

            var r = list.Where(pdc2.Compile());
            ;


            Console.WriteLine("Hello, World!");
        }
    }
}
