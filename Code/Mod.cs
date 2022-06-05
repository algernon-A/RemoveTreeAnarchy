using ICities;


namespace RemoveTreeAnarchy
{    /// <summary>
     /// The base mod class for instantiation by the game.
     /// </summary>
    public class RTAMod : IUserMod
    {
        public static string ModName => "Remove Tree Anarchy";
        public static string Version => "1.0";

        public string Name => ModName + " " + Version;
        public string Description => "Repacks trees created with Tree Anarchy into the vanilla array";
    }
}
