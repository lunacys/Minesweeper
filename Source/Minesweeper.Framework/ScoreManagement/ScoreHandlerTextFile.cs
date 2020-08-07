using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Minesweeper.Framework.ScoreManagement
{
    public class ScoreHandlerTextFile : IScoreHandler
    {
        public string FilePath { get; set; }
        
        public ScoreHandlerTextFile(string filePath)
        {
            FilePath = filePath;
        }
        
        public void Store(string playerId, Score score)
        {
            if (File.Exists(FilePath))
            {
                ScoreContainer scoreContainer = null;
                
                using (var sr = new StreamReader(FilePath))
                {
                    var text = sr.ReadToEnd();
                    
                    try
                    {
                        scoreContainer = JsonConvert.DeserializeObject<ScoreContainer>(text);
                    }
                    catch (Exception e)
                    {
                        scoreContainer = new ScoreContainer();
                    }
                }
                
                if (scoreContainer == null)
                {
                    scoreContainer = new ScoreContainer();
                }
                    
                scoreContainer.Add(playerId, score);
                SaveToFile(scoreContainer);
            }
            else
            {
                var scoreContainer = new ScoreContainer();
                scoreContainer.Add(playerId, score);
                
                SaveToFile(scoreContainer);
            }
        }

        public IEnumerable<Score> GetScoresForPlayerId(string playerId)
        {
            if (!File.Exists(FilePath))
                return null;
                // throw new FileNotFoundException("Not found scores file: " + FilePath);

            using (var sr = new StreamReader(FilePath))
            {
                var text = sr.ReadToEnd();
                var dict = JsonConvert.DeserializeObject<ScoreContainer>(text);

                return dict.Scores[playerId].Values;
            }
        }

        private void SaveToFile(ScoreContainer container)
        {
            using (var sw = new StreamWriter(FilePath))
            {
                var obj = JsonConvert.SerializeObject(container, Formatting.Indented);
                sw.WriteLine(obj);
            }
        }
    }
}