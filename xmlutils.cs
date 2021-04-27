// Originally written by algernon for Find It 2.
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Propitize
{
    /// <summary>
    /// XML serialization/deserialization utilities class.
    /// </summary>
    internal static class XMLUtils
    {
        internal const string SettingsFileName = "PropitizedTrees.xml";

        /// <summary>
        /// Load settings from XML file.
        /// </summary>
        internal static void LoadSettings()
        {
            try
            {
                // Check to see if configuration file exists.
                if (File.Exists(SettingsFileName))
                {
                    // Read it.
                    using (StreamReader reader = new StreamReader(SettingsFileName))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLSettingsFile));
                        if (!(xmlSerializer.Deserialize(reader) is XMLSettingsFile xmlSettingsFile))
                        {
                            Debug.Log("Propitize: Couldn't deserialize settings file");
                        }
                    }
                }
                else
                {
                    Debug.Log("Propitize: No settings file found");
                }
            }
            catch (Exception e)
            {
                Debug.Log("Propitize: Exception reading XML settings file");
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        internal static void SaveSettings()
        {
            try
            {
                // Pretty straightforward.  Serialisation is within GBRSettingsFile class.
                using (StreamWriter writer = new StreamWriter(SettingsFileName))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLSettingsFile));
                    xmlSerializer.Serialize(writer, new XMLSettingsFile());
                }
            }
            catch (Exception e)
            {
                Debug.Log("Propitize: exception saving XML settings file");
                Debug.LogException(e);
            }
        }
    }

    /// <summary>
    /// Class to hold global mod settings.
    /// </summary>
    [XmlRoot(ElementName = "Propitize", Namespace = "", IsNullable = false)]
    internal static class Settings
    {
        internal static List<PropitizedTreeEntry> PropitizedTreeEntries = new List<PropitizedTreeEntry>();
    }

    /// <summary>
    /// Defines the XML settings file.
    /// </summary>
    [XmlRoot(ElementName = "Propitize", Namespace = "", IsNullable = false)]
    public class XMLSettingsFile
    {
        [XmlArray("PropitizedTreesArray")]
        [XmlArrayItem("PropitizedTreesEntries")]
        public List<PropitizedTreeEntry> PropitizedTreeEntries { get => Settings.PropitizedTreeEntries; set => Settings.PropitizedTreeEntries = value; }

    }

    public class PropitizedTreeEntry
    {
        [XmlAttribute("Name")]
        public string name = "";

        public PropitizedTreeEntry() { }

        public PropitizedTreeEntry(string newName) { name = newName; }
    }
}