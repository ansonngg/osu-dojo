namespace OsuDojo.Exam.Stage;

public interface IStageResult
{
    int Great { get; }
    int Ok { get; }
    int Miss { get; }
    int LargeBonus { get; }
    int SmallBonus { get; }
    int MaxCombo { get; }
}
