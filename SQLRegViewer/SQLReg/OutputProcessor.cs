using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.Xml.Serialization;
using System.Xml;
using System.IO;
 
using System.Runtime.Serialization;

namespace SQLReg
{
    public static class OutputProcessor
    {
        //https://www.codeproject.com/Articles/483055/XML-Serialization-and-Deserialization-Part


 
        public static Object DeserializeFromXML<T>(string fileName)
        {

            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            TextReader reader = new StreamReader(fileName);
            object obj = deserializer.Deserialize(reader);
            T XmlData = (T)obj;

            return obj;
        }

        public static string SerializeToXML<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                

                var xmlserializer = new XmlSerializer(typeof(T));
              

                var stringWriter = new StringWriter();
             
                using (XmlTextWriter writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[SerializeToXML]An error occurred", ex);
            }
        }

        public static string DataContractSerializeToXML<T>(this T value)
        {

            var serializer = new DataContractSerializer(typeof(T));
            string xmlString="";
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                    serializer.WriteObject(writer,value);
                    writer.Flush();
                    xmlString = sw.ToString();
                }
            }

            return xmlString;
        }



        public static T DataContractDeSerializeToXML<T>(string fileName)
        {

            FileStream fs = new FileStream(fileName,
             FileMode.Open);

            XmlDictionaryReaderQuotas qt = new XmlDictionaryReaderQuotas();
            qt.MaxStringContentLength = 809200;

            XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(fs,qt );
            DataContractSerializer ser = new DataContractSerializer(typeof(T));

            // Deserialize the data and read it from the instance.
            T obj =
                (T)ser.ReadObject(reader, true);
            reader.Close();
            fs.Close();

            return obj;


        }




    }
}
