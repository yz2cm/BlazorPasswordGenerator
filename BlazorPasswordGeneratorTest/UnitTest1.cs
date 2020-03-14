using System;
using Xunit;
using BlazorPasswordGenerator;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace BlazorPasswordGeneratorTest
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper logger;
        public UnitTest1(ITestOutputHelper logger)
        {
            this.logger = logger;
        }        
        [Fact]
        public async Task Test1()
        {
            string usableChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var passGen = new RandomPassword(32, usableChars);

            passGen = passGen.CharacterShuoldHaveOverThreeKinds();            

            await foreach(var x in passGen.GeneratePasswordAsync(100))
            {
                this.logger.WriteLine(x.Password);
                Assert.Equal(32, x.Password.Length);
            }
        }
    }
}
