using Simego.DataSync.Providers.Pipedrive.Interfaces;
using Simego.DataSync.Providers.Pipedrive.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simego.DataSync.Providers.Pipedrive
{ 
    public enum PipedriveDataSchemaItemType
    {
        FieldInteger,           //int
        FieldString,            //varchar
        FieldVisibleTo,         //visible_to
        FieldText,              //text
        FieldDouble,            //double
        FieldDecimal,           //monetary
        FieldUser,              //user
        FieldOrg,               //org
        FieldDate,              //date
        FieldPhone,             //phone
        FieldEnum,              //enum
        FieldEmail,             //email - custom as pipedrive returns this as varchar
        FieldInstantMessenger,  //im - custom as pipedrive returns this as varchar
        FieldPeople,            //people
        FieldBoolean,           //bool
    }
    public class PipedriveDataSchemaItem
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public int Order_nr { get; set; }
        public string Picklist_Data { get; set; }
        public PipedriveDataSchemaItemType Field_Type { get; set; }
        public DateTime? Add_time { get; set; }
        public DateTime? Update_time { get; set; }
        public int Last_updated_by_user_id { get; set; }
        public bool Active_flag { get; set; }
        public bool Edit_flag { get; set; }
        public bool Index_visible_flag { get; set; }
        public bool Details_visible_flag { get; set; }
        public bool Add_visible_flag { get; set; }
        public bool Important_flag { get; set; }
        public bool Bulk_edit_allowed { get; set; }
        public bool Searchable_flag { get; set; }
        public bool Filtering_allowed { get; set; }
        public bool Sortable_flag { get; set; }
        public bool Mandatory_flag { get; set; }
        public IDictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
        public IPipedriveValueParser Parser { get; set; } = new DefaultValueParser();
        public bool ReadOnly => !Edit_flag;

        //{
        //      "id": 9047,
        //      "key": "visible_to",
        //      "name": "Visible to",
        //      "order_nr": 10,
        //      "field_type": "visible_to",
        //      "add_time": "2019-06-14 08:57:44",
        //      "update_time": "2019-06-14 08:57:43",
        //      "last_updated_by_user_id": null,
        //      "active_flag": true,
        //      "edit_flag": false,
        //      "index_visible_flag": true,
        //      "details_visible_flag": true,
        //      "add_visible_flag": false,
        //      "important_flag": false,
        //      "bulk_edit_allowed": true,
        //      "searchable_flag": false,
        //      "filtering_allowed": true,
        //      "sortable_flag": true,
        //      "options": [
        //        {
        //          "id": 1,
        //          "label": "Owner & followers"
        //        },
        //        {
        //          "id": 3,
        //          "label": "Entire company"
        //        }
        //      ],
        //      "mandatory_flag": true
        //    }

        public DataSchemaItem ToDataSchemaItem()
        {
            switch (Field_Type)
            {
                case PipedriveDataSchemaItemType.FieldInteger:
                    return new DataSchemaItem(Key, Name, typeof(int), Key == "id", ReadOnly, Key != "id", -1);

                case PipedriveDataSchemaItemType.FieldDouble:
                    return new DataSchemaItem(Key, Name, typeof(double), false, ReadOnly, true, -1);

                case PipedriveDataSchemaItemType.FieldDecimal:
                    return new DataSchemaItem(Key, Name, typeof(decimal), false, ReadOnly, true, -1);

                case PipedriveDataSchemaItemType.FieldDate:
                    return new DataSchemaItem(Key, Name, typeof(DateTime), false, ReadOnly, true, -1);

                case PipedriveDataSchemaItemType.FieldText:
                    return new DataSchemaItem(Key, Name, typeof(string), false, ReadOnly, true, -1);

                case PipedriveDataSchemaItemType.FieldBoolean:
                    return new DataSchemaItem(Key, Name, typeof(bool), false, ReadOnly, true, -1);

                default:
                    return new DataSchemaItem(Key, Name, typeof(string), false, ReadOnly, true, 255);

            }
        }
    }
}
