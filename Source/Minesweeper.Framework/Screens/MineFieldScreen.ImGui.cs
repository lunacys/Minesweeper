using System;
using System.Globalization;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Minesweeper.Framework.GameStateManagement;
using Minesweeper.Framework.MinePutters;

namespace Minesweeper.Framework.Screens
{
    public partial class MineFieldScreen
    {
        private int? _minimapTurnId = 1;
        private bool _isMinimapVisible = false;
        
        private void RenderImGuiLayout(GameTime gameTime)
        {
            _imGuiRenderer.BeforeLayout(gameTime);

            // ImGui.ShowMetricsWindow();
            
            RenderPlayerMoves();
            RenderGameSettings();
            // RenderMinimap();

            _imGuiRenderer.AfterLayout();
        }

        private void RenderPlayerMoves()
        {
            ImGui.Begin("Player Turns");
            for (int i = 0; i < _playerTurnsContainer.PlayerTurns.Count; i++)
            {
                var turn = _playerTurnsContainer.PlayerTurns[i];

                if (turn.PlayerTurnSnapshot == null) // Not a field turn
                {
                    ImGui.Text($"Turn #{i}: {turn.Description}");
                }
                else
                {
                    ImGui.Text($"Turn #{i} at {turn.PlayerTurnSnapshot.Position}: {turn.Description} | {turn.Time:F1}");
                }

                if (i != 0 && i == _playerTurnsContainer.PlayerTurns.Count - 1)
                {
                    ImGui.SameLine();
                    if (ImGui.Button("Undo"))
                    {
                        _playerTurnsContainer.UndoTurn(i);
                    }    
                }
            }
            ImGui.End();
        }

        private void RenderGameSettings()
        {
            bool useRecursiveOpen = _field.UseRecursiveOpen;
            
            ImGui.Begin("Game Settings");
            ImGui.Text($"TIME: {_gameTimeHandler.SecondsElapsed.ToString("F1", CultureInfo.InvariantCulture)}");
            ImGui.Text($"Total Mines: {_field.TotalMines}");
            ImGui.Text($"Total Cells: {_field.TotalCells}");
            ImGui.Text($"Field Resolution: {_field.Width}x{_field.Height}");
            ImGui.Text($"Mines Left: {_field.MinesLeft}");
            ImGui.Text($"Free cells: {_field.FreeCellsLeft}");
            ImGui.Text($"Total open cells: {_field.TotalOpenCells}");
            /*int seed = _field.Seed;
            ImGui.InputInt("Seed", ref seed, 1, 10);*/
            
            ImGui.Checkbox($"Use recursive open", ref useRecursiveOpen);
            ImGui.SameLine();
            HelpMarker("Recursively opens cells around the clicked cell if its number value is the same as flags around");

            if (ImGui.Button("New Game"))
            {
                Start();
            }
            ImGui.SameLine();
            if (ImGui.Button("Restart"))
            {
                _field.Reset();
            }

            if (_playerTurnsContainer.IsUndoAvailable)
            {
                ImGui.SameLine();
                if (ImGui.Button("Undo"))
                {
                    _playerTurnsContainer.UndoTurn();
                }
            }
            if (_playerTurnsContainer.IsRedoAvailable)
            {
                ImGui.SameLine();
                if (ImGui.Button("Redo"))
                {
                    _playerTurnsContainer.RedoTurn();
                }                
            }

            if (_gameStateManager.CurrentState != GameState.NewGame)
            {
                ImGui.SameLine();
                if (ImGui.Button("Solve"))
                {
                    var snapshot = _field.CreateSnapshot();
                    _field.Solve();
                    _playerTurnsContainer.AddTurn(
                        snapshot, null, "Solved the board automatically",
                        _gameTimeHandler.SecondsElapsed
                    );
                    _gameStateManager.CurrentState = GameState.Won;
                }    
            }

            ImGui.Separator();
            ImGui.SetNextItemWidth(ImGui.GetFontSize() * 6f);
            ImGui.InputInt("Field Width", ref _fieldWidth);
            ImGui.SetNextItemWidth(ImGui.GetFontSize() * 6f);
            ImGui.InputInt("Field Height", ref _fieldHeight);
            ImGui.SetNextItemWidth(ImGui.GetFontSize() * 6f);
            ImGui.InputInt("Total Mines", ref _totalMines);
            ImGui.SetNextItemWidth(ImGui.GetFontSize() * 8f);
            var names = Enum.GetNames(typeof(MinePutterDifficulty)); 
            ImGui.ListBox("Mine Putter Difficulty", ref _minePutterDifficulty,
                names, names.Length);
            
            if (ImGui.Button("Submit"))
            {
                Start();
            }
            
            ImGui.Separator();
            
            ImGui.Text("Difficulty Presets:");
            if (ImGui.Button("Easy"))
            {
                _fieldWidth = 9;
                _fieldHeight = 9;
                _totalMines = 10;
                Start();
            }
            ImGui.SameLine();
            HelpMarker("9x9, 10 mines");
            ImGui.SameLine();
            if (ImGui.Button("Medium"))
            {
                _fieldWidth = 16;
                _fieldHeight = 16;
                _totalMines = 40;
                Start();
            }
            
            ImGui.SameLine();
            HelpMarker("16x16, 40 mines");
            ImGui.SameLine();
            if (ImGui.Button("Hard"))
            {
                _fieldWidth = 30;
                _fieldHeight = 16;
                _totalMines = 99;
                Start();
            }
            ImGui.SameLine();
            HelpMarker("30x16, 99 mines");
            
            ImGui.End();

            _field.UseRecursiveOpen = useRecursiveOpen;
        }

        private void RenderMinimap()
        {
            if (_isMinimapVisible && _minimapTurnId.HasValue)
            {
                ImGui.BeginPopup("Modal");
                ImGui.Text("Modal text");
                ImGui.EndPopup();
            }
        }
    }
}