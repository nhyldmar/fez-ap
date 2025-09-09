using FezEngine.Tools;
using FezGame;
using FEZAP.Features;
using FEZAP.Helpers;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace FEZAP
{
    public class Fezap : DrawableGameComponent
    {
        public static string Version = "v0.1.0";

        public List<IFezapFeature> Features { get; private set; }

        public static Fezap Instance { get; private set; }
        public static Fez Fez { get; private set; }

        public Fezap(Game game) : base(game)
        {
            Fez = (Fez)game;
            Instance = this;
            Enabled = true;
            Visible = true;
            DrawOrder = 99999;

            LocationHandler locationHandler;
            ServiceHelper.AddComponent(locationHandler = new LocationHandler(game));
        }

        public override void Initialize()
        {
            base.Initialize();

            DrawingTools.Init();

            Features = [];
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && typeof(IFezapFeature).IsAssignableFrom(t)))
            {
                IFezapFeature feature = (IFezapFeature)Activator.CreateInstance(type);
                ServiceHelper.InjectServices(feature);
                Features.Add(feature);
            }

            foreach (var feature in Features)
            {
                feature.Initialize();
            }
        }

        public static T GetFeature<T>()
        {
            return (T)GetFeature(typeof(T));
        }

        public static IFezapFeature GetFeature(Type type)
        {
            foreach (var feature in Instance.Features)
            {
                if (feature.GetType() == type) return feature;
            }
            return null;
        }

        public override void Update(GameTime gameTime)
        {
            InputHelper.Update(gameTime);

            foreach (var feature in Features)
            {
                feature.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            DrawingTools.BeginBatch();

            foreach(var feature in Features)
            {
                feature.DrawHUD(gameTime);
            }

            DrawingTools.EndBatch();
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

        public override void Draw(GameTime gameTime)
        {
            foreach (var feature in Fezap.Instance.Features)
            {
                feature.DrawLevel(gameTime);
            }
        }
    }
}
