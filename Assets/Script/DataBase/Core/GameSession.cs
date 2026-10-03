public static class GameSession
{
    public static int PlayerId = -1;
    public static bool IsLoggedIn => PlayerId > 0;

    public static void LogOut() => PlayerId = -1;
}
