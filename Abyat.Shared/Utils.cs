using System.IO;
using System.Linq;

namespace Abyat.Shared
{
    public class Utils
    {
        /// <summary>
        /// returns file type. ex:basketball
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public static string GetFileType(string fileName)
        {
            return File.ReadLines(fileName).First().Trim().ToLower();
        }
    }
}