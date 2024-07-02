using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TransferBatch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: TransferBatch <Path_to_transfers_file>");
                return;
            }

            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            var transfers = new List<Transfer>();
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line!.Split(',');

                        var transfer = new Transfer
                        {
                            AccountId = values[0],
                            TransferId = values[1],
                            TransferAmount = decimal.Parse(values[2], CultureInfo.InvariantCulture)
                        };

                        transfers.Add(transfer);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return;
            }

            var accountCommissions = CalculateCommissions(transfers);

            foreach (var account in accountCommissions)
            {
                Console.WriteLine($"{account.Key},{account.Value.ToString("0.##", CultureInfo.InvariantCulture)}");
            }
        }

        public static Dictionary<string, decimal> CalculateCommissions(List<Transfer> transfers)
        {
            var accountCommissions = new Dictionary<string, decimal>();

            var groupedTransfers = transfers.GroupBy(t => t.AccountId);

            foreach (var group in groupedTransfers)
            {
                var accountId = group.Key;
                var transfersForAccount = group.ToList();

                if (transfersForAccount.Count == 0)
                {
                    continue;
                }

                var maxTransferAmount = transfersForAccount.Max(t => t.TransferAmount);

                var maxTransfers = transfersForAccount
                    .Where(t => t.TransferAmount == maxTransferAmount)
                    .ToList();

                var excludeTransfer = true;
                var totalCommission = transfersForAccount
                    .Where(t =>
                    {
                        if (excludeTransfer && maxTransfers.Contains(t))
                        {
                            excludeTransfer = false;
                            return false;
                        }
                        return true;
                    })
                    .Sum(t => t.TransferAmount * 0.1m);

                accountCommissions[accountId!] = totalCommission;
            }

            return accountCommissions;
        }
    }

    public class Transfer
    {
        public string? AccountId { get; set; }
        public string? TransferId { get; set; }
        public decimal TransferAmount { get; set; }
    }
}
