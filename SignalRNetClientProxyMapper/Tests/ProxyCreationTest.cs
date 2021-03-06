﻿using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using NUnit.Framework;
using SignalRNetClientProxyMapper;

namespace Tests
{
    [TestFixture]
    public class ProxyCreationTest
    {
        [SetUp]
        public void SetUp() {
            _hubProxy = A.Fake<IHubProxy>();
            _hubConnection = A.Fake<HubConnection>();
        }

        IHubProxy _hubProxy;
        const ITestProxy TestProxy = null;
        const ITestProxyWithBaseMembers TestProxyWithBaseMembers = null;
        HubConnection _hubConnection;

        [Test]
        public void CanCallActionsWithNoParameters() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.ActionWithNoParameters();

            A.CallTo(() => _hubProxy.Invoke(A<string>.Ignored, A<object[]>.That.IsEmpty()))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CanCallActionsWithNoParametersAndAlternativeName() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.ActionWithNoParametersAndAlternativeName();

            A.CallTo(() => _hubProxy.Invoke(A<string>.Ignored, A<object[]>.That.IsEmpty()))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CanCallActionsWithParameters() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.ActionWithParameter("test");

            A.CallTo(() => _hubProxy.Invoke(A<string>.Ignored, new object[] {"test"}))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CanCallFunctionsWithNoParameters() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.FunctionWithNoParameters();

            A.CallTo(() => _hubProxy.Invoke<string>(A<string>.Ignored, A<object[]>.That.IsEmpty()))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CanCallFunctionsWithNoParametersAndAlternativeName() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.FunctionWithNoParametersAndAlternativeName();

            A.CallTo(() => _hubProxy.Invoke<string>(A<string>.Ignored, A<object[]>.That.IsEmpty()))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CanCallFunctionsWithParameters() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.FunctionWithParameter("test");

            A.CallTo(() => _hubProxy.Invoke<string>(A<string>.Ignored, new object[] {"test"}))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CanSubscribeToEventWithNoParameters() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.subscribableEventWithNoParameters(() => { });

            A.CallTo(() => _hubProxy.Subscribe("subscribableEventWithNoParameters"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CanSubscribeToEventWithNoParametersAndAlternativeName() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.subscribableEventWithNoParametersAndAlternativeName(() => { });

            A.CallTo(() => _hubProxy.Subscribe("AlternativeName"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CanSubscribeToEventWithParameters() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.subscribableEventWithParameter(paramter => { });

            A.CallTo(() => _hubProxy.Subscribe("subscribableEventWithParameter"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void CreateProxyFromConnection() {
            var testProxy = _hubConnection.CreateStrongHubProxy<ITestProxy>();

            testProxy.ActionWithNoParameters();
        }

        [Test]
        public void CreateProxyWithBaseMembers() {
            var testProxy = TestProxyWithBaseMembers.GetStrongTypedClientProxy(_hubProxy);

            testProxy.ActionInTopLevelInterface();
            testProxy.ActionInBaseInterface();

            A.CallTo(() => _hubProxy.Invoke("ActionInTopLevelInterface"))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _hubProxy.Invoke("ActionInBaseInterface"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void GetHubNameDesNotStripI() {
            ClientHubProxyExtensions.GetHubName<ITestProxy>(false)
                .Should()
                .Be("ITestProxy",
                    "ITestProxy starts with the interface  convention 'I' and should not be dropped when dropInterfaceI = false");
        }

        [Test]
        public void GetHubNameStripsI() {
            ClientHubProxyExtensions.GetHubName<ITestProxy>()
                .Should()
                .Be("TestProxy", "ITestProxy Starts with the Interface Convention 'I' and should be dropped by default");
        }

        [Test]
        public void GetHubNameWithAttributeGetsAttributeName() {
            ClientHubProxyExtensions.GetHubName<ITestProxyWithAttributeHubName>()
                .Should()
                .Be("TestProxy",
                    "When the HubName Attribute is defined the Hubs Internal name should be derived from that Attribute.");
        }

        [Test]
        public void ShouldFailBecauseNonTasksInProxy() {
            IFailingProxyWithInvalidMethodSignature testProxy = null;

            testProxy.Invoking(x => x.GetStrongTypedClientProxy(_hubProxy))
                .ShouldThrow<ArgumentException>("Proxy should never accept anything other than Task & Task<T>");
        }

        [Test]
        public void ShouldFailBecauseMethodHasBeenSpecifiedAsNotMapped() {
            var testProxy = TestProxy.GetStrongTypedClientProxy(_hubProxy);

            testProxy.ActionThatIsNotMapped();

            A.CallTo(() => _hubProxy.Invoke("ActionThatIsNotMapped")).MustNotHaveHappened();
        }

        [Test]
        public void ShouldFailBecauseOverloadsNotSupported() {
            IFailingProxyWithOverloadingMethod testProxy = null;

            testProxy.Invoking(x => x.GetStrongTypedClientProxy(_hubProxy))
                .ShouldThrow<NotSupportedException>("Proxy do not currently support Overloading Methods");
        }

        [Test]
        public void ShouldFailBecuaseOnlyInterfacesAccepted() {
            var test = new FailingClass(_hubProxy);
            test.Invoking(x => x.GetStrongTypedClientProxy(_hubProxy)).ShouldThrow<InvalidCastException>();
        }
    }

    public interface IEmptyProxy : IClientHubProxyBase {}


    public class FailingClass : IEmptyProxy
    {
        public FailingClass(IHubProxy hubProxy) {}

        public Task Invoke(string method, params object[] args) {
            throw new NotImplementedException();
        }

        public Task<T> Invoke<T>(string method, params object[] args) {
            throw new NotImplementedException();
        }

        public Subscription Subscribe(string eventName) {
            throw new NotImplementedException();
        }
    }

    public interface IFailingProxyWithInvalidMethodSignature : IClientHubProxyBase
    {
        void ActionWithVoidReturn();
    }

    public interface IFailingProxyWithOverloadingMethod : IClientHubProxyBase
    {
        Task ActionWithVoidReturn();
        Task ActionWithVoidReturn(object variable);
    }

    public interface ITestProxyBaseWithMembers : IClientHubProxyBase
    {
        Task ActionInBaseInterface();
    }
    public interface ITestProxyWithBaseMembers : ITestProxyBaseWithMembers
    {
        Task ActionInTopLevelInterface();
    }

    [HubName("TestProxy")]
    public interface ITestProxyWithAttributeHubName : IClientHubProxyBase {}

    public interface ITestProxy : IClientHubProxyBase
    {
        //IObservable<string> ObservedEvent { get; set; }
        IDisposable subscribableEventWithNoParameters(Action action);
        IDisposable subscribableEventWithParameter(Action<string> action);

        [HubMethodName("AlternativeName")]
        IDisposable subscribableEventWithNoParametersAndAlternativeName(Action action);

        Task ActionWithNoParameters();
        Task ActionWithParameter(string message);

        [HubMethodName("AlternativeName")]
        Task ActionWithNoParametersAndAlternativeName();

        [NotMapped]
        Task ActionThatIsNotMapped();

        Task<string> FunctionWithNoParameters();
        Task<string> FunctionWithParameter(string message);

        [HubMethodName("AlternativeName")]
        Task<string> FunctionWithNoParametersAndAlternativeName();
    }
}