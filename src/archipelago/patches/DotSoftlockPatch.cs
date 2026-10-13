using System.Reflection;
using FezEngine.Tools;
using FezGame;
using FezGame.Components;
using FEZUG.Features.Console;
using MonoMod.RuntimeDetour;

/*
 * This patch fixes a softlock which occurs if any Dot dialogue starts while the camera is spiraling down. This bug is
 * possible to encounter in the vanilla game, but it is even more important for FEZAP since you can be sent Emotional
 * Support at any time. This patch delays the Dot text from appearing until after the camera spiral animation is done.
 *
 * The method we need to patch is inside a delegate with a compiler-generated name which can be different on different
 * platforms or even different compilations. We scan for the method by its signature which should be solid. Worst case,
 * if we don't find it, bail instead of crashing the mod (I think it should always find it).
 */
namespace FEZAP.Archipelago
{
    public class DotSoftlockPatch : IFezapPatch
    {
        private Hook DotServiceSayDelegateHook;

        [ServiceDependency]
        public IDotManager Dot { get; set; }

        public void Init()
        {
            // Patch DotHost.Say delegate to prevent getting interrupted by Emotional Support
            MethodInfo DotServiceSayDelegate = FindDotServiceSayDelegate();
            if (DotServiceSayDelegate != null)
                DotServiceSayDelegateHook = new Hook(DotServiceSayDelegate, DotServiceSayDelegateHooked);
        }

        private MethodInfo FindDotServiceSayDelegate()
        {
            // The thing we need to patch is a delegate function inside DotService.Say. That delegate is also in a
            // closure which relys on local variables, and these names are compiler generate and compiler dependant...
            // I know the function signature (returns bool, takes two floats) so I try to find that one. Hopefully it
            // finds the right one (there aren't many, it should find the right one)
            Type DotServiceType = typeof(Fez).Assembly.GetType("FezGame.Services.Scripting.DotService");
            List<MethodInfo> candidates = [];
            foreach (Type type in DotServiceType.GetNestedTypes(BindingFlags.NonPublic))
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (type.Name.Contains("<Say>") || method.Name.Contains("<Say>")) // On Linux+Mac the type contains "<Say>", on Windows the function contains "<Say>"
                    {
                        if (method.ReturnParameter.ParameterType == typeof(bool)
                                && method.GetParameters().Length == 2
                                && method.GetParameters().All(param => param.ParameterType == typeof(float)))
                        {
                            candidates.Add(method);
                        }
                    }
                }
            }
            if (candidates.Count() == 0)
            {
                FezugConsole.Print("WARNING: Couldn't find DotService.Say delegate method to patch... Emotional Support softlocks are still possible. Please report this!", FezugConsole.OutputType.Warning);
                return null;
            }
            if (candidates.Count() > 1)
                FezugConsole.Print("WARNING: MORE THAN ONE DotService.Say delegate method matched... Patching one but it could be wrong! Please report this!", FezugConsole.OutputType.Warning);
            return candidates[0];
        }

        private bool DotServiceSayDelegateHooked(Func<object, float, float, bool> original, object self, float f1, float f2)
        {
            if (Dot.Behaviour == DotHost.BehaviourType.SpiralAroundWithCamera || Dot.Behaviour == DotHost.BehaviourType.MoveToTargetWithCamera)
                return false;

            return original(self, f1, f2);
        }

        public void Dispose()
        {
            DotServiceSayDelegateHook?.Dispose();
        }
    }
}
