using System.Collections.Generic;
using System;
using Xunit;
using ProjectBank.Infrastructure;
namespace ReferenceSystem.Tests
{

    public class SignatureTests
    {
        static readonly string expectedAgricultureSHA1 = "96c842e9da8fd2e3d76890658ce7e43d08de90c2";
        static readonly string expectedAgricultureSHA256 = "3f52a423f8ebd4ccad56c627fa1287d3286e15e20968d590bfbbbb6d53162a1c";
        static readonly string expectedAgricultureMD5 = "f8ea9e07f542c0cdf0805e5f30c76cb6";
        static readonly string expectedAgricultureSHA384 = "ff5facdc5d5ee3e511b93375d0f73aab14d0b4afce62d38007c76a089fcf103ef8da82b360a1e42ae654a49ac270ba65";
        static readonly string expectedAgricultureSHA512 = "8e92a5e09ab90a391102756ea6496af289b5eefd9c6ffa3453b1987f725381953ffa3b43fc63d7dd5b6af3859e40e3bf654da02b722392f10d204f354e2d9e1e";
        static readonly string expectedAgricultureNoHash = "agriculture";
        static readonly List<string> expectedAgriculture = new List<string> { expectedAgricultureSHA1, expectedAgricultureSHA256, expectedAgricultureMD5, expectedAgricultureSHA384, expectedAgricultureSHA512, expectedAgricultureNoHash };
        static readonly string expectedComputerScienceSHA256 = "13a5670cc77404aa14b878e0b1a82651e705d21f7ba2956918a1406b26871a1f";
        static readonly string expectedComputerScienceSHA384 = "8c067704ddd0e69aee028cfec66fe56f1d192a65ce51e7512adee32e5c0b53e4cb76b92af6ca49302301e4a775a8016c";
        

        //algorithms
        static readonly string expectedAlgorithmsMD5 = "66270707424a729c3e557fceb03f45c9";
     
        //databases

        static readonly string expectedDatabaseseSHA1 = "0167bbf5aa9e9c0c005b2c62e0191b73a1e34df0";
        static readonly string expectedDatabasesSHA512 = "11ab34ebc30b79bfbc551909d52ff311e59acd88766ee9ee9b625fd0660bde0501b23a5abd7bc1f378fbb61b315771095c56e9329cee27ac186041c27352882c";
        static readonly List<string> expectedAgricultureComputerScience = new List<string> { expectedDatabaseseSHA1, expectedComputerScienceSHA256, expectedAlgorithmsMD5, expectedComputerScienceSHA384, expectedDatabasesSHA512, expectedAgricultureNoHash };


        [Fact]
        public void Constructor_Throws_Exception_Given_No_Tags()
        {
            var tags = new List<Tag>{}.AsReadOnly();

            Assert.Throws<ArgumentException>(() => new Signature(tags));   
        }
        
        [Fact]
        public void Constructor_Returns_Signature_Given_One_Tag()
        {
            //Arrange
            var expected = expectedAgriculture.AsReadOnly();
            var tags = new List<Tag> { new Tag ("agriculture") }.AsReadOnly();
            
            //Act
            var actual = new Signature(tags);

            //Assert
            Assert.Equal(expected, actual.Hashes);
        }

        [Fact]
        public void Constructor_Returns_Signature_With_Correct_Hashes_Given_Multiple_Tags()
        {
            //Arrange
            var expected = expectedAgricultureComputerScience.AsReadOnly();
            var tags = new List<Tag> { new Tag("agriculture"), new Tag("computer science"), new Tag("algorithms"), new Tag("databases")}.AsReadOnly();
            
            //Act
            var actual = new Signature(tags);

            //Assert
            Assert.Equal(expected, actual.Hashes);
        }
    }
}