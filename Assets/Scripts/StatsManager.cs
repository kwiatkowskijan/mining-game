namespace MiningGame
{
    public class StatsManager
    {
        private static StatsManager _instance;

        public static StatsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StatsManager();
                return _instance;
            }
        }
        
        //Stats
        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        private int health;
    }
}
