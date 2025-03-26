using ApiUsers.Extensions;
using ApiUsers.Interfaces.Helpers;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Globalization;
using ApiUsers.Models.Common;

namespace ApiUsers.Helpers
{
    //filter object, con string o con un array de arrays
    public class FilterParserHelper : IFilterParserHelper
    {
        private readonly string[] ComparisonOperators = ["<", ">", "==", "!=", ">=", "<=", "like"];
        private readonly string[] AggroupedOperators = ["(", ")"];
        private readonly string[] LogicalOperators = ["and", "or"];

        public LambdaExpression? Generate<TSource, TOut>(IList<string[]> filters) where TSource : class
        {
            if (!filters.Any())
                return null;

            StringBuilder query_filter = new StringBuilder();
            var values = new List<object>();
            int index = 0;

            TSource instance = Activator.CreateInstance<TSource>();

            int countFiltersInGroup = 0;
            bool openedGroup = false;

            foreach (var filter in filters)
            {
                //TODO. Validate open close and exist expresion innner group
                if (!filter.Any())
                    continue;
                
                bool isOpenGroup = filter.Length == 1 && IsOpenGroupChar(filter[0]);
                if (isOpenGroup)
                {
                    //si no hay grupos abiertos marcamos como abierto
                    if (openedGroup)
                        throw new Exception("Error in filters schema secuence [Unexpected '(' char].");
                    else 
                    {
                        query_filter = query_filter.Append("(");
                        openedGroup = true;
                    }
                    continue;
                }

                bool isCloseGroup = filter.Length == 1 && IsCloseGroupChar(filter[0]);
                if (isCloseGroup)
                {
                    //si hay uno o mas entonces lo cerramos e inicializamos de nuevo el contador, si no lanzamos exception
                    if (openedGroup && countFiltersInGroup > 0)
                    {
                        query_filter = query_filter.Append(")");
                        openedGroup = false;
                        countFiltersInGroup = 0;
                    }
                    else
                        throw new Exception("Error in filters schema secuence [Unexpected ')' char].");
                    
                    continue;
                }

                bool isLogicOperator = filter.Length == 1 && IsLogicOperator(filter[0]);
                if (isLogicOperator)
                {
                    //validar que una si hay mas de una expresion siempre este precedido por un operador logico, para evitar tener comparer AND, COMPARER AND ), etc
                    query_filter = query_filter.Append(filter[0]);
                    continue;
                }

                bool isValidSingleFilter = filter.Length == 4;
                if (isValidSingleFilter)
                {
                    string _type = filter[0].Trim();
                    string _name = filter[1].Trim();
                    string _operator = filter[2].Trim();
                    string _value = filter[3].Trim();

                    _name.Guard(nameof(_name));
                    _operator.Guard(nameof(_operator));

                    if (!instance.GetType().HasPropertie(_name) 
                        && IsValidComparisonOperator(_operator))
                    {
                        continue;
                    }

                    string str_filter = _operator.Equals("like", StringComparison.OrdinalIgnoreCase)
                        ? $"{_name}.Contains(@{index})"
                        : $"{_name} {_operator} @{index}";

                    query_filter.Append(str_filter);
                    values.Add(Parse(_type, _value));

                    if (openedGroup)
                        countFiltersInGroup++;
                    
                    index++;
                }
            }

            //DynamicExpressionParser.ParseLambda(typeof(TSource), typeof(bool), query_filter.ToString(), values.ToArray());
            return DynamicExpressionParser.ParseLambda<TSource, TOut>(null, false, query_filter.ToString(), values.ToArray());
        }

        public Expression<Func<TSource, TOut>>? Generate<TSource, TOut>(IList<Filter> filters) where TSource : class
        {
            if (!filters.Any())
                return null;

            StringBuilder query_filter = new StringBuilder();
            var values = new List<object>();
            int index = 0;

            TSource instance = Activator.CreateInstance<TSource>();

            //Add support for AND and OR logic operatos
            foreach (var filter in filters)
            {
                filter.Name.Guard(nameof(filter.Name));
                filter.Operator.Guard(nameof(filter.Operator));

                if (!instance.GetType().GetProperties().Any(p => p.Name.Equals(filter.Name)))
                    continue;

                if (!IsValidComparisonOperator(filter.Operator))
                    throw new Exception($"[{filter.Operator}] Not supported operator.");

                string str_filter = filter.Operator.Equals("like", StringComparison.OrdinalIgnoreCase)
                    ? $"{filter.Name}.Contains(@{index})"
                    : $"{filter.Name} {filter.Operator} @{index}";

                query_filter.Append(str_filter);
                values.Add(Parse(filter.Type, filter.Value));
            }

            return DynamicExpressionParser.ParseLambda<TSource, TOut>(null, false, query_filter.ToString(), values.ToArray());
        }

        public object Parse(string type, string value)
        {
            type.Guard(nameof(type));

            if (value is null)
                throw new Exception("Value cannot be null.");

            return type switch 
            { 
                "TEXT" => Convert.ToString(value),
                "INTEGER" => int.Parse(value),
                "DECIMAL" => double.Parse(value),
                "DATE" => DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
                "BOOL" => bool.Parse(value),
                _ => throw new Exception("Not supported type.")
            };
        }

        private bool IsValidComparisonOperator(string comparisonOperator)
            => ComparisonOperators.Contains(comparisonOperator, StringComparer.OrdinalIgnoreCase);

        private bool IsLogicOperator(string strOperator)
            => LogicalOperators.Contains(strOperator, StringComparer.OrdinalIgnoreCase);

        private bool IsOpenGroupChar(string aggroupedOperator)
            => AggroupedOperators[0].Equals(aggroupedOperator, StringComparison.OrdinalIgnoreCase);

        private bool IsCloseGroupChar(string aggroupedOperator)
            => AggroupedOperators[1].Equals(aggroupedOperator, StringComparison.OrdinalIgnoreCase);
    }
}
