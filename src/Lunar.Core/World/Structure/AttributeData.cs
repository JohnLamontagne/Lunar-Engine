using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lunar.Core.World.Structure
{
    [Serializable]
    public class AttributeData
    {
        public byte[] Serialize()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(memoryStream, this);
            return memoryStream.ToArray();
        }

        public static AttributeData Deserialize(byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream(data);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            return (AttributeData) binaryFormatter.Deserialize(memoryStream);
        }
    }
}
