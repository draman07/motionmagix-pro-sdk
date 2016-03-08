using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MotionMagixSimulator.Utility
{
    public static class CustomSerilization
    {
    
		public static string Log = "";

    /// <summary>
    /// This method converts any object into xml format.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
        public static string SerializeData(object data)
        {
            //StringBuilder sbData = new StringBuilder();
            //StringWriter swWriter;
            //XmlSerializer employeeSerializer = new XmlSerializer(typeof(object));

            //swWriter = new StringWriter(sbData);
            //employeeSerializer.Serialize(swWriter, data);
            //return sbData.ToString();

            if (data == null)
            {
                return string.Empty;
            }
            try
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(String.Empty, String.Empty);
                var xmlserializer = new XmlSerializer(data.GetType(),String.Empty);
                var stringWriter = new StringWriter();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                using (var writer = XmlWriter.Create(stringWriter,settings))
                {
                    xmlserializer.Serialize(writer, data,ns);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static  Object DeserializeData(string XMLString, Object YourClassObject)
        {

            XmlSerializer oXmlSerializer = new XmlSerializer(YourClassObject.GetType());

            //The StringReader will be the stream holder for the existing XML file 
            YourClassObject = oXmlSerializer.Deserialize(new StringReader(XMLString));


            //initially deserialized, the data is represented by an object without a defined type 
            return YourClassObject;
        }
    }
}
