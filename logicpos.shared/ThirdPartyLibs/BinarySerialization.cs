using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using logicpos;

namespace DansCSharpLibrary.BinarySerialization
{
    //Writing and Reading an object to / from a Binary file
    //
    //Writes and reads ALL object properties and variables to / from the file (i.e. public, protected, public, and private).
    //The data saved to the file is not human readable, and thus cannot be edited outside of your application.
    //Have to decorate class (and all classes that it contains) with a [Serializable] attribute.
    //Use the [NonSerialized] attribute to exclude a variable from being written to the file; there is no way to prevent an auto-property from being serialized besides making it use a backing variable and putting the [NonSerialized] attribute on that.

    /// <summary>
    /// Functions for performing common binary Serialization operations.
    /// <para>All properties and variables will be serialized.</para>
    /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
    /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
    /// </summary>
    public static class BinarySerialization
    {
        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Test Serialization

    [Serializable]
    public class Person
    {
        public string Name { get; set; }
        public int Age = 20;
        public Address HomeAddress { get; set; }
#pragma warning disable
        private string _thisWillGetWrittenToTheFileToo = "even though it is a private variable.";
#pragma warning restore
    }

    [Serializable]
    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
    }

    //Test With:
    //using DansCSharpLibrary.BinarySerialization;
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
            BinarySerialization.WriteToBinaryFile<Person>(Path.Combine(pFilePath, "person1.bin"), person1);
            BinarySerialization.WriteToBinaryFile<Person>(Path.Combine(pFilePath, "person2.bin"), person2);
            BinarySerialization.WriteToBinaryFile<Person>(Path.Combine(pFilePath, "person3.bin"), person3);
            //Write List<Person> Object
            BinarySerialization.WriteToBinaryFile<List<Person>>(Path.Combine(pFilePath, "people.bin"), people);
        }

        public static void Read(String pFilePath)
        {
            Person person1 = BinarySerialization.ReadFromBinaryFile<Person>(Path.Combine(pFilePath, "person1.bin"));
            Person person2 = BinarySerialization.ReadFromBinaryFile<Person>(Path.Combine(pFilePath, "person2.bin"));
            Person person3 = BinarySerialization.ReadFromBinaryFile<Person>(Path.Combine(pFilePath, "person3.bin"));
            List<Person> people = BinarySerialization.ReadFromBinaryFile<List<Person>>(Path.Combine(pFilePath, "people.bin"));

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