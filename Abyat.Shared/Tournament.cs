using System.Collections.Generic;

namespace Abyat.Shared
{
    public class Tournament
    {
        /// <summary>
        /// playerName, score
        /// </summary>
        private Dictionary<string, decimal> scores = new Dictionary<string, decimal>();
        private string mvp = string.Empty;

        public void UpdateScores(IEnumerable<PlayerScore> playerScores)
        {
            foreach (var playerScore in playerScores)
            {
                if (!scores.ContainsKey(playerScore.Nickname))
                {
                    scores[playerScore.Nickname] = playerScore.Score;
                }
                else
                {
                    scores[playerScore.Nickname] += playerScore.Score;
                }
                if (mvp == string.Empty || scores[playerScore.Nickname] > scores[mvp])
                {
                    mvp = playerScore.Nickname;
                }
            }
        }

        public string GetMVP()
        {
            return mvp;
        }

        public decimal GetPlayerScore(string nickname)
        {
            return scores.ContainsKey(nickname) ? scores[nickname] : 0;
        }
    }
}