using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DansCSharpLibrary.JsonSerialization
{
    /// <summary>
    /// Functions for performing common Json Serialization operations.
    /// <para>Requires the Newtonsoft.Json assembly (Json.Net package in NuGet Gallery) to be referenced in your project.</para>
    /// <para>Only public properties and variables will be serialized.</para>
    /// <para>Use the [JsonIgnore] attribute to ignore specific public properties or variables.</para>
    /// <para>Object to be serialized must have a parameterless constructor.</para>
    /// </summary>
    public static class JsonSerialization
    {
        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false, Formatting formatting = Formatting.None) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, formatting);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Test Serialization

    public class Person
    {
        public string Name { get; set; }
        public int Age = 20;
        public Address HomeAddress { get; set; }
#pragma warning disable
        private string _thisWillNotGetWrittenToTheFile = "because it is not public.";
#pragma warning restore

        [JsonIgnore]
        public string ThisWillNotBeWrittenToTheFile = "because of the [JsonIgnore] attribute.";
    }

    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
    }

    //Test With:
    //using DansCSharpLibrary.JsonSerialization;
    //..
    //TestSerialization.Write(GlobalFramework.Path["temp"].ToString());
    //TestSerialization.Read(GlobalFramework.Path["temp"].ToString());
    public static class TestSerialization
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Write(String pFilePath)
        {
            //Init List<Person>
            List<Person> people = new List<Person>();
            //Init Persons
            Person person1 = new Person() { Name = "Mário", Age = 28, HomeAddress = new Address() { StreetAddress = "123 My St", City = "Figueira da Foz" } };
            Person person2 = new Person() { Name = "Carlos", Age = 30, HomeAddress = new Address() { StreetAddress = "456 My St", City = "Figueira da Foz" } };
            Person person3 = new Person() { Name = "Luís", Age = 32, HomeAddress = new Address() { StreetAddress = "789 My St", City = "Figueira da Foz" } };
            people.Add(person1);
            people.Add(person2);
            people.Add(person3);
            //Write Individual Persons
            JsonSerialization.WriteToJsonFile<Person>(Path.Combine(pFilePath, "person1.json"), person1);
            JsonSerialization.WriteToJsonFile<Person>(Path.Combine(pFilePath, "person2.json"), person2);
            JsonSerialization.WriteToJsonFile<Person>(Path.Combine(pFilePath, "person3.json"), person3);
            //Write List<Person> Object
            JsonSerialization.WriteToJsonFile<List<Person>>(Path.Combine(pFilePath, "people.json"), people);
        }

        public static void Read(String pFilePath)
        {
            Person person1 = JsonSerialization.ReadFromJsonFile<Person>(Path.Combine(pFilePath, "person1.json"));
            Person person2 = JsonSerialization.ReadFromJsonFile<Person>(Path.Combine(pFilePath, "person2.json"));
            Person person3 = JsonSerialization.ReadFromJsonFile<Person>(Path.Combine(pFilePath, "person3.json"));
            List<Person> people = JsonSerialization.ReadFromJsonFile<List<Person>>(Path.Combine(pFilePath, "people.json"));

            _log.Debug(string.Format("Read(): Name:[{0}], Age:[{1}], StreetAddress:[{2}], City:[{3}]", person1.Name, person1.Age, person1.HomeAddress.StreetAddress, person1.HomeAddress.City));
            _log.Debug(string.Format("Read(): Name:[{0}], Age:[{1}], StreetAddress:[{2}], City:[{3}]", person2.Name, person2.Age, person2.HomeAddress.StreetAddress, person2.HomeAddress.City));
            _log.Debug(string.Format("Read(): Name:[{0}], Age:[{1}], StreetAddress:[{2}], City:[{3}]", person3.Name, person3.Age, person3.HomeAddress.StreetAddress, person3.HomeAddress.City));

            foreach (Person person in people)
            {
                if (person.Name != null) Console.Write(string.Format("Name:[{0}] ", person.Name));
                if (person.Age > 0) Console.Write(string.Format("Age:[{0}] ", person.Age));
                if (person.HomeAddress != null)
                {
                    if (person.HomeAddress.StreetAddress != null) Console.Write(string.Format("StreetAddress:[{0}] ", person.HomeAddress.StreetAddress));
                    if (person.HomeAddress.City != null) Console.Write(string.Format("City:[{0}] ", person.HomeAddress.City));
                }
                Console.WriteLine();
            }
        }
    }
}