namespace SoloLife.UnitTests.Services;

using Shouldly;
using SoloLife.Infrastructure.Services;
using Xunit;

public class PasswordHasherTests
{
    private readonly PasswordHasher _sut = new();

    [Fact]
    public void Hash_E_Verify_ComMesmaSenha_RetornaTrue()
    {
        var hash = _sut.Hash("senha-secreta-123");

        _sut.Verify("senha-secreta-123", hash).ShouldBeTrue();
    }

    [Fact]
    public void Verify_ComSenhaErrada_RetornaFalse()
    {
        var hash = _sut.Hash("senha-secreta-123");

        _sut.Verify("senha-errada", hash).ShouldBeFalse();
    }

    [Fact]
    public void Hash_GeraValoresDiferentesParaMesmaSenha_PorCausaDoSalt()
    {
        _sut.Hash("mesma-senha").ShouldNotBe(_sut.Hash("mesma-senha"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("sem-separador")]
    [InlineData("abc.def")]
    public void Verify_ComHashMalformado_RetornaFalse(string hash)
    {
        _sut.Verify("qualquer", hash).ShouldBeFalse();
    }
}
