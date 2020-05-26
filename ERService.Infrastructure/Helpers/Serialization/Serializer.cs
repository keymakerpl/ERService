using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ERService.Infrastructure.Helpers
{
    public static class Serializer
    {
        public static void Serialize(string fileName, object model)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, model);
            }
        }

        //TODO: make generic, return new obj type if note exist or 0 leng stream
        public static object Deserialize(string fileName)
        {
            object result = null;
            try
            {
                var formatter = new BinaryFormatter();
                using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    result = formatter.Deserialize(stream);
                }
            }
            catch (System.Exception ex)
            {
                
            }

            return result;
        }
    }
}
