using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AdventFileIO
{
    public class FileIOHelper
    {
        //transfer session ID to file.
        private static FileIOHelper _instance;
        private const string BASE_FILE_PATH = "inputs";

        private string _url;
        private string _session;
        

        private FileIOHelper()
        {

        }

        public static FileIOHelper getInstance()
        {
            return _instance ?? (_instance = new FileIOHelper());
        }

        public void Initialize()
        {
            IConfiguration Config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();

            _url = Config.GetSection("advent_of_code_url").Value;
            _session = Config.GetSection("session").Value;
        }

        public string InitFileInput(int year, int day, string filename)
        {
            string basePath = $"{BASE_FILE_PATH}/{year}-{day}/";
            Directory.CreateDirectory(basePath);

            string pathToFile = basePath + filename;

            if (!File.Exists(pathToFile))
                GetFileInputFromAOC(year, day, pathToFile);

            return pathToFile;
        }

        public string ReadDataAsString(string file)
        {
            return File.ReadAllText(file);
        }

        public string[] ReadDataAsLines(string file)
        {
            return File.ReadAllLines(file);
        }

        public int[] ReadDataToIntArray(string file)
        {
            return Array.ConvertAll(File.ReadAllLines(file), int.Parse);
        }

        public long[] ReadDataToLongArray(string file)
        {
            return Array.ConvertAll(File.ReadAllLines(file), long.Parse);
        }

        public Dictionary<(int, int), int> GetDataAsMap(string file)
        {
            Dictionary<(int, int), int> grid = new Dictionary<(int, int), int>();

            //read in the
            string[] lines = ReadDataAsLines(file);

            //Read the data
            var mapData = lines.Select(x => x.ToArray()).ToArray();

            for (int x = 0; x < mapData.Length; x++)
                for (int y = 0; y < mapData[x].Length; y++)
                    grid.Add((x, y), int.Parse(mapData[x][y].ToString()));

            return grid;
        }

        public List<(string,string)> ReadDataAsTupleList(string file)
        {
            List<(string, string)> tupleList = new();
            try
            {
                string[] lines = File.ReadAllLines(file);

                foreach (string line in lines)
                {
                    string[] dataInput = line.Split(' ');

                    tupleList.Add((dataInput[0], dataInput[1]));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return tupleList;
        }

        private void GetFileInputFromAOC(int year, int day, string saveFileLocation)
        {
            System.Net.WebClient wc = new System.Net.WebClient();

            wc.Headers.Add(System.Net.HttpRequestHeader.Cookie, "session=" + _session);

            string url = string.Format(_url, year, day);
            string data = wc.DownloadString(url);

            File.WriteAllText(saveFileLocation, data);
        }

    }
}
