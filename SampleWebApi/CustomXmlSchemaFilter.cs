using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SampleWebApi
{
    public class CustomXmlSchemaFilter : ISchemaFilter
    {
        private const string _SCHEMA_ARRAY_TYPE = "array";
        private const string _SCHEMA_STRING_TYPE = "string";
        private const string _PREFIX_ARRAY = "ArrayOf";

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.ApiModel.Type.IsValueType)
                return;

            if (schema.Type == _SCHEMA_STRING_TYPE)
                return;

            schema.Xml = new OpenApiXml
            {
                Name = context.ApiModel.Type.Name
            };

            if (schema.Type == _SCHEMA_ARRAY_TYPE)
            {
                var itemName = string.Empty;
                if (schema.Items.Reference != null)
                {
                    itemName = schema.Items.Reference.Id;
                }
                else
                {
                    schema.Items.Xml = new OpenApiXml
                    {
                        Name = schema.Items.Type,
                    };
                    itemName = schema.Items.Type;
                }

                schema.Xml = new OpenApiXml
                {
                    Name = $"{_PREFIX_ARRAY}{itemName}",
                    Wrapped = true,
                };
            }

            if (schema.Properties == null)
            {
                return;
            }

            foreach (var property in schema.Properties.Where(x => x.Value.Type == _SCHEMA_ARRAY_TYPE))
            {
                property.Value.Items.Xml = new OpenApiXml
                {
                    Name = property.Value.Items.Type,
                };
                property.Value.Xml = new OpenApiXml
                {
                    Name = property.Key,
                    Wrapped = true
                };
            }
        }
    }
}
