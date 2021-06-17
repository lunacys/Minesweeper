using System.Collections.Generic;

namespace Minesweeper.Framework.ScoreManagement
{
    public interface IScoreHandler
    {
        // TODO: Need to add separation by field size
        void Store(string playerId, Score score);
        IEnumerable<Score> GetScoresForPlayerId(string playerId);
    }
}