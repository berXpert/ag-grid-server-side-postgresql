namespace Api.Contracts;

public record OlympicWinners(
    string Athlete,
    int Age,
    string Country,
    string CountryGroup,
    int Year,
    string Date,
    string Sport,
    int Gold,
    int Silver,
    int Bronze,
    int Total
)
{
    public OlympicWinners() :
        this(
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default)
    {}
}