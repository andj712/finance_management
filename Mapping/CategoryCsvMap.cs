using CsvHelper.Configuration;
using finance_management.DTOs.ImportCategory;

namespace finance_management.Mapping
{
    public class CategoryCsvMap : ClassMap<CategoryDto>
    {
        public CategoryCsvMap()
        {
            Map(m => m.Code).Name("code");
            Map(m => m.Name).Name("name");
            Map(m => m.ParentCode).Name("parent-code").Optional().Convert(args =>
            {
                var value = args.Row.GetField("parent-code");
                return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            });
        }
    }
}
