// This code is taken from the ImGui.NET.SampleProgram.XNA and adapted a little bit.
// ﻿https://github.com/mellinoe/ImGui.NET
// Copyright (c) 2017 Eric Mellino and ImGui.NET contributors

using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper.Framework.ImGUI
{
    public static class DrawVertexDeclaration
    {
        public static readonly VertexDeclaration Declaration;

        public static readonly int Size;

        static DrawVertexDeclaration()
        {
            unsafe
            {
                Size = sizeof(ImDrawVert);

                Declaration = new VertexDeclaration(
                    Size,

                    // Position
                    new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),

                    // UV
                    new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),

                    // Color
                    new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
                );
            }
        }
    }
}