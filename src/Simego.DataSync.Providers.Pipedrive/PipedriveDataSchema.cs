using Newtonsoft.Json.Linq;
using Simego.DataSync.Providers.Pipedrive.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simego.DataSync.Providers.Pipedrive
{
    public class PipedriveDataSchema
    {
        public static IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper, string name, string url)
        {
            var result = new Dictionary<string, PipedriveDataSchemaItem>();

            var schema = helper.GetRequestAsJson(url);

            foreach (var item in schema["data"])
            {
                var pdsi = new PipedriveDataSchemaItem();

                pdsi.Id = ConvertTo<int>(item["id"]?.ToObject<object>(), -1);
                pdsi.Key = item["key"].ToObject<string>();
                pdsi.Name = item["name"]?.ToObject<string>();
                pdsi.Order_nr = ConvertTo<int>(item["order_nr"]?.ToObject<object>(), 0);
                pdsi.Picklist_Data = item["picklist_data"]?.ToObject<string>();
                
                switch (item["field_type"]?.ToObject<string>())
                {
                    case "int":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldInteger;
                            break;
                        }
                    case "varchar":
                        {
                            switch (pdsi.Key)
                            {
                                case "email":
                                    {
                                        pdsi.Field_Type = PipedriveDataSchemaItemType.FieldEmail;
                                        pdsi.Parser = new EmailValueParser();
                                        break;
                                    }
                                case "im":
                                    {
                                        pdsi.Field_Type = PipedriveDataSchemaItemType.FieldInstantMessenger;
                                        pdsi.Parser = new EmailValueParser();
                                        break;
                                    }
                                default:
                                    {
                                        pdsi.Field_Type = PipedriveDataSchemaItemType.FieldString;
                                        break;
                                    }
                            }
                            break;
                        }
                    case "visible_to":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldVisibleTo;
                            break;
                        }
                    case "text":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldText;
                            break;
                        }
                    case "double":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldDouble;
                            break;
                        }
                    case "user":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldUser;
                            pdsi.Parser = new UserValueParser();
                            break;
                        }
                    case "org":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldOrg;
                            pdsi.Parser = new OrgValueParser();
                            break;
                        }
                    case "date":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldDate;
                            break;
                        }
                    case "phone":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldPhone;
                            pdsi.Parser = new EmailValueParser();
                            break;
                        }
                    case "enum":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldEnum;
                            pdsi.Parser = new EnumValueParser()
                            {
                                Options = pdsi.Options
                            };
                            break;
                        }
                    case "people":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldPeople;
                            pdsi.Parser = new PeopleValueParser();
                            break;
                        }
                    case "monetary":
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldDecimal;
                            break;
                        }
                    default:
                        {
                            pdsi.Field_Type = PipedriveDataSchemaItemType.FieldString;
                            break;
                        }
                }

                pdsi.Add_time = item["add_time"]?.ToObject<DateTime?>();
                pdsi.Update_time = item["update_time"]?.ToObject<DateTime?>();
                pdsi.Last_updated_by_user_id = ConvertTo<int>(item["last_updated_by_user_id"]?.ToObject<object>(), -1);

                pdsi.Active_flag = ConvertTo<bool>(item["active_flag"]?.ToObject<object>(), false);
                pdsi.Edit_flag = ConvertTo<bool>(item["edit_flag"]?.ToObject<object>(), false);
                pdsi.Index_visible_flag = ConvertTo<bool>(item["index_visible_flag"]?.ToObject<object>(), false);
                pdsi.Details_visible_flag = ConvertTo<bool>(item["details_visible_flag"]?.ToObject<object>(), false);
                pdsi.Add_visible_flag = ConvertTo<bool>(item["add_visible_flag"]?.ToObject<object>(), false);
                pdsi.Important_flag = ConvertTo<bool>(item["important_flag"]?.ToObject<object>(), false);
                pdsi.Bulk_edit_allowed = ConvertTo<bool>(item["bulk_edit_allowed"]?.ToObject<object>(), false);
                pdsi.Searchable_flag = ConvertTo<bool>(item["searchable_flag"]?.ToObject<object>(), false);
                pdsi.Filtering_allowed = ConvertTo<bool>(item["filtering_allowed"]?.ToObject<object>(), false);
                pdsi.Sortable_flag = ConvertTo<bool>(item["sortable_flag"]?.ToObject<object>(), false);
                //pdsi.Mandatory_flag = ConvertTo<bool>(item["mandatory_flag"]?.ToObject<object>(), false); // TODO: This errors as it can return an expression.

                if (item["options"] is JArray array)
                {
                    foreach (var i in array)
                    {
                        pdsi.Options[i["id"].ToObject<string>()] = i["label"].ToObject<string>();
                    }
                }

                ApplyCustomFixesForPipedriveErrors(pdsi, name);

                result[pdsi.Key] = pdsi;
            }

            return result;

        }

        private static T ConvertTo<T>(object value, T defaultValue)
        {
            if (value == null)
                return defaultValue;

            return DataSchemaTypeConverter.ConvertTo<T>(value);
        }

        private static void ApplyCustomFixesForPipedriveErrors(PipedriveDataSchemaItem item, string name)
        {
            if (string.Equals(name, "deal", StringComparison.OrdinalIgnoreCase))
            {
                //Pipedrive returns pipeline rather than pipeline_id.

                if (item.Key == "pipeline")
                {
                    item.Key = "pipeline_id";
                    item.Field_Type = PipedriveDataSchemaItemType.FieldInteger;
                }
            }
        }
    }
}
