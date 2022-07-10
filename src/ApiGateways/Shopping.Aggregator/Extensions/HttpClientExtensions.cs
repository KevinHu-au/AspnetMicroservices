using System;
using System.Text.Json;

namespace Shopping.Aggregator.Extensions
{
  public static class HttpClientExtensions
  {
    public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
    {
      if (!response.IsSuccessStatusCode)
        throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

      using var contentStream = await response.Content.ReadAsStreamAsync();

      return await JsonSerializer.DeserializeAsync<T>(contentStream);
    }
  }
}
