using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestTask
{
    class Term
    {
        string _startFolder;
        public string StartFolder
        {
            get { return _startFolder; }
            set { _startFolder = value; }
        }
        string _pattern;
        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; }
        }
        string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Term()
        {
            
        }

        public Term(string startFolder, string pattern, string text)
        {
            StartFolder = startFolder;
            Pattern = pattern;
            Text = text;
        }

        public void LoadTermFile(string saveFile)
        {
            var xDoc = new XmlDocument();
            if (!File.Exists(saveFile))
            {
                XmlTextWriter textWritter = new XmlTextWriter(saveFile, Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("term");
                textWritter.WriteEndDocument();
                textWritter.Close();
            }
            xDoc.Load(saveFile);
            XmlElement xRoot = xDoc.DocumentElement;             
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == "StartFolder")
                    this.StartFolder = xnode.InnerText;
                if (xnode.Name == "Pattern")
                    this.Pattern = xnode.InnerText;
                if (xnode.Name == "Text")
                    this.Text = xnode.InnerText;
            }
            xDoc.Save(saveFile);
        }

        public void SaveTermFile(string saveFile)
        {            
            var xDoc = new XmlDocument();
            xDoc.Load(saveFile);
            // удаление из файла существующей информации
            XmlElement xRoot = xDoc.DocumentElement;
            xRoot.RemoveAll();
            // добавление актуальных данных
            XmlNode element = xDoc.CreateElement("StartFolder");
            element.InnerText = this.StartFolder;
            xDoc.DocumentElement.AppendChild(element);

            element = xDoc.CreateElement("Pattern");
            element.InnerText = this.Pattern;
            xDoc.DocumentElement.AppendChild(element);

            element = xDoc.CreateElement("Text");
            element.InnerText = this.Text;
            xDoc.DocumentElement.AppendChild(element);
            xDoc.Save(saveFile);
        }
    }
}
