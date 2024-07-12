using System.Collections.Generic;
using Xunit;
using TransferBatch;

namespace TransferBatch.Tests
{
    public class CalculateCommissionsTests
    {
        [Fact]
        public void ShouldReturnCorrectCommissions()
        {
            // Arrange
            var transfers = new List<Transfer>
            {
                new Transfer { AccountId = "A10", TransferId = "T1000", TransferAmount = 100.00m },
                new Transfer { AccountId = "A11", TransferId = "T1001", TransferAmount = 100.00m },
                new Transfer { AccountId = "A10", TransferId = "T1002", TransferAmount = 200.00m },
                new Transfer { AccountId = "A10", TransferId = "T1003", TransferAmount = 300.00m },
            };

            // Act
            var result = Program.CalculateCommissions(transfers);

            // Assert
            Assert.Equal(30.00m, result["A10"]);
            Assert.Equal(10.00m, result["A11"]);
        }

        [Fact]
        public void ShouldHandleSingleTransfer()
        {
            // Arrange
            var transfers = new List<Transfer>
            {
                new Transfer { AccountId = "A12", TransferId = "T1004", TransferAmount = 500.00m }
            };
            
            // Act
            var result = Program.CalculateCommissions(transfers);
            // Assert
            Assert.Equal(50.00m, result["A12"]);
        }

        [Fact]
        public void ShouldHandleMultipleTransfersSameAmount()
        {
            // Arrange
            var transfers = new List<Transfer>
            {
                new Transfer { AccountId = "A13", TransferId = "T1005", TransferAmount = 100.00m },
                new Transfer { AccountId = "A13", TransferId = "T1006", TransferAmount = 100.00m },
                new Transfer { AccountId = "A13", TransferId = "T1007", TransferAmount = 100.00m }
            };

            // Act
            var result = Program.CalculateCommissions(transfers);

            // Assert
            Assert.Equal(20.00m, result["A13"]);
        }

        [Fact]
        public void ShouldntCalculateComissionEqualOrLessThanZero()
        {
            // Arrange
            var transfers = new List<Transfer>
            {
                new Transfer { AccountId = "A13", TransferId = "T1005", TransferAmount = 100.00m },
                new Transfer { AccountId = "A13", TransferId = "T1006", TransferAmount = -10.00m },
                new Transfer { AccountId = "A13", TransferId = "T1007", TransferAmount = 100.00m }
            };

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Program.CalculateCommissions(transfers));

            // Assert
            Assert.Equal("Transfer cannot equal or less than 0.", exception.Message);
        }

        
    }
}
