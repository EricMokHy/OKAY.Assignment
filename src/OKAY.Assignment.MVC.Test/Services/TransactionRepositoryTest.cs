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

        [Fact]
        public async Task Find_ShouldSuccess()
        {
            var m = new Mock<IPropertyRepository>();
            repo = new TransactionReporsitory(context, accessor, m.Object);

            var result = await repo.FindAsync("admin", 1, 10, "", "");

            Assert.True(result.Count == 4);
        }

        [Fact]
        public async Task Find_ByUser_ShouldSuccess()
        {
            var m = new Mock<IPropertyRepository>();
            await SwitchUser(false);

            repo = new TransactionReporsitory(context, accessor, m.Object);

            var result = await repo.FindAsync("", 1, 10, "", "");

            Assert.True(result.Count == 3);

            await SwitchUser(true);
        }

        [Fact]
        public async Task FindById_ShouldSuccess()
        {
            var m = new Mock<IPropertyRepository>();
            repo = new TransactionReporsitory(context, accessor, m.Object);

            var result = await repo.FindByIdAsync(4);

            Assert.True(result.id == 4);
        }

        [Fact]
        public async Task FindById_NotFound_ShouldSuccess()
        {
            var m = new Mock<IPropertyRepository>();
            repo = new TransactionReporsitory(context, accessor, m.Object);

            var result = await repo.FindByIdAsync(1000);

            Assert.True(result == null);
        }

        [Fact]
        public async Task FindById_NotAccessable_ShouldSuccess()
        {
            var m = new Mock<IPropertyRepository>();
            await SwitchUser(false);
            repo = new TransactionReporsitory(context, accessor, m.Object);

            var result = await repo.FindByIdAsync(4);

            Assert.True(result == null);

            await SwitchUser(true);
        }

        [Fact]
        public async Task Update_ShouldSuccess()
        {
            var admin = await userManager.FindByNameAsync("admin@example.com");
            var m = new Mock<IPropertyRepository>();
            m.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new PropertyViewModel()));
            repo = new TransactionReporsitory(context, accessor, m.Object);

            var result = await repo.UpdateAsync(1, 1, admin.Id, new DateTime(2020, 10, 31));

            Assert.True(result != null, "result is null");
            Assert.True(result.propertyId != 1, "property not updated");
            Assert.True(result.TransactionDate != new DateTime(2020, 10, 31), "transaction date not updated");
        }

        [Fact]
        public async Task Update_PropertyNotFound_ShouldFail()
        {
            var admin = await userManager.FindByNameAsync("admin@example.com");
            var m = new Mock<IPropertyRepository>();
            m.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult<PropertyViewModel>(null));
            repo = new TransactionReporsitory(context, accessor, m.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                var result = await repo.UpdateAsync(1, 1, admin.Id, new DateTime(2020, 1, 1));
            });
        }

        [Fact]
        public async Task Update_TransactionNotFound_ShouldFail()
        {
            var admin = await userManager.FindByNameAsync("admin@example.com");
            var m = new Mock<IPropertyRepository>();
            m.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new PropertyViewModel()));
            repo = new TransactionReporsitory(context, accessor, m.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                var result = await repo.UpdateAsync(100, 1, admin.Id, new DateTime(2020, 1, 1));
            });
        }

        [Fact]
        public async Task Update_UserNotFound_ShouldFail()
        {
            var m = new Mock<IPropertyRepository>();
            m.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new PropertyViewModel()));
            repo = new TransactionReporsitory(context, accessor, m.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                var result = await repo.UpdateAsync(1, 1, Guid.NewGuid(), new DateTime(2020, 1, 1));
            });
        }

        [Fact]
        public async Task Update_ChangeUserByNotAdmin_ShouldFail()
        {
            var admin = await userManager.FindByNameAsync("admin@example.com");
            await SwitchUser(false);
            var m = new Mock<IPropertyRepository>();
            m.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new PropertyViewModel()));
            repo = new TransactionReporsitory(context, accessor, m.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                var result = await repo.UpdateAsync(1, 1, admin.Id, new DateTime(2020, 1, 1));
            });

            await SwitchUser(true);
        }
    }
}
