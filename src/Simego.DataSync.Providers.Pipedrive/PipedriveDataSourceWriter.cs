using Newtonsoft.Json;
using Simego.DataSync.Engine;
using Simego.DataSync.Interfaces;
using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Simego.DataSync.Providers.Pipedrive
{
    public class PipedriveDataSourceWriter : DataWriterProviderBase
    {
        private PipedriveDatasourceReader DataSourceReader { get; set; }
        private IPipedriveDatasourceInfo DatasourceInfo { get; set; }
        private DataSchemaMapping Mapping { get; set; }
        private HttpWebRequestHelper WebRequestHelper { get; set; }
        private IDictionary<string, PipedriveDataSchemaItem> DataSchema { get; set; }

        public override void AddItems(List<DataCompareItem> items, IDataSynchronizationStatus status)
        {
            if (items != null && items.Count > 0)
            {
                int currentItem = 0;

                foreach (var item in items)
                {
                    if (!status.ContinueProcessing)
                        break;

                    var itemInvariant = new DataCompareItemInvariant(item);

                    try
                    {                        
                        //Call the Automation BeforeAddItem 
                        Automation?.BeforeAddItem(this, itemInvariant, null);

                        if (itemInvariant.Sync)
                        {
                            #region Add Item

                            //Get the Target Item Data
                            var targetItem = AddItemToDictionary(Mapping, itemInvariant);
                            var targetItemToSend = new Dictionary<string, object>();

                            foreach (var k in targetItem.Keys)
                            {
                                var val = targetItem[k];

                                if (val == null)
                                    continue;

                                var pdsi = DataSchema[k];
                                targetItemToSend[pdsi.Key] = pdsi.Parser.ConvertValue(val);
                            }

                            if (targetItemToSend.Any())
                            {
                                var json = JsonConvert.SerializeObject(targetItemToSend, Formatting.None);
                                var result = WebRequestHelper.PostRequestAsJson(json, DatasourceInfo.PipedriveEndpointUrl);
                                var item_id = result["data"]["id"].ToObject<int>();

                                //Call the Automation AfterAddItem
                                Automation?.AfterAddItem(this, itemInvariant, item_id);
                            }
                            
                            #endregion
                        }                        

                        ClearSyncStatus(item); //Clear the Sync Flag on Processed Rows

                    }
                    catch (WebException e)
                    {
                        Automation?.ErrorItem(this, itemInvariant, null, e);
                        HandleError(status, e);
                    }
                    catch (SystemException e)
                    {
                        Automation?.ErrorItem(this, itemInvariant, null, e);
                        HandleError(status, e);
                    }
                    finally
                    {
                        status.Progress(items.Count, ++currentItem); //Update the Sync Progress
                    }

                }
            }
        }

        public override void UpdateItems(List<DataCompareItem> items, IDataSynchronizationStatus status)
        {
            if (items != null && items.Count > 0)
            {
                int currentItem = 0;

                foreach (var item in items)
                {
                    if (!status.ContinueProcessing)
                        break;

                    var itemInvariant = new DataCompareItemInvariant(item);

                    //Example: Get the item ID from the Target Identifier Store 
                    var item_id = itemInvariant.GetTargetIdentifier<int>();

                    try
                    {                        
                        //Call the Automation BeforeUpdateItem
                        Automation?.BeforeUpdateItem(this, itemInvariant, item_id);

                        if (itemInvariant.Sync)
                        {
                            #region Update Item

                            //Get the Target Item Data
                            var targetItem = UpdateItemToDictionary(Mapping, itemInvariant);
                            var targetItemToSend = new Dictionary<string, object>();

                            foreach(var k in targetItem.Keys)
                            {
                                var val = targetItem[k];

                                var pdsi = DataSchema[k];
                                targetItemToSend[pdsi.Key] = pdsi.Parser.ConvertValue(val);
                            }

                            if (targetItemToSend.Any())
                            {
                                var json = JsonConvert.SerializeObject(targetItemToSend, Formatting.None);
                                var result = WebRequestHelper.PutRequestAsJson(json, DatasourceInfo.GetPipedriveItemEndpointUrl(item_id));

                                //Call the Automation AfterUpdateItem 
                                Automation?.AfterUpdateItem(this, itemInvariant, item_id);
                            }
                            
                            #endregion
                        }

                        ClearSyncStatus(item); //Clear the Sync Flag on Processed Rows
                    }
                    catch(WebException e)
                    {
                        Automation?.ErrorItem(this, itemInvariant, item_id, e);
                        HandleError(status, e);
                    }
                    catch (SystemException e)
                    {
                        Automation?.ErrorItem(this, itemInvariant, item_id, e);
                        HandleError(status, e);
                    }
                    finally
                    {
                        status.Progress(items.Count, ++currentItem); //Update the Sync Progress
                    }

                }
            }
        }

        public override void DeleteItems(List<DataCompareItem> items, IDataSynchronizationStatus status)
        {
            if (items != null && items.Count > 0)
            {
                int currentItem = 0;

                foreach (var item in items)
                {
                    if (!status.ContinueProcessing)
                        break;

                    var itemInvariant = new DataCompareItemInvariant(item);

                    //Example: Get the item ID from the Target Identifier Store 
                    var item_id = itemInvariant.GetTargetIdentifier<int>();

                    try
                    {                        
                        //Call the Automation BeforeDeleteItem (Optional only required if your supporting Automation Item Events)
                        Automation?.BeforeDeleteItem(this, itemInvariant, item_id);

                        if (itemInvariant.Sync)
                        {
                            #region Delete Item

                            var result = WebRequestHelper.DeleteRequestAsJson(null, DatasourceInfo.GetPipedriveItemEndpointUrl(item_id));

                            #endregion

                            //Call the Automation AfterDeleteItem 
                            Automation?.AfterDeleteItem(this, itemInvariant, item_id);
                        }

                        ClearSyncStatus(item); //Clear the Sync Flag on Processed Rows
                    }
                    catch (WebException e)
                    {
                        Automation?.ErrorItem(this, itemInvariant, item_id, e);
                        HandleError(status, e);
                    }
                    catch (SystemException e)
                    {
                        Automation?.ErrorItem(this, itemInvariant, item_id, e);
                        HandleError(status, e);
                    }
                    finally
                    {
                        status.Progress(items.Count, ++currentItem); //Update the Sync Progress
                    }

                }
            }
        }

        public override void Execute(List<DataCompareItem> addItems, List<DataCompareItem> updateItems, List<DataCompareItem> deleteItems, IDataSourceReader reader, IDataSynchronizationStatus status)
        {
            DataSourceReader = reader as PipedriveDatasourceReader;

            if (DataSourceReader != null)
            {
                Mapping = new DataSchemaMapping(SchemaMap, DataCompare);

                WebRequestHelper = DataSourceReader.GetWebRequestHelper();
                DatasourceInfo = DataSourceReader.GetDatasourceInfo();
                DataSchema = DatasourceInfo.GetPipedriveDataSchema(WebRequestHelper);

                //Process the Changed Items
                if (addItems != null && status.ContinueProcessing) AddItems(addItems, status);
                if (updateItems != null && status.ContinueProcessing) UpdateItems(updateItems, status);
                if (deleteItems != null && status.ContinueProcessing) DeleteItems(deleteItems, status);
            }
        }

        private static void HandleError(IDataSynchronizationStatus status, Exception e)
        {
            if (!status.FailOnError)
            {
                status.LogMessage(e.Message);
            }
            if (status.FailOnError)
            {
                throw e;
            }
        }

        private void HandleError(IDataSynchronizationStatus status, WebException e)
        {
            if (status.FailOnError)
            {
                throw e;
            }

            if (e.Response != null)
            {
                using (var response = e.Response.GetResponseStream())
                {
                    if (response != null)
                        using (var sr = new StreamReader(response))
                        {
                            string result = sr.ReadToEnd();
                            if (!string.IsNullOrEmpty(result))
                            {
                                status.LogMessage(string.Concat(e.Message, Environment.NewLine, result));
                            }
                        }
                }
            }
            else
            {
                if (!status.FailOnError)
                {
                    status.LogMessage(e.Message);
                }
            }
        }
    }
}
