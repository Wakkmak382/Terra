﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace CExternal
{
    internal class Renderer : Overlay
    {
        public bool aimbot = true;
        public bool aimOnTeam = false;

        protected override void Render()
        {
            ImGui.Begin("menu");

            ImGui.Checkbox("aimbot", ref aimbot);
            ImGui.Checkbox("Disable Teammate Aimbot", ref aimOnTeam);
        }
    }
}
