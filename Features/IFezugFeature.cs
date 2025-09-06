using Microsoft.Xna.Framework;

namespace FEZAP.Features
{
    public interface IFezapFeature
    {
        void Initialize();
        void Update(GameTime gameTime);
        void DrawHUD(GameTime gameTime);
        void DrawLevel(GameTime gameTime);
    }
}
