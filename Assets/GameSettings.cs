public static class GameSettings
{
    public enum Difficulty
    {
        Normal,
        Hard
    }

    // This static variable will hold our choice and persist between scenes.
    public static Difficulty selectedDifficulty;
}