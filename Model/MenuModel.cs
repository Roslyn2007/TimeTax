using System;
using System.Collections.Generic;

namespace TimeTax.Model
{
    public class MenuModel
    {
        private int selectedOption = 0;
        private bool inOptions = false;
        private readonly string[] options = { "START GAME", "OPTIONS", "QUIT" };

        public int SelectedOption => selectedOption;
        public bool IsInOptions => inOptions;
        public IReadOnlyList<string> Options => options;

        public event Action<int>? SelectedOptionChanged;
        public event Action<bool>? OptionsStateChanged;
        public event Action? StartGameRequested;
        public event Action? QuitRequested;
        public event Action? SoundToggleRequested;

        public void SelectNext()
        {
            if (inOptions) return;
            selectedOption = (selectedOption + 1) % options.Length;
            SelectedOptionChanged?.Invoke(selectedOption);
        }

        public void SelectPrevious()
        {
            if (inOptions) return;
            selectedOption = (selectedOption - 1 + options.Length) % options.Length;
            SelectedOptionChanged?.Invoke(selectedOption);
        }

        public void ActivateSelected()
        {
            if (inOptions)
            {
                SoundToggleRequested?.Invoke();
                return;
            }

            switch (selectedOption)
            {
                case 0: StartGameRequested?.Invoke(); break;
                case 1:
                    inOptions = true;
                    OptionsStateChanged?.Invoke(true);
                    break;
                case 2: QuitRequested?.Invoke(); break;
            }
        }

        public void GoBack()
        {
            if (inOptions)
            {
                inOptions = false;
                OptionsStateChanged?.Invoke(false);
            }
        }
    }
}