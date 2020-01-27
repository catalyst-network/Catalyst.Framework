﻿using System.IO;
using System.Threading.Tasks;
using Lib.P2P.Protocols;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lib.P2P.Tests.Protocols
{
    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public async Task Encoding()
        {
            var ms = new MemoryStream();
            await Message.WriteAsync("a", ms);
            var buf = ms.ToArray();
            Assert.AreEqual(3, buf.Length);
            Assert.AreEqual(2, buf[0]);
            Assert.AreEqual((byte) 'a', buf[1]);
            Assert.AreEqual((byte) '\n', buf[2]);
        }

        [TestMethod]
        public async Task RoundTrip()
        {
            var msg = "/foobar/0.42.0";
            var ms = new MemoryStream();
            await Message.WriteAsync(msg, ms);
            ms.Position = 0;
            var result = await Message.ReadStringAsync(ms);
            Assert.AreEqual(msg, result);
        }
    }
}
