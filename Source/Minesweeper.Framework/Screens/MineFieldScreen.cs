using System;
using System.Globalization;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minesweeper.Framework.GameStateManagement;
using Minesweeper.Framework.ImGUI;
using Minesweeper.Framework.Inputs;
using Minesweeper.Framework.MinePutters;
using MonoGame.Extended;
using MonoGame.Extended.Screens;

namespace Minesweeper.Framework.Screens
{
    public class MineFieldScreen : GameScreen
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _tilesetTexture;
        private OrthographicCamera _camera;
        private MineField _field;
        private MineFieldRenderer _fieldRenderer;
        private ImGuiRenderer _imGuiRenderer;
        private GameStateManager _gameStateManager;
        private GameTimeHandler _gameTimeHandler;

        private Color _loseColor = Color.Red * 0.01f;

        private int _fieldWidth = 9;
        private int _fieldHeight = 9;
        private int _totalMines = 15;
        private int _minePutterDifficulty = (int) MinePutterDifficulty.Easy;

        private ICommand _leftMouseButtonCommand = new NullCommand();
        private ICommand _rightMouseButtonCommand = new NullCommand();
        private ICommand _middleMouseButtonCommand = new NullCommand();

        private PlayerTurnsContainer _playerTurnsContainer;

        public MineFieldScreen(Game game) 
            : base(game)
        {
            _field = new MineField(9, 9, 15, true, MinePutterDifficulty.Easy);
            
            _gameStateManager = new GameStateManager();
            _gameTimeHandler = new GameTimeHandler(_gameStateManager);
        }

        public override void Initialize()
        {
            Console.WriteLine("Initializing!");

            _imGuiRenderer = new ImGuiRenderer(Game);
            _imGuiRenderer.RebuildFontAtlas();

            _camera = new OrthographicCamera(GraphicsDevice);
            _camera.LookAt(new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height / 2.0f));
            _camera.Zoom = 0.5f;

            _playerTurnsContainer = new PlayerTurnsContainer(_field, _gameStateManager);
            SetUpCommands();

            base.Initialize();
        }

        private void SetUpCommands()
        {
            _leftMouseButtonCommand = new RevealCellCommand(_field, _camera, _playerTurnsContainer);
            _rightMouseButtonCommand = new PlaceFlagCommand(_field, _camera, _playerTurnsContainer);
            _middleMouseButtonCommand = new MoveCameraCommand(_camera);
        }

        public override void LoadContent()
        {
            Console.WriteLine("Loading Content!");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tilesetTexture = Content.Load<Texture2D>("Images/Tileset_Field");
            
            _fieldRenderer = new MineFieldRenderer(_field, GraphicsDevice, _tilesetTexture);

            _field.Generate();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();

            if (_gameStateManager.HasLost)
            {
                double time = gameTime.TotalGameTime.TotalSeconds;
                float pulsate = (float) Math.Sin(time * 8) + 1;
                
                _loseColor = new Color(0 + pulsate * 0.25f, 0f, 0f, 0 + pulsate * 0.25f);
            }

            _gameTimeHandler.Update(gameTime);
        }

        private void HandleInput()
        {
            if (InputManager.IsMouseButtonDown(MouseButton.Middle))
            {
                _middleMouseButtonCommand.Execute(_gameTimeHandler.SecondsElapsed);
                Mouse.SetCursor(MouseCursor.SizeAll);
            }
            else
            {
                Mouse.SetCursor(MouseCursor.Arrow);
            }
            
            if (_gameStateManager.IsPlaying || _gameStateManager.IsNewGame)
            {
                if (InputManager.WasMouseButtonReleased(MouseButton.Right))
                {
                    _rightMouseButtonCommand.Execute(_gameTimeHandler.SecondsElapsed);
                }

                if (InputManager.WasMouseButtonReleased(MouseButton.Left))
                {
                    var fieldSnapshot = _field.CreateSnapshot();
                    var snapshot = _leftMouseButtonCommand.Execute(_gameTimeHandler.SecondsElapsed);
                    if (snapshot != null)
                    {
                        if (!_gameStateManager.IsPlaying)
                            _gameStateManager.CurrentState = GameState.Playing;

                        if (snapshot.NewCellState.IsMine && snapshot.NewCellState.IsOpen)
                        {
                            _gameStateManager.CurrentState = GameState.Lost;
                        }
                        else if (_field.FreeCellsLeft == 0)
                        {
                            _gameStateManager.CurrentState = GameState.Won;
                            _playerTurnsContainer.AddTurn(fieldSnapshot, snapshot, "Won!", _gameTimeHandler.SecondsElapsed);
                        }
                    }
                }
            }
            
            if (InputManager.WasKeyPressed(Keys.Space))
            {
                _field.Generate();
                _gameStateManager.CurrentState = GameState.NewGame;
                _loseColor = Color.Red * 0.01f;
            }

            if (InputManager.WasWheelScrolledUp())
            {
                if (_camera.Zoom < 1.5f)
                    _camera.ZoomIn(0.05f);
            }
            if (InputManager.WasWheelScrolledDown())
            {
                if (_camera.Zoom > 0.3f)
                    _camera.ZoomOut(0.05f);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin(samplerState: SamplerState.AnisotropicClamp, transformMatrix: _camera.GetViewMatrix());
            
            if (_fieldRenderer != null)
            {
                _spriteBatch.Draw(_fieldRenderer.RenderTarget, Vector2.Zero, null, Color.White, 0,
                    Vector2.Zero,
                    Vector2.One, SpriteEffects.None, 0.0f);
            }

            var mousePos = _camera.ScreenToWorld(InputManager.MousePosition);
            
            if ((_gameStateManager.CurrentState == GameState.Playing ||
                 _gameStateManager.CurrentState == GameState.NewGame) &&
                mousePos.X > 0 &&
                mousePos.Y > 0 &&
                mousePos.X < _field.Width * _field.CellSize &&
                mousePos.Y < _field.Height * _field.CellSize)
            {
                // Drawing a gray cell appended to the cursor
                
                var mousePosSnapped = new Vector2(mousePos.X - mousePos.X % _field.CellSize, mousePos.Y - mousePos.Y % _field.CellSize);
                if (InputManager.IsMouseButtonUp(MouseButton.Left)) // Draw an overlay
                {
                    _spriteBatch.FillRectangle(mousePosSnapped, new Size2(_field.CellSize, _field.CellSize), Color.Black * 0.25f);
                }
                else // Draw an empty cell just for sake of beauty
                {
                    // _spriteBatch.FillRectangle(mousePosSnapped, new Size2(_field.CellSize, _field.CellSize), Color.LightGray);
                    var positions = _field.GetSuitableCellPositionsAt((int)mousePos.X / _field.CellSize, (int)mousePos.Y / _field.CellSize);

                    foreach (var position in positions)
                    {
                        _spriteBatch.FillRectangle(position * _field.CellSize, new Size2(_field.CellSize, _field.CellSize), Color.LightGray);
                        _spriteBatch.DrawRectangle(position * _field.CellSize, new Size2(64, 64), Color.Gray);
                    }
                }
            }

            if (_gameStateManager.HasLost)
            {
                _spriteBatch.FillRectangle(Vector2.Zero, new Size2(_field.Width * _field.CellSize, _field.Height * _field.CellSize), _loseColor);
            }
            
            _spriteBatch.End();
            
            /*_spriteBatch.Begin();
            
            _spriteBatch.FillRectangle(Vector2.Zero, new Size2(GraphicsDevice.Viewport.Width, 48), Color.DarkGray);
            _spriteBatch.DrawString(_mainFont, _secondsElapsed.ToString("F0", CultureInfo.InvariantCulture), Vector2.One, Color.Black);
            
            _spriteBatch.End();*/
            
            RenderImGuiLayout(gameTime);
        }

        private void RenderImGuiLayout(GameTime gameTime)
        {
            _imGuiRenderer.BeforeLayout(gameTime);
            bool useRecursiveOpen = _field.UseRecursiveOpen;
            
            // ImGui.ShowMetricsWindow();

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

            _imGuiRenderer.AfterLayout();

            _field.UseRecursiveOpen = useRecursiveOpen;
            /*if (_field.Seed != seed)
            {
                _field.Seed = seed;
            }*/
        }

        private void Start()
        {
            var snapshot = _field.CreateSnapshot();
            _playerTurnsContainer.AddTurn(snapshot, null, "Started a new game", 0);
            _gameStateManager.CurrentState = GameState.NewGame;
            _loseColor = Color.Red * 0.01f;
            _field = new MineField(_fieldWidth, _fieldHeight, _totalMines, true, (MinePutterDifficulty) _minePutterDifficulty);
            _fieldRenderer = new MineFieldRenderer(_field, GraphicsDevice, _tilesetTexture);
            _playerTurnsContainer.MineField = _field;
            SetUpCommands();
            _field.Generate();
        }

        private void HelpMarker(string description)
        {
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.TextUnformatted(description);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
    }
}