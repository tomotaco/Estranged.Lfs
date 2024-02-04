using Amazon.DynamoDBv2;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Estranged.Lfs.Adapter.DynamoDB;

namespace Estranged.Lfs.Tests.Adapter.DynamoDB
{
    public class DynamoDBLockAdapterTests : IDisposable
    {
        private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);

        public void Dispose() => mockRepository.VerifyAll();

/*        [Fact]
        public async Task TestCreateLock()
        {
            var mockClient = mockRepository.Create<IAmazonDynamoDB>();
            mockClient.Setup();
        }*/
    }
}
