namespace OnRail.ResultDetails;

public class ResultDetail {
    public ResultDetail(string title, string? message = null, int? statusCode = null, object? moreDetails = null) {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

        Title = title;
        StatusCode = statusCode;
        Message = message;
        if (moreDetails is not null)
            MoreDetails = new List<object> {moreDetails};
    }

    public string Title { get; }
    public string? Message { get; }
    public int? StatusCode { get; }
    public List<object>? MoreDetails { get; private set; }

    public ResultDetail AddDetail(object newDetail) {
        if (newDetail == null) throw new ArgumentNullException(nameof(newDetail));
        MoreDetails ??= new List<object>();
        MoreDetails.Add(newDetail);
        return this;
    }

    public List<T> GetMoreDetailProperties<T>(string? name = null) {
        if (MoreDetails is null || !MoreDetails.Any())
            return new List<T>();

        var result = new List<T>();
        foreach (var detail in MoreDetails) {
            if (string.IsNullOrWhiteSpace(name) && detail.GetType() == typeof(T)) {
                //The whole object is our target
                result.Add((T) detail);
                continue;
            }

            var props = detail.GetType().GetProperties()
                .Where(prop => prop.PropertyType == typeof(T));

            if (!string.IsNullOrWhiteSpace(name))
                props = props.Where(prop => prop.Name == name);

            var objs = props.Select(prop => prop.GetValue(detail, null))
                .Where(obj => obj is not null)
                .Select(obj => (T) obj!);
            result.AddRange(objs);
        }

        return result;
    }
}