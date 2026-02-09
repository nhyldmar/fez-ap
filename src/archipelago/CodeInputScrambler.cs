using FezEngine.Components;
using FezEngine.Structure.Input;
using FezEngine.Tools;
using FezGame;
using FEZUG.Features.Console;

// Scrambler by Jenna1337: https://gist.github.com/Jenna1337/814b2f833632f712af304311ed13a14d 
namespace FEZAP.Archipelago
{
    public class CodeInputScrambler
    {
        private CodeInputScrambler() { }
        private static void ShuffleInputs(Random random, CodeInput[] array)
        {
            var n = array.Length;
            while (n > 1)
            {
                // Pick a random element from the remaining elements
                var k = random.Next(n);
                n--;
                // Swap the current element with the randomly chosen element
                CodeInput value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }
        private static IInputManager InputManager;
        private static readonly Type volHostType = typeof(Fez).Assembly.GetType("FezGame.Components.VolumesHost");
        private static readonly Dictionary<CodeInput, CodeInput> codeInputMap = new();
        private static bool CustomCodeInputMethod(object self)
        {
            var inputField = volHostType.GetField("Input", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            CodeInput codeInput = CodeInput.None;
            if (InputManager.Jump == FezButtonState.Pressed)
            {
                codeInput = CodeInput.Jump;
            }
            else if (InputManager.RotateRight == FezButtonState.Pressed)
            {
                codeInput = CodeInput.SpinRight;
            }
            else if (InputManager.RotateLeft == FezButtonState.Pressed)
            {
                codeInput = CodeInput.SpinLeft;
            }
            else if (InputManager.Left == FezButtonState.Pressed)
            {
                codeInput = CodeInput.Left;
            }
            else if (InputManager.Right == FezButtonState.Pressed)
            {
                codeInput = CodeInput.Right;
            }
            else if (InputManager.Up == FezButtonState.Pressed)
            {
                codeInput = CodeInput.Up;
            }
            else if (InputManager.Down == FezButtonState.Pressed)
            {
                codeInput = CodeInput.Down;
            }
            if (codeInput == CodeInput.None)
            {
                return false;
            }
            var Input = (List<CodeInput>)inputField.GetValue(self);
            Input.Add(codeInputMap[codeInput]);
            if (Input.Count > 16)
            {
                Input.RemoveAt(0);
            }
            return true;
        }
        private static Dictionary<CodeInput, int[]> codemachinemapping;
        private static Dictionary<CodeInput, int[]> original;
        private static volatile bool DidInit = false;
        public static void ShuffleCodeInputs(int seed)
        {
            if (!DidInit)
            {
                Waiters.Wait(() => DidInit, () => ShuffleCodeInputs(seed));
                return;
            }
            CodeInput[] c = Enum.GetValues(typeof(CodeInput)).Cast<CodeInput>().Where(ci => ci != CodeInput.None).ToArray();
            CodeInput[] k = (CodeInput[])c.Clone();
            Random random = new Random(seed);
            ShuffleInputs(random, k);
            for (int i = 0; i < c.Length; ++i)
            {
                codeInputMap.Add(c[i], k[i]);
                FezugConsole.Print($"{c[i]}: {k[i]}");
            }
            foreach (var key in original.Keys)
            {
                codemachinemapping[key] = original[codeInputMap[key]];
            }
        }
        static CodeInputScrambler()
        {
            _ = Waiters.Wait(() => ServiceHelper.FirstLoadDone,
            () =>
            {
                InputManager = ServiceHelper.Get<IInputManager>();
                codemachinemapping = (Dictionary<CodeInput, int[]>)typeof(Fez).Assembly.GetType("FezGame.Components.CodeMachineHost").GetField("BitPatterns", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
                original = new Dictionary<CodeInput, int[]>(codemachinemapping);

                var detour = new MonoMod.RuntimeDetour.Hook(
                    volHostType.GetMethod("GrabInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
                    new Func<object, bool>(CustomCodeInputMethod));
                DidInit = true;
            });
        }
    }
}
