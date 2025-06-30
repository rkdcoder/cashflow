using CashFlow.Shared.Utilities;

namespace CashFlow.Shared.Tests.Utilities
{
    public class TextSanitizerServiceTests
    {
        private readonly TextSanitizerService _service = new();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        public void Normalize_Should_Return_Empty_If_Null_Or_Whitespace(string? input)
        {
            var result = _service.Normalize(input);
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("João", "JOAO")]
        [InlineData("José", "JOSE")]
        [InlineData("ação", "ACAO")]
        [InlineData("ÇÃÕ", "CAO")]
        [InlineData("aéíõü", "AEIOU")]
        public void Normalize_Should_Remove_Diacritics_And_ToUpper(string input, string expected)
        {
            var result = _service.Normalize(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(" hello   world  ", "HELLO WORLD")]
        [InlineData("multiple     spaces", "MULTIPLE SPACES")]
        [InlineData(" leading and trailing   ", "LEADING AND TRAILING")]
        public void Normalize_Should_Collapse_And_Trim_Spaces(string input, string expected)
        {
            var result = _service.Normalize(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("á. é-í@/õ&", "A. E-I@/O&")]
        public void Normalize_Should_Handle_Mixed_Diacritics_And_Symbols(string input, string expected)
        {
            var result = _service.Normalize(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Normalize_Should_Handle_Mixed_Complex_Input()
        {
            var input = "Olá,    João!     Você/tem 100% de aprovação?   ";
            var expected = "OLA JOAO VOCE/TEM 100 DE APROVACAO";
            var result = _service.Normalize(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Normalize_Should_Return_Uppercase()
        {
            var input = "Teste Minusculo";
            var result = _service.Normalize(input);
            Assert.Equal("TESTE MINUSCULO", result);
        }
    }
}
