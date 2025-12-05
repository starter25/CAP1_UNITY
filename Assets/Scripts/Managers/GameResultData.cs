public static class GameResultData
{
    public static int Score { get; set; }
    public static int Perfect { get; set; }
    public static int Great { get; set; }
    public static int Good { get; set; }
    public static int Miss { get; set; }
    public static int MaxCombo { get; set; }
    public static float FeverGauge { get; set; }
    public static int FeverCount { get; set; }
    public static void Reset()
    {
        Score = 0;
        Perfect = 0;
        Great = 0;
        Good = 0;
        Miss = 0;
        MaxCombo = 0;
        FeverGauge = 0f;
        FeverCount = 0;
    }
}
