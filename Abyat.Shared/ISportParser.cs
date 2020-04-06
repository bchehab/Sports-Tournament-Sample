using System.Collections.Generic;

namespace Abyat.Shared
{
    public interface ISportParser
    {
        /// <summary>
        /// returns true if the file was successfully read, and is a valid file
        /// </summary>
        /// <returns></returns>
        (bool, string) ProcessFile(string fileName);

        /// <summary>
        /// parser name
        /// </summary>
        string Name { get; }

        IEnumerable<PlayerScore> GetScores();
    }
}