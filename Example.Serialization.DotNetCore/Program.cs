using System;
using RockLib.Serialization;

namespace Example.Serialization.DotNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var item = new ParentExampleClass
            {
                DateProperty = DateTime.Now,
                ClassProperty = new NestedExampleClass
                {
                    IntProperty = 15,
                    BoolProperty = true,
                    StringProperty = "The string to serialize"
                }
            };

            var json = item.ToJson();

            Console.WriteLine("Serialized Json:");
            Console.WriteLine(json);

            var jsonDeserialized = json.FromJson<ParentExampleClass>();

            Console.WriteLine();
            if (item.DateProperty.ToString("O") == jsonDeserialized.DateProperty.ToString("O")
                && item.ClassProperty.IntProperty == jsonDeserialized.ClassProperty.IntProperty
                && item.ClassProperty.BoolProperty == jsonDeserialized.ClassProperty.BoolProperty
                && item.ClassProperty.StringProperty == jsonDeserialized.ClassProperty.StringProperty)
                Console.WriteLine("Json Deserialization Result: Success");
            else
                Console.WriteLine("Json Deserialization Result: Failed");

            var xml = item.ToXml();

            Console.WriteLine();
            Console.WriteLine("Serialized Xml:");
            Console.WriteLine(xml);

            var xmlDeserialized = xml.FromXml<ParentExampleClass>();

            Console.WriteLine();
            if (item.DateProperty.ToString("O") == xmlDeserialized.DateProperty.ToString("O")
                && item.ClassProperty.IntProperty == xmlDeserialized.ClassProperty.IntProperty
                && item.ClassProperty.BoolProperty == xmlDeserialized.ClassProperty.BoolProperty
                && item.ClassProperty.StringProperty == xmlDeserialized.ClassProperty.StringProperty)
                Console.WriteLine("Xml Deserialization Result: Success");
            else
                Console.WriteLine("Xml Deserialization Result: Failed");

            Console.ReadKey();
        }
    }

    public class ParentExampleClass
    {
        public DateTime DateProperty { get; set; }
        public NestedExampleClass ClassProperty { get; set; }
    }

    public class NestedExampleClass
    {
        public int IntProperty { get; set; }
        public bool BoolProperty { get; set; }
        public string StringProperty { get; set; }
    }
}
