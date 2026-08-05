namespace Huskui.Gallery.Models;

public sealed record Country(
    string Name,
    string Region,
    string Capital,
    long Population,
    double Area,
    double Gdp)
{
    public string Status => Gdp >= 1000 ? "High Income" : "Emerging";
}
