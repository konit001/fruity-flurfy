// สถิติของรอบที่กำลังเล่น — ศัตรูตาย / ได้ทอง บวกเพิ่มที่นี่ แล้วเขียนลง RunHistory ตอนตาย
public static class RunStats
{
    public static int Kills;
    public static int GoldEarned;

    public static void Reset()
    {
        Kills = 0;
        GoldEarned = 0;
    }
}
