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
            var transfers = new List<Transfer>
            {
                new Transfer { AccountId = "A10", TransferId = "T1000", TransferAmount = 100.00m },
                new Transfer { AccountId = "A11", TransferId = "T1001", TransferAmount = 100.00m },
                new Transfer { AccountId = "A10", TransferId = "T1002", TransferAmount = 200.00m },
                new Transfer { AccountId = "A10", TransferId = "T1003", TransferAmount = 300.00m },
            };

            var result = Program.CalculateCommissions(transfers);

            Assert.Equal(30.00m, result["A10"]);
            Assert.Equal(10.00m, result["A11"]);
        }

        [Fact]
        public void ShouldHandleSingleTransfer()
        {
            var transfers = new List<Transfer>
            {
                new Transfer { AccountId = "A12", TransferId = "T1004", TransferAmount = 500.00m }
            };

            var result = Program.CalculateCommissions(transfers);

            Assert.Equal(0.00m, result["A12"]);
        }

        [Fact]
        public void ShouldHandleMultipleTransfersSameAmount()
        {
            var transfers = new List<Transfer>
            {
                new Transfer { AccountId = "A13", TransferId = "T1005", TransferAmount = 100.00m },
                new Transfer { AccountId = "A13", TransferId = "T1006", TransferAmount = 100.00m },
                new Transfer { AccountId = "A13", TransferId = "T1007", TransferAmount = 100.00m }
            };

            var result = Program.CalculateCommissions(transfers);

            Assert.Equal(20.00m, result["A13"]);
        }

        [Fact]
        public void ShouldntCalculateComissionEqualOrLessThanZero()
        {
            var transfers = new List<Transfer>
            {
                new Transfer { AccountId = "A13", TransferId = "T1005", TransferAmount = 100.00m },
                new Transfer { AccountId = "A13", TransferId = "T1006", TransferAmount = -10.00m },
                new Transfer { AccountId = "A13", TransferId = "T1007", TransferAmount = 100.00m }
            };

            var result = Program.CalculateCommissions(transfers);

            //

        }

        
    }
}
