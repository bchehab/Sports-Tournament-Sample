using Abyat.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BasketBall
{
    public class HandBallParser : ISportParser
    {
        public string Name => "handball";

        private const int MaxRosterCount = 14;
        private const int WinnerTeamBonus = 10;

        private enum columns { PlayerName = 0, Nickname, Number, TeamName, Position, GoalsMade, GoalsReceived, InitialRatingPoints };

        private enum positions { G, F, C };

        private Dictionary<string, Dictionary<columns, int>> ratings = new Dictionary<string, Dictionary<columns, int>>
        {
            {"G", new Dictionary<columns, int>{ {columns.InitialRatingPoints, 50 }, { columns.GoalsMade, 5 }, { columns.GoalsReceived, -2 }}},
            {"F", new Dictionary<columns, int>{ {columns.InitialRatingPoints, 20 }, { columns.GoalsMade, 1 }, { columns.GoalsReceived, -1 }}}
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
                               GoalsMade = Convert.ToInt32(x[(int)columns.GoalsMade]),
                               GoalsReceived = Convert.ToInt32(x[(int)columns.GoalsReceived])
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

                var winnerTeam = teams.OrderByDescending(t => t.Sum(x => x.GoalsMade)).First().Key;

                playerScores = data.Select(x => new PlayerScore { Nickname = x.Nickname, Score = CalculatePlayerScore(x.Team, winnerTeam, x.Position, x.GoalsMade, x.GoalsReceived) });
            }
            catch (Exception ex)
            {
                //log exception
                return (false, "Invalid File");
            }
            return (true, string.Empty);
        }

        private decimal CalculatePlayerScore(string playerTeam, string winnerTeam, string position, int goalsMade, int goalsReceived)
        {
            var rating = ratings[position];

            var playerScore = rating[columns.InitialRatingPoints] + goalsMade * rating[columns.GoalsMade] - goalsReceived * rating[columns.GoalsReceived];

            return playerTeam == winnerTeam ? playerScore + WinnerTeamBonus : playerScore;
        }

        public IEnumerable<PlayerScore> GetScores()
        {
            return playerScores;
        }
    }
}
