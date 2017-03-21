using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UIAssistant.Infrastructure.Session;

namespace SessionTest
{
    [TestClass]
    public class SessionTest
    {
        [TestMethod]
        public void PauseTest()
        {
            Session session = new Session();
            var isCalled = false;

            session.Pausing += (o, e) => { isCalled = true; };

            Assert.AreEqual(false, isCalled);

            session.Pause();

            Assert.AreEqual(true, isCalled);
        }

        [TestMethod]
        public void ResumeTest()
        {
            Session session = new Session();
            var isCalled = false;

            session.Resumed += (o, e) => { isCalled = true; };

            Assert.AreEqual(false, isCalled);

            session.Resume();

            Assert.AreEqual(true, isCalled);
        }

        [TestMethod]
        public void FinishedTest()
        {
            Session session = new Session();
            var isPausedCalled = false;
            var isResumedCalled = false;
            var isFinishedCalled = false;

            session.Pausing += (o, e) => { isPausedCalled = true; };
            session.Resumed += (o, e) => { isResumedCalled = true; };
            session.Finished += (o, e) => { isFinishedCalled = true; };

            Assert.AreEqual(false, isPausedCalled);
            Assert.AreEqual(false, isResumedCalled);
            Assert.AreEqual(false, isFinishedCalled);

            session.Dispose();
            session.Pause();
            session.Resume();

            Assert.AreEqual(false, isPausedCalled);
            Assert.AreEqual(false, isResumedCalled);
            Assert.AreEqual(true, isFinishedCalled);
        }
    }
}
