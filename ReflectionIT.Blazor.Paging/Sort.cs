using System;

namespace ReflectionIT.Blazor.Paging {

    public record Sort {

        public string Key { get; }
        public string SortExpression { get; }
        public string SortExpression2 { get; init; }
        public string? Text { get; init; }
        public bool IsDecending { get; init; }

        public Sort(string key) : this(key, key) {
        }

        public Sort(string key, string sortExpression)  {
            Key = Text = key ?? throw new ArgumentNullException(nameof(key));
            SortExpression = sortExpression ?? throw new ArgumentNullException(nameof(sortExpression));
            IsDecending = sortExpression.EndsWith(" desc");
            var pos = sortExpression.IndexOf(',');
            if (pos > -1) {
                IsDecending = pos < 6 ? false : sortExpression.Substring(pos - 5, 5) == " desc";
            }
            SortExpression2 = (pos, IsDecending) switch {
                (-1, false) => sortExpression + " desc",
                (-1, true) => sortExpression.Replace(" desc", string.Empty),
                (_, true) => $"{sortExpression.Substring(0, pos - 5)}{sortExpression[pos..]}",
                _ => $"{sortExpression.Substring(0, pos)} desc{sortExpression[pos..]}",
            };
        }

        public Sort(string key, string sortExpression, string text) : this(key, sortExpression) {
            Text = text;
        }
    }

}
