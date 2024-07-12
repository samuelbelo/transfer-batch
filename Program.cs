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

            ProcessCommissionsInBatches(filePath);
        }

        public static void ProcessCommissionsInBatches(string filePath, int batchSize = 1000)
        {
            using (var reader = new StreamReader(filePath))
            {
                var buffer = new List<Transfer>();
                var totalCommissions = new Dictionary<string, decimal>();  // Dicionário para acumular as comissões

                while (!reader.EndOfStream)
                {
                    for (int i = 0; i < batchSize && !reader.EndOfStream; i++)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        var transfer = new Transfer
                        {
                            AccountId = values[0],
                            TransferId = values[1],
                            TransferAmount = decimal.Parse(values[2], CultureInfo.InvariantCulture)
                        };
                        buffer.Add(transfer);
                    }

                    var accountCommissions = CalculateCommissions(buffer);

                    foreach (var account in accountCommissions)
                    {
                        if (totalCommissions.ContainsKey(account.Key))
                        {
                            totalCommissions[account.Key] += account.Value;
                        }
                        else
                        {
                            totalCommissions.Add(account.Key, account.Value);
                        }
                    }

                    buffer.Clear(); // Clear the buffer for next batch
                }

                foreach (var account in totalCommissions)
                {
                    Console.WriteLine($"{account.Key},{account.Value.ToString("0.##", CultureInfo.InvariantCulture)}");
                }
            }
        }

        public static Dictionary<string, decimal> CalculateCommissions(List<Transfer> transfers)
        {
            if (transfers.Any(t => t.TransferAmount <= 0))
            {
                throw new ArgumentException("Transfer cannot equal or less than 0.");
            }

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

                var maxTransferToExclude = maxTransfers.First();

                if (transfersForAccount.Count == 1 || maxTransfers.Count == transfersForAccount.Count)
                {
                    var totalCommission = transfersForAccount.Sum(t => t.TransferAmount * 0.1m);
                    accountCommissions[accountId] = totalCommission;
                }
                else
                {
                    var totalCommission = transfersForAccount
                        .Where(t => t != maxTransferToExclude)
                        .Sum(t => t.TransferAmount * 0.1m);

                    accountCommissions[accountId] = totalCommission;
                }
            }

            return accountCommissions;
        }
    }

    public class Transfer
    {
        public string AccountId { get; set; }
        public string TransferId { get; set; }
        public decimal TransferAmount { get; set; }
    }
}