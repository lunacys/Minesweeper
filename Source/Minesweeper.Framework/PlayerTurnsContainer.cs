using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Framework.GameStateManagement;

namespace Minesweeper.Framework
{
    public class PlayerTurnsContainer
    {
        private List<PlayerTurnData> _playerTurns = new List<PlayerTurnData>();
        private List<PlayerTurnData> _undoneTurns = new List<PlayerTurnData>();

        public List<PlayerTurnData> PlayerTurns => _playerTurns;
        
        public MineField MineField { get; set; }
        public GameStateManager GameStateManager { get; }
        public bool IsUndoAvailable => _playerTurns.Count > 0;
        public bool IsRedoAvailable => _undoneTurns.Count > 0;

        public PlayerTurnsContainer(MineField mineField, GameStateManager gameStateManager)
        {
            MineField = mineField;
            GameStateManager = gameStateManager;
        }

        public void Clear()
        {
            _playerTurns.Clear();
        }

        public void AddTurn(MineFieldSnapshot mineFieldSnapshot, PlayerTurnSnapshot playerTurnSnapshot, string description, float time)
        {
            _playerTurns.Add(new PlayerTurnData(mineFieldSnapshot, playerTurnSnapshot, description, time, GameStateManager.CurrentState));
        }

        public void UndoTurn()
        {
            UndoTurn(_playerTurns.Count - 1);
        }

        public void RedoTurn()
        {
            var lastTurn = _undoneTurns.Last();
            MineField.RestoreFromSnapshot(lastTurn.MineFieldSnapshot);
            AddTurn(lastTurn.MineFieldSnapshot, lastTurn.PlayerTurnSnapshot, lastTurn.Description, lastTurn.Time);
            _undoneTurns.RemoveAt(_undoneTurns.Count - 1);
        }

        public void UndoTurn(int turnId)
        {
            var turn = _playerTurns[turnId];
            MineField.RestoreFromSnapshot(turn.MineFieldSnapshot);
            if (turn.GameState != GameStateManager.CurrentState)
            {
                GameStateManager.CurrentState = turn.GameState;
            }
            _undoneTurns.Add(turn);
            _playerTurns.RemoveAt(turnId);
        }
    }
}