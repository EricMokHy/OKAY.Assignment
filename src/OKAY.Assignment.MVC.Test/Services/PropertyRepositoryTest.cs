using OKAY.Assignment.MVC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OKAY.Assignment.MVC.Test.Services
{
    public class PropertyRepositoryTest : RepositoryTestBase
    {
        private PropertyRepository repo;
        public PropertyRepositoryTest()
        {
            repo = new PropertyRepository(context, accessor);
        }

        [Fact]
        public void InitTest()
        {
            bool a = true;
            Assert.True(accessor.HttpContext.User != null);
            Assert.True(accessor.HttpContext.User.HasClaim(Constraints.ClaimTypes.Role, "Administrator"));
            var id = accessor.HttpContext.User.FindFirst(Constraints.ClaimTypes.NameIdentifier).Value;
            var b = Guid.TryParse(id, out var ID);
            Assert.True(b);
        }

        [Fact]
        public async Task Create_ShouldSuccess()
        {
            string name = "this is a test property";
            int bedroom = 2;
            bool isAvaiable = true;
            decimal leasePrice = 10000;
            await repo.CreateAsync(name, bedroom, isAvaiable, leasePrice);

            var p = context.Properties.Where(x => x.name == name).FirstOrDefault();
            Assert.True(p != null);
        }

        [Fact]
        public async Task Create_ShouldFailed()
        {
            string name = "";
            int bedroom = 2;
            bool isAvaiable = true;
            decimal leasePrice = 10000;

            await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
            {
                await repo.CreateAsync(name, bedroom, isAvaiable, leasePrice);
            });

            name = new string('a', 220);
            await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
            {
                await repo.CreateAsync(name, bedroom, isAvaiable, leasePrice);
            });
        }

        [Fact]
        public async Task Delete_ShouldSuccess()
        {
            string name = "this is a test property";
            int bedroom = 2;
            bool isAvaiable = true;
            decimal leasePrice = 10000;
            var m = await repo.CreateAsync(name, bedroom, isAvaiable, leasePrice);

            await repo.DeleteAsync(m.id);

            var p = context.Properties.Where(x => x.id == m.id).FirstOrDefault();
            Assert.True(p == null);
        }

        [Fact]
        public async Task Delete_No_Such_Id_ShouldFail()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await repo.DeleteAsync(1000);
            });
        }

        [Fact]
        public async Task Delete_No_Access_Right_ShouldFail()
        {
            await SwitchUser(false);
            repo = new PropertyRepository(context, accessor);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await repo.DeleteAsync(1);
            });
            await SwitchUser(true);
            repo = new PropertyRepository(context, accessor);
        }

        [Fact]
        public async Task Find_ShouldSuccess()
        {
            var result = await repo.FindAsync("", 1, 10, "", "");

            Assert.True(result.pageCount == 1);
            Assert.True(result.Count == 10);

            result = await repo.FindAsync("admin", 1, 10, "", "");
            Assert.True(result.pageCount == 1);
            Assert.True(result.Count == 5);

            result = await repo.FindAsync("user", 1, 10, "", "");
            Assert.True(result.pageCount == 1);
            Assert.True(result.Count == 5);


            // switch to user
            await SwitchUser(false);
            repo = new PropertyRepository(context, accessor);

            result = await repo.FindAsync("", 1, 10, "", "");
            Assert.True(result.pageCount == 1);
            Assert.True(result.Count == 5);

            // switch back to admin
            await SwitchUser(true);
            repo = new PropertyRepository(context, accessor);
        }

        [Fact]
        public async Task FindById_ShouldSuccess()
        {
            var result = await repo.FindByIdAsync(1);
            Assert.True(result != null);

            // switch to user
            await SwitchUser(false);
            repo = new PropertyRepository(context, accessor);
            result = await repo.FindByIdAsync(1);
            Assert.True(result == null);

            await SwitchUser(true);
            repo = new PropertyRepository(context, accessor);
        }

        [Fact]
        public async Task Update_ShouldSuccess()
        {
            string name = "this is a updated property";
            int bedroom = 2;
            bool isAvaiable = true;
            decimal leasePrice = 10000;
            int id = 1;

            var result = await repo.UpdateAsync(id, name, bedroom, isAvaiable, leasePrice);

            Assert.True(result.name == name);
        }

        [Fact]
        public async Task Update_Empty_Name_Should_Fail()
        {
            string name = "";
            int bedroom = 2;
            bool isAvaiable = true;
            decimal leasePrice = 10000;
            int id = 1;

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await repo.UpdateAsync(id, name, bedroom, isAvaiable, leasePrice);
            });
        }

        [Fact]
        public async Task Update_LengthExceed_ShouldFail()
        {
            string name = new string('a', 250);
            int bedroom = 2;
            bool isAvaiable = true;
            decimal leasePrice = 10000;
            int id = 1;

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await repo.UpdateAsync(id, name, bedroom, isAvaiable, leasePrice);
            });
        }

        [Fact]
        public async Task Update_NotFound_ShouldFail()
        {
            string name = "this is a updated property";
            int bedroom = 2;
            bool isAvaiable = true;
            decimal leasePrice = 10000;
            int id = 1000;

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await repo.UpdateAsync(id, name, bedroom, isAvaiable, leasePrice);
            });
        }

        [Fact]
        public async Task Update_NotAccessable_ShouldFail()
        {
            string name = "this is a updated property";
            int bedroom = 2;
            bool isAvaiable = true;
            decimal leasePrice = 10000;
            int id = 1;

            await SwitchUser(false);
            repo = new PropertyRepository(context, accessor);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await repo.UpdateAsync(id, name, bedroom, isAvaiable, leasePrice);
            });

            await SwitchUser(true);
            repo = new PropertyRepository(context, accessor);
        }
    }
}
