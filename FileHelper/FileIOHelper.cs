using System;
using System.IO;
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
