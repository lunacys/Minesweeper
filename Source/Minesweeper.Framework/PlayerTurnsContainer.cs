using System;
using System.Collections.Generic;

namespace Minesweeper.Framework
{
    public class PlayerTurnsContainer
    {
        private List<Tuple<MineFieldSnapshot, PlayerTurnSnapshot>> _playerTurns =
            new List<Tuple<MineFieldSnapshot, PlayerTurnSnapshot>>();

        public List<Tuple<MineFieldSnapshot, PlayerTurnSnapshot>> PlayerTurns => _playerTurns;
        
        public MineField MineField { get; set; }

        public PlayerTurnsContainer(MineField mineField)
        {
            MineField = mineField;
        }

        public void Clear()
        {
            _playerTurns.Clear();
        }

        public void AddTurn(MineFieldSnapshot mineFieldSnapshot, PlayerTurnSnapshot playerTurnSnapshot)
        {
            _playerTurns.Add(new Tuple<MineFieldSnapshot, PlayerTurnSnapshot>(mineFieldSnapshot, playerTurnSnapshot));
        }

        public void UndoTurn()
        {
            UndoTurn(_playerTurns.Count - 1);
        }

        public void UndoTurn(int turnId)
        {
            MineField.RestoreFromSnapshot(_playerTurns[turnId].Item1);
            _playerTurns.RemoveAt(turnId);
        }
    }
}