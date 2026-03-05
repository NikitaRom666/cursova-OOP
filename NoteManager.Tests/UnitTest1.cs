using Xunit;
using NoteManager.Models;
namespace NoteManager.Tests {
    public class LogicTests {
        [Fact]
        public void NoteCollection_IsSingleton() {
            var instance1 = NoteCollection.Instance;
            var instance2 = NoteCollection.Instance;
            Assert.Same(instance1, instance2);
        }
    }
}