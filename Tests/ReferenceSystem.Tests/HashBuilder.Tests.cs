using Xunit;
using ProjectBank.Infrastructure;
using System.Security.Cryptography;

namespace ReferenceSystem.Tests
{
    public class HashBuilderTests
    {

        [Fact]
        public void HashString_Returns_SHA1()
        {
            //Arrange
            var expected = "5f0f56a9485a7a39786b596e02a73af72715e320";
            var teststring = "algorithms";

            //Act
            var actual = HashBuilder.HashString(teststring, SHA1.Create());

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HashString_Returns_SHA256()
        {
            //Arrange
            var expected = "da14b0a09bc8067819790ffa99587dfbab37e1f3423f1c63cc13c83deca10aa0";
            var teststring = "datastructures";

            //Act
            var actual = HashBuilder.HashString(teststring, SHA256.Create());

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HashString_Returns_SHA384()
        {
            //Arrange
            var expected = "0f54a9962dccc169954070905f7316fa7fa8f6c9103903bf24fccfc9ed026968c60e0e31291f2461c0be64dafbe7a720";
            var teststring = "stringtheory";

            //Act
            var actual = HashBuilder.HashString(teststring, SHA384.Create());
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HashString_Returns_SHA512()
        {
            //Arrange
            var expected = "624c5fc8227879c2d019b133dee064ff5a876c8f5b8788e52e380b2df9f1cc715859eb90abc5ed7f1f76969c45ad507431384ceee693f760f7a68fd8ea498cda";
            var teststring = "globaldevelopment";

            //Act
            var actual = HashBuilder.HashString(teststring, SHA512.Create());
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HashString_Returns_MD5()
        {
            //Arrange
            var expected = "0274150daa794673dd69f7eb838f8290";
            var teststring = "islamichistory";

            //Act
            
            var actual = HashBuilder.HashString(teststring, MD5.Create());
            //Assert
            Assert.Equal(expected, actual);
        }
    }
}