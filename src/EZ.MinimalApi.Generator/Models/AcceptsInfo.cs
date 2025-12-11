using System.Collections.Generic;

namespace EZ.MinimalApi.Generator.Models;

public class AcceptsInfo
{
    public string RequestType { get; set; }
    public string ContentType { get; set; }
    public List<string> AdditionalContentTypes { get; set; } = new List<string>();
}
