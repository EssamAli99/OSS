namespace OSS.Services
{
    public class QueryExtension
    {
        // using System.Linq.Dynamic.Core;
        //using linq dynamic to pass a string to the Where 
        //IQueryable TextFilter_Strings(IQueryable source, Dictionary<string, string> param)
        //{
        //    if (param == null || param.Count == 0) { return source; }

        //    var elementType = source.ElementType;


        //    // Get all the string property names on this specific type.
        //    var stringProperties = elementType.GetProperties()
        //            .Where(x => x.PropertyType == typeof(string))
        //            .ToArray();
        //    if (!stringProperties.Any()) { return source; }

        //    // Build the string expression
        //    string filterExpr = string.Join(" || ", stringProperties.Select(prp => $"{prp.Name}.Contains(@0)"));

        //    return source.Where(filterExpr);

        //    var allProperties = elementType.GetProperties().ToDictionary(x => x.Name);
        //    foreach (var item in param)
        //    {
        //        if (allProperties[item.Key].PropertyType == typeof(string))
        //        {
        //            filterExpr = string.Join(" || ", stringProperties.Select(prp => $"{prp.Name}.Contains({item.Value})"));
        //        }

        //    }
        //}


        //// using static System.Linq.Expressions.Expression;

        //IQueryable<T> TextFilter<T>(IQueryable<T> source, string term)
        //{
        //    if (string.IsNullOrEmpty(term)) { return source; }

        //    // T is a compile-time placeholder for the element type of the query.
        //    Type elementType = typeof(T);

        //    // Get all the string properties on this specific type.
        //    PropertyInfo[] stringProperties =
        //        elementType.GetProperties()
        //            .Where(x => x.PropertyType == typeof(string))
        //            .ToArray();
        //    if (!stringProperties.Any()) { return source; }

        //    // Get the right overload of String.Contains
        //    MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        //    // Create a parameter for the expression tree:
        //    // the 'x' in 'x => x.PropertyName.Contains("term")'
        //    // The type of this parameter is the query's element type
        //    ParameterExpression prm = Parameter(elementType);

        //    // Map each property to an expression tree node
        //    IEnumerable<Expression> expressions = stringProperties
        //        .Select(prp =>
        //            // For each property, we have to construct an expression tree node like x.PropertyName.Contains("term")
        //            Call(                  // .Contains(...) 
        //                Property(          // .PropertyName
        //                    prm,           // x 
        //                    prp
        //                ),
        //                containsMethod,
        //                Constant(term)     // "term" 
        //            )
        //        );

        //    // Combine all the resultant expression nodes using ||
        //    Expression body = expressions
        //        .Aggregate(
        //            (prev, current) => Or(prev, current)
        //        );

        //    // Wrap the expression body in a compile-time-typed lambda expression
        //    Expression<Func<T, bool>> lambda = Lambda<Func<T, bool>>(body, prm);

        //    // Because the lambda is compile-time-typed (albeit with a generic parameter), we can use it with the Where method
        //    return source.Where(lambda);
        //        }
    }
}
