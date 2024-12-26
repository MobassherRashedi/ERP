using System.Text;
using System;

namespace DevSkill.Inventory.Domain
{
    public abstract class DataTables
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public SortColumn[] Order { get; set; } = Array.Empty<SortColumn>();
        public DataTablesSearch Search { get; set; } = new DataTablesSearch();

        public int PageIndex
        {
            get
            {
                if (Length > 0)
                    return (Start / Length) + 1;
                else
                    return 1;
            }
        }

        public int PageSize
        {
            get
            {
                if (Length == 0)
                    return 10;
                else
                    return Length;
            }
        }

        public static object EmptyResult
        {
            get
            {
                return new
                {
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = (new string[] { }).ToArray()
                };
            }
        }

        public string? FormatSortExpression(params string[] columns)
        {
            StringBuilder columnBuilder = new StringBuilder();
            Console.WriteLine($"Total columns available: {columns.Length}");

            for (int i = 0; i < Order.Length; i++)
            {
                Console.WriteLine($"Processing Order[{i}]: ColumnIndex={Order[i].Column}, Direction={Order[i].Dir}");

                if (Order[i].Column >= 0 && Order[i].Column < columns.Length)
                {
                    columnBuilder.Append(columns[Order[i].Column])
                                 .Append(" ")
                                 .Append(Order[i].Dir);

                    if (i < Order.Length - 1)
                        columnBuilder.Append(", ");
                }
                else
                {
                    Console.WriteLine($"Warning: Order[{i}].Column index {Order[i].Column} is out of bounds for columns array.");
                }
            }

            var orderString = columnBuilder.ToString();
            if (string.IsNullOrEmpty(orderString))
            {
                Console.WriteLine("No valid sort expressions were created.");
            }
            else
            {
                Console.WriteLine($"Final sort expression: {orderString}");
            }

            return string.IsNullOrEmpty(orderString) ? null : orderString;
        }



    }

    public struct SortColumn
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public struct DataTablesSearch
    {
        public bool Regex { get; set; }
        public string Value { get; set; }
    }
}