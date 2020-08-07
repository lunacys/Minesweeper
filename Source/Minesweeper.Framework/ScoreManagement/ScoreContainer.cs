using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Framework.ScoreManagement
{
    public class ScoreContainer
    {
        // The scores are sorted by time
        public Dictionary<string, SortedList<float, Score>> Scores { get; set; }

        public ScoreContainer()
        {
            Scores = new Dictionary<string, SortedList<float, Score>>();
        }

        public Score GetHighScore(string playerId)
        {
            if (!Scores.ContainsKey(playerId))
                throw new Exception("No scores for player id: " + playerId);
            
            return Scores[playerId].FirstOrDefault().Value;
        }

        public void Add(string playerId, Score score)
        {
            if (Scores.ContainsKey(playerId))
            {
                if (Scores[playerId] == null)
                    Scores[playerId] = new SortedList<float, Score> {{ score.Time, score }};
                else
                    Scores[playerId].Add(score.Time, score);
            }
            else
            {
                Scores[playerId] = new SortedList<float, Score> {{ score.Time, score }};
            }
        }
    }
}