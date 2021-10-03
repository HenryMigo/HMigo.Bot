namespace Hmigo.Bot.Game.Models
{
    public class Player
    {
        /// <summary>
        /// This ID is the twitch username
        /// </summary>
        public string Username { get; set; }

        public int Gold {  get; set; }

        public Player(string userName, int gold)
        {
            Username = userName;
            Gold = gold;
        }
    }
}