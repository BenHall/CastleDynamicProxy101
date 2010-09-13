using System;

namespace CastleDynamicProxy101
{
    public class TestabilityTest
    {
        public static void CreateUntestableTestableObject()
        {
            Testability testability = new Testability();
            var instanceMissing = testability.MakeUntestableTestable<INice>(typeof(NastyStatic));
            var instanceMissing2 = testability.MakeUntestableTestable<INice>(new NastySealed());

            Console.WriteLine(instanceMissing.Blah());
            Console.WriteLine(instanceMissing2.Blah());
        }

        public static void CreateUntestableTestableFileReader()
        {
            Testability testability = new Testability();
            IFileReader fileReader = testability.MakeUntestableTestable<IFileReader>(typeof(System.IO.File));
            Console.WriteLine(fileReader.ReadAllText("C:\\tmp.txt"));
        }

        public interface IFileReader
        {
            string ReadAllText(string path);
        }

    }


}