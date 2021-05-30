namespace MoviesApi.Core.Helpers
{
    public static class MovieHelper
    {
        public static bool ConvertIdToInt(string id, out int idAsInt)
        {
            idAsInt = 0;
            if (string.IsNullOrWhiteSpace(id))
                return false;

            id = id.Replace("tt", "");
            return int.TryParse(id, out idAsInt);
        }
    }
}