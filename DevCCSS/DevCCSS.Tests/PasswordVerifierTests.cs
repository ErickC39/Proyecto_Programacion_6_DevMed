using System.Security.Cryptography;
using System.Text;
using DevCCSS.Wcf.Models;
using Xunit;

namespace DevCCSS.Tests
{
    public class PasswordVerifierTests
    {
        private static (byte[] salt, byte[] hash) HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var passwordBytes = Encoding.Unicode.GetBytes(password);
            var combined = new byte[salt.Length + passwordBytes.Length];
            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);
            return (salt, SHA512.HashData(combined));
        }

        [Fact]
        public void VerifySha512_ContraseniaCorrecta_DevuelveTrue()
        {
            var (salt, hash) = HashPassword("ClaveSegura123!");

            var resultado = PasswordVerifier.VerifySha512(salt, hash, "ClaveSegura123!");

            Assert.True(resultado);
        }

        [Fact]
        public void VerifySha512_ContraseniaIncorrecta_DevuelveFalse()
        {
            var (salt, hash) = HashPassword("ClaveSegura123!");

            var resultado = PasswordVerifier.VerifySha512(salt, hash, "OtraClave456!");

            Assert.False(resultado);
        }

        [Fact]
        public void VerifySha512_EsSensibleAMayusculasMinusculas()
        {
            var (salt, hash) = HashPassword("ClaveSegura123!");

            var resultado = PasswordVerifier.VerifySha512(salt, hash, "clavesegura123!");

            Assert.False(resultado);
        }

        [Fact]
        public void VerifySha512_ContraseniaVacia_DevuelveFalseCuandoElHashNoEsDeVacio()
        {
            var (salt, hash) = HashPassword("ClaveSegura123!");

            var resultado = PasswordVerifier.VerifySha512(salt, hash, "");

            Assert.False(resultado);
        }

        [Fact]
        public void VerifySha512_MismaContraseniaConSaltDistinto_DevuelveFalse()
        {
            var (_, hash) = HashPassword("ClaveSegura123!");
            var otroSalt = RandomNumberGenerator.GetBytes(16);

            var resultado = PasswordVerifier.VerifySha512(otroSalt, hash, "ClaveSegura123!");

            Assert.False(resultado);
        }
    }
}
