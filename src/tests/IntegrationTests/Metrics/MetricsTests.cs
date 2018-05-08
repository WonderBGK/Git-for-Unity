﻿using GitHub.Unity;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace IntegrationTests
{
    [TestFixture]
    class MetricsTests : BaseIntegrationTest
    {

        [TestCase(nameof(Measures.AssetExplorerContextMenuLfsLock))]
        [TestCase(nameof(Measures.AssetExplorerContextMenuLfsUnlock))]
        [TestCase(nameof(Measures.AuthenticationViewButtonAuthentication))]
        [TestCase(nameof(Measures.BranchesViewButtonCheckoutLocalBranch))]
        [TestCase(nameof(Measures.BranchesViewButtonCheckoutRemoteBranch))]
        [TestCase(nameof(Measures.BranchesViewButtonCreateBranch))]
        [TestCase(nameof(Measures.BranchesViewButtonDeleteBranch))]
        [TestCase(nameof(Measures.ChangesViewButtonCommit))]
        [TestCase(nameof(Measures.HistoryViewToolbarButtonFetch))]
        [TestCase(nameof(Measures.HistoryViewToolbarButtonPull))]
        [TestCase(nameof(Measures.HistoryViewToolbarButtonPush))]
        [TestCase(nameof(Measures.NumberOfStartups))]
        [TestCase(nameof(Measures.ProjectsInitialized))]
        [TestCase(nameof(Measures.SettingsViewUnlockButtonLfsUnlock))]
        public void IncrementMetricsWorks(string measureName)
        {
            InitializeEnvironment(TestBasePath, null, false, false);
            var userId = Guid.NewGuid().ToString();
            var appVersion = ApplicationConfiguration.AssemblyName.Version.ToString();
            var unityVersion = "2017.3f1";
            var instanceId = Guid.NewGuid().ToString();
            var usageLoader = Substitute.For<IUsageLoader>();
            var usageStore = new UsageStore();
            usageStore.Model.Guid = userId;
            usageLoader.Load(Arg.Is<string>(userId)).Returns(usageStore);

            var usageTracker = new UsageTracker(Substitute.For<IMetricsService>(), Substitute.For<ISettings>(),
                usageLoader, userId, unityVersion, instanceId);

            var currentUsage = usageStore.GetCurrentMeasures(appVersion, unityVersion, instanceId);
            var prop = currentUsage.GetType().GetProperty(measureName);
            Assert.AreEqual(0, prop.GetValue(currentUsage, null));
            var meth = usageTracker.GetType().GetMethod("Increment" + measureName);
            meth.Invoke(usageTracker, null);
            currentUsage = usageStore.GetCurrentMeasures(appVersion, unityVersion, instanceId);
            Assert.AreEqual(1, prop.GetValue(currentUsage, null));
        }
    }
}
