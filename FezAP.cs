using Common;
using FezEngine.Components;
using FezEngine.Tools;
using FezGame;
using FezGame.Components;
using FezGame.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FezAP
{
    public class Fezap : DrawableGameComponent
    {
        public static string Version = "v0.1";

        public static Fezap Instance { get; private set; }
        public static Fez Fez { get; private set; }

        public Fezap(Game game) : base(game)
        {
            Fez = (Fez)game;
            Instance = this;
            Enabled = true;
            Visible = true;
            DrawOrder = 99999;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }

    public class FezapInGameRendering : DrawableGameComponent
    {
        public FezapInGameRendering(Game game) : base(game)
        {
            Enabled = true;
            Visible = true;
            DrawOrder = 101;
        }
    }
}
