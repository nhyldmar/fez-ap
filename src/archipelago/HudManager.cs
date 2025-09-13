using Microsoft.Xna.Framework;

namespace FEZAP.Helpers
{
    class HudManager()
    {
        public static bool isConnected = false;
        public static void DrawHUD()
        {
            string versionInfo = $"FEZAP {Fezap.Version}";
            string connectionInfo = isConnected ? "Connected" : "Disconnected";
            Color connectionColor = isConnected ? Color.Green : Color.Red;
            DrawingTools.DrawRect(new Rectangle(5, 5, 230, 70), new(0, 0, 0, 0.8f));
            DrawingTools.DrawText(versionInfo, new Vector2(15, 0), Color.White);
            DrawingTools.DrawText(connectionInfo, new Vector2(15, 27), connectionColor);
        }

        public static void Print(string text, Color color)
        {
            // TODO: Create list of slowly fading text with new entries appearing below the old one and all bumping up when one fades fully
        }

        public static void Print(string text)
        {
            Print(text, Color.White);
        }
    }
}