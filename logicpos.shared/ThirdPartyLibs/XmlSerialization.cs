using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DansCSharpLibrary.XMLSerialization
{
    //Writing and Reading an object to / from an XML file (Using System.Xml.Serialization.XmlSerializer in the System.Xml assembly)
    //
    //Only writes and reads the Public properties and variables to / from the file.
    //Classes to be serialized must contain a public parameterless constructor.
    //The data saved to the file is human readable, so it can easily be edited outside of your application.
    //Use the [XmlIgnore] attribute to exclude a public property or variable from being written to the file.

    /// <summary>
    /// Functions for performing common XML Serialization operations.
    /// <para>Only public properties and variables will be serialized.</para>
    /// <para>Use the [XmlIgnore] attribute to prevent a property/variable from being serialized.</para>
    /// <para>Object to be serialized must have a parameterless constructor.</para>
    /// </summary>
    public static class XmlSerialization
    {
        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
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

        [XmlIgnore]
        public string ThisWillNotBeWrittenToTheFile = "because of the [XmlIgnore] attribute.";
    }

    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
    }

    //Test With:
    //using DansCSharpLibrary.XMLSerialization;
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
            XmlSerialization.WriteToXmlFile<Person>(Path.Combine(pFilePath, "person1.xml"), person1);
            XmlSerialization.WriteToXmlFile<Person>(Path.Combine(pFilePath, "person2.xml"), person2);
            XmlSerialization.WriteToXmlFile<Person>(Path.Combine(pFilePath, "person3.xml"), person3);
            //Write List<Person> Object
            XmlSerialization.WriteToXmlFile<List<Person>>(Path.Combine(pFilePath, "people.xml"), people);
        }

        public static void Read(String pFilePath)
        {
            Person person1 = XmlSerialization.ReadFromXmlFile<Person>(Path.Combine(pFilePath, "person1.xml"));
            Person person2 = XmlSerialization.ReadFromXmlFile<Person>(Path.Combine(pFilePath, "person2.xml"));
            Person person3 = XmlSerialization.ReadFromXmlFile<Person>(Path.Combine(pFilePath, "person3.xml"));
            List<Person> people = XmlSerialization.ReadFromXmlFile<List<Person>>(Path.Combine(pFilePath, "people.xml"));

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