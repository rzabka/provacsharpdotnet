using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Humanizer.Bytes;

namespace Portal.Models
{
    public class PortalViewModel
    {
        public string Title { get; set; }
        public string DateTimeNow { get; set; }
        public string Key { get; set; }
        public int Sum { get; set; }
        public int VirtualMachines { get;  set; }
        public int RequestsCounter { get;  set; }
        public int GC0 { get;  set; }
        public int GC1 { get;  set; }
        public int GC2 { get;  set; }
        public string CurrentMemory { get;  set; }
        public ByteSize PrivateBytes { get;  set; }

        public void PrepareViewModel(HttpClient httpClient, int requestsCounter)
        {
            var result = GetDateTime(httpClient);
            var key = GenerateKey(result);

            this.Title = "Portal do tempo";
            this.DateTimeNow = result;
            this.Key = key;
            this.Sum = GetSumOfOddNumbers(key);
            this.VirtualMachines = Program.NUMBER_OF_VIRTUAL_MACHINES;
            this.RequestsCounter = requestsCounter;
            this.GC0 = GC.CollectionCount(0);
            this.GC1 = GC.CollectionCount(1);
            this.GC2 = GC.CollectionCount(2);
            this.CurrentMemory = ByteSize.FromBytes(GC.GetTotalMemory(false)).ToString();
            this.PrivateBytes = ByteSize.FromBytes(Process.GetCurrentProcess().WorkingSet64);
        }

        /// <summary>
        /// Obtém a data da API
        /// </summary>
        /// <returns></returns>
        private static string GetDateTime(HttpClient _httpClient)
        {
            return _httpClient.GetStringAsync(Program.API_ADDRESS).Result;
        }

        /// <summary>
        /// Calcula e gera a chave
        /// </summary>
        /// <returns>string Key</returns>
        private static string GenerateKey(string result)
        {
            var day = DateTime.Parse(result).Day;
            var key = string.Empty;

            for (int i = 0; i < 4096; i++)
            {
                var random = new Random();
                key = string.Concat(key, day * i * random.Next(100, 9999));
            }

            return key;
        }

        /// <summary>
        /// Obtem a soma dos números ímpares gerados na chave.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>int Sum of Odd Numbers</returns>
        private static int GetSumOfOddNumbers(string key)
        {
            var oddNumbers = new List<int>();
            foreach (var character in key.ToArray())
                if (int.Parse(character.ToString()) % 2 != 0)
                    oddNumbers.Add(int.Parse(character.ToString()));

            return oddNumbers.Sum(x => x);
        }

    }
}
