using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easymish.Web.ConfigurationSections
{
    public class BundlesConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection=true)]
        public BundleCollection Bundles
        {
            get
            {
                return (BundleCollection)this[""];
            }
            
        }
        //<bundles>
        //  <bundle name="~/bundles/js/script">
        //    <script>~/js/stickyfooter.js</script>
        //  </bundle>
        //</bundles>

        public class BundleCollection : ConfigurationElementCollection
        {
            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.BasicMap;
                }
            }
            protected override ConfigurationElement CreateNewElement()
            {
                return new BundleElement();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((BundleElement)element).Name;
            }

            protected override string ElementName
            {
                get
                {
                    return "bundle";
                }
            }
        }

        public class BundleElement : ConfigurationElement
        {
            [ConfigurationProperty("name", IsRequired=true, IsKey=true)]
            public string Name
            {
                get
                {
                    return this["name"].ToString();
                }
                set
                {
                    this["name"] = value;
                }
            }

            [ConfigurationProperty("", IsDefaultCollection = true)]
            public ScriptCollection Scripts
            {
                get
                {
                    return (ScriptCollection)this[""];
                }
            }
        }

        public class ScriptCollection : ConfigurationElementCollection
        {
            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.BasicMap;
                }
            }
            protected override ConfigurationElement CreateNewElement()
            {
                return new ScriptElement();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((ScriptElement)element).Value;
            }

            protected override string ElementName
            {
                get
                {
                    return "script";
                }
            }
        }

        public class ScriptElement : ConfigurationElement
        {
            protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
            {
                Value = reader.ReadInnerXml();
            }
            public string Value { get; set; }
        }
    }
}
