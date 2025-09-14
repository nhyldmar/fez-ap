using FEZAP.Helpers;
using Microsoft.Xna.Framework;

namespace FEZAP.Archipelago
{
    public readonly struct DisplayText(string text, Color color)
    {
        public readonly string text = text;
        public readonly Color color = color;
    };

    class HudManager()
    {
        public static bool isConnected = false;
        private static readonly List<DisplayText> messageLog = [new("Test text", Color.Red), new("More text here", Color.Green)];
        private static readonly int paddingSize = 10;
        private static readonly int charHeight = 27;
        private static readonly int charWidth = 17;
        private static readonly Color boxColor = new(0, 0, 0, 0.8f);
        private static readonly Rectangle infoBoxRect = new(5, 5, 230, 70);
        private static Rectangle messageLogRect = new(5, 900, 0, 0);

        public static void DrawHUD()
        {
            DrawInfoBox();
            DrawMessageLog();
        }

        private static void DrawInfoBox()
        {
            string versionInfo = $"FEZAP {Fezap.Version}";
            string connectionInfo = isConnected ? "Connected" : "Disconnected";
            Color connectionColor = isConnected ? Color.Green : Color.Red;
            DrawingTools.DrawRect(infoBoxRect, boxColor);
            DrawingTools.DrawText(versionInfo, new(infoBoxRect.X + paddingSize, 0), Color.White);
            DrawingTools.DrawText(connectionInfo, new(infoBoxRect.X + paddingSize, charHeight), connectionColor);
        }

        private static void DrawMessageLog()
        {
            int maxCharCount = messageLog.Max(textInfo => textInfo.text.Length);
            messageLogRect.Y = 900 - messageLog.Count * charHeight;
            messageLogRect.Width = maxCharCount * charWidth + paddingSize * 2;
            messageLogRect.Height = messageLog.Count * charHeight + paddingSize * 2;
            DrawingTools.DrawRect(messageLogRect, boxColor);
            int messageLogRectLowerY = messageLogRect.Y - messageLogRect.Height;

            for (int i = 0; i < messageLog.Count; i++)
            {
                int yCoord = messageLogRectLowerY - paddingSize - i * charHeight;
                DrawingTools.DrawText(messageLog[i].text, new(5 + paddingSize, yCoord), messageLog[i].color);
            }
        }

        public static void Print(string text, Color color)
        {
            messageLog.Insert(0, new(text, color));
            messageLog.RemoveAt(5);
        }

        public static void Print(string text)
        {
            Print(text, Color.White);
        }
    }
}