using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Xunit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ChargesApi.Tests
{
    public class DynamoDbIntegrationTests<TStartup> : IDisposable where TStartup : class
    {
        //private const string Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTIyNDE2MjU4ODQ1MjgxMDQxNDAiLCJlbWFpbCI6ImRlbmlzZS5udWRnZUBoYWNrbmV5Lmdvdi51ayIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiRGVuaXNlIE51ZGdlIiwiZ3JvdXBzIjpbInNvbWUtdmFsaWQtZ29vZ2xlLWdyb3VwIl0sImlhdCI6MTYzOTQxNzE4OX0.Rai_olTwhVugBY8L8bpyhSGxX3lLB-ZLqxlSDQh96nE";
        private const string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ0ZXN0IiwiaWF0IjoxNjM5NDIyNzE4LCJleHAiOjE5ODY1Nzc5MTgsImF1ZCI6InRlc3QiLCJzdWIiOiJ0ZXN0IiwiZ3JvdXBzIjpbInNvbWUtdmFsaWQtZ29vZ2xlLWdyb3VwIiwic29tZS1vdGhlci12YWxpZC1nb29nbGUtZ3JvdXAiXSwibmFtZSI6InRlc3RpbmcifQ.IcpQ00PGVgksXkR_HFqWOakgbQ_PwW9dTVQu4w77tmU";
        protected HttpClient Client { get; private set; }
        private readonly DynamoDbMockWebApplicationFactory<TStartup> _factory;
        protected IDynamoDBContext DynamoDbContext => _factory?.DynamoDbContext;
        protected List<Action> CleanupActions { get; set; }

        private readonly List<TableDef> _tables = new List<TableDef>
        {
            new TableDef()
            {
                TableName = "Charges",
                PartitionKey = new AttributeDef()
                {
                    KeyName = "target_id",
                    KeyType = KeyType.HASH,
                    KeyScalarType = ScalarAttributeType.S
                }

            },
            new TableDef()
            {
                TableName = "ChargesMaintenance",
                PartitionKey = new AttributeDef()
                {
                    KeyName = "id",
                    KeyType = KeyType.HASH,
                    KeyScalarType = ScalarAttributeType.S
                }
            },
            new TableDef()
            {
                TableName = "ChargesList",
                PartitionKey = new AttributeDef()
                {
                    KeyName = "id",
                    KeyType = KeyType.HASH,
                    KeyScalarType = ScalarAttributeType.S
                },
                Indices = new List<GlobalIndexDef>{
                    new GlobalIndexDef()
                    {
                        KeyName = "charge_type",
                        KeyType = KeyType.HASH,
                        KeyScalarType = ScalarAttributeType.S,
                        IndexName = "charge_type_dx",
                        ProjectionType = "ALL"
                    }
                }
            },
        };

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
            {
                Environment.SetEnvironmentVariable(name, defaultValue);
            }
        }

        public DynamoDbIntegrationTests()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");
            EnsureEnvVarConfigured("DynamoDb_LocalSecretKey", "2cl9i");
            EnsureEnvVarConfigured("DynamoDb_LocalAccessKey", "vymxp");
            EnsureEnvVarConfigured("REQUIRED_GOOGL_GROUPS", "some-valid-google-group");
            _factory = new DynamoDbMockWebApplicationFactory<TStartup>(_tables);

            Client = _factory.CreateClient();
            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(Token);
            CleanupActions = new List<Action>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                foreach (var act in CleanupActions)
                {
                    act();
                }
                Client.Dispose();

                if (null != _factory)
                    _factory.Dispose();
                _disposed = true;
            }
        }
    }

    public class TableDef
    {
        public string TableName { get; set; }
        public AttributeDef PartitionKey { get; set; }
        public List<GlobalIndexDef> Indices { get; set; }
    }

    public class AttributeDef
    {
        public string KeyName { get; set; }
        public ScalarAttributeType KeyScalarType { get; set; }
        public KeyType KeyType { get; set; }
    }

    public class GlobalIndexDef : AttributeDef
    {
        public string IndexName { get; set; }
        public string ProjectionType { get; set; }
    }

    [CollectionDefinition("DynamoDb collection", DisableParallelization = true)]
    public class DynamoDbCollection : ICollectionFixture<DynamoDbIntegrationTests<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

}
