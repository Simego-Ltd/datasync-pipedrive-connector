using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedriveDatasourceFactory
    {
        public static IPipedriveDatasourceInfo GetDatasourceInfo(string name, string apiToken)
        {
            switch (name.ToLower())
            {
                case "person":
                    {
                        return new PipedrivePersonDatasource(apiToken);
                    }
                case "deal":
                    {
                        return new PipedriveDealDatasource(apiToken);
                    }
                case "organization":
                    {
                        return new PipedriveOrgainizationDatasource(apiToken);
                    }
                case "user":
                    {
                        return new PipedriveUserDatasource(apiToken);
                    }
                case "pipeline":
                    {
                        return new PipedrivePipelineDatasource(apiToken);
                    }
                case "product":
                    {
                        return new PipedriveProductDatasource(apiToken);
                    }
                case "activity":
                    {
                        return new PipedriveActivityDatasource(apiToken);
                    }
                case "note":
                    {
                        return new PipedriveNoteDatasource(apiToken);
                    }
                case "stage":
                    {
                        return new PipedriveStageDatasource(apiToken);
                    }
            }

            throw new ArgumentException($"Invalid Pipedrive datasource: {name}", name);
        }
    }
}
