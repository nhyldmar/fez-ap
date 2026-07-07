using System.Reflection;
using FezEngine.Components;
using FezEngine.Structure.Input;
using FezEngine.Tools;
using FezGame;
using FEZUG.Features.Console;
using Microsoft.Xna.Framework;

// Scrambler by Jenna1337: https://gist.github.com/Jenna1337/814b2f833632f712af304311ed13a14d
namespace FEZAP.Archipelago
{
    public class CodeInputScrambler(Game game) : GameComponent(game)
    {

        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        private static readonly Dictionary<CodeInput, CodeInput> codeInputMap = new();
        private static readonly Dictionary<CodeInput, CodeInput> reverseInputMap = new();

        private static Dictionary<CodeInput, int[]> codeMachine;
        private static Dictionary<CodeInput, int[]> originalCodeMachine;

        private static CodeInput[] achievementCode;
        private static CodeInput[] originalAchievementCode;
        private static CodeInput[] qrMapCode;
        private static CodeInput[] originalQrMapCode;

        public override void Initialize()
        {
            codeMachine = (Dictionary<CodeInput, int[]>)typeof(Fez).Assembly.GetType("FezGame.Components.CodeMachineHost").GetField("BitPatterns", Flags).GetValue(null);
            originalCodeMachine = new Dictionary<CodeInput, int[]>(codeMachine);

            var GameWideCodes = typeof(Fez).Assembly.GetType("FezGame.Components.GameWideCodes");
            achievementCode = (CodeInput[])GameWideCodes.GetField("AchievementCode", Flags).GetValue(null);
            originalAchievementCode = (CodeInput[])achievementCode.Clone();
            qrMapCode = (CodeInput[])GameWideCodes.GetField("MapCode", Flags).GetValue(null);
            originalQrMapCode = (CodeInput[])qrMapCode.Clone();
        }

        public static void ShuffleCodeInputs(string seed)
        {
            CodeInput[] c = Enum.GetValues(typeof(CodeInput)).Cast<CodeInput>().Where(ci => ci != CodeInput.None).ToArray();
            CodeInput[] k = (CodeInput[])c.Clone();
            ShuffleInputs(seed, k);

            codeInputMap.Clear();
            reverseInputMap.Clear();
            for (int i = 0; i < c.Length; ++i)
            {
                codeInputMap.Add(c[i], k[i]);
                reverseInputMap.Add(k[i], c[i]);
            }
            foreach (var key in originalCodeMachine.Keys)
            {
                codeMachine[key] = originalCodeMachine[codeInputMap[key]];
            }
            UpdateGameWideCodes();
            FezugConsole.Print("Tetrominos scrambled");
        }

        public static CodeInput GetScrambledCode(CodeInput codeInput)
        {
            if (codeInputMap.ContainsKey(codeInput))
                return codeInputMap[codeInput];
            return codeInput;
        }

        private static void ShuffleInputs(string seed, CodeInput[] array)
        {
            var random = new Random(seed.GetHashCode());
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

        public static void ResetScramble()
        {
            foreach (var key in originalCodeMachine.Keys)
            {
                codeMachine[key] = originalCodeMachine[key];
            }
            codeInputMap.Clear();
            reverseInputMap.Clear();
            UpdateGameWideCodes();
        }

        private static void UpdateGameWideCodes()
        {
            // reset codes
            if (reverseInputMap.Count == 0)
            {
                Array.Copy(originalAchievementCode, achievementCode, originalAchievementCode.Length);
                Array.Copy(originalQrMapCode, qrMapCode, originalQrMapCode.Length);
                return;
            }

            // Instead of updating the method to check for the 3 game wide codes, we just modify the codes themselves
            for (var i = 0; i < achievementCode.Length; i++)
            {
                achievementCode[i] = reverseInputMap[originalAchievementCode[i]];
            }
            for (var i = 0; i < qrMapCode.Length; i++)
            {
                qrMapCode[i] = reverseInputMap[originalQrMapCode[i]];
            }
        }
    }
}
