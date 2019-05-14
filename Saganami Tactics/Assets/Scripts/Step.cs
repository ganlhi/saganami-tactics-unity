public enum Step
{
    Plotting,
    SetupSalvos,
    EarlySalvo,
    HalfMove,
    MiddleSalvo,
    FirstBeamImpact,
    FullMove,
    LateSalvo,
    SecondBeamImpact,
    EndOfTurn,
};

public static class StepExtentions
{
    public static string ToString(this Step step)
    {
        switch (step)
        {
            case Step.Plotting:
                return "Plotting";
            case Step.SetupSalvos:
                return "Setup salvos";
            case Step.EarlySalvo:
                return "Early salvo";
            case Step.HalfMove:
                return "Half movement";
            case Step.MiddleSalvo:
                return "Middle salvo";
            case Step.FirstBeamImpact:
                return "1st beam impact";
            case Step.FullMove:
                return "Full movement";
            case Step.LateSalvo:
                return "Late salvo";
            case Step.SecondBeamImpact:
                return "2nd beam impact";
            case Step.EndOfTurn:
                return "End of turn";
            default:
                throw new System.Exception();
        }
    }
}