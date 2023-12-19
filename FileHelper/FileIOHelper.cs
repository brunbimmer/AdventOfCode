using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common;
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

        public Dictionary<(int, int), int> GetDataAsIntegerMap(string file)
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

        public Dictionary<(int, int), int> GetDataAsCharMapWithValues(string file)
        {
            Dictionary<(int, int), int> grid = new Dictionary<(int, int), int>();

            //read in the
            string[] lines = ReadDataAsLines(file);

            //Read the data
            var mapData = lines.Select(x => x.ToArray()).ToArray();

            for (int x = 0; x < mapData.Length; x++)
                for (int y = 0; y < mapData[x].Length; y++)
                {
                    int value = CalculateLetterValue(mapData[x][y]);
                    grid.Add((x, y), value);
                }
                    

            return grid;
        }


        public Dictionary<Coordinate2D, char> GetDataAsCoordinates(string file)
        {
            Dictionary<Coordinate2D, char> grid = new Dictionary<Coordinate2D, char>();

            //read in the
            string[] lines = ReadDataAsLines(file);

            //Read the data
            var mapData = lines.Select(x => x.ToArray()).ToArray();

            for (int x = 0; x < mapData.Length; x++)
                for (int y = 0; y < mapData[x].Length; y++)
                    grid.Add(new Coordinate2D(x , y), mapData[x][y]);

            return grid;
        }

        public (Dictionary<Coordinate2D, int>, int, int) GetDataAsIntMap(string file)
        {
            Dictionary<Coordinate2D, int> grid = new Dictionary<Coordinate2D, int>();

            //read in the
            string[] lines = ReadDataAsLines(file);

            //Read the data
            var mapData = lines.Select(x => x.ToArray()).ToArray();

            int maxX = mapData.Length;
            int maxY = mapData[0].Length;

            for (int x = 0; x < mapData.Length; x++)
                for (int y = 0; y < mapData[x].Length; y++)
                    grid.Add(new Coordinate2D(x, y), mapData[x][y]);

            return (grid, maxX, maxY);
        }

        public int[][] GetDataAsDoubleIntArray(string file)
        {
            string[] lines = ReadDataAsLines(file);

            int[][] grid = lines.Select(x => x.Select(y => int.Parse(y.ToString())).ToArray()).ToArray();

            return grid;
        }

        public char[][] GetDataAsDoubleCharArray(string file)
        {
            string[] lines = ReadDataAsLines(file);

            char[][] grid = lines.Select(x => x.Select(y => y).ToArray()).ToArray();

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
            string data = "";

            using (HttpClient hc = new HttpClient(new HttpClientHandler {UseCookies = false}))
            {
                hc.DefaultRequestHeaders.Add("Cookie", "session=" + _session);

                string url = string.Format(_url, year, day);

                var webRequest = new HttpRequestMessage(HttpMethod.Get, url);

                HttpResponseMessage response = hc.Send(webRequest);
             
                using var reader = new StreamReader(response.Content.ReadAsStream());
            
                data = reader.ReadToEnd();
            }

            File.WriteAllText(saveFileLocation, data);
        }

        private int CalculateLetterValue(char character)
        {
            int ascii = (int)character;
            if (Char.IsUpper(character)) 
                return (ascii - 38);
            else
                return (ascii - 96);
        }
    }
}
