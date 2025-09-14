using FezGame;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace FEZAP.Archipelago
{
    // Unfortunately loads of things here are inaccessible, so need to be referenced through the assembly
    class MenuManager
    {
        // Types
        private static Type MenuLevelType;
        private static Type MenuItemType;
        private static Type MenuBaseType;
        private static Type MainMenuType;

        private static readonly BindingFlags privateBindFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        public MenuManager()
        {
            MenuLevelType = Assembly.GetAssembly(typeof(Fez)).GetType("FezGame.Structure.MenuLevel");
            MenuItemType = Assembly.GetAssembly(typeof(Fez)).GetType("FezGame.Structure.MenuItem");
            MenuBaseType = Assembly.GetAssembly(typeof(Fez)).GetType("FezGame.Components.MenuBase");
            MainMenuType = Assembly.GetAssembly(typeof(Fez)).GetType("FezGame.Components.MainMenu");

            var AddApMenuItem = new Action<Action<object>, object>((orig, self) => { orig(self); CreateAndAddModLevel(self); });
            _ = new Hook(MenuBaseType.GetMethod("Initialize"), AddApMenuItem);
        }

        private static object GetMenuRoot(object MenuBase)
        {
            object MenuRoot = null;
            if (MenuBase.GetType() == MainMenuType)
            {
                MenuRoot = MainMenuType.GetField("RealMenuRoot", privateBindFlags).GetValue(MenuBase);
            }

            if (MenuBase.GetType() != MainMenuType || MenuRoot == null)
            {
                MenuRoot = MenuBaseType.GetField("MenuRoot", privateBindFlags).GetValue(MenuBase);
            }

            return MenuRoot;
        }

        private static void CreateAndAddModLevel(object MenuBase)
        {
            // Setup menu root
            var menuRoot = GetMenuRoot(MenuBase);
            MenuLevelType.GetField("IsDynamic").SetValue(menuRoot, true);

            // Create AP submenu
            object apMenu = Activator.CreateInstance(MenuLevelType);
            MenuLevelType.GetProperty("Title").SetValue(apMenu, "@ARCHIPELAGO");
            MenuLevelType.GetField("Parent").SetValue(apMenu, menuRoot);
            MenuLevelType.GetField("IsDynamic").SetValue(apMenu, true);
            MenuLevelType.GetField("Oversized").SetValue(apMenu, true);

            // TODO: Add menu stuff
        }
    }
}