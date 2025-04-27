using Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSimulator.Tests
{
    public class CommandsTests
    {
        [Fact]
        public void MOVLW_ShouldSetWRegToLiteral()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.WReg, 0);
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            var commands = new Commands(mockPic.Object);

            // Act
            commands.MOVLW(42);

            // Assert
            Assert.Equal(42, mockPic.Object.WReg);
        }

        [Fact]
        public void DECFSZ_ShouldDecrementAndWriteBackToFile()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            mockPic.Object.Ram[0x10] = 42;
            var commands = new Commands(mockPic.Object);

            //Act
            commands.DECFSZ(0b1001_0000);

            //Assert
            Assert.Equal(41, mockPic.Object.Ram[0x10]);
        }

        [Fact]
        public void DECFSZ_ShouldDecrementAndWriteToWReg()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.WReg, 0);
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            mockPic.Object.Ram[0x10] = 42;
            var commands = new Commands(mockPic.Object);

            //Act
            commands.DECFSZ(0b0001_0000);

            //Assert
            Assert.Equal(41, mockPic.Object.WReg);
        }

        [Fact]
        public void DECFSZ_ShouldSkip()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            mockPic.SetupProperty(p => p.PC, 20);
            mockPic.Object.Ram[0x10] = 1;
            var commands = new Commands(mockPic.Object);

            //Act
            commands.DECFSZ(0b1001_0000);

            //Assert
            Assert.Equal(21, mockPic.Object.PC);
        }

        [Fact]
        public void ADDWF_ShouldAddWRegToFileAndWriteToFile()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.WReg, 5);
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            mockPic.Object.Ram[0x10] = 10;
            var commands = new Commands(mockPic.Object);

            //Act
            commands.ADDWF(0b1001_0000);

            //Assert
            Assert.Equal(15, mockPic.Object.Ram[0x10]);
        }

        [Fact]
        public void ADDWF_ShouldAddWRegToFileAndWriteToWReg()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.WReg, 5);
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            mockPic.Object.Ram[0x10] = 10;
            var commands = new Commands(mockPic.Object);

            //Act
            commands.ADDWF(0b0001_0000);

            //Assert
            Assert.Equal(15, mockPic.Object.WReg);
        }

        [Fact]
        public void ADDWF_ShouldSetZeroFlag()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.WReg, -42);
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            mockPic.Object.Ram[0x10] = 42;
            mockPic.Object.Ram[3] = 0b0000_0000;
            var commands = new Commands(mockPic.Object);

            //Act
            commands.ADDWF(0b1001_0000);

            //Assert
            Assert.Equal(0b0100, mockPic.Object.Ram[3] & 0b0000_0100);
        }

        [Fact]
        public void ADDWF_ShouldClearZeroFlag()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.WReg, -4);
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            mockPic.Object.Ram[0x10] = 42;
            mockPic.Object.Ram[3] = 0b0000_0100;
            var commands = new Commands(mockPic.Object);

            //Act
            commands.ADDWF(0b1001_0000);

            //Assert
            Assert.Equal(0b0000, mockPic.Object.Ram[3] & 0b0000_0100);
        }

        [Fact]
        public void ADDWF_ShouldSetCarryFlag()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.WReg, 50);
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            mockPic.Object.Ram[0x10] = 250;
            mockPic.Object.Ram[3] = 0b0000_0000;
            var commands = new Commands(mockPic.Object);

            //Act
            commands.ADDWF(0b1001_0000);

            //Assert
            Assert.Equal(1, mockPic.Object.Ram[3] & 0b0001);
        }

        [Fact]
        public void ADDWF_ShouldClearCarryFlag()
        {
            // Arrange
            var mockPic = new Mock<IPic>();
            mockPic.SetupProperty(p => p.WReg, 5);
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            mockPic.Object.Ram[0x10] = 250;
            mockPic.Object.Ram[3] = 0b0000_0001;
            var commands = new Commands(mockPic.Object);

            //Act
            commands.ADDWF(0b1001_0000);

            //Assert
            Assert.Equal(0, mockPic.Object.Ram[3] & 0b0001);
        }
    }
}
