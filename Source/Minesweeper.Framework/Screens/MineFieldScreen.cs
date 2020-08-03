using System;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minesweeper.Framework.ImGUI;
using Minesweeper.Framework.Inputs;
using Minesweeper.Framework.MinePutters;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
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
        private Texture2D[] _textures;
        private Vector2 _lastCameraPos;
        private ImGuiRenderer _imGuiRenderer;

        private bool _lose;
        private Color _loseColor = Color.Red * 0.01f;

        private int _fieldWidth = 9;
        private int _fieldHeight = 9;
        private int _totalMines = 15;
        private int _minePutterDifficulty = (int) MinePutterDifficulty.Easy;

        public MineFieldScreen(Game game) 
            : base(game)
        {
            _field = new MineField(9, 9, 15, true, MinePutterDifficulty.Easy);
            _textures = new Texture2D[4 * 3];
        }

        public override void Initialize()
        {
            Console.WriteLine("Initializing!");

            _imGuiRenderer = new ImGuiRenderer(Game);
            _imGuiRenderer.RebuildFontAtlas();

            _camera = new OrthographicCamera(GraphicsDevice);
            _camera.LookAt(new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height / 2.0f));
            _camera.Zoom = 0.5f;

            _lastCameraPos = _camera.Position;

            base.Initialize();
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
            var mousePos = _camera.ScreenToWorld(InputManager.MousePosition);

            if (InputManager.IsMouseButtonDown(MouseButton.Middle))
            {
                _camera.Move(-InputManager.MouseVelocity / _camera.Zoom);
                Mouse.SetCursor(MouseCursor.SizeAll);
            }
            else
            {
                Mouse.SetCursor(MouseCursor.Arrow);
            }

            if (!_lose)
            {
                if (InputManager.WasMouseButtonReleased(MouseButton.Right))
                {
                    _field.FlagAt((int)mousePos.X / _field.CellSize, (int)mousePos.Y / _field.CellSize);
                }
                if (InputManager.WasMouseButtonReleased(MouseButton.Left))
                {
                    /*if (Math.Abs((_lastCameraPos - _camera.Position).Length()) < 20f &&
                        mousePos.X > 0 &&
                        mousePos.Y > 0 &&
                        mousePos.X < _field.Width * _field.CellSize &&
                        mousePos.Y < _field.Height * _field.CellSize)
                    {*/
                    var res = _field.RevealAt((int)mousePos.X / _field.CellSize, (int)mousePos.Y / _field.CellSize);
                    if (res)
                        _lose = true;
                    // }
                    
                    _lastCameraPos = _camera.Position;
                }
            }
            else
            {
                if (_loseColor.A < 100)
                    _loseColor = new Color(_loseColor, _loseColor.A + 1);
            }

            if (InputManager.WasKeyPressed(Keys.Space))
            {
                _field.Generate();
                _lose = false;
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
            
            if (!_lose &&
                mousePos.X > 0 &&
                mousePos.Y > 0 &&
                mousePos.X < _field.Width * _field.CellSize &&
                mousePos.Y < _field.Height * _field.CellSize)
            {
                var mousePosSnapped = new Vector2(mousePos.X - mousePos.X % _field.CellSize, mousePos.Y - mousePos.Y % _field.CellSize);
                _spriteBatch.FillRectangle(mousePosSnapped, new Size2(_field.CellSize, _field.CellSize), Color.Black * 0.25f);
            }

            if (_lose)
            {
                _spriteBatch.FillRectangle(Vector2.Zero, new Size2(_field.Width * _field.CellSize, _field.Height * _field.CellSize), _loseColor);
            }
            
            _spriteBatch.End();
            
            RenderImGuiLayout(gameTime);
        }

        private void RenderImGuiLayout(GameTime gameTime)
        {
            _imGuiRenderer.BeforeLayout(gameTime);
            bool useRecursiveOpen = _field.UseRecursiveOpen;

            ImGui.Begin("Game Settings");
            ImGui.Text($"Total Mines: {_field.TotalMines}");
            ImGui.Text($"Total Cells: {_field.TotalCells}");
            ImGui.Text($"Field Resolution: {_field.Width}x{_field.Height}");
            ImGui.Text($"Mines Left: {_field.MinesLeft}");
            ImGui.Text($"Free cells: {_field.FreeCellsLeft}");
            
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
            ImGui.SameLine();
            if (ImGui.Button("Undo"))
            {
                // TODO: Undo
            }
            ImGui.SameLine();
            if (ImGui.Button("Redo"))
            {
                // TODO: Redo
            }
            ImGui.SameLine();
            if (ImGui.Button("Solve"))
            {
                _field.Solve();
            }
            
            ImGui.Separator();
            ImGui.SetNextItemWidth(ImGui.GetFontSize() * 6f);
            ImGui.InputInt("Field Width", ref _fieldWidth);
            ImGui.SetNextItemWidth(ImGui.GetFontSize() * 6f);
            ImGui.InputInt("Field Height", ref _fieldHeight);
            ImGui.SetNextItemWidth(ImGui.GetFontSize() * 6f);
            ImGui.InputInt("Total Mines", ref _totalMines);
            ImGui.SetNextItemWidth(ImGui.GetFontSize() * 8f);
            ImGui.ListBox("Mine Putter Difficulty", ref _minePutterDifficulty,
                Enum.GetNames(typeof(MinePutterDifficulty)), 3);
            
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
        }

        private void Start()
        {
            _lose = false;
            _loseColor = Color.Red * 0.01f;
            _field = new MineField(_fieldWidth, _fieldHeight, _totalMines, true, (MinePutterDifficulty) _minePutterDifficulty);
            _fieldRenderer = new MineFieldRenderer(_field, GraphicsDevice, _tilesetTexture);
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