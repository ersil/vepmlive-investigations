﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPMLiveCore.Tests.Shared
{
    // TODO: (CC-76591, 2018-07-10) Share the helpers across Test projects
    public static class AssertException
    {
        public static void Throws<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is TException, "Expected exception of type " + typeof(TException) + " but type of " + ex.GetType() + " was thrown instead.");
                return;
            }
            Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
        }

        public static void Throws<TException>(Action action, string expectedMessage) where TException : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is TException, "Expected exception of type " + typeof(TException) + " but type of " + ex.GetType() + " was thrown instead.");
                Assert.AreEqual(expectedMessage, ex.Message, "Expected exception with a message of '" + expectedMessage + "' but exception with message of '" + ex.Message + "' was thrown instead.");
                return;
            }
            Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
        }
    }
}
