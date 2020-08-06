using System;
using System.Collections.Generic;

namespace Minesweeper.Framework
{
    public class PlayerTurnsContainer
    {
        private List<PlayerTurnData> _playerTurns =
            new List<PlayerTurnData>();

        public List<PlayerTurnData> PlayerTurns => _playerTurns;
        
        public MineField MineField { get; set; }

        public PlayerTurnsContainer(MineField mineField)
        {
            MineField = mineField;
        }

        public void Clear()
        {
            _playerTurns.Clear();
        }

        public void AddTurn(MineFieldSnapshot mineFieldSnapshot, PlayerTurnSnapshot playerTurnSnapshot, string description, float time)
        {
            _playerTurns.Add(new PlayerTurnData(mineFieldSnapshot, playerTurnSnapshot, description, time));
        }

        public void UndoTurn()
        {
            UndoTurn(_playerTurns.Count - 1);
        }

        public void UndoTurn(int turnId)
        {
            MineField.RestoreFromSnapshot(_playerTurns[turnId].MineFieldSnapshot);
            _playerTurns.RemoveAt(turnId);
        }
    }
}