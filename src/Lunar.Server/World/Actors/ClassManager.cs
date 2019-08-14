using Lunar.Core.Utilities;
using Lunar.Core.World.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lunar.Server.World.Actors
{
    class ClassManager : IService
    {
        private Dictionary<string, ClassInformation> _classes;


        public ICollection<ClassInformation> Classes { get => _classes.Values; }

        public ClassManager()
        {
            _classes = new Dictionary<string, ClassInformation>();
        }


        private void LoadClasses(string filePath)
        {
            Console.WriteLine("Loading class information...");

            var doc = XDocument.Load(filePath);

            var classNodes = doc.Element("Classes").Elements("Class");

            foreach (var classNode in classNodes)
            {
                string className = classNode.Attribute("name").Value.ToString();

                string texturePath = classNode.Element("Texture")?.Value;

                Stats stats = new Stats()
                {
                    Health = int.Parse(classNode.Element("Health")?.Value),
                    Strength = int.Parse(classNode.Element("Strength")?.Value),
                    Intelligence = int.Parse(classNode.Element("Intelligence")?.Value),
                    Defense = int.Parse(classNode.Element("Defense")?.Value),
                    CurrentHealth = int.Parse(classNode.Element("Health")?.Value),
                    Dexterity = int.Parse(classNode.Element("Dexterity")?.Value)
                };

                string startMap = classNode.Element("Start_Map")?.Value;

                ClassInformation classInfo = new ClassInformation(className, texturePath, stats, startMap);
                _classes.Add(classInfo.Name, classInfo);
            }

            Console.WriteLine($"Loaded {_classes.Count} classes.");
        }

        public void Initalize()
        {
            this.LoadClasses(Constants.FILEPATH_DATA + "classes.xml");
        }
    }
}
