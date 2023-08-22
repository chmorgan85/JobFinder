namespace JobFinder.Models.JSON
{
    public record class AdzunaResults(List<AdzunaJob> results);

    public record class AdzunaJob(
        AdzunaCompany company,
        DateTime created,
        string description,
        AdzunaLocation location,
        string redirect_url,
        float salary_max,
        float salary_min,
        string title);

    public record class AdzunaCompany(string display_name);

    public record class AdzunaLocation(string display_name);
}
