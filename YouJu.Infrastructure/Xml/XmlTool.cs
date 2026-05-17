using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace YouJu.Infrastructure.Xml
{
    public static class XmlTool
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> _cache;

        private static XmlSerializerNamespaces _defaultNamespace;

        static XmlTool()
        {
            _defaultNamespace = new XmlSerializerNamespaces();
            _defaultNamespace.Add(string.Empty, string.Empty);
            _cache = new ConcurrentDictionary<Type, XmlSerializer>();
        }

        private static XmlSerializer GetSerializer<T>()
        {
            XmlSerializer value = null;
            Type typeFromHandle = typeof(T);
            if (_cache.ContainsKey(typeFromHandle))
            {
                _cache.TryGetValue(typeFromHandle, out value);
            }

            if (value == null)
            {
                value = new XmlSerializer(typeFromHandle);
                _cache.TryAdd(typeFromHandle, value);
            }

            return value;
        }

        public static string XmlSerialize<T>(this T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                GetSerializer<T>().Serialize(memoryStream, obj, _defaultNamespace);
                return Encoding.UTF8.GetString(memoryStream.GetBuffer());
            }
        }

        public static T XmlDeserialize<T>(this string xml)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                object obj = xmlSerializer.Deserialize(stream);
                return (obj == null) ? default(T) : ((T)obj);
            }
        }

        public static bool SaveXml(string strXmlContent, string strFilePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(strXmlContent);
            xmlDocument.Save(strFilePath);
            return true;
        }

        public static bool SaveXml<T>(T t, string strFilePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(t.XmlSerialize());
            xmlDocument.Save(strFilePath);
            return true;
        }

        public static T GetModel<T>(string strFilePath)
        {
            if (!File.Exists(strFilePath))
            {
                return default(T);
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(strFilePath);
            string innerXml = xmlDocument.InnerXml;
            return innerXml.XmlDeserialize<T>();
        }
    }
}

