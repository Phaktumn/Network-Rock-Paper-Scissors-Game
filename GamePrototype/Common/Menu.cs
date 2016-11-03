using System.Collections.Generic;
using System.Drawing;
using Colorful;
using Common;

namespace Core.Core
{
    public class Menu
    {
        /// <summary>
        /// Helper para o formatter
        /// </summary>
        private List<string> optionsList;
        private List<Formatter> optionsFormatter;

        private string optionsString;
        private List<string> indexStrings = new List<string>();

        private List<MenuOption> menuOptions;

        public string ExtraText = null;
        public Color ExtraTextColor = Color.Yellow;

        private Color OptionsColor { get; set; } = Color.GreenYellow;
        private Color IndexColor { get; set; } = Color.Red;
        private Color DelimiterColor { get; set; } = Color.Red;

        private int OptionsCount { get; set; }

        public Menu(int numberofOptions)
        {
            OptionsCount = numberofOptions;
            optionsList = new List<string>();
            optionsFormatter = new List<Formatter>();
            menuOptions = new List<MenuOption>();
        }

        public void Show()
        {
            if (ExtraText != null) Colorful.Console.Write("\n" + ExtraText + "\n");
            Colorful.Console.WriteLine("====================", DelimiterColor);
            Colorful.Console.WriteLineFormatted(optionsString, IndexColor, optionsFormatter.ToArray());
            Colorful.Console.Write("====================\n", DelimiterColor);
        }

        public void AddOption(MenuOption option, string triggerCode)
        {
            option.IndexTriggerCode = (optionsList.Count + 1);
            option.OptionStringTriggerCode = triggerCode;
            menuOptions.Add(option);
            optionsFormatter.Add(new Formatter(option.OptionString, OptionsColor));
            optionsList.Add(optionsFormatter.Count + "»{" + (optionsFormatter.Count - 1) + "}\n");
            //Criar a string Completa para o formatter usar
            optionsString += optionsList[optionsList.Count - 1];
        }

        public void StartEvent(string option)
        {
            foreach (var t in menuOptions)
            {
                if (option == t.OptionStringTriggerCode || option == t.IndexTriggerCode.ToString())
                {
                    t.OnExecuteEvent(option);
                }
            }
        }

        public string ReadLine(string read)
        {
            var line = read;
            line = line.ToLower();

            if (line.ToLowerInvariant() == "quit")
            {
                return line;
            }

            foreach (var option in menuOptions)
            {
                if (line == option.IndexTriggerCode.ToString().ToLowerInvariant() || line == option.OptionStringTriggerCode.ToLowerInvariant())
                {
                    return option.OptionStringTriggerCode;
                }
            }
            return null;
        }
    }
}