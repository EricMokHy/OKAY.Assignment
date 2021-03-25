using OKAY.Assignment.MVC.Services;
using OKAY.Assignment.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace OKAY.Assignment.MVC.Test.Services
{
    public class TransactionRepositoryTest : RepositoryTestBase
    {
        TransactionReporsitory repo;
        IPropertyRepository property;
        public TransactionRepositoryTest()
        {
        }

        [Fact]
        public async Task Create_ShouldSuccess()
        {
            var m = new Mock<IPropertyRepository>();
            m.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new PropertyViewModel()));
            repo = new TransactionReporsitory(context, accessor, m.Object);

            await repo.CreateAsync(10);

            var result = context.Transactions.Where(x => x.propertyId == 10).ToList();
            Assert.True(result.Count == 1);
        }

        [Fact]
        public async Task Create_PropertyNotFound_ShouldFail()
        {
            var m = new Mock<IPropertyRepository>();
            m.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult<PropertyViewModel>(null));
            repo = new TransactionReporsitory(context, accessor, m.Object);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                var result = await repo.CreateAsync(1);
            });
        }

        [Fact]
        public async Task Delete_ShouldSuccess()
        {
            var m = new Mock<IPropertyRepository>();
            repo = new TransactionReporsitory(context, accessor, m.Object);
            await repo.DeleteAsync(1);

            var result = context.Transactions.Where(x => x.id == 1).FirstOrDefault();
            Assert.True(result == null);
        }

        [Fact]
        public async Task Delete_TransactionNotFound_ShouldFail()
        {
            var m = new Mock<IPropertyRepository>();
            repo = new TransactionReporsitory(context, accessor, m.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await repo.DeleteAsync(10);
            });
        }

        [Fact]
        public async Task Delete_TransactionNoAccessRight_ShouldFaild()
        {
            var m = new Mock<IPropertyRepository>();
            await SwitchUser(false);
            repo = new TransactionReporsitory(context, accessor, m.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await repo.DeleteAsync(7);
            });
        }
    }
}
