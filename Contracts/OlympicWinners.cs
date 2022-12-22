namespace Contracts;

public record class OlympicWinners
{
    public string Athlete { get; set; } = null!;
    public int Age { get; set; }
    public string Country { get; set; } = null!;
    public string CountryGroup { get; set; } = null!;
    public int Year { get; set; }
    public string Date { get; set; } = null!;
    public string Sport { get; set; } = null!;
    public int Gold { get; set; }
    public int Silver { get; set; }
    public int Bronze { get; set; }
    public int Total { get; set; }
}