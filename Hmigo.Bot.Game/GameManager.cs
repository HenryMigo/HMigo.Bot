using Hmigo.Bot.Game.Models;
using System.Text.Json;

namespace Hmigo.Bot.Game
{
    public class GameManager
    {
        private readonly string GameDataLocation = @"C:\GameData";

        /// <summary>
        /// Here we is where the initial setup for the game load is.
        /// </summary>
        public Data? SetupGame()
        {
            try
            {
                if (!Directory.Exists(GameDataLocation))
                {
                    Directory.CreateDirectory(GameDataLocation);
                }

                if (!File.Exists($"{GameDataLocation}\\data.json"))
                {
                    Console.WriteLine("File does not exist, creating new one...");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("If this was not meant to happen the load file has been lost.");
                    Console.ForegroundColor = ConsoleColor.White;
                    
                    var data = new Data(new List<Player>());
                    SaveGame(data);
                    
                    return data;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("File already exists, reading instead.");
                    Console.ForegroundColor = ConsoleColor.White;

                    return LoadGameData();
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }

            return null;
        }

        public void SaveGame(Data gameData)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var dataJson = JsonSerializer.Serialize(gameData, options);

            File.WriteAllText($"{GameDataLocation}\\data.json", dataJson);
        }

        private Data? LoadGameData()
        {
            var data = File.ReadAllText($"{GameDataLocation}\\data.json");

            if (!string.IsNullOrWhiteSpace(data))
            {
                return JsonSerializer.Deserialize<Data>(data);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not read file...");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return null;
        }

        public string JoinGame(string userName, ref Data gameData)
        {
            var doesExist = gameData.Players.Exists(x => x.Username == userName);
            
            if (doesExist)
            {
                return $"{userName} you've already joined the game!";
            }
            else
            {
                gameData.Players.Add(new Player(userName, gold: 0));
                SaveGame(gameData);
            }

            return $"{userName} you've successfully joined the game!";
        }
    }
}