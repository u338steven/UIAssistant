using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

using UIAssistant.Infrastructure.Commands;

namespace CommandsTest
{
    [TestClass]
    public class CommandsTest
    {
        CommandSyntax Syntax;
        CommandParser Parser;
        CommandValidator Validator;
        CandidatesGenerator Generator;
        MockCommand MockMethods;

        #region Rules
        ArgumentRule Switch;
        ArgumentRule Click;
        ArgumentRule RightClick;
        ArgumentRule DoubleClick;

        ArgumentRule ArgApplication;
        ArgumentRule ArgWindow;
        ArgumentRule ArgTaskbar;
        ArgumentRule ArgDividedscreen;

        ArgumentRule ArgText;
        ArgumentRule ArgCommand;
        ArgumentRule ArgInItemsControl;

        ArgumentRule OptButton;
        ArgumentRule OptCheckBox;
        ArgumentRule OptGenerator;
        ArgumentRule OptTarget;

        CommandRule Hah;
        CommandRule SearchByText;
        CommandRule Hah2;
        CommandRule MouseEmulation;
        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            Syntax.Clear();
        }

        [TestInitialize]
        public void Setup()
        {
            Syntax = new CommandSyntax();
            Parser = new CommandParser(Syntax);
            Validator = new CommandValidator(Parser, new MockLocalizer());
            MockMethods = new MockCommand();
            Generator = new CandidatesGenerator(Syntax);

            Switch = new ArgumentRule("switch", MockMethods.Switch);
            Click = new ArgumentRule("click", MockMethods.Click);
            RightClick = new ArgumentRule("rightclick", MockMethods.RightClick);
            DoubleClick = new ArgumentRule("doubleclick", MockMethods.DoubleClick);

            ArgApplication = new ArgumentRule("application", MockMethods.Application, ArgsFactory(Switch));
            ArgWindow = new ArgumentRule("window", MockMethods.Window, ArgsFactory(Click, RightClick, DoubleClick));
            ArgTaskbar = new ArgumentRule("taskbar", MockMethods.Taskbar, ArgsFactory(Click, RightClick));
            ArgDividedscreen = new ArgumentRule("windowdividedscreen", MockMethods.WindowDividedScreen, ArgsFactory(Click, DoubleClick));

            ArgText = new ArgumentRule("Text", MockMethods.Text);
            ArgCommand = new ArgumentRule("Command", MockMethods.Command);
            ArgInItemsControl = new ArgumentRule("TextInItemsControl", MockMethods.TextInItemsControl);

            OptButton = new ArgumentRule("Button", MockMethods.Button);
            OptCheckBox = new ArgumentRule("CheckBox", MockMethods.CheckBox);

            OptGenerator = new ArgumentRule("-hintGenerator", MockMethods.HintGenerator);
            OptTarget = new ArgumentRule("-searchTarget", MockMethods.SearchTarget, null, ArgsFactory(OptButton, OptCheckBox));

            Hah = new CommandRule("hah", MockMethods.HitaHint, ArgsFactory(ArgWindow, ArgApplication, ArgTaskbar, ArgDividedscreen), ArgsFactory(OptGenerator, OptTarget));
            SearchByText = new CommandRule("/", MockMethods.SearchByText, ArgsFactory(ArgText, ArgCommand, ArgInItemsControl), ArgsFactory(OptTarget));
            Hah2 = new CommandRule("hah2", MockMethods.HitaHint2, ArgsFactory(ArgWindow, ArgDividedscreen));
            MouseEmulation = new CommandRule("me", MockMethods.MouseEmulation);

            Syntax.Add(Hah);
            Syntax.Add(SearchByText);
            Syntax.Add(Hah2);
            Syntax.Add(MouseEmulation);
        }

        private ICollection<ArgumentRule> ArgsFactory(params ArgumentRule[] rules)
        {
            return new List<ArgumentRule>(rules);
        }

        [TestMethod]
        public void SyntaxTest()
        {
            Assert.AreEqual("hah", Syntax.FindRule("hah").Name);
            Assert.AreEqual("hah2", Syntax.FindRule("hah2").Name);

            Syntax.FindRule("hah").Action.Invoke(new Command("SyntaxTest"));
            MockMethods.Logger.CheckResults("HitaHint:SyntaxTest:");
        }

        [TestMethod]
        public void ValidateTest()
        {
            Validate("", new ValidationResult("Invalid command: "));
            Validate("h", new ValidationResult("Invalid command: h"));
            Validate("hah window click", ValidationResult.Success);
            Validate("hah window clic", new ValidationResult("Invalid command: clic"));
            Validate("hah application switch", ValidationResult.Success);
            Validate("hah WindowDividedScreen switch", new ValidationResult("Invalid command: switch"));
            Validate("hah application click", new ValidationResult("Invalid command: click"));
            Validate("hah apliation click", new ValidationResult("Invalid command: apliation"));
            Validate("hah application Switch", ValidationResult.Success);
        }

        [TestMethod]
        public void GeneratorTest()
        {
            CandidatesTest("", Syntax.Select(x => x.Name).OrderBy(x => x));
            CandidatesTest("hah", Empty);
            CandidatesTest("hah ", Hah.RequiredArgs.Select(x => x.Name).OrderBy(x => x));
            CandidatesTest("hah window", Empty);
            CandidatesTest("hah win", Hah.RequiredArgs.Where(x => x.Name.StartsWithCaseIgnored("win")).Select(x => x.Name).OrderBy(x => x));
            CandidatesTest("hah window ", ArgWindow.RequiredArgs.Select(x => x.Name).OrderBy(x => x));
            CandidatesTest("hah window click", Empty);
            CandidatesTest("hah window click ", Empty);
            CandidatesTest("hah WindowDividedScreen DoubleClick ", Empty);
            CandidatesTest("hah WindowDividedScreen Click", Empty);
            CandidatesTest("me", Empty);
            CandidatesTest("me ", Empty);
        }

        [TestMethod]
        public void ParsedCommandInvokeTest()
        {
            var cmd = "hah window click -hintGenerator -searchTarget:Button";
            var tokens = Parser.Parse(cmd);

            tokens.ToList().ForEach(x => x?.Invoke());
            MockMethods.Logger.CheckResults("HitaHint:hah:", "Window:window:", "Click:click:", "HintGenerator:-hintGenerator:", "SearchTarget:-searchTarget:Button");
        }

        [TestMethod]
        public void AddOptionLazy()
        {
            var input = "hah -searchTarget:";
            var generator = new CandidatesGenerator(Syntax);
            var result = generator.GenerateCandidates(input);
            Assert.AreEqual(2, result.Count());

            var optionArgTextBox = new ArgumentRule("TextBox", MockMethods.TextBox);
            var targetArgs = Syntax[0].OptionalArgs.ElementAt(1).OptionalArgs;
            targetArgs.Add(optionArgTextBox);

            result = generator.GenerateCandidates(input);
            Assert.AreEqual(3, result.Count());

            CollectionAssert.AreEqual(targetArgs.Select(x => x.Name).ToArray(), result.Select(x => x.Name).ToArray());
        }

        [TestMethod]
        public void DescriptionTest()
        {
            Syntax.FindRule("hah").Description = "Hit-a-hint description";
            CandidatesTest("ha", new[] { "hah", "hah2" }, new[] { Syntax.FindRule("hah").Description, null });
        }

        static List<string> Empty = new List<string>();
        [TestMethod]
        public void VariationTest()
        {
            var searchTarget = "hah window click -hintGenerator -searchTarget:";
            Validate(searchTarget, ValidationResult.Success);
            CandidatesTest(searchTarget, OptTarget.OptionalArgs.Select(x => x.Name));

            var wordsOrder = "hah -hintGenerator window click -searchTarget:";
            Validate(wordsOrder, ValidationResult.Success);
            CandidatesTest(wordsOrder, OptTarget.OptionalArgs.Select(x => x.Name));

            var lackWords = "hah WindowDividedScreen lick Click ";
            Validate(lackWords, new ValidationResult("Invalid command: lick"));
            CandidatesTest(lackWords, Empty);

            // option
            CandidatesTest("hah window -", Hah.OptionalArgs.Select(x => x.Name).OrderBy(x => x));

            // non option
            Validate("hah2 -searchTarget:", new ValidationResult("Invalid command: -searchTarget:"));
            CandidatesTest("hah2 -searchTarget:", Empty);

            Validate("hah2 -", new ValidationResult("Invalid command: -"));
            CandidatesTest("hah2 window -", Empty);

            Validate("hah2 window -searchTarget", new ValidationResult("Invalid command: -searchTarget"));
            CandidatesTest("hah2 window -searchTarget:", Empty);
        }

        private void Validate(string input, ValidationResult expect)
        {
            var result = Validator.Validate(input);
            System.Diagnostics.Trace.WriteLine($"Input:[{input}]\nResult:[{result?.ToString() ?? "Success"}]\n");
            Assert.AreEqual(expect?.ErrorMessage, result?.ErrorMessage);
        }

        private void CandidatesTest(string input, IEnumerable<string> expected, IEnumerable<string> expectedDescriptions = null)
        {
            System.Diagnostics.Trace.WriteLine($"Input: [{input}]");
            var result = Generator.GenerateCandidates(input);

            System.Diagnostics.Trace.WriteLine($"Candidates: {string.Join(", ", result.Select(x => x.Name))}\n");
            Assert.AreEqual(expected.Count(), result.Count());
            CollectionAssert.AreEqual(expected.ToArray(), result.Select(x => x.Name).ToArray());

            if (expectedDescriptions != null)
            {
                CollectionAssert.AreEqual(expectedDescriptions.ToArray(), result.Select(x => x.Description).ToArray());
            }
        }
    }

    internal class OutputLog
    {
        List<string> _log = new List<string>();

        public void Add(string value)
        {
            _log.Add(value);
        }

        public void CheckResults(params string[] expected)
        {
            for (int i = 0; i < _log.Count; i++)
            {
                Assert.AreEqual(expected[i], _log[i]);
            }
        }

        public void Clear()
        {
            _log.Clear();
        }
    }

    internal class MockLocalizer : UIAssistant.Infrastructure.Resource.ILocalizer
    {
        public string GetLocalizedText(string key)
        {
            return "Invalid command: {0}";
        }
    }

    class MockCommand
    {
        public void Switch(ICommand command) { OutputLog(command); }
        public void Click(ICommand command) { OutputLog(command); }
        public void RightClick(ICommand command) { OutputLog(command); }
        public void DoubleClick(ICommand command) { OutputLog(command); }

        public void Application(ICommand command) { OutputLog(command); }
        public void Window(ICommand command) { OutputLog(command); }
        public void Taskbar(ICommand command) { OutputLog(command); }
        public void WindowDividedScreen(ICommand command) { OutputLog(command); }

        public void Text(ICommand command) { OutputLog(command); }
        public void Command(ICommand command) { OutputLog(command); }
        public void TextInItemsControl(ICommand command) { OutputLog(command); }
        public void Button(ICommand command) { OutputLog(command); }
        public void CheckBox(ICommand command) { OutputLog(command); }
        public void TextBox(ICommand command) { OutputLog(command); }

        public void HintGenerator(ICommand command) { OutputLog(command); }
        public void SearchTarget(ICommand command) { OutputLog(command); }

        public void HitaHint(ICommand command) { OutputLog(command); }
        public void SearchByText(ICommand command) { OutputLog(command); }
        public void HitaHint2(ICommand command) { OutputLog(command); }
        public void MouseEmulation(ICommand command) { OutputLog(command); }

        public OutputLog Logger { get; set; } = new OutputLog();

        private void OutputLog(ICommand command, [CallerMemberName] string name = null)
        {
            System.Diagnostics.Trace.WriteLine($"CallerMemberName: {name}, Name: {command.Name}, Value=[{command.Value}]");
            Logger.Add(string.Join(":", name, command.Name, command.Value));
        }
    }
}
