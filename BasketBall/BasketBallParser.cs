using Abyat.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BasketBall
{
    public class BasketBallParser : ISportParser
    {
        public string Name => "basketball";

        private const int MaxRosterCount = 12;
        private const int WinnerTeamBonus = 10;

        private enum columns { PlayerName = 0, Nickname, Number, TeamName, Position, ScoredPoints, Rebounds, Assists };

        private enum positions { G, F, C };

        private Dictionary<string, Dictionary<columns, int>> ratings = new Dictionary<string, Dictionary<columns, int>>
        {
            {"G", new Dictionary<columns, int>{ {columns.ScoredPoints,2 }, { columns.Rebounds, 3 }, { columns.Assists, 1 }}},
            {"F", new Dictionary<columns, int>{ {columns.ScoredPoints,2 }, { columns.Rebounds, 2 }, { columns.Assists, 2 }}},
            {"C", new Dictionary<columns, int>{ {columns.ScoredPoints,2 }, { columns.Rebounds, 1 }, { columns.Assists, 3 }}}
        };

        private IEnumerable<PlayerScore> playerScores = null;

        public (bool, string) ProcessFile(string fileName)
        {
            try
            {
                var data = from l in File.ReadLines(fileName).Skip(1)
                           let x = l.Split(";")
                                    .ToArray()
                           select new
                           {
                               PlayerName = x[(int)columns.PlayerName],
                               Nickname = x[(int)columns.Nickname],
                               Number = Convert.ToInt32(x[(int)columns.Number]),
                               Team = x[(int)columns.TeamName],
                               Position = x[(int)columns.Position],
                               ScoredPoints = Convert.ToInt32(x[(int)columns.ScoredPoints]),
                               Rebounds = Convert.ToInt32(x[(int)columns.Rebounds]),
                               Assists = Convert.ToInt32(x[(int)columns.Assists]),
                           };

                var playersCount = data.Select(x => x.Nickname).Distinct().Count();

                if (playersCount != data.Count())
                {
                    return (false, "Nickname should be unique");
                }

                var teams = data.GroupBy(x => x.Team);

                var NumberofTeams = teams.Count();

                if (NumberofTeams != 2)
                {
                    return (false, "File should contain two teams only!");
                }

                if (teams.First().Count() != teams.Last().Count())
                {
                    return (false, "Teams should have the same number of players");
                }

                if (teams.First().Count() > MaxRosterCount)
                {
                    return (false, $"A basketaball team can consist of a max of {MaxRosterCount}");
                }

                var winnerTeam = teams.OrderByDescending(t => t.Sum(x => x.ScoredPoints)).First().Key;

                playerScores = data.Select(x => new PlayerScore { Nickname = x.Nickname, Score = CalculatePlayerScore(x.Team, winnerTeam, x.Position, x.ScoredPoints, x.Assists, x.Rebounds) });
            }
            catch (Exception ex)
            {
                //log exception
                return (false, "Invalid File");
            }
            return (true, string.Empty);
        }

        private decimal CalculatePlayerScore(string playerTeam, string winnerTeam, string position, int scoredPoints, int assists, int rebounds)
        {
            var rating = ratings[position];

            var playerScore = scoredPoints * rating[columns.ScoredPoints] + assists * rating[columns.Assists] + rebounds * rating[columns.Rebounds];

            return playerTeam == winnerTeam ? playerScore + WinnerTeamBonus : playerScore;
        }

        public IEnumerable<PlayerScore> GetScores()
        {
            return playerScores;
        }
    }
}
