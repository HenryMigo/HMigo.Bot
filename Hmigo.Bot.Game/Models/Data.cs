namespace Hmigo.Bot.Game.Models
{
    public class Data
    {
        public List<Player> Players {  get; set; }

        public Data(List<Player> players)
        {
            Players = players;
        }
    }
}
