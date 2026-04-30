using Lunar.Core.Utilities;
using Lunar.Core.World.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lunar.Server.World.Actors
{
    internal class ClassManager
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

            var jsonString = System.IO.File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;

            foreach (var classElement in root.GetProperty("classes").EnumerateArray())
            {
                string className = classElement.GetProperty("name").GetString();
                string texturePath = classElement.GetProperty("texture").GetString();

                var statsJson = classElement.GetProperty("stats");
                Stats stats = new Stats()
                {
                    Vitality = statsJson.GetProperty("health").GetInt32(),
                    Strength = statsJson.GetProperty("strength").GetInt32(),
                    Intelligence = statsJson.GetProperty("intelligence").GetInt32(),
                    Defense = statsJson.GetProperty("defense").GetInt32(),
                    Dexterity = statsJson.GetProperty("dexterity").GetInt32()
                };

                string startMap = classElement.GetProperty("startMap").GetString();

                ClassInformation classInfo = new ClassInformation(className, texturePath, stats, startMap);
                _classes.Add(classInfo.Name, classInfo);
            }

            Console.WriteLine($"Loaded {_classes.Count} classes.");
        }

        public void Initalize()
        {
            this.LoadClasses(Constants.FILEPATH_DATA + "classes.json");
        }
    }
}