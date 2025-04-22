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
            //int wRegValue = 0;
            //var ram = new int[256];
            mockPic.SetupProperty(p => p.WReg, 0);
            mockPic.SetupProperty(p => p.Ram, new int[256]);
            var commands = new Commands(mockPic.Object);

            // Act
            commands.MOVLW(42);

            // Assert
            Assert.Equal(42, mockPic.Object.WReg);
        }
    }
}
