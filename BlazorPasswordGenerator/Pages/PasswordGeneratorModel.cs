using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPasswordGenerator.Pages
{
    public class PasswordGeneratorModel : ComponentBase
    {
        public int passwordCount { get; set; } = 100;
        public int passwordLength { get; set; } = 12;
        public bool useUniqueCharacter { get; set; }
        public bool useCharacterOver3Kinds { get; set; }
        public bool notUseSimilarCharacter { get; set; }
        public bool notStartWithExcelSpecialCharacter { get; set; }
        public string generatedPasswordList { get; set; }
        public string usableCharacters { get; set; } = RandomPassword.LowerCaseAlphabets + RandomPassword.UpperCaseAlphabets + RandomPassword.NumericCharacters;
        public int passwordGeneratedCount { get; set; } = 0;
        public bool generatingIsCanceled { get; set; }
        public bool completeIconVisible { get; set; }
        public bool generateButtonIsEnabled { get; set; } = true;
        public bool cancelButtonIsEnabled { get; set; }
        public int progressMax { get; set; } = 0;

        public void checkboxClicked_UseUpperCase(object checkedValue)
        {
            bool useUpperCase = (bool)checkedValue;
            var characters = RandomPassword.UpperCaseAlphabets;

            if (useUpperCase)
            {
                this.usableCharacters += string.Join(string.Empty, characters.Where(c => !this.usableCharacters.Contains(c)));
            }
            else
            {
                this.usableCharacters = string.Join(string.Empty, this.usableCharacters.Where(c => !characters.Contains(c)));
            }
        }

        public void checkboxClicked_UseLowerCase(object checkedValue)
        {
            bool useUpperCase = (bool)checkedValue;
            var characters = RandomPassword.LowerCaseAlphabets;

            if (useUpperCase)
            {
                this.usableCharacters += string.Join(string.Empty, characters.Where(c => !this.usableCharacters.Contains(c)));
            }
            else
            {
                this.usableCharacters = string.Join(string.Empty, this.usableCharacters.Where(c => !characters.Contains(c)));
            }
        }

        public void checkboxClicked_UseNumber(object checkedValue)
        {
            bool useUpperCase = (bool)checkedValue;
            var characters = RandomPassword.NumericCharacters;

            if (useUpperCase)
            {
                this.usableCharacters += string.Join(string.Empty, characters.Where(c => !this.usableCharacters.Contains(c)));
            }
            else
            {
                this.usableCharacters = string.Join(string.Empty, this.usableCharacters.Where(c => !characters.Contains(c)));
            }
        }

        public void checkboxClicked_UseSpecialCharacter(object checkedValue)
        {
            bool useUpperCase = (bool)checkedValue;
            var characters = RandomPassword.SpecialCharacters;

            if (useUpperCase)
            {
                this.usableCharacters += string.Join(string.Empty, characters.Where(c => !this.usableCharacters.Contains(c)));
            }
            else
            {
                this.usableCharacters = string.Join(string.Empty, this.usableCharacters.Where(c => !characters.Contains(c)));
            }
        }
        public void checkboxClicked_NotUseSimilarCharacter(object checkedValue)
        {
            bool notUseSimilarCharacter = (bool)checkedValue;
            var characters = RandomPassword.SimilarCharacters;

            if (notUseSimilarCharacter)
            {
                this.usableCharacters = string.Join(string.Empty, this.usableCharacters.Where(c => !characters.Contains(c)));
            }
            else
            {
                this.usableCharacters += string.Join(string.Empty, characters.Where(c => !this.usableCharacters.Contains(c)));
            }
        }
        [Inject]
        private IJSRuntime jsRuntime { get; set; }
        public async Task onClipBoardButton_Clicked()
        {
            if (this.generatedPasswordList != null && this.generatedPasswordList.Length > 0)
            {
                await jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", this.generatedPasswordList);
            }
        }
        public async Task GeneratePasswordsAsync()
        {
            if(this.passwordLength < 8 || this.passwordCount < 1 || this.usableCharacters.Distinct().Count() < 1)
            {
                return;
            }

            this.generateButtonIsEnabled = false;
            this.cancelButtonIsEnabled = true;
            this.passwordGeneratedCount = 0;
            this.progressMax = this.passwordCount;
            this.completeIconVisible = false;

            var generator = new RandomPassword(passwordLength, this.usableCharacters);
            if (this.useCharacterOver3Kinds && RandomPassword.CalculateCharacterKindCount(this.usableCharacters) >= 3)
            {
                generator = generator.CharacterShuoldHaveOverThreeKinds();
            }
            if (this.useUniqueCharacter && this.passwordLength < (this.usableCharacters.Length / 3))
            {
                generator = generator.CharacterShouldNotDuplicate();
            }
            if (this.notStartWithExcelSpecialCharacter && this.passwordLength > 1)
            {
                generator = generator.NotStartWithExcelSpecialCharacter();
            }
            this.passwordGeneratedCount = 0;
            string result = string.Empty;
            this.generatedPasswordList = string.Empty;
            string passwordList = string.Empty;
            this.generatingIsCanceled = false;

            await foreach (var passwordWithRuby in generator.GeneratePasswordAsync(this.passwordCount))
            {
                passwordList += passwordWithRuby.Password + "\t" + passwordWithRuby.Ruby + "\r\n";
                this.passwordGeneratedCount++;

                if (this.passwordGeneratedCount % 10 == 0)
                {
                    if (this.generatingIsCanceled)
                    {
                        this.generatingIsCanceled = false;
                        break;
                    }
                    StateHasChanged();
                    await Task.Delay(10);
                }
            }

            this.generatedPasswordList = passwordList;
            this.completeIconVisible = true;
            this.generateButtonIsEnabled = true;
            this.cancelButtonIsEnabled = false;
        }
        public void CancelGenerating()
        {
            this.cancelButtonIsEnabled = false;
            this.generatingIsCanceled = true;
        }
    }
}
