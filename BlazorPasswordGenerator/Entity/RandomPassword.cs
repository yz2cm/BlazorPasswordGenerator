using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;

namespace BlazorPasswordGenerator
{
    public class RandomPasswordWithRuby
    {
        internal RandomPasswordWithRuby(string password)
        {
            this.password = password;
        }
        public override string ToString()
        {
            return this.password;
        }
        public string Password => this.password;
        public string Ruby {
            get
            {
                var rubyTable = new Dictionary<char, string>()
                {
                    { 'a', "エー" }, { 'b', "ビー" }, { 'c', "シー" }, {  'd', "ディー" }, { 'e', "イー" }, { 'f', "エフ" }, { 'g', "ジー"}, { 'h', "エイチ" },
                    { 'i', "アイ" }, { 'j', "ジェー" }, { 'k', "ケー" }, {  'l', "エル" }, { 'm', "エム" }, { 'n', "エヌ" }, { 'o', "オー"}, { 'p', "ピー" },
                    { 'q', "キュー" }, { 'r', "アール" }, { 's', "エス" }, {  't', "ティー" }, { 'u', "ユー" }, { 'v', "ブイ" }, { 'w', "ダブリュ"}, { 'x', "エックス" },
                    { 'y', "ワイ" }, { 'z', "ゼット" },
                    { 'A', "エー" }, { 'B', "ビー" }, { 'C', "シー" }, {  'D', "ディー" }, { 'E', "イー" }, { 'F', "エフ" }, { 'G', "ジー"}, { 'H', "エイチ" },
                    { 'I', "アイ" }, { 'J', "ジェー" }, { 'K', "ケー" }, {  'L', "エル" }, { 'M', "エム" }, { 'N', "エヌ" }, { 'O', "オー"}, { 'P', "ピー" },
                    { 'Q', "キュー" }, { 'R', "アール" }, { 'S', "エス" }, {  'T', "ティー" }, { 'U', "ユー" }, { 'V', "ブイ" }, { 'W', "ダブリュ"}, { 'X', "エックス" },
                    { 'Y', "ワイ" }, { 'Z', "ゼット" },
                    { '0', "ぜろ" }, { '1', "いち"}, { '2', "に"}, { '3', "さん" }, { '4', "よん" }, { '5', "ご" }, { '6', "ろく" }, { '7', "なな" }, { '8', "はち" }, { '9', "きゅー" },
                    { '!', "ビックリ" }, { '"', "ダブルクォート" }, { '#', "シャープ" }, { '$', "ドル" }, { '%', "パーセント" }, { '&', "アンド" }, {  '\'', "シングルクォート"},
                    { '(', "マルヒラキカッコ" }, { ')', "マルトジカッコ" }, { '=', "イコール" }, { '-', "マイナス" }, { '^' , "ハット" }, { '~', "チルダ" }, { '|', "パイプ" }, { '\\', "エン" },
                    { '@' , "アットマーク" }, { '`', "バッククォート" }, { '[', "チュウヒラキカッコ"}, { ']', "チュウトジカッコ" }, { '{', "ナミヒラキカッコ" }, { '}', "ナミトジカッコ"  },
                    { ';' , "セミコロン" }, { '+', "プラス" }, { '*', "アスタリスク" }, { ':', "コロン" }, { '<', "ショウナリ" }, { '>', "ダイナリ" }, { ',', "コンマ" }, { '.', "ピリオド" },
                    { '/', "スラッシュ" }, { '?', "ハテナ" }, { '_', "アンダーバー"},
                };
                string ruby = string.Empty;
                var rs = new List<string>();
                foreach (var c in this.password)
                {
                    string r;
                    rubyTable.TryGetValue(c, out r);
                    rs.Add(r);
                }

                return string.Join(" ", rs);
            }
        }
        private string password;
    }
    public class RandomPassword
    {
        public RandomPassword(int maxLength, string usableCharacters) : this(maxLength, usableCharacters.ToCharArray()) { }
        public RandomPassword(int maxLength, char[] usableCharacters)
        {
            this.maxLength = maxLength;
            this.usableCharacters = usableCharacters;
        }
        public RandomPassword CharacterShuoldHaveOverThreeKinds()
        {
            var clone = this.Clone();
            clone.characterShuoldHaveOverThreeKinds = true;

            return clone;
        }
        public RandomPassword CharacterShouldNotDuplicate()
        {
            var clone = this.Clone();
            clone.characterShuoldNotDuplicate = true;

            return clone;
        }
        public RandomPassword SimilarCharacterNotExist()
        {
            var clone = this.Clone();
            clone.similarCharacterExist = true;

            return clone;
        }
        public RandomPassword NotStartWithExcelSpecialCharacter()
        {
            var clone = this.Clone();
            clone.notStartWithExcelSpecialCharacter = true;

            return clone;
        }
        private byte generateRandomByteInRange(byte min, byte max)
        {
            using (var generator = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[1];

                while (true)
                {
                    generator.GetBytes(bytes);
                    if(min <= bytes[0] && bytes[0] <= max)
                    {
                        return bytes[0];
                    }
                }
            }
        }
        private string generatePasswordCandidate()
        {
            using (var generator = RandomNumberGenerator.Create())
            {
                string candidate = string.Empty;

                byte[] bytes = new byte[this.maxLength];
                generator.GetBytes(bytes);

                int maxValidInteger = ((byte.MaxValue + 1) / this.usableCharacters.Length) * this.usableCharacters.Length - 1;
                byte maxValidByte = (byte)maxValidInteger;

                for(int i = 0; i < bytes.Length; ++i)
                {
                    if(maxValidByte < bytes[i])
                    {
                        bytes[i] = this.generateRandomByteInRange(0, maxValidByte);
                    }
                }

                var sb = new StringBuilder(bytes.Length);
                for(int i = 0; i < bytes.Length; ++i)
                {
                    sb.Append(this.usableCharacters[bytes[i] % this.usableCharacters.Length]);
                }

                return sb.ToString();
            }

        }
        public async IAsyncEnumerable<RandomPasswordWithRuby> GeneratePasswordAsync(int passwordCount)
        {
            for(int i = 0; i < passwordCount; ++i)
            {
                var password = await Task.Run(() => GeneratePassword());
                yield return password;
            }
            yield break;
        }
        public RandomPasswordWithRuby GeneratePassword()
        {
            while (true)
            {
                string candidate = generatePasswordCandidate();

                if (this.characterShuoldHaveOverThreeKinds && RandomPassword.CalculateCharacterKindCount(candidate) < 3)
                {
                    continue;
                }
                if(this.characterShuoldNotDuplicate && candidate.Distinct().Count() != candidate.ToArray().Count())
                {
                    continue;
                }
                if(this.notStartWithExcelSpecialCharacter && RandomPassword.ExcelSpecialCharacters.Any(x => candidate.StartsWith(x)))
                {
                    continue;
                }

                return new RandomPasswordWithRuby(candidate);
            }
        }
        public static int CalculateCharacterKindCount(string candidate)
        {
            int characterKindCount = 0;

            if (RandomPassword.UpperCaseAlphabets.Any(c => candidate.Contains(c)))
            {
                ++characterKindCount;
            }
            if (RandomPassword.LowerCaseAlphabets.Any(c => candidate.Contains(c)))
            {
                ++characterKindCount;
            }
            if (RandomPassword.NumericCharacters.Any(c => candidate.Contains(c)))
            {
                ++characterKindCount;
            }
            if (RandomPassword.SpecialCharacters.Any(c => candidate.Contains(c)))
            {
                ++characterKindCount;
            }

            return characterKindCount;
        }
        private RandomPassword Clone()
        {
            var clone = new RandomPassword(this.maxLength, this.usableCharacters);
            clone.characterShuoldHaveOverThreeKinds = this.characterShuoldHaveOverThreeKinds;
            clone.characterShuoldNotDuplicate = this.characterShuoldNotDuplicate;
            clone.similarCharacterExist = this.similarCharacterExist;
            clone.notStartWithExcelSpecialCharacter = this.notStartWithExcelSpecialCharacter;

            return clone;
        }

        public static string UpperCaseAlphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string LowerCaseAlphabets = "abcdefghijklmnopqrstuvwxyz";
        public static string NumericCharacters = "0123456789";
        public static string SpecialCharacters = "+-*=!#$%&@|(){}[]<>;:?,._/\\\"'`^~";
        public static string SimilarCharacters = "0Oo1Il|ij";
        public static string ExcelSpecialCharacters = "+-=@'";

        private int maxLength;
        private char[] usableCharacters = new char[0];
        private bool characterShuoldHaveOverThreeKinds = false;
        private bool characterShuoldNotDuplicate = false;
        private bool similarCharacterExist = false;
        private bool notStartWithExcelSpecialCharacter = false;
    }
}
