namespace JobFinder.Models.JSON
{
	public record class FindworkRoot(List<FindworkResult> results);

	public record class FindworkResult(
		string role,
		string company_name,
		string location,
		string url,
		string text,
		DateTime date_posted);
}
